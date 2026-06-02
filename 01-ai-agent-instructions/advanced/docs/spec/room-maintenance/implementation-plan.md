---
artifact: implementation-plan
status: draft
source-of-truth: false
requires-approval: true
approved-by: ~
related-spec: docs/spec/room-maintenance/spec.md
related-decision: docs/spec/room-maintenance/decision.md
---

# Implementation Plan — Room Maintenance

## Overview

| | Slice 1 | Slice 2 |
|---|---|---|
| Goal | Availability module established; maintenance feature fully operational | Reservation overlap check moved from repository to Availability |
| Complexity | L | M |
| Delivers | AC1–AC11 (maintenance management + reservation blocking via pre-check) | Atomic mutual exclusion via claim registry; repo overlap check removed |
| Tech debt paid | Time-bounded blocking shortcut; room status used directly in scheduling | Availability check embedded in repository |

**Total: XL** — deliver as two sequential slices.

---

## Slice 1 — Establish Availability Module + Maintenance Feature

### S1-1 — Create Availability module structure
**Size:** XS | **Depends on:** nothing

- Create module folder and project: `Modules/Availability/`
- Register module in the DI container (empty, no functionality yet)
- Add the `IAvailabilityService` interface (empty shell) — the public contract all other modules will reference

---

### S1-2 — Implement local room projection in Availability
**Size:** S | **Depends on:** S1-1

Availability must own a local copy of room status to answer availability queries without calling RoomManagement at runtime.

**Persistence:** new table `room_availability_projection` with columns: `room_id (Guid, PK)`, `is_active (bool)`, `updated_at (DateTimeOffset)`

**Operations on the projection:**
- Register room (idempotent: safe to call twice with same room ID — updates `is_active = true`)
- Set room inactive (idempotent: sets `is_active = false`)
- Get room (returns `null` if not found — treated as "room not found" by callers)

---

### S1-3 — Wire room state push from RoomManagement
**Size:** S | **Depends on:** S1-2

RoomManagement command handlers gain outbound calls to Availability at write time.

**CreateRoom handler:** after room is persisted, call `IAvailabilityService.RegisterRoom(roomId)`. Include a compensation call: if RegisterRoom fails, the command fails and the room creation transaction is rolled back. Write the compensation structure now — in the monolith the DB transaction handles it; the code is extraction-ready.

**DeactivateRoom handler:** after deactivation is persisted, call `IAvailabilityService.SetRoomInactive(roomId)`. Same compensation structure.

**Extraction-readiness:** both push operations use only `Guid` as parameter type.

---

### S1-4 — Migration: seed existing rooms into Availability projection
**Size:** XS | **Depends on:** S1-2

One-time migration script (idempotent, safe to re-run): read all rooms from `rooms` table, write each room's ID and active status into `room_availability_projection`. Run on deployment before any maintenance management endpoints are called.

---

### S1-5 — Implement RoomMaintenance aggregate
**Size:** M | **Depends on:** S1-1

**States:** `Planned`, `Completed`, `Cancelled`

**State transition methods:**

`Create(roomId, startDate, endDate)`:
- Guard: `startDate ≤ endDate`
- Produces a RoomMaintenance in `Planned` state with `plannedStartDate` and `plannedEndDate`

`ModifyPeriod(newStartDate, newEndDate)`:
- Guard: status must be `Planned`
- Guard: `newStartDate ≤ newEndDate`

`Cancel()`:
- Guard: status must be `Planned`
- Transitions to `Cancelled`

`Complete(actualEndDate)`:
- Guard: status must be `Planned`
- Guard: `actualEndDate ≤ plannedEndDate`
- Transitions to `Completed`, records `actualEndDate`

**Effective blocking period query:** A `Planned` block covers `[startDate, plannedEndDate]`. A `Completed` block covers `[startDate, actualEndDate]`. A `Cancelled` block covers nothing. Use this in the overlap check.

---

### S1-6 — Implement RoomMaintenance persistence
**Size:** S | **Depends on:** S1-5

