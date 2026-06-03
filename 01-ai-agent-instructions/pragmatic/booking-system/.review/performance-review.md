---
artifact: review
angle: performance
status: approved
created: 2026-06-03
updated: 2026-06-03
approved-by: Dominik Czerwiński
feature: booking-system
scope:
  - src/BookingSystem/Modules/Reservations/Infrastructure/InMemoryReservationStore.cs
  - src/BookingSystem/Modules/Reservations/Infrastructure/InMemoryReservationRepository.cs
  - src/BookingSystem/Modules/Reservations/UseCases/CreateReservation/CreateReservationHandler.cs
  - src/BookingSystem/Modules/Reservations/UseCases/ChangeReservationPeriod/ChangeReservationPeriodHandler.cs
  - src/BookingSystem/Modules/Reservations/UseCases/ConfirmReservation/ConfirmReservationHandler.cs
  - src/BookingSystem/Modules/Reservations/UseCases/CancelReservation/CancelReservationHandler.cs
  - src/BookingSystem/Modules/Reservations/UseCases/GetReservation/GetReservationHandler.cs
  - src/BookingSystem/Modules/Reservations/UseCases/ListReservations/ListReservationsHandler.cs
  - src/BookingSystem/Modules/RoomManagement/Infrastructure/InMemoryRoomStore.cs
  - src/BookingSystem/Modules/RoomManagement/Infrastructure/InMemoryRoomRepository.cs
  - src/BookingSystem/Modules/RoomManagement/Infrastructure/InMemoryRoomReader.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/AddRoom/AddRoomHandler.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/GetRoom/GetRoomHandler.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/ListRooms/ListRoomsHandler.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/EditRoom/EditRoomHandler.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/DeactivateRoom/DeactivateRoomHandler.cs
  - src/BookingSystem/Modules/Reservations/Domain/Reservation.cs
  - src/BookingSystem/Modules/Reservations/Domain/ReservationPeriod.cs
  - src/BookingSystem/Modules/RoomManagement/Domain/Room.cs
  - src/BookingSystem/Modules/Reservations/UseCases/Abstractions/IReservationRepository.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/Abstractions/IRoomRepository.cs
---

# Performance Review — booking-system

## Summary

| Level | Count |
|---|---|
| CRITICAL | 0 |
| WARNING | 5 |
| INFO | 3 |

## Findings

### [WARNING] Full Collection Scan on Reservation Conflict Detection

**File:** `src/BookingSystem/Modules/Reservations/Infrastructure/InMemoryReservationRepository.cs` (lines 25-28, 40-44)

**Details:** The `TryAdd()` and `TryChangePeriod()` methods perform an O(n) scan over all reservations to detect period overlaps. In `TryAdd()`, every call scans the entire reservation list to find conflicts. The overlap detection uses `.Any()` which terminates early, but still requires iteration through active reservations for the target room. With many reservations across multiple rooms, this becomes increasingly expensive. Each period-change operation suffers the same O(n) scan.

**Recommendation:** For production use with large reservation volumes, consider adding a secondary index (e.g., a dictionary keyed by RoomId pointing to lists of reservations for that room). This would reduce the scan from O(all reservations) to O(reservations for that room). The current implementation is acceptable for low-to-moderate data volumes typical in a modular monolith demo.

---

### [WARNING] Linear Search in Repository Update Operations

**File:** `src/BookingSystem/Modules/Reservations/Infrastructure/InMemoryReservationRepository.cs` (line 58)

**Details:** The `Update()` method calls `FindIndex()` on the list, which performs a linear scan to locate the reservation by ID. For each update operation (Cancel, Confirm, ChangeReservationPeriod), this introduces an O(n) lookup cost. In high-frequency update scenarios, this compounds.

**Recommendation:** If update frequency is high, consider maintaining a secondary dictionary-based index keyed by ReservationId for O(1) lookup. For typical usage patterns in a booking system, this is acceptable. Monitor if update operations become a bottleneck.

---

### [WARNING] Incomplete Update Implementation in Room Repository

**File:** `src/BookingSystem/Modules/RoomManagement/Infrastructure/InMemoryRoomRepository.cs` (lines 26-29)

**Details:** The `Update()` method is empty—it returns immediately without performing any update. This means calls to `EditRoom` and `DeactivateRoom` handlers invoke `repository.Update()` but changes are never persisted. This is a functional correctness issue, not strictly performance, but it prevents proper testing of update performance characteristics.

