---
artifact: review
angle: security
status: approved
created: 2026-06-03
updated: 2026-06-03
approved-by: Dominik Czerwiński
feature: booking-system
scope:
  - src/BookingSystem/Program.cs
  - src/BookingSystem/appsettings.json
  - src/BookingSystem/appsettings.Development.json
  - src/BookingSystem/BuildingBlocks/Api/EndpointRouteBuilderExtensions.cs
  - src/BookingSystem/BuildingBlocks/Api/ResultExtensions.cs
  - src/BookingSystem/BuildingBlocks/Application/SystemClock.cs
  - src/BookingSystem/BuildingBlocks/Domain/AggregateRoot.cs
  - src/BookingSystem/BuildingBlocks/Domain/DomainException.cs
  - src/BookingSystem/BuildingBlocks/Domain/Result.cs
  - src/BookingSystem/Modules/Reservations/Domain/Reservation.cs
  - src/BookingSystem/Modules/Reservations/Domain/ReservationPeriod.cs
  - src/BookingSystem/Modules/Reservations/Infrastructure/InMemoryReservationRepository.cs
  - src/BookingSystem/Modules/Reservations/ReservationsModule.cs
  - src/BookingSystem/Modules/Reservations/UseCases/CancelReservation/CancelReservationEndpoint.cs
  - src/BookingSystem/Modules/Reservations/UseCases/CancelReservation/CancelReservationHandler.cs
  - src/BookingSystem/Modules/Reservations/UseCases/ChangeReservationPeriod/ChangeReservationPeriodEndpoint.cs
  - src/BookingSystem/Modules/Reservations/UseCases/ChangeReservationPeriod/ChangeReservationPeriodHandler.cs
  - src/BookingSystem/Modules/Reservations/UseCases/ChangeReservationPeriod/ChangeReservationPeriodValidator.cs
  - src/BookingSystem/Modules/Reservations/UseCases/ConfirmReservation/ConfirmReservationEndpoint.cs
  - src/BookingSystem/Modules/Reservations/UseCases/ConfirmReservation/ConfirmReservationHandler.cs
  - src/BookingSystem/Modules/Reservations/UseCases/CreateReservation/CreateReservationEndpoint.cs
  - src/BookingSystem/Modules/Reservations/UseCases/CreateReservation/CreateReservationHandler.cs
  - src/BookingSystem/Modules/Reservations/UseCases/CreateReservation/CreateReservationRequest.cs
  - src/BookingSystem/Modules/Reservations/UseCases/CreateReservation/CreateReservationValidator.cs
  - src/BookingSystem/Modules/Reservations/UseCases/GetReservation/GetReservationEndpoint.cs
  - src/BookingSystem/Modules/Reservations/UseCases/GetReservation/GetReservationHandler.cs
  - src/BookingSystem/Modules/Reservations/UseCases/ListReservations/ListReservationsEndpoint.cs
  - src/BookingSystem/Modules/Reservations/UseCases/ListReservations/ListReservationsHandler.cs
  - src/BookingSystem/Modules/RoomManagement/Domain/Room.cs
  - src/BookingSystem/Modules/RoomManagement/Infrastructure/InMemoryRoomRepository.cs
  - src/BookingSystem/Modules/RoomManagement/RoomManagementModule.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/AddRoom/AddRoomEndpoint.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/AddRoom/AddRoomHandler.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/AddRoom/AddRoomValidator.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/DeactivateRoom/DeactivateRoomEndpoint.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/DeactivateRoom/DeactivateRoomHandler.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/EditRoom/EditRoomEndpoint.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/EditRoom/EditRoomHandler.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/EditRoom/EditRoomValidator.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/GetRoom/GetRoomEndpoint.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/GetRoom/GetRoomHandler.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/ListRooms/ListRoomsEndpoint.cs
  - src/BookingSystem/Modules/RoomManagement/UseCases/ListRooms/ListRoomsHandler.cs
---

# Security Review — booking-system

## Summary

| Level | Count |
|---|---|
| CRITICAL | 1 |
| WARNING | 4 |
| INFO | 2 |

## Findings

### [CRITICAL] No authorization on any endpoint

**File:** `src/BookingSystem/Program.cs` (line N/A — file-level finding)
**Details:** `Program.cs` does not call `app.UseAuthentication()` or `app.UseAuthorization()`. No endpoint in either module has `.RequireAuthorization()`, a policy attribute, or any other auth guard. All write endpoints — `POST /api/rooms`, `PUT /api/rooms/{id}`, `DELETE /api/rooms/{id}`, `POST /api/reservations`, `POST /api/reservations/{id}/confirm`, `POST /api/reservations/{id}/cancel`, `PUT /api/reservations/{id}/period` — are fully open to anonymous callers. Read endpoints are similarly unguarded.
**Recommendation:** Add `builder.Services.AddAuthentication(...)` and `app.UseAuthentication(); app.UseAuthorization();` to `Program.cs`. Apply `.RequireAuthorization()` (or a named policy) to all write-endpoint groups at minimum. Determine which read endpoints (e.g., `ListReservations`, `GetReservation`) also require protection and guard them accordingly. If this is a deliberate demo choice, document it explicitly; do not ship to production without auth.

---

### [WARNING] No upper-bound validation on GuestName

