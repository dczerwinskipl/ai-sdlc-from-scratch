<!-- Type: reasoning -->

# Open Questions and Assumptions

Identify open questions before finalizing acceptance criteria and before selecting a solution model.

## Open question classification

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

## Assumption risk classification

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
