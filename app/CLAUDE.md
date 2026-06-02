# CLAUDE.md

This file provides guidance to Claude Code when working inside `/app`.

Read `.ai/context.md` for project context and `.ai/git.md` for branching and commit conventions.

## Working in This Folder

Open `booking-system/` as a separate project in your editor — it is fully self-contained.

```
code app/booking-system/
```

## Architecture Notes

- Single project, two modules: `RoomManagement` and `Reservations`
- Modules may only reference each other through `PublicContracts`
- Infrastructure is always hidden behind repository/reader abstractions
- No MediatR — handlers are injected directly and registered per module

## Visibility Rules

| Layer | Visibility |
|-------|-----------|
| Module registration | `public` |
| Public contracts | `public` |
| Handlers, commands, queries, endpoints | `internal` |
| Infrastructure, domain (unless shared) | `internal` |