**File:** `src/BookingSystem/Modules/Reservations/UseCases/CreateReservation/CreateReservationValidator.cs` (line 10)
**Details:** The validator rejects null/whitespace guest names but applies no maximum-length constraint. An arbitrarily long string passes validation and is stored in `ReservationGuest.Name`, then reflected in `GetReservation` and `ListReservations` responses. This allows unbounded payload amplification against the list endpoint and may cause issues when the backing store is replaced with a real database (column-length violations surfacing as unhandled exceptions).
**Recommendation:** Add a maximum-length guard, e.g., `command.GuestName.Length > 200` returning a `ValidationError`. Mirror the same limit in `ReservationGuest.Create` if it does its own validation.

---

### [WARNING] No upper-bound validation on RoomName or Capacity

**File:** `src/BookingSystem/Modules/RoomManagement/UseCases/AddRoom/AddRoomValidator.cs` (line 9), `src/BookingSystem/Modules/RoomManagement/UseCases/EditRoom/EditRoomValidator.cs` (line 9)
**Details:** `AddRoomValidator` and `EditRoomValidator` both check for empty name and capacity > 0, but impose no upper bound on either field. An extremely large capacity value (e.g., `int.MaxValue`) or a very long name string passes validation silently. The same concern about column-length violations and response-payload amplification applies as with guest name.
**Recommendation:** Add a maximum-length check for name (e.g., 200 characters) and a reasonable upper bound for capacity (e.g., <= 10,000 or a configured limit) in both validators.

---

### [WARNING] AllowedHosts wildcard in production appsettings

**File:** `src/BookingSystem/appsettings.json` (line 8)
**Details:** `"AllowedHosts": "*"` disables the ASP.NET Core host-header filtering middleware entirely. When combined with a reverse proxy that forwards arbitrary `Host` headers, this can facilitate host-header injection attacks (password-reset-link poisoning, cache poisoning, open-redirect construction).
**Recommendation:** Set `AllowedHosts` to the explicit hostname(s) the service is expected to serve, e.g., `"AllowedHosts": "booking.example.com"`. The wildcard is appropriate only in local development and should be overridden per environment.

---

### [WARNING] Resource IDs interpolated directly into error-message strings surfaced in HTTP responses

**File:** `src/BookingSystem/Modules/Reservations/UseCases/CancelReservation/CancelReservationHandler.cs` (line 15), `src/BookingSystem/Modules/Reservations/UseCases/ConfirmReservation/ConfirmReservationHandler.cs` (line 14), `src/BookingSystem/Modules/Reservations/UseCases/ChangeReservationPeriod/ChangeReservationPeriodHandler.cs` (lines 23, 27), `src/BookingSystem/Modules/Reservations/UseCases/CreateReservation/CreateReservationHandler.cs` (line 22), `src/BookingSystem/Modules/RoomManagement/UseCases/DeactivateRoom/DeactivateRoomHandler.cs` (line 15), `src/BookingSystem/Modules/RoomManagement/UseCases/EditRoom/EditRoomHandler.cs` (line 20), `src/BookingSystem/Modules/RoomManagement/UseCases/GetRoom/GetRoomEndpoint.cs` (line 18), `src/BookingSystem/Modules/Reservations/UseCases/GetReservation/GetReservationEndpoint.cs` (line 18)
**Details:** Error message strings are constructed with `$"Reservation {command.ReservationId} not found."` / `$"Room {command.RoomId} not found."` and forwarded verbatim into `Results.Problem(e.Message, ...)` which reaches the HTTP response body. Although GUIDs have no inherent sensitivity, the pattern of embedding caller-supplied identifiers in response messages establishes a habit that is dangerous when extended to user-supplied free-text fields. It also means the error messages are formed by string interpolation rather than structured logging parameters, violating the structured-logging principle. If a future field (e.g., user email, room name) is inadvertently included, it will be logged and surfaced in responses without any redaction.
**Recommendation:** Use fixed error message templates (e.g., `"Reservation not found."`) and pass IDs only as structured log parameters via `ILogger`. Reserve the ID-embedded form for server-side logs, not client-facing responses.

---

### [INFO] InMemoryRoomRepository.Update is a silent no-op

**File:** `src/BookingSystem/Modules/RoomManagement/Infrastructure/InMemoryRoomRepository.cs` (lines 27-30)
**Details:** The `Update` method returns `Task.CompletedTask` without modifying the in-memory store. `EditRoomHandler` calls `repository.Update(room, ...)` after mutating the aggregate and receives a success result, but the mutation is never persisted because the in-memory `List<Room>` holds the original reference. As long as the in-memory store holds object references directly (i.e., no cloning on read), this is accidentally harmless — mutations on the returned object reference propagate through. However, if the store is ever changed to clone on read (a common defensive pattern), all room edits will silently fail. In a real database migration the same no-op stub would silently discard all updates.
**Recommendation:** Implement the `Update` method correctly: find the existing entry by ID and replace it (following the same pattern used by `InMemoryReservationRepository.Update`). This eliminates the latent data-loss risk and makes the implementation safe to migrate.

---

### [INFO] OpenAPI / Scalar UI exposed in Development only — no action required

**File:** `src/BookingSystem/Program.cs` (lines 17-21)
**Details:** `MapOpenApi()` and `MapScalarApiReference()` are correctly gated behind `app.Environment.IsDevelopment()`. No schema or interactive UI is exposed in non-development environments.
**Recommendation:** No action required. Note that if the service is ever deployed with `ASPNETCORE_ENVIRONMENT=Development`, the API schema will be public. Ensure environment variables are set correctly in each deployment target.