**Schema:** new table `room_maintenance`
- `maintenance_id (Guid, PK)`
- `room_id (Guid, indexed)`
- `planned_start_date (Date)`
- `planned_end_date (Date)`
- `actual_end_date (Date, nullable)`
- `status (varchar: Planned | Completed | Cancelled)`
- `created_at (DateTimeOffset)`
- `updated_at (DateTimeOffset)`

**Repository operations:**
- `Add(maintenance)` — insert new record
- `GetById(maintenanceId)` — fetch by ID
- `GetByRoomId(roomId)` — list all maintenance for a room
- `GetActiveBlocksOverlapping(roomId, startDate, endDate)` — returns all `Planned` or `Completed` maintenance records whose effective blocking period overlaps `[startDate, endDate]`. Used for conflict detection.

**Overlap logic for the query:** A maintenance record overlaps with `[queryStart, queryEnd]` when:
- Status = `Planned` AND `plannedStartDate ≤ queryEnd AND plannedEndDate ≥ queryStart`
- Status = `Completed` AND `plannedStartDate ≤ queryEnd AND actualEndDate ≥ queryStart`
- Status = `Cancelled` → never overlaps

---

### S1-7 — Implement IAvailabilityService: IsAvailableForPeriod
**Size:** S | **Depends on:** S1-2, S1-6

Implement the `IsAvailableForPeriod(roomId, startDate, endDate)` method on `IAvailabilityService`.

**Check sequence:**
1. Load room from projection → if null: return `RoomNotFound`
2. If `room.isActive == false`: return `RoomInactive`
3. Call `GetActiveBlocksOverlapping(roomId, startDate, endDate)` → if any results: return `MaintenanceBlock(maintenanceId)`
4. Return `Available`

Note: reservation claim check is NOT part of Slice 1 — added in Slice 2 (S2-2).

---

### S1-8 — Implement CreateMaintenancePeriod command
**Size:** M | **Depends on:** S1-5, S1-6, S1-7

**Handler steps:**
1. Validate room exists and is active: call `IAvailabilityService.IsAvailableForPeriod` — if not available for the period, return the specific error.
2. **[Slice 1 temporary — removed in S2-8]** Check for conflicting active reservations: query `IReservationRepository.GetActiveReservationsOverlapping(roomId, startDate, endDate)`. If any results: return `ConflictingReservations(reservationIds)`.
3. Check for conflicting maintenance: call `GetActiveBlocksOverlapping` — if overlap found, return `ConflictingMaintenance`.
4. Create and persist `RoomMaintenance` in `Planned` state.

The `IReservationRepository` dependency in step 2 is intentionally temporary. In Slice 2 (S2-8), once reservation claims are in Availability's own store, this step is replaced by querying the claim store — no call to Reservations needed.

---

### S1-9 — Implement ModifyMaintenancePeriod command
**Size:** S | **Depends on:** S1-8

**Handler steps:**
1. Load `RoomMaintenance` by ID → if not found: return `NotFound`
2. Guard: status must be `Planned` → if not: return `InvalidState`
3. Same conflict checks as S1-8 (steps 2 and 3), excluding the maintenance being modified itself from the overlap query
4. Call `maintenance.ModifyPeriod(newStartDate, newEndDate)` → persist

---

### S1-10 — Implement CancelMaintenance command
**Size:** XS | **Depends on:** S1-5, S1-6

**Handler steps:**
1. Load `RoomMaintenance` by ID
2. Call `maintenance.Cancel()` (guard: status must be `Planned`)
3. Persist updated state

---

### S1-11 — Implement CompleteMaintenance command
**Size:** XS | **Depends on:** S1-5, S1-6

**Handler steps:**
1. Load `RoomMaintenance` by ID
2. Call `maintenance.Complete(actualEndDate)` (guards: `Planned` state; `actualEndDate ≤ plannedEndDate`)
3. Persist updated state

---

### S1-12 — Update CreateReservation handler
**Size:** S | **Depends on:** S1-7

