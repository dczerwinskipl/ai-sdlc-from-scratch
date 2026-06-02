---
artifact: spec
status: draft
source-of-truth: false
requires-approval: true
approved-by: ~
related-spec: ~
related-decision: docs/spec/room-maintenance/decision.md
---

# Room Maintenance — Feature Specification

## Feature Summary

Adds maintenance management for hotel rooms as a first-class scheduling concern. Hotel Staff can plan, modify, cancel, and complete maintenance periods. A planned maintenance blocks new reservations for the affected room during the maintenance period only — the room remains reservable outside the maintenance window.

This feature introduces the `Availability` module — a new architectural boundary that becomes the single authority for "can this room be used for this period?" It consolidates all temporal blocking mechanisms and resolves the two known availability shortcuts documented in `known-decisions.md`.

---

## Requirements

| ID | Requirement |
|---|---|
| R1 | Hotel Staff can plan a maintenance period for a room with a start date and end date. Single-day maintenance: end date = start date. |
| R2 | A Planned maintenance blocks new reservations for the room during the maintenance period only. |
| R3 | Rooms remain reservable for periods with no active maintenance. |
| R4 | Maintenance transitions through states: Planned → Completed (with actual end date) or Planned → Cancelled. |
| R5 | Cancelled maintenance does not block any reservation. |
| R6 | Completing maintenance before the planned end date makes the room available from the actual end date onward. |
| R7 | Hotel Staff can cancel a Planned maintenance. |
| R8 | Hotel Staff can complete a Planned maintenance, recording an actual end date ≤ planned end date. |
| R9 | Hotel Staff can modify the period (start and end dates) of a Planned maintenance. |

---

## Acceptance Criteria

| ID | Criterion |
|---|---|
| AC1 | Creating a maintenance period requires a room ID, start date, and optional end date (defaults to start date for single-day). |
| AC2 | Creating a maintenance period is rejected if any Pending or Confirmed reservation overlaps with the requested period. |
| AC3 | Creating a maintenance period is rejected if another active maintenance block overlaps with the requested period for the same room. |
| AC4 | A Planned maintenance prevents creation of reservations that overlap with its period. |
| AC5 | A Planned maintenance prevents period changes on reservations if the new period overlaps with it. |
| AC6 | A Cancelled maintenance does not prevent reservation creation or period changes. |
| AC7 | Completing a maintenance with an actual end date earlier than the planned end date makes the room available from that date onward. |
| AC8 | Modifying a maintenance period is rejected if the new period conflicts with existing active reservations or other maintenance blocks for the same room. |
| AC9 | Modifying a maintenance period is only allowed when the maintenance status is Planned. |
| AC10 | Cancelling a maintenance is only allowed when the maintenance status is Planned. |
| AC11 | Completing a maintenance is only allowed when the maintenance status is Planned; the actual end date must be ≤ the planned end date. |

---

## Domain Model

### RoomMaintenance Aggregate (owned by: Availability module)

| State | Blocks reservations? | Effective blocking period |
|---|---|---|
| Planned | Yes | `[startDate, plannedEndDate]` |
| Completed | Only past portion | `[startDate, actualEndDate]` — room is unblocked from `actualEndDate + 1 day` |
| Cancelled | No | — |

State transitions:

| From | To | Trigger | Guard |
|---|---|---|---|
| (new) | Planned | Create maintenance | Room active; no conflicting reservation claims or maintenance blocks |
| Planned | Planned | Modify period | New period has no conflicting claims or blocks |
| Planned | Completed | Complete maintenance | Actual end date ≤ planned end date |
| Planned | Cancelled | Cancel maintenance | — |
| Completed | (terminal) | — | — |
| Cancelled | (terminal) | — | — |

---

## Module Structure (Post-Decision)

### Availability [NEW]

Single authority for "can this room be used for this period?"

**Owns:**
- `RoomMaintenance` aggregate with full lifecycle (Planned, Completed, Cancelled)
- Local room status projection — populated by push from RoomManagement at write time; never queried from RoomManagement at query time
- `ReservationClaim` records (Slice 2) — registered when reservations are created or period-changed; released when cancelled

**Provides:**
- Availability query: answers "is this room available for this period?" by combining local room status + active maintenance blocks + active reservation claims
- Maintenance management operations: create, modify period, cancel, complete
- Claim operations (Slice 2): register claim, release claim, replace claim period — all atomic and idempotent

**No outbound module dependencies at query time.** All state needed to answer availability queries is owned locally.

---

### RoomManagement (Extended)

Unchanged: Room aggregate, descriptive data, operational status (Active/Inactive).

**New behavior:** CreateRoom and DeactivateRoom operations push room state changes to Availability at write time. Availability stores the result in its local room projection. This is a write-time push — Availability never calls RoomManagement at query time.

---

### Reservations (Extended)