**Recommendation:** Implement the Update method using the same pattern as the reservation repository: find the room by ID via linear search and update in-place. Once implemented, monitor update operation frequency and consider the same secondary indexing strategy recommended above if needed.

---

### [WARNING] No Pagination in List Operations

**File:** `src/BookingSystem/Modules/Reservations/UseCases/ListReservations/ListReservationsHandler.cs` (lines 11-20) and `src/BookingSystem/Modules/RoomManagement/UseCases/ListRooms/ListRoomsHandler.cs` (lines 11-14)

**Details:** Both `ListReservationsHandler` and `ListRoomsHandler` call `GetAll()` and return every item in the collection. If the in-memory store grows to thousands of reservations or rooms, the entire collection is loaded, mapped, and returned. This causes unnecessary memory allocation and serialization overhead. There is no query parameter support for pagination, filtering, or limiting result size.

**Recommendation:** For a production system, implement pagination support: add `Skip` and `Take` parameters to the query commands and repository methods. This allows clients to fetch 20–50 items per request rather than the entire collection. Consider also adding optional filters (by room ID, date range, status) to the repository methods to support efficient filtering before full-collection return.

---

### [WARNING] Extended Lock Hold Duration During Full Collection Operations

**File:** `src/BookingSystem/Modules/Reservations/Infrastructure/InMemoryReservationStore.cs` (lines 10-24) and `src/BookingSystem/Modules/RoomManagement/Infrastructure/InMemoryRoomStore.cs` (lines 10-24)

**Details:** The store's `Execute()` methods hold a lock for the entire duration of the lambda function. When `GetAll()` is called, the lock is held while the entire list is copied and potentially transformed. In a multi-threaded scenario with many concurrent requests, this centralized lock becomes a bottleneck. All read and write operations serialize through a single lock, even though the read operations (like `GetAll()`) do not modify state.

**Recommendation:** For improved concurrency, consider using `ReaderWriterLockSlim` instead of a plain `lock`, which allows multiple concurrent readers but exclusive access for writers. This is a no-cost improvement for the demo but becomes important at higher request volumes. Alternatively, use immutable collection types (e.g., `ImmutableList<T>`) to enable lock-free reads—at the cost of higher allocation overhead on writes.

---

### [INFO] Overlap Detection Logic is Correct but Could Be Optimized

**File:** `src/BookingSystem/Modules/Reservations/Domain/ReservationPeriod.cs` (line 23-24)

**Details:** The `Overlaps()` method correctly implements temporal overlap detection: `Start < other.End && other.Start < End`. This is the standard half-open interval comparison and is not a performance concern. However, it is called inside a repository method that scans all reservations, multiplying the total comparison count by the number of reservations.

**Recommendation:** No change needed to the logic itself. Once a secondary room-based index is added (as recommended above), this overlap check will operate on a smaller subset of reservations, improving overall performance indirectly.

---

### [INFO] GetAll() Creates a List Copy

**File:** `src/BookingSystem/Modules/Reservations/Infrastructure/InMemoryReservationRepository.cs` (line 17) and `src/BookingSystem/Modules/RoomManagement/Infrastructure/InMemoryRoomRepository.cs` (line 16)

**Details:** Both `GetAll()` methods call `.ToList()` to create a copy of the in-memory list before returning. This is a defensive copy that prevents external code from modifying the internal store. The copy is necessary for thread safety and encapsulation. However, it means every list operation allocates a new collection object proportional to the number of items.

**Recommendation:** This is acceptable and necessary for safety. No change recommended. The allocation is a fair trade for encapsulation and correctness.

---

### [INFO] Query Handlers Make Appropriate Repository Calls

**File:** `src/BookingSystem/Modules/Reservations/UseCases/GetReservation/GetReservationHandler.cs`, `src/BookingSystem/Modules/RoomManagement/UseCases/GetRoom/GetRoomHandler.cs`, and single-item command handlers

**Details:** Single-item queries and commands (GetReservation, GetRoom, CancelReservation, ConfirmReservation, EditRoom, DeactivateRoom) all call `GetById()`, which performs a single linear search. No N+1 pattern exists here because each handler makes exactly one repository call, not one per item in a collection. The pattern is correct.

**Recommendation:** No change needed. Continue this pattern for all single-item operations.

