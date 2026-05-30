<!-- Type: project-context -->

# Booking Domain Context

This file contains project-specific domain facts and known rules.

Do not add generic DDD guidance here. Generic discovery rules live in `instructions/core/ddd/`.

For intentional shortcuts, accepted trade-offs, and API stability decisions, see `instructions/project/booking/known-decisions.instructions.md`.

---

## Module: Reservations

Owns reservation intent and reservation lifecycle.

Aggregate: `Reservation`

State transitions:
- `Pending → Confirmed` via ConfirmReservation
- `Pending → Cancelled` via CancelReservation
- `Confirmed → Cancelled` via CancelReservation
- `Confirmed → Pending` when the reservation period is changed

Known rules:
- A reservation can only be created for an active room.
- A reservation period may not overlap with any active reservation for the same room.
- A cancelled reservation's period cannot be changed.
- Changing the period of a confirmed reservation reverts its status to Pending.
- Start date must be in the future at the time of creation or period change.

Cross-module dependency: reads room existence and active status from RoomManagement via `IRoomReader` / `RoomInfo`. References room identity only (`ReservableRoomId`) — does not copy room descriptive data.

---

## Module: RoomManagement

Owns room descriptive data (`RoomName`, `RoomCapacity`) and room operational status (`RoomStatus`).

Aggregate: `Room`

Known states: `Active`, `Inactive`

Known rules:
- An inactive room cannot accept new reservations or period changes.
- An inactive room cannot be edited.
- Deactivation is a one-way operation (no reactivation in current implementation).

Public contract exposed to other modules: `IRoomReader` returning `RoomInfo` (id, name, capacity, isActive).
