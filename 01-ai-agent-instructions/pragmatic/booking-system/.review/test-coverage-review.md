---
artifact: review
angle: test-coverage
status: approved
created: 2026-06-03
updated: 2026-06-03
approved-by: Dominik Czerwiński
feature: booking-system
scope:
  - src/BookingSystem/Modules/Reservations/Domain/Reservation.cs
  - src/BookingSystem/Modules/Reservations/Domain/ReservationPeriod.cs
  - src/BookingSystem/Modules/Reservations/Domain/ReservationGuest.cs
  - src/BookingSystem/Modules/Reservations/UseCases/CancelReservation/CancelReservationHandler.cs
  - src/BookingSystem/Modules/Reservations/UseCases/ChangeReservationPeriod/ChangeReservationPeriodHandler.cs
  - src/BookingSystem/Modules/Reservations/UseCases/ChangeReservationPeriod/ChangeReservationPeriodValidator.cs
  - src/BookingSystem/Modules/Reservations/UseCases/ConfirmReservation/ConfirmReservationHandler.cs
  - src/BookingSystem/Modules/Reservations/UseCases/CreateReservation/CreateReservationHandler.cs
  - src/BookingSystem/Modules/Reservations/UseCases/CreateReservation/CreateReservationValidator.cs
  - src/BookingSystem/Modules/Reservations/UseCases/GetReservation/GetReservationHandler.cs
  - src/BookingSystem/Modules/Reservations/UseCases/ListReservations/ListReservationsHandler.cs
  - src/BookingSystem/Modules/RoomManagement/Domain/Room.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/AddRoom/AddRoomHandler.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/AddRoom/AddRoomValidator.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/DeactivateRoom/DeactivateRoomHandler.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/EditRoom/EditRoomHandler.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/EditRoom/EditRoomValidator.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/GetRoom/GetRoomHandler.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/ListRooms/ListRoomsHandler.cs
  - tests/BookingSystem.Tests/Integration/Modules/Reservations/UseCases/CancelReservation/CancelReservationHandlerTests.cs
  - tests/BookingSystem.Tests/Integration/Modules/Reservations/UseCases/ChangeReservationPeriod/ChangeReservationPeriodHandlerTests.cs
  - tests/BookingSystem.Tests/Integration/Modules/Reservations/UseCases/ConfirmReservation/ConfirmReservationHandlerTests.cs
  - tests/BookingSystem.Tests/Integration/Modules/Reservations/UseCases/CreateReservation/CreateReservationHandlerTests.cs
  - tests/BookingSystem.Tests/Integration/Modules/RoomManagement/UseCases/AddRoom/AddRoomHandlerTests.cs
  - tests/BookingSystem.Tests/Integration/Modules/RoomManagement/UseCases/DeactivateRoom/DeactivateRoomHandlerTests.cs
  - tests/BookingSystem.Tests/Unit/Modules/Reservations/Domain/ReservationPeriodTests.cs
  - tests/BookingSystem.Tests/Unit/Modules/Reservations/Domain/ReservationTests.cs
  - tests/BookingSystem.Tests/Unit/Modules/Reservations/UseCases/CreateReservation/CreateReservationValidatorTests.cs
  - tests/BookingSystem.Tests/Unit/Modules/RoomManagement/UseCases/AddRoom/AddRoomValidatorTests.cs
---

# Test Coverage Review — booking-system

## Summary

| Level | Count |
|---|---|
| CRITICAL | 2 |
| WARNING | 10 |
| INFO | 0 |

---

## Use Case Coverage

| Use Case | Handler | Status | Test File |
|---|---|---|---|
| CreateReservation | CreateReservationHandler | FOUND | CreateReservationHandlerTests.cs |
| ConfirmReservation | ConfirmReservationHandler | FOUND | ConfirmReservationHandlerTests.cs |
| CancelReservation | CancelReservationHandler | FOUND | CancelReservationHandlerTests.cs |
| ChangeReservationPeriod | ChangeReservationPeriodHandler | FOUND | ChangeReservationPeriodHandlerTests.cs |
| GetReservation | GetReservationHandler | MISSING | — |
| ListReservations | ListReservationsHandler | MISSING | — |
| AddRoom | AddRoomHandler | FOUND | AddRoomHandlerTests.cs |
| EditRoom | EditRoomHandler | MISSING | — |
| DeactivateRoom | DeactivateRoomHandler | FOUND | DeactivateRoomHandlerTests.cs |
| GetRoom | GetRoomHandler | MISSING | — |
| ListRooms | ListRoomsHandler | MISSING | — |

---

## Findings

### CRITICAL Handler coverage gaps

**File:** `src/BookingSystem/Modules/Reservations/UseCases/GetReservation/GetReservationHandler.cs` (N/A)
**Details:** GetReservationHandler exists in production code but has no corresponding test file. This query handler retrieves a single reservation by ID and returns null if not found, which is a non-trivial operation that should be tested for correctness.
**Recommendation:** Create GetReservationHandlerTests.cs in tests/BookingSystem.Tests/Integration/Modules/Reservations/UseCases/GetReservation/ with tests for: successful retrieval by ID, null return when reservation does not exist.

