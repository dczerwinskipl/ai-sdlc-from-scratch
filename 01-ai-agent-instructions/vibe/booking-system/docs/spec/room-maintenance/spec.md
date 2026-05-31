# Room Maintenance

## Overview

Rooms can be scheduled for maintenance over a specific period (single date or date range). During that period the room is blocked for new reservations. Outside the period it remains bookable as normal. Maintenance is not permanent — it differs from the existing `Deactivate` use case which takes a room offline indefinitely.

---

## Domain Model

### `RoomMaintenance` entity (child of `Room` aggregate)

| Field | Type | Notes |
|---|---|---|
| `Id` | `MaintenanceId` (Guid wrapper) | |
| `PlannedPeriod` | `MaintenancePeriod` | start < end, end is inclusive day boundary |
| `Status` | `MaintenanceStatus` | `Planned`, `Completed`, `Cancelled` |
| `ActualCompletedAt` | `DateTimeOffset?` | Set when completed early; null otherwise |

`MaintenancePeriod` is a value object with the same `Overlaps` semantics as `ReservationPeriod` (half-open interval: `Start < other.End && other.Start < End`).

### `MaintenanceStatus` transitions

```
Planned → Completed
Planned → Cancelled
```

`Completed` and `Cancelled` are terminal; no further transitions are allowed.

### Effective blocking period

The period during which a maintenance record blocks reservations depends on its status:

| Status | Blocks reservations for |
|---|---|
| `Planned` | Full `PlannedPeriod` |
| `Completed` | `PlannedPeriod.Start` → `ActualCompletedAt` (which may be earlier than planned end) |
| `Cancelled` | No blocking |

### Room aggregate changes

`Room` gains a private collection of `RoomMaintenance` entities and the following methods:

```csharp
Result<MaintenanceId> ScheduleMaintenance(MaintenanceId id, MaintenancePeriod period);
Result CompleteMaintenance(MaintenanceId id, DateTimeOffset actualCompletedAt);
Result CancelMaintenance(MaintenanceId id);
bool HasMaintenanceConflict(DateTimeOffset start, DateTimeOffset end);
```

`ScheduleMaintenance` rejects scheduling if the proposed period overlaps an existing `Planned` maintenance on the same room (prevents double-scheduling for the same window).

`HasMaintenanceConflict` computes the effective blocking period for each non-cancelled maintenance entry and returns `true` if any overlaps `[start, end)`.

---

## Cross-Module Contract

The `Reservations` module must check maintenance before accepting a reservation. This check is added to `IRoomReader` in `RoomManagement/PublicContracts/`:

```csharp
Task<bool> HasMaintenanceConflict(
    Guid roomId,
    DateTimeOffset start,
    DateTimeOffset end,
    CancellationToken cancellationToken);
```

`RoomInfo` is **not** changed — the Reservations module does not need to know maintenance details, only whether a given period conflicts.

`InMemoryRoomReader` implements this by loading the room from the store and delegating to `Room.HasMaintenanceConflict`.

---

## Use Cases

All use cases live in `RoomManagement/UseCases/`.

### ScheduleMaintenance

**Command**: `ScheduleMaintenanceCommand(RoomId, Start, End)`  
**Endpoint**: `POST /api/rooms/{roomId}/maintenance`  
**Request body**: `{ "start": "...", "end": "..." }`  
**Response**: `201 Created` with `{ "maintenanceId": "..." }`

Validation rules (in `ScheduleMaintenanceValidator`):
- Room must exist → `404`
- Room must be active (deactivated rooms cannot have new maintenance scheduled) → `409`
- `Start < End` → `400`
- Start must not be in the past → `400`

Domain rule enforced in aggregate:
- New period must not overlap an existing `Planned` maintenance on this room → `409`

### CompleteMaintenance

**Command**: `CompleteMaintenanceCommand(RoomId, MaintenanceId, ActualCompletedAt?)`  
**Endpoint**: `PUT /api/rooms/{roomId}/maintenance/{maintenanceId}/complete`  
**Request body**: `{ "actualCompletedAt": "..." }` (optional; defaults to `IClock.UtcNow`)

If `ActualCompletedAt` is after `PlannedPeriod.End`, it is clamped to `PlannedPeriod.End` (completing after the planned end is the same as completing on time).

Errors:
- Room or maintenance not found → `404`
- Maintenance not in `Planned` status → `409`

### CancelMaintenance

**Command**: `CancelMaintenanceCommand(RoomId, MaintenanceId)`  
**Endpoint**: `PUT /api/rooms/{roomId}/maintenance/{maintenanceId}/cancel`  
**Request body**: none

Errors:
- Room or maintenance not found → `404`
- Maintenance not in `Planned` status → `409`

### ListMaintenance

**Query**: `ListMaintenanceQuery(RoomId)`  
**Endpoint**: `GET /api/rooms/{roomId}/maintenance`  
**Response**: list of maintenance records including status and periods

---

## Impact on Existing Use Cases

### `CreateReservation`

After the existing `room.IsActive` check, add:

```csharp
var hasConflict = await roomReader.HasMaintenanceConflict(
    command.RoomId, command.Start, command.End, cancellationToken);
if (hasConflict)
    return new ConflictError("Room is under maintenance during the requested period.");
```

### `ChangeReservationPeriod`

Same check added after loading the reservation and before calling `repository.TryChangePeriod`.

---

## Infrastructure

### `IRoomRepository` changes

Add methods mirroring the new aggregate operations:

```csharp
Task<Result<MaintenanceId>> ScheduleMaintenance(RoomId roomId, MaintenanceId id, MaintenancePeriod period, CancellationToken ct);
Task<Result> CompleteMaintenance(RoomId roomId, MaintenanceId id, DateTimeOffset actualCompletedAt, CancellationToken ct);
Task<Result> CancelMaintenance(RoomId roomId, MaintenanceId id, CancellationToken ct);
Task<IReadOnlyList<RoomMaintenance>> GetMaintenance(RoomId roomId, CancellationToken ct);
```

`InMemoryRoomRepository` implements these by loading the room from the store, calling the aggregate method, then writing back (the store's `Execute` lock covers the read-modify-write atomically).

---

## Out of Scope

- Notifying guests with existing reservations that overlap a newly scheduled maintenance (no notification system exists).
- Automatically cancelling overlapping reservations when maintenance is scheduled.
- Recurring maintenance schedules.
