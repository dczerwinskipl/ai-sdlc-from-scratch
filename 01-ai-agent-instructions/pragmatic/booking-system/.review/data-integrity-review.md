---
artifact: review
angle: data-integrity
status: approved
created: 2026-06-03
updated: 2026-06-03
approved-by: Dominik Czerwiński
feature: booking-system
scope:
  - src/BookingSystem/Modules/Reservations/Infrastructure/InMemoryReservationRepository.cs
  - src/BookingSystem/Modules/Reservations/Infrastructure/InMemoryReservationStore.cs
  - src/BookingSystem/Modules/Reservations/UseCases/CancelReservation/CancelReservationHandler.cs
  - src/BookingSystem/Modules/Reservations/UseCases/ChangeReservationPeriod/ChangeReservationPeriodHandler.cs
  - src/BookingSystem/Modules/Reservations/UseCases/ConfirmReservation/ConfirmReservationHandler.cs
  - src/BookingSystem/Modules/Reservations/UseCases/CreateReservation/CreateReservationHandler.cs
  - src/BookingSystem/Modules/Reservations/UseCases/Abstractions/IReservationRepository.cs
  - src/BookingSystem/Modules/RoomManagement/Infrastructure/InMemoryRoomReader.cs
  - src/BookingSystem/Modules/RoomManagement/Infrastructure/InMemoryRoomRepository.cs
  - src/BookingSystem/Modules/RoomManagement/Infrastructure/InMemoryRoomStore.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/AddRoom/AddRoomHandler.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/DeactivateRoom/DeactivateRoomHandler.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/EditRoom/EditRoomHandler.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/Abstractions/IRoomRepository.cs
  - src/BookingSystem/Modules/Reservations/Domain/Reservation.cs
  - src/BookingSystem/Modules/Reservations/Domain/ReservationPeriod.cs
  - src/BookingSystem/Modules/RoomManagement/Domain/Room.cs
  - src/BookingSystem/Modules/RoomManagement/Domain/RoomStatus.cs
  - src/BookingSystem/BuildingBlocks/Domain/AggregateRoot.cs
  - src/BookingSystem/BuildingBlocks/Domain/Result.cs
---

# Data Integrity Review — booking-system

## Summary

| Level | Count |
|---|---|
| CRITICAL | 2 |
| WARNING | 3 |
| INFO | 2 |

## Findings

### [CRITICAL] Cross-store room availability check is not atomic with reservation creation

**File:** `src/BookingSystem/Modules/Reservations/UseCases/CreateReservation/CreateReservationHandler.cs` (line 21-38)

**Details:** `CreateReservationHandler.Handle` reads room availability from `IRoomReader` (backed by `InMemoryRoomStore`) and then, in a separate call, writes the reservation to `IReservationRepository` (backed by `InMemoryReservationStore`). The two stores are protected by independent locks. Between the `roomReader.GetById` call returning `IsActive = true` and `repository.TryAdd` executing, a concurrent `DeactivateRoomHandler` can deactivate the room. The reservation will then be created for a room that is already inactive — a state invariant violation. The same race exists in `ChangeReservationPeriodHandler` (lines 25-34).

**Recommendation:** Either (a) introduce a single shared lock or a combined unit-of-work object that the availability check and the reservation write both acquire together, or (b) re-validate room status inside the same `InMemoryReservationStore` lock during `TryAdd`, by injecting a room-status snapshot into that call. A simpler short-term fix is to accept this as a known limitation of the in-memory prototype and document it explicitly before any real persistence layer is introduced.

---

### [CRITICAL] `InMemoryRoomRepository.Update` is a no-op — mutations are silently lost on deactivate and edit

**File:** `src/BookingSystem/Modules/RoomManagement/Infrastructure/InMemoryRoomRepository.cs` (line 27-30)

**Details:** `Update` returns `Task.CompletedTask` without writing anything to the store. Both `DeactivateRoomHandler` and `EditRoomHandler` call `repository.Update(room, ...)` to persist mutations applied to the in-memory `Room` object. Because `Room` is a reference type stored directly in the `List<Room>` inside `InMemoryRoomStore`, mutations to the object happen to be visible through the shared reference at runtime — but this is an accidental side-effect of reference semantics, not an intentional design. Any future change that snapshots or replaces the object (e.g., value-type records, EF Core tracking, a real DB) will silently discard all writes. The method contract (`Update`) implies persistence, but the implementation violates it. This creates a latent data-loss defect.

**Recommendation:** Either implement `Update` to explicitly replace the item in the list (as `InMemoryReservationRepository.Update` already does at line 58-61), or document prominently that the in-memory Room store relies on reference identity and that `Update` must be implemented before any persistence migration.

---

