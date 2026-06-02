---
artifact: decision
status: approved
source-of-truth: true
requires-approval: true
approved-by: Dominik Czerwiński
related-spec: docs/spec/room-maintenance/spec.md
related-decision: ~
---

# Decision — Room Maintenance

## Selected Model

**Model C: Target Domain — Dedicated Availability Module, implemented in two slices**

## Decision Basis

The user chose long-term foundation over delivery speed, confirmed that additional block types are planned (owner-use, short-term rental without reservation, seasonal closures), and explicitly stated that the architecture must not prevent a future maintenance+compensation workflow.

Given those constraints:
- Model A extends the known technical debt in the wrong direction and cannot support future block types without repeated contract extension.
- Model B partially addresses the room-side fragmentation but leaves the reservation overlap shortcut intact. Because future block types will arrive, Model B would require a Model C migration within one or two features — the incremental step does not justify its cost.
- Model C establishes the correct module structure, resolves both known shortcuts, and handles check-and-act concurrency atomically. It is better positioned for temporal consistency than A or B.

## Confirmed Requirements (revised after direction questions)

| ID | Requirement |
|---|---|
| R1 | Hotel Staff can create a maintenance period for a room with a start date and end date (end date defaults to start date for single-day maintenance) |
| R2 | During a `Planned` maintenance period, the room is blocked for new reservations |
| R3 | Blocking applies only to the maintenance period — the room remains reservable outside it |
| R4 | Maintenance has status: `Planned`, `Completed`, `Cancelled` |
| R5 | `Cancelled` maintenance stops blocking the room |
| R6 | Early completion: if maintenance is completed before the planned end date, the room is unblocked from the actual end date onward |
| R7 | Hotel Staff can cancel a `Planned` maintenance |
| R8 | Hotel Staff can complete a maintenance, recording an actual end date ≤ planned end date |
| R9 | Hotel Staff can modify the period (start date, end date) of a `Planned` maintenance |

## RoomMaintenance Lifecycle

```
Created → Planned
Planned → Completed  (actual end date recorded; must be ≤ planned end date)
Planned → Cancelled
Planned → Planned    (period modified; status unchanged)
Completed → (terminal)
Cancelled → (terminal)
```

Guards:
- Only `Planned` maintenance can be cancelled, completed, or have its period modified
- `Cancelled` maintenance does not block any reservation
- `Completed` maintenance blocks only up to the actual end date
- `Planned` maintenance blocks the full planned period
- Effective blocking period for query: `Planned` uses `[startDate, plannedEndDate]`; `Completed` uses `[startDate, actualEndDate]`

## Architectural Decisions

### New module: Availability

A new `Availability` module is introduced within the hotel booking domain (same bounded context as RoomManagement and Reservations). It is not a new bounded context.

**Ownership:** The `Availability` module is the single authority for "can this room be used for this period?" It owns all temporal blocking mechanisms, beginning with `RoomMaintenance`. Future block types (owner-use, short-term rental, seasonal closures) belong in this module.

**Integration patterns:**
- Availability → RoomManagement: Customer/Supplier downstream — reads room active status from RoomManagement via a narrowed `IRoomReader`
- Reservations → Availability: Customer/Supplier downstream — Reservations calls Availability to check availability and register/deregister claims
- No circular dependency: Availability does not query Reservations at runtime

### RoomMaintenance aggregate ownership

`RoomMaintenance` is owned by the `Availability` module. It is not added to the `Room` aggregate or to RoomManagement. All temporal block types share this ownership boundary. RoomManagement retains clean ownership of room descriptive and operational data only.

### Claim registration model

Reservations registers a claim with Availability when a reservation is created or its period is changed. Availability enforces mutual exclusion across all claim types (reservation claims + maintenance blocks + future block types) atomically.

The claim registration and the reservation persistence happen within the same database transaction. If either fails, both are rolled back. This eliminates the orphaned-claim failure scenario and provides the atomic check-and-act guarantee that Models A and B cannot provide for concurrent requests.

### Temporal coupling

Three coupling points exist and are each addressed:

**Availability → RoomManagement (read, room active status):** In-process: no availability risk. Future service extraction: mitigated by a local room status projection maintained via domain events. Not required now; the module boundary supports it.

**Reservations → Availability (write, claim registration):** Temporal coupling on the write path is present in all models. Model C concentrates it into one explicit call and resolves the check-and-act race condition via atomic claim registration within a single transaction. The orphaned-claim risk is eliminated by co-locating the claim write and the reservation write in the same transaction. If the modules are ever extracted to separate services, a distributed coordination pattern (optimistic concurrency + idempotent compensation) would be needed at that point.

**Check-and-act concurrency:** Availability performs all checks (room active + maintenance block + reservation claims) and registers the new claim as one atomic operation. Two simultaneous reservation requests for the same room-period: only one acquires the claim; the other receives a conflict. This is a concurrency improvement over Models A and B, which have a check-then-write gap.

### IRoomReader contract change

`IRoomReader` is narrowed to room data only: room existence and active status. All temporal availability concerns are removed from this contract. Reservations calls Availability (not RoomManagement) for availability queries.

### Conflict behavior

If a reservation is created or its period is changed to overlap with a `Planned` maintenance block: rejected. If a maintenance block is created or its period modified to overlap with existing active reservation claims: rejected. The conflict check returns which claims conflict — this is the hook for the future compensation workflow without any structural change to Availability.

## Alternatives Considered and Rejected

**Model A — Minimal Change:**
Rejected. Extends IRoomReader with temporal semantics it was not designed for. Two-phase check (IRoomReader call + TryAdd in repository) has a check-and-act race condition under concurrent requests. Does not address the known technical debt. Future block types require repeated contract extension.

**Model B — Incremental Domain:**
Rejected. Better contract design than Model A, but reservation overlap check stays in the repository — same race condition. Given confirmed future block types and the long-term foundation goal, Model B would require a Model C migration within one or two features.

## Accepted Trade-offs

- Higher implementation cost and broader regression scope than A or B
- Reservation write path restructured: claim registration added within the same database transaction
- `IRoomReader` contract narrowed: transition required

## Deferred Decisions

- Whether `Availability` becomes a separately deployable unit (in-process module for now)
- Local room status projection for future service extraction (not required in current architecture)
- Block type model for owner-use, short-term rental, seasonal closures (structure prepared; none implemented in this iteration)
- Maintenance+compensation workflow: out of scope; architecture positions it cleanly via conflicting-claim return on block creation/modification

## Open Questions Resolved

| Question | Resolution |
|---|---|
| OQ1: Conflicting reservations on maintenance creation | Rejected — maintenance creation and period modification are blocked if active reservations overlap |
| OQ2: Can maintenance period be modified after creation? | Yes — confirmed as R9; allowed only when `Planned` |
| OQ3: Overlapping maintenance periods for same room | Not allowed — treated as implementation detail |

## Implementation Slices

**Slice 1 — Establish Availability module with maintenance (L):**
Deliver the full maintenance feature. New `Availability` module with `RoomMaintenance` aggregate. Maintenance management operations: create, modify period, cancel, complete. Reservation handlers call Availability for room-side check. Reservation overlap check remains in the repository during this slice.

**Slice 2 — Elevate reservation overlap into Availability (M):**
Move the reservation overlap check from the repository into Availability. Reservations registers claims atomically with availability check. Availability becomes the single authority. Embedded overlap check removed from repository.

Both slices are within this feature iteration unless the user explicitly defers Slice 2.
