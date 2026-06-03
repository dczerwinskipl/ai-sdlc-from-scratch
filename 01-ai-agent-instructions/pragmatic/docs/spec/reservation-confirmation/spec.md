---
artifact: spec
status: draft
source-of-truth: false
requires-approval: true
---

# Reservation Confirmation — Feature Specification

## Feature Summary

Changes the conflict detection rule so that only **Confirmed** reservations block new reservations and period changes. A **Pending** (unconfirmed) reservation no longer prevents other reservations from being created for the same room and period.

Confirming a reservation introduces two new behaviors: a conflict check against existing Confirmed reservations (confirmation fails if one exists), and a cascade that automatically cancels all other Pending reservations for the same room and overlapping period.

**Module:** `Reservations`
**Extends:** `Reservation` aggregate, `IReservationRepository`, `ConfirmReservationHandler`
**No cross-module contract changes.**

## What it does not do

- Does not change the Pending, Confirmed, or Cancelled status values (they already exist)
- Does not affect `RoomManagement` or any cross-module contract
- Does not notify guests of cascade cancellations
- Does not change the `CancelReservation` use case

---

## Acceptance Criteria

| ID | Criterion | Status |
|---|---|---|
| AC1 | Creating a reservation succeeds when only Pending reservations exist for the room and period. | Confirmed |
| AC2 | Creating a reservation fails when a Confirmed reservation overlaps with the requested period for the same room. | Confirmed |
| AC3 | Confirming a reservation fails when a Confirmed reservation already exists for the same room and overlapping period. | Confirmed |
| AC4 | Confirming a reservation automatically cancels all other Pending reservations for the same room whose period overlaps with the confirmed period. | Confirmed |
| AC5 | Cancelled reservations are not affected by the confirmation cascade. | Confirmed |
| AC6 | Changing the period of a reservation fails only if the new period overlaps with a Confirmed reservation (not with Pending ones). | Confirmed |
