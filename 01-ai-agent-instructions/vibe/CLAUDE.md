# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

All commands run from `booking-system/`.

```bash
# Build
dotnet build BookingSystem.slnx

# Run API (http://localhost:5000, Scalar UI at /scalar/v1)
dotnet run --project src/BookingSystem

# Run all tests
dotnet test BookingSystem.slnx

# Run a single test class
dotnet test --filter "FullyQualifiedName~CreateReservationHandlerTests"

# Run a single test method
dotnet test --filter "FullyQualifiedName~Handle_WhenRoomIsActive_ShouldCreateReservation"
```

## Architecture

.NET 10 Minimal API with two vertical-slice modules: **RoomManagement** and **Reservations**. Both are registered and mapped in `Program.cs` via extension methods on their respective `*Module` static class.

### Module layout

Each module follows the same internal structure:

```
Modules/<ModuleName>/
  Domain/           — aggregates, value objects, enums
  UseCases/
    Abstractions/   — repository interfaces used by handlers
    <UseCase>/      — Command/Query + Handler + Endpoint + Request/Response + Validator
  Infrastructure/   — in-memory store (singleton) + repository (scoped) + reader (scoped)
  PublicContracts/  — interfaces and DTOs exposed to other modules
  <ModuleName>Module.cs — DI registration + endpoint mapping
```

### Cross-module communication

Modules must not reference each other's internals. The only allowed coupling is through `PublicContracts/`. `Reservations` consumes `IRoomReader` (defined in `RoomManagement/PublicContracts`) to check room state before creating a reservation.

### Building blocks

`BuildingBlocks/` provides shared infrastructure:

- **Domain**: `AggregateRoot<TId>`, `Entity<TId>`, `ValueObject`, `Result`/`Result<T>`, `Error` hierarchy (`DomainError`, `ValidationError`, `NotFoundError`, `ConflictError`), `DomainEvent`, `DomainException`
- **Application**: `ICommand<TResponse>`, `IQuery<TResponse>`, `ICommandHandler`, `IQueryHandler`, `IClock` / `SystemClock`
- **Api**: `ResultExtensions.ToHttpResult()` maps `Result`/`Error` types to HTTP status codes; `EndpointRouteBuilderExtensions` for mapping endpoints

### In-memory storage

Each module has a singleton `InMemory*Store` (thread-safe via lock). Scoped `InMemory*Repository` (write side) and `InMemory*Reader` (read/cross-module side) both take the store by DI. This separation keeps command and query paths distinct.

### Validation

Validation lives in a dedicated `*Validator` class instantiated per-handler (not a pipeline). Validators return `Error?` (null = valid). The handler checks this before domain logic.

## Tests

Tests live in `tests/BookingSystem.Tests/` with the same module/use-case folder hierarchy as the source.

- `Unit/` — domain and validator tests; no I/O
- `Integration/` — handler tests; wire real in-memory stores/repositories directly (no mocks for infrastructure)
- `Builders/` — fluent test builders (`RoomBuilder`, `ReservationBuilder`) for constructing domain objects
- `Fakes/` — `FakeClock` for controlling time in tests

Test naming convention: `MethodName_WhenCondition_ShouldExpectedResult`.