### CRITICAL Handler coverage gaps

**File:** `src/BookingSystem/Modules/Reservations/UseCases/ListReservations/ListReservationsHandler.cs` (N/A)
**Details:** ListReservationsHandler exists in production code but has no corresponding test file. This query handler retrieves all reservations and maps them to DTOs. Without tests, correctness of the mapping and empty-list handling cannot be verified.
**Recommendation:** Create ListReservationsHandlerTests.cs in tests/BookingSystem.Tests/Integration/Modules/Reservations/UseCases/ListReservations/ with tests for: empty list, multiple reservations with correct DTO mapping, cancellation status preservation.

### WARNING Handler coverage gaps

**File:** `src/BookingSystem/Modules/RoomManagement/UseCases/EditRoom/EditRoomHandler.cs` (N/A)
**Details:** EditRoomHandler exists in production code but has no corresponding test file. The handler contains two guard conditions: room existence check and inactive-room check. Both guards need test verification.
**Recommendation:** Create EditRoomHandlerTests.cs in tests/BookingSystem.Tests/Integration/Modules/RoomManagement/UseCases/EditRoom/ with tests for: successful edit of active room, error when room not found, error when room is inactive, capacity and name update correctness.

### WARNING Handler coverage gaps

**File:** `src/BookingSystem/Modules/RoomManagement/UseCases/GetRoom/GetRoomHandler.cs` (N/A)
**Details:** GetRoomHandler exists in production code but has no corresponding test file. This query handler retrieves a single room and returns null if not found.
**Recommendation:** Create GetRoomHandlerTests.cs in tests/BookingSystem.Tests/Integration/Modules/RoomManagement/UseCases/GetRoom/ with tests for: successful retrieval, null return when room not found.

### WARNING Handler coverage gaps

**File:** `src/BookingSystem/Modules/RoomManagement/UseCases/ListRooms/ListRoomsHandler.cs` (N/A)
**Details:** ListRoomsHandler exists in production code but has no corresponding test file. This query handler retrieves all rooms and maps them to DTOs.
**Recommendation:** Create ListRoomsHandlerTests.cs in tests/BookingSystem.Tests/Integration/Modules/RoomManagement/UseCases/ListRooms/ with tests for: empty list, multiple rooms with correct DTO mapping, status preservation.

### WARNING Domain guard — Validator coverage

**File:** `src/BookingSystem/Modules/Reservations/UseCases/ChangeReservationPeriod/ChangeReservationPeriodValidator.cs` (line 13)
**Details:** ChangeReservationPeriodValidator checks `command.Start < clock.UtcNow` (must be in the future). The integration test ChangeReservationPeriodHandlerTests.cs does not test the failure case when start is in the past. The unit-level validation is untested.
**Recommendation:** Add a unit test to CreateReservationValidatorTests (or create ChangeReservationPeriodValidatorTests.cs) verifying the "start in past" guard fires correctly.

### WARNING Domain guard — Coverage

**File:** `src/BookingSystem/Modules/Reservations/Domain/ReservationGuest.cs` (line 13)
**Details:** ReservationGuest.Create() throws DomainException if name is empty or whitespace. This domain guard is exercised indirectly in CreateReservationHandlerTests via the validator, but no direct unit test exists for the ReservationGuest domain guard itself.
**Recommendation:** Add a unit test to verify ReservationGuest.Create() with null/empty/whitespace inputs throws DomainException.

### WARNING Validator coverage — EditRoomValidator

**File:** `src/BookingSystem/Modules/RoomManagement/UseCases/EditRoom/EditRoomValidator.cs` (N/A)
**Details:** EditRoomValidator exists but has no corresponding unit test file (AddRoomValidatorTests.cs exists and covers AddRoomValidator, but EditRoomValidator is identical and untested in isolation).
**Recommendation:** Create EditRoomValidatorTests.cs in tests/BookingSystem.Tests/Unit/Modules/RoomManagement/UseCases/EditRoom/ with tests mirroring AddRoomValidatorTests scope (name and capacity validation).

### WARNING Validator coverage — ChangeReservationPeriodValidator

**File:** `src/BookingSystem/Modules/Reservations/UseCases/ChangeReservationPeriod/ChangeReservationPeriodValidator.cs` (N/A)
**Details:** ChangeReservationPeriodValidator exists but has no dedicated unit test file (only indirectly tested in ChangeReservationPeriodHandlerTests.cs integration tests). The "start in past" guard specifically needs isolated validation testing.
**Recommendation:** Create ChangeReservationPeriodValidatorTests.cs in tests/BookingSystem.Tests/Unit/Modules/Reservations/UseCases/ChangeReservationPeriod/ with unit tests for: valid command, start >= end, start in past.

### WARNING Domain guard — Edge case

**File:** `src/BookingSystem/Modules/Reservations/Domain/Reservation.cs` (line 50)
**Details:** ChangePeriod() has a side-effect guard: if status is Confirmed, it reverts to Pending. This behavior is tested in ReservationTests.cs (line 129) but the integration test ChangeReservationPeriodHandlerTests.cs does not verify this status reversion end-to-end through the handler. The integration test only covers Pending reservations.
**Recommendation:** Add an integration test to ChangeReservationPeriodHandlerTests.cs verifying that changing the period of a Confirmed reservation reverts status to Pending.