**Changes:**
- Add: call `IAvailabilityService.IsAvailableForPeriod(roomId, startDate, endDate)` before `TryAdd`. If not available: return the specific error (RoomNotFound, RoomInactive, or MaintenanceBlock).
- Remove: the direct `IRoomReader.IsActive` check — this is now answered by `IsAvailableForPeriod`.
- Keep: `IReservationRepository.TryAdd` (reservation overlap check remains in the repository for Slice 1).

---

### S1-13 — Update ChangeReservationPeriod handler
**Size:** S | **Depends on:** S1-7

Same pattern as S1-12:
- Add: `IsAvailableForPeriod` pre-check for the new period
- Remove: direct `IRoomReader.IsActive` check
- Keep: `IReservationRepository.TryChangePeriod`

---

### S1-14 — Maintenance API endpoints
**Size:** M | **Depends on:** S1-8, S1-9, S1-10, S1-11

Endpoints:
- `POST /rooms/{roomId}/maintenance` — CreateMaintenancePeriod (request: `startDate`, `endDate?`)
- `PUT /rooms/{roomId}/maintenance/{maintenanceId}/period` — ModifyMaintenancePeriod (request: `startDate`, `endDate`)
- `POST /rooms/{roomId}/maintenance/{maintenanceId}/cancel` — CancelMaintenance
- `POST /rooms/{roomId}/maintenance/{maintenanceId}/complete` — CompleteMaintenance (request: `actualEndDate`)
- `GET /rooms/{roomId}/maintenance` — list maintenance for room (query, returns all records with status and dates)
- `GET /rooms/{roomId}/maintenance/{maintenanceId}` — get single maintenance record

Response shapes: include `maintenanceId`, `roomId`, `plannedStartDate`, `plannedEndDate`, `actualEndDate`, `status`.

---

### S1-15 — Unit tests: RoomMaintenance aggregate
**Size:** S | **Depends on:** S1-5

- All valid state transitions produce correct state
- Guard violations raise domain exceptions: modify/cancel/complete when not `Planned`; `actualEndDate > plannedEndDate`; `startDate > endDate`
- Effective blocking period computed correctly for each status

---

### S1-16 — Integration tests: Maintenance management
**Size:** M | **Depends on:** S1-14

- Create maintenance: happy path (room active, no conflicts)
- Create maintenance: room not found
- Create maintenance: room inactive
- Create maintenance: conflicting active reservation (rejected)
- Create maintenance: conflicting maintenance block (rejected)
- Modify period: happy path
- Modify period: wrong status (Completed or Cancelled)
- Modify period: new period conflicts
- Cancel: happy path
- Cancel: wrong status
- Complete: happy path (full period consumed)
- Complete: early end date (room available from actual end date + 1 day)
- Complete: `actualEndDate > plannedEndDate` (rejected)

---

### S1-17 — Integration tests: Reservation with maintenance blocking
**Size:** M | **Depends on:** S1-12, S1-13, S1-16

- CreateReservation: blocked when `Planned` maintenance overlaps with requested period
- CreateReservation: allowed when maintenance exists but for a different period
- CreateReservation: allowed after maintenance is cancelled
- CreateReservation: allowed after maintenance completes with early end date (for the unblocked period)
- ChangeReservationPeriod: blocked when new period overlaps with `Planned` maintenance
- ChangeReservationPeriod: allowed when new period is outside all maintenance windows

---

## Slice 2 — Elevate Reservation Overlap into Availability

### S2-1 — Implement ReservationClaim in Availability
**Size:** M | **Depends on:** S1-7

**Persistence:** new table `reservation_claims`
- `claim_id (Guid, PK)`
- `room_id (Guid, indexed)`
- `start_date (Date)`
- `end_date (Date)`
- `reservation_id (Guid, unique — one claim per reservation)`
- `is_active (bool)`
- `created_at (DateTimeOffset)`

**Operations (all idempotent):**

