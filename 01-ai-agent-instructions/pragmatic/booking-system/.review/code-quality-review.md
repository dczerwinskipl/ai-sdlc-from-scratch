---
artifact: review
angle: code-quality
status: approved
created: 2026-06-03
updated: 2026-06-03
approved-by: Dominik Czerwiński
feature: booking-system
scope:
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
  - src/BookingSystem/Modules/Reservations/Domain/ReservationId.cs
  - src/BookingSystem/Modules/Reservations/Domain/ReservationStatus.cs
  - src/BookingSystem/Modules/Reservations/Domain/ReservationPeriod.cs
  - src/BookingSystem/Modules/Reservations/Domain/ReservationGuest.cs
  - src/BookingSystem/Modules/Reservations/Domain/Reservation.cs
  - src/BookingSystem/Modules/Reservations/Infrastructure/InMemoryReservationStore.cs
  - src/BookingSystem/Modules/Reservations/Infrastructure/InMemoryReservationRepository.cs
  - src/BookingSystem/Modules/Reservations/UseCases/Abstractions/IReservationRepository.cs
  - src/BookingSystem/Modules/Reservations/UseCases/CreateReservation/CreateReservationCommand.cs
  - src/BookingSystem/Modules/Reservations/UseCases/CreateReservation/CreateReservationRequest.cs
  - src/BookingSystem/Modules/Reservations/UseCases/CreateReservation/CreateReservationResponse.cs
  - src/BookingSystem/Modules/Reservations/UseCases/CreateReservation/CreateReservationValidator.cs
  - src/BookingSystem/Modules/Reservations/UseCases/CreateReservation/CreateReservationHandler.cs
  - src/BookingSystem/Modules/Reservations/UseCases/CreateReservation/CreateReservationEndpoint.cs
  - src/BookingSystem/Modules/Reservations/UseCases/ConfirmReservation/ConfirmReservationCommand.cs
  - src/BookingSystem/Modules/Reservations/UseCases/ConfirmReservation/ConfirmReservationHandler.cs
  - src/BookingSystem/Modules/Reservations/UseCases/ConfirmReservation/ConfirmReservationEndpoint.cs
  - src/BookingSystem/Modules/Reservations/UseCases/CancelReservation/CancelReservationCommand.cs
  - src/BookingSystem/Modules/Reservations/UseCases/CancelReservation/CancelReservationHandler.cs
  - src/BookingSystem/Modules/Reservations/UseCases/CancelReservation/CancelReservationEndpoint.cs
  - src/BookingSystem/Modules/Reservations/UseCases/ChangeReservationPeriod/ChangeReservationPeriodCommand.cs
  - src/BookingSystem/Modules/Reservations/UseCases/ChangeReservationPeriod/ChangeReservationPeriodRequest.cs
  - src/BookingSystem/Modules/Reservations/UseCases/ChangeReservationPeriod/ChangeReservationPeriodValidator.cs
  - src/BookingSystem/Modules/Reservations/UseCases/ChangeReservationPeriod/ChangeReservationPeriodHandler.cs
  - src/BookingSystem/Modules/Reservations/UseCases/ChangeReservationPeriod/ChangeReservationPeriodEndpoint.cs
  - src/BookingSystem/Modules/Reservations/UseCases/GetReservation/GetReservationQuery.cs
  - src/BookingSystem/Modules/Reservations/UseCases/GetReservation/GetReservationResponse.cs
  - src/BookingSystem/Modules/Reservations/UseCases/GetReservation/GetReservationHandler.cs
  - src/BookingSystem/Modules/Reservations/UseCases/GetReservation/GetReservationEndpoint.cs
  - src/BookingSystem/Modules/Reservations/UseCases/ListReservations/ListReservationsQuery.cs
  - src/BookingSystem/Modules/Reservations/UseCases/ListReservations/ReservationListItem.cs
  - src/BookingSystem/Modules/Reservations/UseCases/ListReservations/ListReservationsHandler.cs
  - src/BookingSystem/Modules/Reservations/UseCases/ListReservations/ListReservationsEndpoint.cs
  - src/BookingSystem/Modules/Reservations/ReservationsModule.cs
  - src/BookingSystem/Modules/RoomManagement/Domain/RoomId.cs
  - src/BookingSystem/Modules/RoomManagement/Domain/RoomName.cs
  - src/BookingSystem/Modules/RoomManagement/Domain/RoomCapacity.cs
  - src/BookingSystem/Modules/RoomManagement/Domain/RoomStatus.cs
  - src/BookingSystem/Modules/RoomManagement/Domain/Room.cs
  - src/BookingSystem/Modules/RoomManagement/PublicContracts/IRoomReader.cs
  - src/BookingSystem/Modules/RoomManagement/PublicContracts/RoomInfo.cs
  - src/BookingSystem/Modules/RoomManagement/Infrastructure/InMemoryRoomStore.cs
  - src/BookingSystem/Modules/RoomManagement/Infrastructure/InMemoryRoomRepository.cs
  - src/BookingSystem/Modules/RoomManagement/Infrastructure/InMemoryRoomReader.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/Abstractions/IRoomRepository.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/AddRoom/AddRoomCommand.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/AddRoom/AddRoomRequest.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/AddRoom/AddRoomResponse.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/AddRoom/AddRoomValidator.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/AddRoom/AddRoomHandler.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/AddRoom/AddRoomEndpoint.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/EditRoom/EditRoomCommand.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/EditRoom/EditRoomRequest.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/EditRoom/EditRoomResponse.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/EditRoom/EditRoomValidator.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/EditRoom/EditRoomHandler.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/EditRoom/EditRoomEndpoint.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/DeactivateRoom/DeactivateRoomCommand.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/DeactivateRoom/DeactivateRoomHandler.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/DeactivateRoom/DeactivateRoomEndpoint.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/GetRoom/GetRoomQuery.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/GetRoom/GetRoomResponse.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/GetRoom/GetRoomHandler.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/GetRoom/GetRoomEndpoint.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/ListRooms/ListRoomsQuery.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/ListRooms/RoomListItem.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/ListRooms/ListRoomsHandler.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/ListRooms/ListRoomsEndpoint.cs
  - src/BookingSystem/Modules/RoomManagement/RoomManagementModule.cs
