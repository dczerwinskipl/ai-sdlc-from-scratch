---
artifact: implementation-plan
status: draft
source-of-truth: false
requires-approval: false
---

# Reservation Confirmation — Implementation Plan

## Tasks

### T1 — Fix conflict check in `TryAdd`
**Location:** `Modules/Reservations/Infrastructure/InMemoryReservationRepository.cs` — `TryAdd()`
Change the conflict predicate from `r.IsActive()` to `r.Status == ReservationStatus.Confirmed`.
Only Confirmed reservations block creation. Pending reservations are ignored.

### T2 — Fix conflict check in `TryChangePeriod`
**Location:** `Modules/Reservations/Infrastructure/InMemoryReservationRepository.cs` — `TryChangePeriod()`
Same change as T1: replace `r.IsActive()` with `r.Status == ReservationStatus.Confirmed` in the conflict predicate.
*Depends on T1 (same file, same pattern).*

### T3 — Extend `IReservationRepository` with overlap query
**Location:** `Modules/Reservations/UseCases/Abstractions/IReservationRepository.cs`
Add method: `Task<IReadOnlyList<Reservation>> GetPendingOverlapping(ReservableRoomId roomId, ReservationPeriod period, ReservationId excludeId, CancellationToken cancellationToken)`
Returns all Pending reservations for the given room whose period overlaps, excluding the reservation being confirmed.
*Required by T4.*

### T4 — Implement `GetPendingOverlapping` in repository
**Location:** `Modules/Reservations/Infrastructure/InMemoryReservationRepository.cs`
Implement the method added in T3. Filter: `Status == Pending`, `RoomId == roomId`, `Id != excludeId`, `Period.Overlaps(period)`.
*Depends on T3.*

### T5 — Update `ConfirmReservationHandler`
**Location:** `Modules/Reservations/UseCases/ConfirmReservation/ConfirmReservationHandler.cs`
After loading the reservation and calling `reservation.Confirm()`:
1. Check for Confirmed conflicts: call `repository.GetAll()` and filter for `Status == Confirmed`, same room, overlapping period, excluding self. If found → return `ConflictError`.
2. Save the confirmed reservation via `repository.Update()`.
3. Fetch overlapping Pending reservations via `repository.GetPendingOverlapping()`.
4. For each: call `r.Cancel()` and `repository.Update(r)`.
*Depends on T3, T4.*

---

## Test Plan

| ID | What | Type | AC | Notes |
|---|---|---|---|---|
| TS1 | Create reservation when only Pending exists for period → success | Unit | AC1 | Seed one Pending reservation; assert TryAdd returns true. |
| TS2 | Create reservation when Confirmed exists for period → ConflictError | Unit | AC2 | Seed one Confirmed reservation; assert TryAdd returns false. |
| TS3 | Confirm reservation when Confirmed conflict exists → ConflictError | Integration | AC3 | Seed two reservations for same room/period; confirm first; attempt confirm second → error. |
| TS4 | Confirm reservation → other Pending reservations are cancelled | Integration | AC4 | Seed three Pending reservations for same room/period; confirm one; assert other two are Cancelled. |
| TS5 | Confirm reservation → Cancelled reservations remain Cancelled | Unit | AC5 | Seed one Confirmed and one Cancelled for same room/period; confirm → Cancelled unchanged. |
| TS6 | Change period to overlap with Pending only → success | Unit | AC6 | Seed one Pending; TryChangePeriod to overlapping period → succeeds. |
| TS7 | Change period to overlap with Confirmed → ConflictError | Unit | AC6 | Seed one Confirmed; TryChangePeriod to overlapping period → ConflictError. |