`RegisterClaim(roomId, startDate, endDate, reservationId)`:
- Load room from projection → if null: return `RoomNotFound`
- If room inactive: return `RoomInactive`
- Check `GetActiveBlocksOverlapping(roomId, startDate, endDate)` → if any: return `MaintenanceBlock`
- Check active claim overlap for room: `SELECT * FROM reservation_claims WHERE room_id = ? AND is_active = true AND start_date <= ? AND end_date >= ?` → if any: return `ClaimConflict`
- Insert claim with `is_active = true`
- Return success
- Idempotency: if a claim for this `reservationId` already exists with the same period and `is_active = true`, return success (no-op)

`ReleaseClaim(reservationId)`:
- Set `is_active = false` where `reservation_id = ?`
- Idempotent: if already inactive or not found, return success (no-op)

`ReplaceClaimPeriod(reservationId, newStartDate, newEndDate)`:
- Load existing active claim for `reservationId` → if not found: treat as `RegisterClaim`
- Run same conflict checks as `RegisterClaim` excluding the claim being replaced
- If no conflict: set old claim `is_active = false` and insert new claim for the same `reservationId` with the new period — within a single transaction
- Return success or the specific conflict error
- Idempotency: if called with the same new period for the same `reservationId`, return success (no-op)

---

### S2-2 — Add reservation claim check to IsAvailableForPeriod
**Size:** XS | **Depends on:** S2-1

Add a fourth check to the existing `IsAvailableForPeriod` sequence (after the maintenance block check):

4. Query active reservation claims overlapping with the period → if any: return `ClaimConflict`

---

### S2-3 — Update maintenance conflict check to use claim store
**Size:** S | **Depends on:** S2-1

Update `CreateMaintenancePeriod` and `ModifyMaintenancePeriod` handlers to use Availability's own claim store for the reservation conflict check instead of the temporary IReservationRepository query:

- Replace: `IReservationRepository.GetActiveReservationsOverlapping(...)` (Slice 1 temporary, see S1-8)
- With: active claim overlap query against `reservation_claims` table in Availability

The `IReservationRepository` dependency in these handlers is eliminated entirely.

---

### S2-4 — Update CreateReservation handler
**Size:** M | **Depends on:** S2-1

**Changes:**
- Replace: `IReservationRepository.TryAdd(...)` 
- With: `IAvailabilityService.RegisterClaim(roomId, startDate, endDate, reservationId)` + plain `IReservationRepository.Add(reservation)` in the same database transaction

**Compensation structure (extraction-ready):**
```
// Step 1: register claim in Availability
var claimResult = availabilityService.RegisterClaim(roomId, period, reservationId);
if (claimResult.IsFailure) return claimResult.Error;

// Step 2: persist reservation (same DB transaction)
var addResult = reservationRepository.Add(reservation);
if (addResult.IsFailure)
{
    // Compensation: release the claim
    // In the monolith: DB transaction rollback makes this a no-op.
    // After service extraction: this call becomes load-bearing.
    availabilityService.ReleaseClaim(reservationId);
    return addResult.Error;
}
```

In the monolith with a shared database: wrap both operations in a single transaction so the rollback handles compensation automatically. Write the compensation call anyway — it is the extraction-ready pattern.

---

### S2-5 — Update CancelReservation handler
**Size:** S | **Depends on:** S2-1

**Changes:**
- Add: `IAvailabilityService.ReleaseClaim(reservationId)` as part of the cancellation
- Both the reservation status update and the claim release must be in the same database transaction

---

### S2-6 — Update ChangeReservationPeriod handler
**Size:** M | **Depends on:** S2-1

**Changes:**
- Replace: `IReservationRepository.TryChangePeriod(...)`
- With: `IAvailabilityService.ReplaceClaimPeriod(reservationId, newPeriod)` + plain period update in repository, in the same transaction

If `ReplaceClaimPeriod` returns a conflict, the handler returns the conflict error without modifying the reservation. The existing claim for the original period remains active.

---