---

# Code Quality Review - booking-system

## Summary

| Level | Count |
|---|---|
| CRITICAL | 1 |
| WARNING | 5 |
| INFO | 8 |

## Findings

### CRITICAL No logging in command handlers

**File:** Handlers across both modules (CancelReservationHandler, ConfirmReservationHandler, CreateReservationHandler, ChangeReservationPeriodHandler, AddRoomHandler, EditRoomHandler, DeactivateRoomHandler)
**Details:** The code has no logging infrastructure at all. No single command handler logs entry, progress, or exit events. Per step 7 of the code-quality instructions, each command handler must log at least the command type/key identifiers and success/failure outcome. This is a critical gap for production observability - handlers execute domain operations that modify system state with zero visibility.
**Recommendation:** Add ILogger dependency to all handlers. Log entry with command ID/identifiers, log success with outcome, and log any caught exceptions at Error level. Implement structured logging (LogInformation, LogError) with named parameters.

### WARNING Inconsistent error handling pattern in repository conflicts

**File:** `src/BookingSystem/Modules/Reservations/Infrastructure/InMemoryReservationRepository.cs` (line 36-52)
**Details:** The TryChangePeriod method returns a Result that could contain a ConflictError, but this method is used in the handler without explicit error-type checking. While it works, the pattern diverges from other repository methods (TryAdd returns bool). The handler checks TryAdd's bool and converts to ConflictError itself, but TryChangePeriod delegates error creation to the repository. This inconsistency could cause confusion about which layer owns conflict detection logic.
**Recommendation:** Consider normalizing both methods to the same pattern: either both return Result, or both return bool and let handlers translate to Result. This improves predictability.

### WARNING Missing validation in ValueObject factory methods

**File:** `src/BookingSystem/Modules/Reservations/Domain/ReservationGuest.cs` (line 13), `src/BookingSystem/Modules/RoomManagement/Domain/RoomName.cs` (line 13), `src/BookingSystem/Modules/RoomManagement/Domain/RoomCapacity.cs` (line 12)
**Details:** These factory methods throw DomainException for validation failures. However, upstream callers (CreateReservationValidator, AddRoomValidator, EditRoomValidator) already validate the same constraints before calling these methods. The validation happens twice: once as a Result check, once as an exception throw. This redundancy masks where validation truly belongs and makes error handling harder to reason about.
**Recommendation:** Choose one pattern: either have factories validate and throw, or have them assume pre-validated input. If factories validate, return Result instead of throwing DomainException to match the domain error pattern used elsewhere (Result<T>). If handlers own validation entirely, remove checks from factories and add comments that input must be pre-validated.

### WARNING Partial update not persisted in RoomRepository

**File:** `src/BookingSystem/Modules/RoomManagement/Infrastructure/InMemoryRoomRepository.cs` (line 26-29)
**Details:** The Update method receives the modified Room aggregate but does not actually persist the changes. The method is empty except for a Task.CompletedTask return. This works in-memory only because Rooms are passed by reference and were mutated before this call, but it's semantically incorrect and fragile. If the implementation ever changes to a real database, this will silently fail.
**Recommendation:** Find and update the room in the underlying store, similar to how InMemoryReservationRepository.Update works (find by ID and replace in the list within the lock).

### WARNING Query handlers do not consume CancellationToken

