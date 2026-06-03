---
artifact: review
angle: architecture
status: approved
created: 2026-06-03
updated: 2026-06-03
approved-by: Dominik Czerwiński
feature: booking-system
scope:
  - src/BookingSystem/Program.cs
  - src/BookingSystem/BuildingBlocks/Api/EndpointRouteBuilderExtensions.cs
  - src/BookingSystem/BuildingBlocks/Api/ResultExtensions.cs
  - src/BookingSystem/BuildingBlocks/Application/IClock.cs
  - src/BookingSystem/BuildingBlocks/Application/ICommand.cs
  - src/BookingSystem/BuildingBlocks/Application/ICommandHandler.cs
  - src/BookingSystem/BuildingBlocks/Application/IQuery.cs
  - src/BookingSystem/BuildingBlocks/Application/IQueryHandler.cs
  - src/BookingSystem/BuildingBlocks/Application/SystemClock.cs
  - src/BookingSystem/BuildingBlocks/Domain/AggregateRoot.cs
  - src/BookingSystem/BuildingBlocks/Domain/DomainEvent.cs
  - src/BookingSystem/BuildingBlocks/Domain/DomainException.cs
  - src/BookingSystem/BuildingBlocks/Domain/Entity.cs
  - src/BookingSystem/BuildingBlocks/Domain/Error.cs
  - src/BookingSystem/BuildingBlocks/Domain/Result.cs
  - src/BookingSystem/BuildingBlocks/Domain/ValueObject.cs
  - src/BookingSystem/Modules/Reservations/Domain/ReservableRoomId.cs
  - src/BookingSystem/Modules/Reservations/Domain/Reservation.cs
  - src/BookingSystem/Modules/Reservations/Domain/ReservationGuest.cs
  - src/BookingSystem/Modules/Reservations/Domain/ReservationId.cs
  - src/BookingSystem/Modules/Reservations/Domain/ReservationPeriod.cs
  - src/BookingSystem/Modules/Reservations/Domain/ReservationStatus.cs
  - src/BookingSystem/Modules/Reservations/Infrastructure/InMemoryReservationRepository.cs
  - src/BookingSystem/Modules/Reservations/Infrastructure/InMemoryReservationStore.cs
  - src/BookingSystem/Modules/Reservations/ReservationsModule.cs
  - src/BookingSystem/Modules/Reservations/UseCases/Abstractions/IReservationRepository.cs
  - src/BookingSystem/Modules/Reservations/UseCases/CancelReservation/CancelReservationCommand.cs
  - src/BookingSystem/Modules/Reservations/UseCases/CancelReservation/CancelReservationEndpoint.cs
  - src/BookingSystem/Modules/Reservations/UseCases/CancelReservation/CancelReservationHandler.cs
  - src/BookingSystem/Modules/Reservations/UseCases/ChangeReservationPeriod/ChangeReservationPeriodCommand.cs
  - src/BookingSystem/Modules/Reservations/UseCases/ChangeReservationPeriod/ChangeReservationPeriodEndpoint.cs
  - src/BookingSystem/Modules/Reservations/UseCases/ChangeReservationPeriod/ChangeReservationPeriodHandler.cs
  - src/BookingSystem/Modules/Reservations/UseCases/ChangeReservationPeriod/ChangeReservationPeriodRequest.cs
  - src/BookingSystem/Modules/Reservations/UseCases/ChangeReservationPeriod/ChangeReservationPeriodValidator.cs
  - src/BookingSystem/Modules/Reservations/UseCases/ConfirmReservation/ConfirmReservationCommand.cs
  - src/BookingSystem/Modules/Reservations/UseCases/ConfirmReservation/ConfirmReservationEndpoint.cs
  - src/BookingSystem/Modules/Reservations/UseCases/ConfirmReservation/ConfirmReservationHandler.cs
  - src/BookingSystem/Modules/Reservations/UseCases/CreateReservation/CreateReservationCommand.cs
  - src/BookingSystem/Modules/Reservations/UseCases/CreateReservation/CreateReservationEndpoint.cs
  - src/BookingSystem/Modules/Reservations/UseCases/CreateReservation/CreateReservationHandler.cs
  - src/BookingSystem/Modules/Reservations/UseCases/CreateReservation/CreateReservationRequest.cs
  - src/BookingSystem/Modules/Reservations/UseCases/CreateReservation/CreateReservationResponse.cs
  - src/BookingSystem/Modules/Reservations/UseCases/CreateReservation/CreateReservationValidator.cs
  - src/BookingSystem/Modules/Reservations/UseCases/GetReservation/GetReservationEndpoint.cs
  - src/BookingSystem/Modules/Reservations/UseCases/GetReservation/GetReservationHandler.cs
  - src/BookingSystem/Modules/Reservations/UseCases/GetReservation/GetReservationQuery.cs
  - src/BookingSystem/Modules/Reservations/UseCases/GetReservation/GetReservationResponse.cs
  - src/BookingSystem/Modules/Reservations/UseCases/ListReservations/ListReservationsEndpoint.cs
  - src/BookingSystem/Modules/Reservations/UseCases/ListReservations/ListReservationsHandler.cs
  - src/BookingSystem/Modules/Reservations/UseCases/ListReservations/ListReservationsQuery.cs
  - src/BookingSystem/Modules/Reservations/UseCases/ListReservations/ReservationListItem.cs
  - src/BookingSystem/Modules/RoomManagement/Domain/Room.cs
  - src/BookingSystem/Modules/RoomManagement/Domain/RoomCapacity.cs
  - src/BookingSystem/Modules/RoomManagement/Domain/RoomId.cs
  - src/BookingSystem/Modules/RoomManagement/Domain/RoomName.cs
  - src/BookingSystem/Modules/RoomManagement/Domain/RoomStatus.cs
  - src/BookingSystem/Modules/RoomManagement/Infrastructure/InMemoryRoomReader.cs
  - src/BookingSystem/Modules/RoomManagement/Infrastructure/InMemoryRoomRepository.cs
  - src/BookingSystem/Modules/RoomManagement/Infrastructure/InMemoryRoomStore.cs
  - src/BookingSystem/Modules/RoomManagement/PublicContracts/IRoomReader.cs
  - src/BookingSystem/Modules/RoomManagement/PublicContracts/RoomInfo.cs
  - src/BookingSystem/Modules/RoomManagement/RoomManagementModule.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/Abstractions/IRoomRepository.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/AddRoom/AddRoomCommand.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/AddRoom/AddRoomEndpoint.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/AddRoom/AddRoomHandler.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/AddRoom/AddRoomRequest.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/AddRoom/AddRoomResponse.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/AddRoom/AddRoomValidator.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/DeactivateRoom/DeactivateRoomCommand.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/DeactivateRoom/DeactivateRoomEndpoint.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/DeactivateRoom/DeactivateRoomHandler.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/EditRoom/EditRoomCommand.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/EditRoom/EditRoomEndpoint.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/EditRoom/EditRoomHandler.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/EditRoom/EditRoomRequest.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/EditRoom/EditRoomResponse.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/EditRoom/EditRoomValidator.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/GetRoom/GetRoomEndpoint.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/GetRoom/GetRoomHandler.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/GetRoom/GetRoomQuery.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/GetRoom/GetRoomResponse.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/ListRooms/ListRoomsEndpoint.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/ListRooms/ListRoomsHandler.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/ListRooms/ListRoomsQuery.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/ListRooms/RoomListItem.cs
