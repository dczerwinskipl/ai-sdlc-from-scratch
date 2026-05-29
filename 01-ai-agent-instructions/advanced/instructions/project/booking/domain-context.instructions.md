# Booking Domain Context

This file contains project-specific facts, known rules, and documented decisions.

Do not add generic DDD guidance here. Generic discovery rules live in `instructions/core/ddd/`.

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

---

## Documented MVP decisions

These are intentional shortcuts taken to ship quickly. They are known trade-offs, not accidents.

### Availability check embedded in repository

Overlap detection lives inside `IReservationRepository.TryAdd` and `TryChangePeriod` rather than in a separate Availability module or service.

**Reason:** Time pressure. We wanted a working implementation fast. A dedicated Availability module was considered but deferred.

**Known limitation:** Availability rules cannot be changed or tested independently of the repository.

### Room active/inactive status used directly

Reservations read room status via `IRoomReader.IsActive` rather than through a dedicated Availability or scheduling layer.

**Reason:** Time pressure. The simplest model that worked for MVP. A richer availability model (time-bounded blocks, maintenance windows) was considered but deferred.

**Known limitation:** There is no way to temporarily block a room without fully deactivating it.

---

## API stability

There are currently no external consumers of this API.

Breaking changes to endpoints, request shapes, and response shapes are acceptable without versioning or backward compatibility obligations.
