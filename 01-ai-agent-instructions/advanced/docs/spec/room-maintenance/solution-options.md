---
artifact: solution-options
status: historical
source-of-truth: false
requires-approval: false
approved-by: ~
related-spec: docs/spec/room-maintenance/spec.md
related-decision: docs/spec/room-maintenance/decision.md
---

# Solution Options — Room Maintenance

## Context

Room availability in the current system is fragmented across two locations:
- Room active status checked via `IRoomReader.isActive` in Reservations command handlers
- Reservation overlap check embedded in `IReservationRepository` (known shortcut, marked "do not propagate")

Maintenance introduces a new blocking factor. Three meaningfully different models exist for how the system structures that block.

---

## Model A — Minimal Change: Extend IRoomReader for Maintenance Query

### Intent

| Field | Content |
|---|---|
| Proposed because | Fastest path to satisfying all AC without changing module structure |
| Starting assumption | Maintenance is a self-contained addition; the availability fragmentation is acceptable for now |
| Optimizes for | Delivery speed, low regression risk |
| Protects | Existing module structure and contract pattern |
| Sacrifices | Availability answer stays fragmented (room status check + maintenance check + overlap check = three separate checks) |
| Keeps open | All future structural options |
| Makes harder | Adding future block types requires extending IRoomReader again; consolidating availability later means revisiting this extension |
| Good fit when | Scope is time-constrained; maintenance is unlikely to grow in complexity |
| Bad fit when | More block types are expected; the fragmented availability answer creates ongoing confusion |

### What changes

- `RoomMaintenance` aggregate added to the RoomManagement module; owns the planned-period, status, and actual-completion-date lifecycle
- `IRoomReader` extended with a maintenance block query capability for a given period
- Reservation command handlers call `IRoomReader` for maintenance blocks in addition to the existing active-status check
- The existing reservation overlap check in the repository is unchanged
- New maintenance management endpoints: create, cancel, complete, list

### What stays unchanged

- Reservation overlap check remains in the repository (known shortcut retained, not extended)
- Module structure: RoomManagement and Reservations unchanged
- `IRoomReader` remains the single cross-module contract (extended)

### Agent-introduced concerns

- **Availability answer remains fragmented:** The "can this room be reserved for this period?" answer now requires three checks: (1) room active status via `IRoomReader`, (2) maintenance block query via `IRoomReader`, (3) reservation overlap in the repository. These are in different execution locations with different failure modes.
- **IRoomReader purpose drift:** The contract evolves from a simple lookup ("is this room active?") to a temporal range query ("any maintenance in this period?"). These have different query semantics. Combining both in one interface blurs the contract's purpose without expressing intent.
- **Check-and-act race condition:** The maintenance check (IRoomReader call in the handler) and the reservation write (repository TryAdd) are two separate operations. Two simultaneous reservation requests for the same room-period can both pass the maintenance check before either writes, then both succeed in TryAdd if TryAdd's overlap check is also non-atomic. The availability answer is assembled across two non-atomic steps. Flag: **Medium-High domain risk**.
- **Single source of truth test:** Room availability requires querying RoomManagement (via IRoomReader) AND the Reservations repository. No single module can answer "is this room available?" independently. Flag: **Medium domain risk** — acceptable for MVP but worsens over time.
- **No circular dependency introduced.** Module boundary test: pass. Extraction test: pass (Reservations → RoomManagement is one-directional).

### AC coverage

| AC | Coverage |
|---|---|
| AC1: Create maintenance (room, period) | Full |
| AC2: Planned maintenance blocks new reservation creation | Full |
| AC3: Planned maintenance blocks period changes | Full |
| AC4: Reservations possible outside maintenance period | Full |
| AC5: Cancelled maintenance does not block | Full |
| AC6: Early completion unblocks from actual end date | Full |
| AC7: Cancel maintenance | Full |
| AC8: Complete maintenance with actual end date | Full |

### Complexity: **M**

---

## Model B — Incremental Domain: Consolidated Room Availability Contract

### Intent

| Field | Content |
|---|---|
| Proposed because | Fixes the most important room-side ownership issue without full restructuring; expresses availability as intent rather than attributes |
| Starting assumption | Consolidating the room-side availability check into a single query removes the main fragmentation risk without requiring a new module |
| Optimizes for | Domain correctness on the room side, clean cross-module contract |
| Protects | Module boundaries; lifecycle ownership |
| Sacrifices | Reservation overlap check stays in the repository (partial fix, not complete) |
| Keeps open | The path toward a full Availability module if needed |
| Makes harder | Undoing the consolidated contract if the scope assumption turns out to be wrong |
| Good fit when | A single room-side contract fix makes the design clearly better; future block types are possible but a new module is premature |
| Bad fit when | The full Availability module is needed anyway, making this a poor intermediate step |

### What changes

- `RoomMaintenance` aggregate added to the RoomManagement module
- `IRoomReader` is replaced or augmented with a purpose-fit availability contract: RoomManagement exposes a "is this room available for this period?" capability that consolidates room active status and maintenance blocks into a single query
- Reservation command handlers use the consolidated contract for the room-side check — one call replaces two (active check + maintenance check)
- The existing reservation overlap check in the repository is unchanged
- New maintenance management endpoints: create, cancel, complete, list

### What stays unchanged

- Reservation overlap check remains in the repository
- Module structure: RoomManagement and Reservations unchanged
- Reservation lifecycle unchanged

### Agent-introduced concerns

