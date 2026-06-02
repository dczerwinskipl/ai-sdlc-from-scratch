# App Context

Reference implementation for the AI SDLC blog series at [dczerwinski.pl](https://dczerwinski.pl).

## Project

`booking-system/` — a .NET 10 booking system MVP built as a single-project modular monolith.

```
booking-system/
  BookingSystem.sln
  src/BookingSystem/        # Main ASP.NET Core Minimal API project
  tests/BookingSystem.Tests/ # Unit and integration tests
```

## Architecture

- Modular monolith — one deployable, two logical modules
- Vertical slices per use case
- DDD tactical patterns where business behavior exists
- In-memory infrastructure (no database in MVP)
- Modules communicate only through `PublicContracts`

## Modules

| Module | Responsibility |
|--------|---------------|
| `RoomManagement` | Room lifecycle (create, edit, deactivate) |
| `Reservations` | Reservation lifecycle (create, confirm, cancel, reschedule) |

Cross-module dependency allowed: `Reservations → RoomManagement.PublicContracts`