---

# Architecture Review — booking-system

## Summary

| Level | Count |
|---|---|
| CRITICAL | 0 |
| WARNING | 2 |
| INFO | 2 |

## Findings

### [WARNING] InMemoryRoomRepository.Update is a no-op

**File:** `src/BookingSystem/Modules/RoomManagement/Infrastructure/InMemoryRoomRepository.cs` (line 27)
**Details:** The `Update` method returns `Task.CompletedTask` without writing the modified room back to the store. This currently works by reference coincidence: `InMemoryRoomStore` holds live object references, so mutations applied through `room.Rename(...)` and `room.ChangeCapacity(...)` in `EditRoomHandler` are reflected in the store immediately without an explicit write-back. The semantics are misleading and the behaviour is fragile. Any future change to the store that breaks reference identity (e.g. serialization, copy-on-write, EF Core, or a real database) will cause silent data loss for all edit and deactivate operations. The equivalent method in `InMemoryReservationRepository.Update` correctly replaces the element by index, which is the consistent and safe pattern.
**Recommendation:** Implement the method body to match `InMemoryReservationRepository.Update`: locate the room by id in the list and replace it. This makes the no-op intent explicit if it is truly intentional, and prevents silent breakage when persistence is replaced.

### [WARNING] IClock registered inside Reservations module, not in BuildingBlocks

**File:** `src/BookingSystem/Modules/Reservations/ReservationsModule.cs` (line 23)
**Details:** `IClock` and `SystemClock` are defined in `BuildingBlocks.Application` — a shared infrastructure concern — but `SystemClock` is registered as a singleton exclusively inside `AddReservationsModule`. Any future use of `IClock` in RoomManagement or another module would require a second registration or knowledge of which module owns the clock, introducing hidden coupling across module bootstrapping code.
**Recommendation:** Move the `services.AddSingleton<IClock, SystemClock>()` registration to a dedicated `AddBuildingBlocks(...)` extension method (e.g. in a `BuildingBlocksModule` or directly in `Program.cs`) so all modules can resolve the same instance without one module owning a cross-cutting concern.

### [INFO] EndpointRouteBuilderExtensions is empty

**File:** `src/BookingSystem/BuildingBlocks/Api/EndpointRouteBuilderExtensions.cs` (N/A)
**Details:** The class is declared but contains no members. It was likely scaffolded for future use. It does not introduce any architecture violation but adds noise to the BuildingBlocks layer.
**Recommendation:** Either populate the class with the shared endpoint helpers it is intended to hold, or remove it until needed.

### [INFO] Module entry points are public; all internals are correctly scoped

**File:** `src/BookingSystem/Modules/Reservations/ReservationsModule.cs`, `src/BookingSystem/Modules/RoomManagement/RoomManagementModule.cs` (N/A)
**Details:** All domain aggregates, value objects, repositories, handlers, validators, commands, queries, and infrastructure classes are marked `internal`. Module entry points (`ReservationsModule`, `RoomManagementModule`) and the cross-module contract types (`IRoomReader`, `RoomInfo`) are the only `public` declarations. This is the correct access-modifier discipline for a modular monolith and no violations were found.
**Recommendation:** No action required.