### [WARNING] `ConfirmReservationHandler` and `CancelReservationHandler` mutate domain object before `Update` — exception between mutation and persist leaves store inconsistent

**File:** `src/BookingSystem/Modules/Reservations/UseCases/ConfirmReservation/ConfirmReservationHandler.cs` (line 17-20) and `src/BookingSystem/Modules/Reservations/UseCases/CancelReservation/CancelReservationHandler.cs` (line 17-20)

**Details:** Both handlers call `reservation.Confirm()` / `reservation.Cancel()` (which mutates `Status` on the domain object in memory) and then call `repository.Update(reservation, ...)`. Because the reservation lives in the store as a reference-typed object inside the `List<Reservation>`, the status mutation is already visible to all concurrent readers before `Update` is called. If `repository.Update` were to throw (e.g., index not found in a future implementation), the domain object is left in its mutated state with no rollback path. Currently `Update` cannot fail, but the pattern is fragile and will become a real bug the moment `Update` does real I/O.

**Recommendation:** Apply the mutation only after confirming the write succeeded, or perform the mutation inside the store lock (as `TryChangePeriod` correctly does — it calls `reservation.ChangePeriod` inside the `store.Execute` lambda, ensuring atomicity). Handlers for Confirm and Cancel should follow the same pattern.

---

### [WARNING] No compensation or rollback mechanism for any partial-write path

**File:** N/A (cross-cutting)

**Details:** No handler contains any compensation logic for the case where a write fails partway through. While the current in-memory implementation has no I/O that can fail, the handlers are structured as if they will be backed by a real database later (the `IReservationRepository` and `IRoomRepository` interfaces return `Task`, implying async I/O). When persistence is introduced, any handler that performs two sequential writes (e.g., a future handler that writes a reservation and raises an integration event) has no rollback path. There are no rollback-on-failure comments and no `try/catch` compensation blocks anywhere in the handler layer.

**Recommendation:** Before introducing real persistence, add compensation stubs to any handler that will perform multiple writes, with comments marking them as dormant until real I/O is in place. This makes the obligation visible during future migration and prevents silent data divergence.

---

### [WARNING] `ChangeReservationPeriod` — room deactivation check and conflict check operate on separate stores without a shared lock

**File:** `src/BookingSystem/Modules/Reservations/UseCases/ChangeReservationPeriod/ChangeReservationPeriodHandler.cs` (line 25-34)

**Details:** The handler reads room status from `InMemoryRoomStore` and performs the overlap check + period mutation inside `InMemoryReservationStore`. These are two separate locks. A concurrent deactivation of the room between lines 25 and 34 will not be detected, allowing a period change on a room that has just been deactivated. This is the same cross-store race described in CRITICAL finding 1 but specific to the period-change path.

**Recommendation:** Same as the CRITICAL finding above — consolidate the room-status check into the reservation store's lock scope, or document as a known limitation of the prototype.

---

### [INFO] `InMemoryReservationStore` and `InMemoryRoomStore` use a plain `object` lock — correct but not scalable

**File:** `src/BookingSystem/Modules/Reservations/Infrastructure/InMemoryReservationStore.cs` (line 7) and `src/BookingSystem/Modules/RoomManagement/Infrastructure/InMemoryRoomStore.cs` (line 7)

**Details:** Both stores serialise all reads and writes through a single `lock (_lock)`. This is correct for correctness under the current load but will become a throughput bottleneck if the in-memory store is used under any meaningful concurrency. More importantly, it means every `Execute` call holds the lock for the entire duration of the supplied lambda, including any domain logic inside it (e.g., `ChangePeriod` is called inside the lock in `TryChangePeriod`). If a future lambda performs I/O inside the lock, the lock will be held across an async wait — a deadlock risk.

**Recommendation:** Ensure no async operations are ever placed inside `Execute` lambdas. Add an XML doc comment on `InMemoryReservationStore.Execute` and `InMemoryRoomStore.Execute` warning that the lambda must be synchronous and non-blocking.

---

### [INFO] `Reservation.Cancel()` does not guard against cancelling a confirmed reservation being cancelled re-entering a conflict window

**File:** `src/BookingSystem/Modules/Reservations/Domain/Reservation.cs` (line 43-47)

**Details:** `Cancel()` allows cancelling a reservation in any non-cancelled state, including `Confirmed`. This is likely intentional business logic. However, there is no domain event raised on cancellation, which means no downstream subscriber (e.g., a future room-availability projection) will be notified when a confirmed reservation is freed. This is not a current defect — there are no subscribers — but it is a gap that will matter when the system grows.

**Recommendation:** When domain events are wired up, ensure `Cancel()` and `Confirm()` both raise appropriate domain events so downstream projections can react. No action needed now.