**File:** All query handlers: `GetReservationHandler`, `ListReservationsHandler`, `GetRoomHandler`, `ListRoomsHandler` (lines 9-13 in each)
**Details:** Query handlers accept CancellationToken as a parameter but never use it. For read-only operations on in-memory stores this has no practical effect, but it violates async method best practice: if you accept a token, you must propagate it to async calls or document why it is not used. This is a contract violation that confuses future maintainers.
**Recommendation:** Either remove the CancellationToken parameter from the Handle signature and endpoint mappings, or propagate it to all async calls (currently there are no awaitable calls in these handlers, so removal is the cleaner option unless the spec requires the token for future extensibility).

### WARNING Inconsistent error types for not-found scenarios

**File:** `src/BookingSystem/Modules/Reservations/UseCases/GetReservation/GetReservationEndpoint.cs` (line 17-19) vs `src/BookingSystem/Modules/RoomManagement/UseCases/GetRoom/GetRoomEndpoint.cs` (line 17-19)
**Details:** Query handlers return null for not-found cases, and endpoints construct a NotFoundError to convert to HTTP 404. This pattern works but is asymmetric with command handlers which return NotFoundError directly from the handler. The asymmetry suggests the error abstraction could be cleaner.
**Recommendation:** This is low-priority but consider whether query handlers should return Result<T?> to make error handling symmetric across commands and queries, or document why read paths return null while write paths return Result.

### WARNING Unused methods in Module registration classes

**File:** `src/BookingSystem/BuildingBlocks/Api/EndpointRouteBuilderExtensions.cs` (line 3-5)
**Details:** EndpointRouteBuilderExtensions is an empty class with no methods. It appears to be a placeholder for future extensions. While not an error, it adds noise to the codebase and should either have methods or be removed.
**Recommendation:** Delete the file if no immediate use is planned, or add a TODO comment explaining its intended purpose.

### INFO Minimal test scenario coverage in async patterns

**File:** All files (general observation)
**Details:** The codebase uses async/await consistently and correctly. No instances of .Result, .Wait(), or sync-over-async patterns detected. All handlers properly accept and propagate CancellationToken (even if some don't use it). Async consistency is well-maintained.
**Recommendation:** No action; this is a positive finding.

### INFO SOLID adherence is strong in handler and service design

**File:** All handler classes (general observation across modules)
**Details:** Handlers follow Single Responsibility Principle: each handles one command/query. Dependencies are injected via constructor and use interfaces (IReservationRepository, IRoomRepository, IClock, IRoomReader). Cross-module boundaries respect the published contract interface (IRoomReader is public, concrete classes are internal). No concrete cross-module class injection detected.
**Recommendation:** No action; architecture is clean.

### INFO Result type design avoids raw exception propagation

**File:** `src/BookingSystem/BuildingBlocks/Domain/Result.cs` (general observation)
**Details:** The Result<T> and Result types correctly encapsulate domain errors (DomainError, ValidationError, NotFoundError, ConflictError) and prevent raw exception types from reaching the API layer. ResultExtensions.ToHttpResult maps error types to HTTP status codes correctly.
**Recommendation:** No action; error handling strategy is sound.

### INFO Method complexity is appropriate for handlers

**File:** All handler implementations (general observation)
**Details:** Command handlers delegate validation to separate Validator classes and business logic to domain methods. No handler implements validation, persistence, and response mapping as a single flat procedure. The CreateReservationHandler (17 lines) and ChangeReservationPeriodHandler (15 lines) are well-structured with clear responsibility separation. Domain methods (Confirm, Cancel, ChangePeriod, Rename, ChangeCapacity, Deactivate) are concise and single-purpose.
**Recommendation:** No action; complexity is well-managed.

### INFO Domain method naming reflects domain vocabulary

**File:** All domain classes (Reservation, Room) and their methods
**Details:** Domain method names align with the operation names used in command types: Confirm (ConfirmReservationCommand), Cancel (CancelReservationCommand), ChangePeriod (ChangeReservationPeriodCommand), Rename (EditRoomCommand), ChangeCapacity (EditRoomCommand), Deactivate (DeactivateRoomCommand). No generic names like Process or Execute found.
**Recommendation:** No action; naming is domain-aligned.

### INFO ValueObject equality and hash code implementation is correct

**File:** `src/BookingSystem/BuildingBlocks/Domain/ValueObject.cs` (lines 7-21)
**Details:** The abstract ValueObject base class implements Equals and GetHashCode based on atomic values (immutable parts of the value object). All subclasses properly override GetAtomicValues. This ensures ReservationPeriod, ReservationGuest, RoomName, RoomCapacity can be used as dictionary keys and compared correctly.
**Recommendation:** No action; implementation is sound.

## Deferred findings

None.