### S2-7 — Remove embedded overlap check from IReservationRepository
**Size:** S | **Depends on:** S2-4, S2-5, S2-6

- Rename `IReservationRepository.TryAdd` → `Add` (plain write, no overlap check; the constraint is no longer enforced here)
- Rename `IReservationRepository.TryChangePeriod` → `UpdatePeriod` (plain write)
- Remove overlap detection logic from both repository implementations
- Update all callers (only CreateReservation and ChangeReservationPeriod — already updated in S2-4 and S2-6)

---

### S2-8 — Remove temporary IReservationRepository dependency from maintenance handlers
**Size:** XS | **Depends on:** S2-3, S2-7

Remove the `IReservationRepository` dependency from `CreateMaintenancePeriodCommandHandler` and `ModifyMaintenancePeriodCommandHandler` — replaced by S2-3's claim store query.

---

### S2-9 — Unit tests: Claim model
**Size:** S | **Depends on:** S2-1

- RegisterClaim: success, RoomNotFound, RoomInactive, MaintenanceBlock, ClaimConflict
- RegisterClaim idempotency: called twice with same inputs, second call is a no-op success
- ReleaseClaim: success, already released (no-op), not found (no-op)
- ReplaceClaimPeriod: success, conflict on new period, old claim preserved on failure

---

### S2-10 — Integration tests: Reservation with claim management
**Size:** M | **Depends on:** S2-4, S2-5, S2-6

- CreateReservation: claim registered on success
- CreateReservation: conflict returns correct error (maintenance block vs. claim conflict vs. room inactive — each a distinct case)
- CancelReservation: claim released; subsequent reservation for same period succeeds
- ChangeReservationPeriod: old claim released, new claim registered; original period available again
- ChangeReservationPeriod: conflict on new period — original claim preserved, original period blocked

---

### S2-11 — Integration tests: Concurrent reservation creation
**Size:** M | **Depends on:** S2-4

Test database-level mutual exclusion: two simultaneous requests for the same room and overlapping period must result in exactly one success and one conflict error.

This test verifies that the transaction isolation level and claim registration query are sufficient to prevent double-booking. Use a test harness that issues two parallel requests against the real database.

---

### S2-12 — Integration tests: Cross-domain conflicts
**Size:** M | **Depends on:** S2-3, S2-4

- Maintenance creation blocked by existing reservation claim
- Reservation creation blocked by existing maintenance block
- Maintenance cancelled: reservation creation for the same period now succeeds
- Maintenance completed with early end date: reservation creation for the unblocked portion succeeds
- Overlapping maintenance blocks rejected

---

### S2-13 — End-to-end scenario tests
**Size:** S | **Depends on:** S2-12

Full lifecycle scenarios that cross module boundaries:

- **Happy path:** Create room → Create reservation → Plan maintenance for different period → Confirm reservation → Complete maintenance → Reservation unaffected
- **Blocked path:** Create room → Plan maintenance → Attempt reservation for same period → Rejected → Cancel maintenance → Create reservation → Success
- **Early completion:** Create room → Plan maintenance (5 days) → Create reservation for day 4 → Rejected → Complete maintenance on day 2 → Create reservation for day 3 → Success

---

## Open Implementation Decisions

| Decision | Recommendation | Note |
|---|---|---|
| Transaction isolation for concurrent claim registration | `Serializable` or pessimistic row lock on the room-period combination in `reservation_claims` | Verify under load; optimistic concurrency is also viable with retry |
| Compensation call activation | DB transaction rollback in monolith; activate the explicit compensation call when extracting Availability to a separate service | Write the code now, activated by extraction |
| Idempotency key strategy for claim operations | Use `reservation_id` as the natural idempotency key | Matches the one-claim-per-reservation invariant |
| `IReservationRepository.GetActiveReservationsOverlapping` | Add this read method for the Slice 1 temporary use in S1-8; mark it `[Temporary - remove in S2-8]` | Explicit marker avoids confusion during Slice 2 |