### WARNING Query handler null-check consistency

**File:** `src/BookingSystem/Modules/Reservations/UseCases/GetReservation/GetReservationHandler.cs` (line 13)
**Details:** GetReservationHandler returns null (nullable response) when reservation is not found. GetRoomHandler uses the same pattern. These handlers have no tests to verify null handling, while command handlers have NotFoundError patterns. The null return vs error pattern inconsistency is untested.
**Recommendation:** When creating query handler tests, document and verify the null-return semantics explicitly.

---

## Guard Coverage Summary

**Reservation.Confirm() guards:**
- Status != Pending: ✓ TESTED (ConfirmReservationHandlerTests, ReservationTests)

**Reservation.Cancel() guards:**
- Cannot cancel already-cancelled: ✓ TESTED (CancelReservationHandlerTests, ReservationTests)

**Reservation.ChangePeriod() guards:**
- Cannot change cancelled reservation: ✓ TESTED (ChangeReservationPeriodHandlerTests, ReservationTests)
- Status reversion (Confirmed → Pending): ✓ TESTED at domain level (ReservationTests), ⚠ NOT TESTED at integration level

**ReservationPeriod.Create() guards:**
- Start >= End: ✓ TESTED (ReservationPeriodTests)

**ReservationPeriod.Overlaps() logic:**
- ✓ TESTED (ReservationPeriodTests covers all overlap scenarios and adjacent periods)

**ReservationGuest.Create() guards:**
- Empty name: ⚠ TESTED indirectly via validator, NOT TESTED at domain level directly

**CreateReservationValidator guards:**
- Guest name empty: ✓ TESTED (CreateReservationValidatorTests)
- Start >= End: ✓ TESTED (CreateReservationValidatorTests)
- Start in past: ✓ TESTED (CreateReservationValidatorTests)

**ChangeReservationPeriodValidator guards:**
- Start >= End: ✓ TESTED (integration test ChangeReservationPeriodHandlerTests)
- Start in past: ⚠ NOT TESTED in unit-level isolation

**CreateReservationHandler business rules:**
- Room not found: ✓ TESTED
- Room inactive: ✓ TESTED
- Period overlap: ✓ TESTED
- Period adjacency (allowed): ✓ TESTED

**ChangeReservationPeriodHandler business rules:**
- Reservation not found: ✓ TESTED
- Room not found: ⚠ NOT TESTED (no handler test file)
- Room inactive: ✓ TESTED
- Period overlap: ✓ TESTED
- Period adjacency in bounds: ✓ TESTED (implicit in overlap tests)

**AddRoomValidator guards:**
- Empty name: ✓ TESTED (AddRoomValidatorTests)
- Capacity <= 0: ✓ TESTED (AddRoomValidatorTests)

**EditRoomValidator guards:**
- Empty name: ⚠ NOT TESTED in isolation (same as AddRoom but no test file)
- Capacity <= 0: ⚠ NOT TESTED in isolation

**EditRoomHandler guards:**
- Room not found: ⚠ NOT TESTED (no test file)
- Room inactive: ⚠ NOT TESTED (no test file)

**Room.CanBeReserved() logic:**
- Returns IsActive status: ⚠ NOT DIRECTLY TESTED (verified indirectly in CreateReservation tests via room status checks)

---

## Test File Coverage Matrix

| Test File | Handler Covered | Tests | Domain Guards Checked |
|---|---|---|---|
| CreateReservationHandlerTests.cs | CreateReservationHandler | 7 | Room existence, room active, period overlap, period adjacency, guest name validation, period validation |
| ConfirmReservationHandlerTests.cs | ConfirmReservationHandler | 4 | Status != Pending, already confirmed, already cancelled |
| CancelReservationHandlerTests.cs | CancelReservationHandler | 4 | Pending→Cancelled, Confirmed→Cancelled, already cancelled, not found |
| ChangeReservationPeriodHandlerTests.cs | ChangeReservationPeriodHandler | 8 | Pending update, overlap, own-slot overlap, cancelled, not found, start >= end, room inactive |
| AddRoomHandlerTests.cs | AddRoomHandler | 3 | Valid creation, active status, name/capacity validation |
| DeactivateRoomHandlerTests.cs | DeactivateRoomHandler | 3 | Active→Inactive, store retention, not found |
| ReservationPeriodTests.cs | ReservationPeriod domain | 9 | Start < End creation, equal dates, after dates, full overlap, partial overlap, adjacent (false), no overlap |
| ReservationTests.cs | Reservation domain | 10 | Confirm pending, confirm already-confirmed, confirm cancelled, cancel pending, cancel confirmed, cancel already-cancelled, change pending, change confirmed (revert), change cancelled |
| CreateReservationValidatorTests.cs | CreateReservationValidator | 5 | Valid command, empty guest name, start > end, start = end, start in past |
| AddRoomValidatorTests.cs | AddRoomValidator | 3 | Valid command, empty name, non-positive capacity |