- **Availability answer partially consolidated:** Room-side availability is now in a single query (status + maintenance). Reservation overlap check stays in the repository. The full answer still requires two checks, but the room-side is consolidated and expresses intent clearly.
- **IRoomReader migration:** The original `IRoomReader` must be transitioned. Either extended in place (backward-compatible) or replaced by a new interface with a migration step. The former is lower risk; the latter is architecturally cleaner.
- **Check-and-act race condition (same as Model A):** The room availability contract call (handler) and the TryAdd in the repository are still two separate operations. Two simultaneous reservation requests can both pass the room-side check before either writes. Flag: **Medium-High domain risk** — same as Model A.
- **Single source of truth test:** Room availability (room-side) now has a single authority: RoomManagement's availability contract. Reservation overlap still lives in the repository. Improvement over Model A on the room side; race condition remains.
- **No circular dependency introduced.** Module boundary test: pass. Extraction test: pass.

### AC coverage

Identical to Model A — all AC fully covered.

### Complexity: **M-L**

The contract transition (IRoomReader → consolidated availability query) adds scope vs. Model A.

---

## Model C — Target Domain: Dedicated Availability Capability

### Intent

| Field | Content |
|---|---|
| Proposed because | Establishes the correct long-term model; resolves both known shortcuts (availability fragmented across modules, embedded overlap check) |
| Starting assumption | The strategic value of a clean Availability boundary justifies the higher cost and restructuring |
| Optimizes for | Extensibility, architectural consistency, long-term maintenance |
| Protects | All domain boundaries; single source of truth for availability |
| Sacrifices | Higher implementation cost; broader regression scope; more complex module dependency graph |
| Keeps open | All future block types, rule variants, availability policies as first-class Availability concerns |
| Makes harder | Rolling back if maintenance turns out to be a one-off and the Availability module is never extended |
| Good fit when | Future block types, availability policies, or multi-consumer use cases are expected; long-term foundation is the goal |
| Bad fit when | Maintenance is a self-contained addition; the full restructuring cost is not justified now |

### What changes

- New `Availability` module introduced as a first-class domain capability
- `RoomMaintenance` is owned by this module (or owned by RoomManagement and published as state to Availability)
- The Availability module provides the single authority for "can this room be used for this period?"
- It consolidates: room active status (from RoomManagement), maintenance blocks (its own state), and reservation overlap detection (moved out of the Reservations repository)
- Reservations module calls the Availability module for the full availability check before creating or changing reservation periods
- RoomManagement's `IRoomReader` contract may be narrowed (room data only, no availability query)
- New maintenance management endpoints: create, cancel, complete, list

### What stays unchanged

- Reservation and Room aggregate models (restructured how they are called, not their domain rules)
- Reservations still writes and owns reservation records

### Agent-introduced concerns

- **Reservation overlap ownership question:** If Availability owns the overlap check, it needs access to existing reservation data. If it queries Reservations at check time: Reservations → Availability (write path) AND Availability → Reservations (read path). This creates a **bidirectional dependency** that must be resolved explicitly. Safe approach: Availability queries a read-only projection of reservation data via a dedicated read interface owned by Reservations — this breaks the circularity. Must be designed carefully.
- **Single source of truth test:** With the bidirectional dependency managed via read projection, Availability is the single source of truth for the full availability answer. This is the strongest model for availability correctness.
- **Module boundary test:** Requires careful design to avoid Reservations implementing Availability's own interfaces. The read-projection approach (Reservations exposes `IReservationPeriodReader`; Availability calls it) keeps the boundary clean.
- **Extraction test:** If Reservations and Availability were deployed separately, Reservations → Availability (write) + Availability → IReservationPeriodReader (read) creates a call loop at deployment. This must be flagged as **High domain risk** for extraction. For an in-process system, this is manageable but becomes a problem at service extraction.
- **Flag:** This model introduces an Availability module not currently in the system. Confirm as an architecture decision before selecting.

### AC coverage

Identical to Models A and B — all confirmed AC fully covered.

### Complexity: **XL**

New module, cross-module contract restructuring, reservation overlap check migration, dependency graph changes. Should be split into implementation slices if selected.

---

## AC Coverage Table

| AC | Model A | Model B | Model C |
|---|---|---|---|
| AC1: Create maintenance (room, period) | Full | Full | Full |
| AC2: Planned maintenance blocks new reservations | Full | Full | Full |
| AC3: Planned maintenance blocks period changes | Full | Full | Full |
| AC4: Reservations possible outside maintenance period | Full | Full | Full |
| AC5: Cancelled maintenance does not block | Full | Full | Full |
| AC6: Early completion unblocks from actual end date | Full | Full | Full |
| AC7: Cancel maintenance | Full | Full | Full |
| AC8: Complete maintenance with actual end date | Full | Full | Full |

All models satisfy all confirmed AC. Differences are structural, not functional.

---

## Trade-off Dimension Table

| Dimension | Model A | Model B | Model C |
|---|---|---|---|
| Implementation cost | M — lowest | M-L — moderate | XL — highest |
| Availability fragmentation | Worsens (3 separate checks) | Partially fixes (room-side consolidated) | Resolves (single authority) |
| Known shortcut compliance | Shortcut retained (not extended to maintenance) | Shortcut partially addressed (room side only) | Fully resolves both known shortcuts |
| Cross-module contract clarity | IRoomReader overloaded with 2 concerns | New purpose-fit availability contract | RoomManagement narrowed; Availability becomes the contract |
| Future block types | Each requires new IRoomReader extension | Add to availability query without changing callers | Add to Availability module without changing other modules |
| Risk of becoming permanent shortcut | High — fragmented check tends to stay | Medium — room-side fixed, repo check remains | Low — full ownership established |
| Reversibility | High — no structural commitment | Medium — contract transition to undo | Low — new module to remove if wrong |
| Regression scope | Reservation handlers + maintenance endpoints | Same + contract migration | Same + repository restructuring + module dependency changes |