Unchanged: Reservation aggregate and reservation lifecycle rules (Pending, Confirmed, Cancelled; period change reverts Confirmed to Pending).

**Changed behavior:**
- Slice 1: CreateReservation and ChangeReservationPeriod call Availability.IsAvailableForPeriod before proceeding. The direct IRoomReader.isActive check is removed from the reservation path — room active status is now answered by Availability via its local projection.
- Slice 2: Availability claim registration replaces the embedded reservation overlap check in IReservationRepository. Reservation writes become plain writes. CancelReservation and ChangeReservationPeriod gain claim release / claim replacement calls to Availability.

**Dependency change:** The direct Reservations → RoomManagement dependency for room availability and existence checks is eliminated. Reservations depends on Availability only for the reservation creation and period-change paths.

---

## Integration Contracts

**Availability module public interface — provided to Reservations and future consumers:**
- `IsAvailableForPeriod(roomId, period)` — single authoritative availability answer
- `CreateMaintenancePeriod(roomId, period)` → Result
- `ModifyMaintenancePeriod(maintenanceId, newPeriod)` → Result
- `CancelMaintenance(maintenanceId)` → Result
- `CompleteMaintenance(maintenanceId, actualEndDate)` → Result
- `RegisterClaim(roomId, period, reservationId)` → Result (Slice 2)
- `ReleaseClaim(reservationId)` (Slice 2)
- `ReplaceClaimPeriod(reservationId, newPeriod)` → Result (Slice 2)

All write operations are idempotent. The module is designed for extraction-readiness: separate schema, primitive types in the public interface, compensation call structure in callers.

**RoomManagement → Availability (push-based, write-time):**
- Room created: push room registration to Availability
- Room deactivated: push status change to Availability

---

## Existing System Impact

### Known Shortcuts Resolved

| Shortcut | How resolved |
|---|---|
| No time-bounded blocking (Active/Inactive only) | RoomMaintenance as a first-class aggregate in Availability provides period-based blocking |
| Availability check embedded in IReservationRepository (Slice 2) | Overlap check moves to Availability's atomic claim registry; repository writes become plain writes |
| Room active/inactive used directly in scheduling decisions | Availability is now the single authority; IRoomReader.isActive is no longer used for scheduling |

### RoomManagement

- CreateRoom and DeactivateRoom command handlers gain outbound push calls to Availability at write time.
- IRoomReader: active status no longer exposed to Reservations for scheduling. Availability consumes room state via push, not via runtime query.

### Reservations

- Slice 1: CreateReservation and ChangeReservationPeriod gain a pre-check via IAvailabilityService. Direct isActive call removed.
- Slice 2: Repository overlap check removed. CreateReservation, ChangeReservationPeriod, and CancelReservation gain claim management calls to Availability (register, replace, release). Claim and reservation writes occur in the same database transaction.
- Reservations → RoomManagement dependency for the reservation path is eliminated.

### New Module: Availability

New persistence tables required:
- `room_availability_projection` — local copy of room existence and active status
- `room_maintenance` — RoomMaintenance aggregate state
- `reservation_claims` (Slice 2) — active reservation claims, used for mutual exclusion

Data migration on deployment: seed all existing rooms from RoomManagement into `room_availability_projection`.

---

## Slice Summary

**Slice 1 (L):** New Availability module established. Maintenance management fully operational (create, modify, cancel, complete). Availability query (room active + maintenance blocks) wired into Reservation handlers. Temporary: maintenance creation conflict check queries Reservations repository directly from the application layer — eliminated in Slice 2 when Availability's own claim store is available.

**Slice 2 (M):** Claim model added to Availability. Reservation overlap check moves from IReservationRepository into Availability's atomic claim registry. Repository writes become plain. All reservation lifecycle operations (create, change period, cancel) integrate with Availability claim management.

---

## Out of Scope

- Maintenance + compensation workflow: staff-initiated room reallocation or guest notification for conflicting reservations
- Modification of Completed or Cancelled maintenance periods
- Overlapping maintenance periods for the same room (rejected)
- Additional block types (owner-use, short-term rental, seasonal closures) — architecture supports them, none implemented here
- Guest-facing interface
- Role-based access control for maintenance operations

---

## Decision Record Summary

See `docs/spec/room-maintenance/decision.md` for the full record.

**Selected model:** Model C — Dedicated Availability Module, two slices.
**Approved by:** Dominik Czerwiński.

**Alternatives rejected:**
- Model A (Minimal): extends IRoomReader with temporal semantics; check-and-act race condition; does not resolve known shortcuts.
- Model B (Incremental): leaves repository overlap shortcut in place; Model C migration required within 1-2 features.

**Trade-offs accepted:** Higher implementation cost; reservation write path restructured in Slice 2; IRoomReader narrowed; Reservations → RoomManagement dependency for reservation path eliminated.

**Deferred:** Availability module extraction to separate service; additional block types; compensation workflow.
