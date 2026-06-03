---
artifact: domain-discovery
status: draft
source-of-truth: false
requires-approval: false
approved-by: ~
related-spec: docs/spec/room-maintenance/spec.md
related-decision: docs/spec/room-maintenance/decision.md
---

# Domain Discovery — Room Maintenance

## Affected Domain Concepts

### RoomMaintenance

**Archetype match:** Block/Hold (primary) + Plan/Execution/Delta (secondary)

**Behavioral signals:**
- Admin-initiated; no external actor waiting for notification if removed — matches Block/Hold
- Has a planned end date and an optional actual end date (early completion) — matches Plan/Execution/Delta
- Has its own lifecycle: `Planned → Completed | Cancelled`
- Has own identity: must be individually cancellable by ID and listable per room
- Has temporal scope: blocks only for a defined period, not permanently
- Is queried cross-module at reservation creation and period-change time

**Discovery questions:**

| Question | Answer |
|---|---|
| Does it have its own lifecycle? | Yes — `Planned`, `Completed`, `Cancelled`, with early-completion path |
| Independent state transitions from Room? | Yes — Room `Active/Inactive` status is unrelated |
| Independent business rules? | Yes — completion rules (planned vs actual end) are internal to maintenance |
| Integrates with other modules as source of truth? | Yes — Reservations must query it during availability check |
| Historical tracking needed? | Implicitly yes — cancelled and completed maintenance serve as audit trail of what blocked a room |

**Simplest local model considered:**
Add a `MaintenancePeriod` value collection inside the `Room` aggregate in RoomManagement.

**Why insufficient:**
- `MaintenancePeriod` has its own lifecycle with distinct state transitions (`Planned`, `Completed`, `Cancelled`). Embedding this in `Room` pressures `Room` to manage maintenance state logic alongside room descriptive/operational concerns.
- `MaintenancePeriod` must be individually addressable by ID for cancellation and listing. An entity addressed by its own ID from outside the aggregate root is an aggregate boundary violation signal.
- The collection grows without a natural upper bound across the room's lifetime, creating an unbounded-collection loading problem.
- `MaintenancePeriod` is queried by the Reservations module at transaction time — a cross-module query on a nested entity, not the aggregate root, is a boundary violation.

**Extraction signals present:**
- Independent lifecycle
- Queried by another module
- Independent business rules (completion semantics)
- Separate identity (cancelable by ID)
- Unbounded collection growth if embedded in Room

**Conclusion:** `RoomMaintenance` should be a first-class aggregate, not a value collection on `Room`. Module placement is an architectural decision — flagged for option analysis.

---

### Availability (implicit, fragmented)

**Archetype match:** Availability/Capacity

**Current state:**
The "is this room available for this period?" answer is assembled across two locations:
- `IRoomReader.isActive` — checks room operational status (RoomManagement module)
- Embedded overlap check in `IReservationRepository` — checks reservation-to-reservation conflicts (Reservations module)

**With maintenance, a third factor enters:** maintenance blocks.

The archetype guidance is unambiguous: "Do not model availability as a status field on the resource. A resource may be active and still unavailable for a specific period or quantity."

**Extraction signal:** Multiple independent inputs now contribute to the availability answer across module boundaries. The known shortcut (availability embedded in repository) is marked "do not propagate" in `known-decisions.md`. The system must decide how to handle the fragmentation.

---

### Room

**Archetype:** Resource (unchanged). Room has identity, lifecycle, and maximum capacity.

Maintenance is adjacent to Room — it affects room availability but does not change room descriptive or identity data. No changes to the `Room` aggregate model are implied by this feature.

---

## Semantic Ambiguities Detected

| Ambiguity | Classification | Notes |
|---|---|---|
| "Blocked for reservations" — does this apply only forward (new reservations blocked) or also to existing conflicting reservations? | **Blocking** | Directly affects AC9 and scope. Must be resolved before AC is finalized. |
| "Finishes earlier than planned" — date or datetime granularity? | Non-blocking | Resolve as date; consistent with reservation granularity. |
| "Single date" — special flag or start = end date? | Implementation detail | Resolve as start = end. |

---

## Known Shortcut Disposition

From `known-decisions.md`: three patterns are marked "do not propagate":

1. Availability checks embedded at the repository level
2. Room active/inactive status used directly in scheduling decisions
3. Absence of time-bounded blocking rules

This feature directly touches all three. The disposition of each must be decided explicitly in option analysis:

- **Time-bounded blocking** — this feature is the specific use case that was deferred. Cannot be addressed with Active/Inactive status change. A time-bounded blocking mechanism is required.
- **Availability check in repository** — adding a maintenance check must not follow the same repository-embedded pattern. The maintenance check location (service layer vs. dedicated service) is an option analysis decision.
- **Active/inactive used in scheduling** — with a proper maintenance block in place, the active/inactive status check for scheduling purposes should be reconsidered in the context of the selected model.

---

## Domain Extraction Candidates

| Concept | Extraction signal | Recommendation |
|---|---|---|
| `RoomMaintenance` | Independent lifecycle, cross-module query, separate identity, unbounded growth | Should be a first-class aggregate. Module placement is an architecture decision. |
| `Availability` | Multiple independent inputs across module boundaries; known shortcut marked "do not propagate" | Present as option in model analysis. Placement depends on direction chosen. |
