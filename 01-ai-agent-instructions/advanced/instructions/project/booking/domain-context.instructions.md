# Booking Domain Context

This project models a booking or reservation system.

This file is project-specific. Do not copy these rules into generic DDD instructions.

## Known modules

### Reservations (implemented)

Owns reservation intent and reservation lifecycle.

Aggregate: `Reservation`

Known state transitions:
- `Pending → Confirmed` via ConfirmReservation
- `Pending → Cancelled` via CancelReservation
- `Confirmed → Cancelled` via CancelReservation
- `Confirmed → Pending` when the reservation period is changed (confirmation is invalidated)

Known rules:
- A reservation can only be created for an active room.
- A reservation period may not overlap with any active reservation for the same room.
- A cancelled reservation's period cannot be changed.
- Changing the period of a confirmed reservation reverts its status to Pending.
- Start date must be in the future at the time of creation or period change.

Cross-module dependency: reads room existence and active status from RoomManagement via `IRoomReader` / `RoomInfo`. References room identity only — does not copy room descriptive data.

### RoomManagement (implemented)

Owns room descriptive data and room operational status.

Aggregate: `Room`

Known states: `Active`, `Inactive`

Known rules:
- An inactive room cannot accept new reservations or period changes.
- An inactive room cannot be edited.
- Deactivation is a one-way operation (no reactivation in current implementation).

Public contract exposed to other modules: `IRoomReader` returning `RoomInfo` (id, name, capacity, isActive).

Note: this module currently merges what future design might separate into Catalogue (descriptive data) and Availability (operational status). Do not silently split it without an architecture decision.

## Not yet implemented

The following domain areas are known but not implemented:

- **Maintenance**: time-bounded operational blocks that make a room unavailable without deactivating it. Not the same as `Inactive` status, which is permanent.
- **Notifications**: communication triggered by reservation lifecycle events (creation, confirmation, cancellation, period change).

When a feature requirement touches maintenance or notifications, flag it as an open question and do not model it as part of Reservations or RoomManagement without an explicit architecture decision.

## Known behavior implications

When a feature affects room availability, the spec must explicitly check whether it also affects:

- reservation lifecycle
- existing reservations
- the room active/inactive status
- the `IRoomReader` public contract
- search or availability queries

When a feature affects reservation lifecycle, the spec must explicitly define state transition rules.

When a feature uses room data, the spec must distinguish:

- room identity referenced by Reservations (`ReservableRoomId`)
- descriptive data owned by RoomManagement (`RoomName`, `RoomCapacity`)
- operational status owned by RoomManagement (`RoomStatus.Active / Inactive`)

## Default modeling bias

Prefer referencing external concepts by identity across module boundaries.

Do not copy room descriptive data into Reservations unless the spec explicitly requires snapshotting at booking time.

Do not treat room deactivation as equivalent to a maintenance block. Deactivation is a permanent operational decision; maintenance is a time-bounded availability constraint.

Do not split RoomManagement into separate Catalogue and Availability modules without an explicit architecture decision. The current merged model is intentional at this stage.

## Project-specific discovery prompts

When requirements mention a room, booking, reservation, block, maintenance, or availability, ask:

- What is the affected resource?
- Is this a reservation intent or an availability constraint?
- Does the change block new reservations only, or does it also affect existing ones?
- Does it require a state transition in Reservation?
- Does it affect the `IRoomReader` public contract?
- Does it require notification?
- Who should receive the notification?
- Is the behavior visible to guests, staff, both, or neither?
- Does it imply a new actor, guest, owner, payer, or contact distinction?
