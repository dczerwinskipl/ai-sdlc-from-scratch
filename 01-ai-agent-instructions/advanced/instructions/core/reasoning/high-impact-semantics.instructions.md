<!-- Type: reasoning -->

# High-Impact Domain Semantics

Apply this file when the requirement contains ambiguous language around time, identity, reference, ownership, lifecycle, visibility, consistency, or external side effects that may affect AC or model selection.

Detect semantic ambiguity that may affect AC, domain model, architecture, integrations, or user-visible behavior.

## Semantics to check when relevant

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

## Decision rule

For each detected semantic ambiguity, decide whether it is:

- a blocking open question (affects AC, model, or architecture)
- a non-blocking open question (affects details but not model choice)
- a safe assumption (standard behavior, well-understood)
- an implementation detail (safe to decide during implementation)

Do not silently assume high-impact semantics when they affect business behavior or model selection.
