<!-- Archetype: RULES -->

# Analysis Standards

## Open Questions and Assumptions

Identify open questions before finalizing acceptance criteria and before selecting a solution model.

### Open question classification

Classify every open question as one of:

- **Blocking** — must be answered before AC can be finalized or a model selected
- **Non-blocking** — affects details but not model choice; can be deferred
- **Implementation detail** — safe to decide during implementation

For every open question, document:

- why it matters
- what decision it may affect (AC, model selection, scope, architecture)
- whether it blocks AC finalization
- whether it blocks model selection
- whether it can be safely deferred

The agent must not silently convert a blocking open question into an assumption.

If the user cannot answer a blocking question yet, document the unresolved status and the risk of proceeding without it.

### Assumption risk classification

Classify every assumption as one of:

- **Low-risk** — standard behavior, safe to proceed
- **High-impact** — affects scope, AC, or model choice; must be made visible
- **Temporary** — valid for now but must be confirmed before implementation
- **Unsafe** — should be converted to an open question

An assumption is unsafe when it affects:

- scope or feature boundaries
- acceptance criteria
- lifecycle or state transitions
- ownership or data responsibility
- contracts or integrations
- user-visible behavior
- side effects
- operational behavior
- audit, history, or compliance
- billing or security
- existing records

If an unsafe assumption influences the solution model, convert it to an open question or a decision point.

---

## Semantic Ambiguity Checks

Apply this section when the requirement contains ambiguous language around time, identity, reference, ownership, lifecycle, visibility, consistency, or external side effects that may affect AC or model selection.

Detect semantic ambiguity that may affect AC, domain model, architecture, integrations, or user-visible behavior.

### Semantics to check when relevant

When reading requirements, check whether the following types of semantics are ambiguous or underspecified:

- **Time** — when something takes effect, point-in-time vs range, timezone behavior, evaluation at request time vs stored time
- **Identity vs display** — whether a reference is to a live object or a snapshot at a point in time
- **Reference vs snapshot** — whether other objects should hold a live reference or a copy
- **Ownership** — who is responsible for a piece of data or behavior
- **Lifecycle and transitions** — what triggers state changes, whether transitions are reversible, what side effects fire
- **Visibility** — who can see what, under what conditions
- **Permissions** — who can act on what
- **Audit and history** — whether actions must be traceable, whether old state must be recoverable
- **Quantity and capacity** — how limits are enforced, whether they are hard or soft
- **Money and billing** — whether amounts are stored, calculated, or snapshotted
- **Scope of rules** — whether a rule applies globally, per tenant, per resource type, or per entity
- **External side effects** — whether an action triggers notifications, external calls, or downstream behavior
- **Consistency expectations** — whether two things must always be consistent, or whether eventual consistency is acceptable

These are categories to check, not a mandatory list for every feature. Skip categories that clearly do not apply.

### Decision rule

For each detected semantic ambiguity, decide whether it is:

- a blocking open question (affects AC, model, or architecture)
- a non-blocking open question (affects details but not model choice)
- a safe assumption (standard behavior, well-understood)
- an implementation detail (safe to decide during implementation)

Do not silently assume high-impact semantics when they affect business behavior or model selection.

---

## Coupling Checklist

For every change, before generating tasks, verify the following. This applies to all changes — including those that passed Step 5 early termination. A local change can still introduce coupling that makes future extraction significantly more expensive.

**Structural coupling**

- Does the change introduce a new dependency between two modules?
  If yes: is it one-directional? A new bidirectional dependency is an architectural concern requiring explicit approval.
- Does the change make a module implement another module's own interface?
  If yes: this is a Module Boundary violation. Flag immediately.

**Temporal coupling**

- Does any new call require the callee to be available at the moment of the call?
  If yes on a read path: can the data be owned locally instead, eliminating the runtime dependency?
  If yes on a write path: is fail-closed behavior correct, or should the caller proceed on temporary callee unavailability?

**Data coupling**

- Does the change require a module to read or write another module's private schema directly?
  If yes: this is a boundary violation regardless of implementation convenience.
- Does the change pass more data than the callee needs?
  If yes: narrow the interface to what the callee actually requires.

When any check flags a concern, treat it as an architectural concern and do not proceed to implementation tasks without explicit approval.
