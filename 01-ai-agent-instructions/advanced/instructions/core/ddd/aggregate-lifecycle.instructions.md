<!-- Archetype: RULES -->

# Aggregate and Lifecycle Analysis

## Scope

Apply this file when domain discovery has already identified multiple candidate aggregates, lifecycle overlap, or unclear ownership between concepts.

**This file answers:** given two or more candidate aggregates, where should the boundary be?
**Domain discovery answers:** should these concepts be separated at all?

If domain discovery has not yet identified multiple candidates, return to `instructions/core/ddd/domain-discovery.instructions.md` first.

## Lifecycle analysis

For each candidate aggregate or entity, identify:

- creation command — what triggers its existence
- state transitions — what commands or events move it through states
- invariants — what must always be true across its state
- terminal states — what ends its lifecycle
- who can change it — which actors or systems can issue commands
- whether it can exist independently — does it require another aggregate to be valid
- whether it must be transactionally consistent with another aggregate — is there an invariant that spans both

## Consistency boundary analysis

The key question for aggregate boundaries is: **which invariants must be enforced within a single transaction?**

Two concepts belong in the same aggregate when:
- there is a business rule that spans both and must never be violated, even temporarily
- one cannot have a valid state without the other existing

Two concepts should be separate aggregates when:
- their lifecycles are independent (one can be created, changed, or terminated without the other)
- the only link between them is a reference by identity (ID)
- eventual consistency between them is acceptable
- they have different owners or different rates of change

When unsure, prefer separate aggregates. A missed invariant can be added later; merging aggregates later is significantly more costly.

## Extraction signal

If two concepts have different lifecycles, they should usually not be modeled as one aggregate unless there is a strong invariant requiring immediate consistency.

## Warning signs

Flag a concern when:

- one aggregate contains state from another lifecycle
- an entity is added only because a use case needs it, not because an invariant requires it
- a value object starts carrying identity or its own lifecycle
- an aggregate becomes a workflow coordinator across unrelated processes
- an aggregate requires data from unrelated modules to validate its own invariants
- a descriptive concept is treated as locally owned only because it is needed for display
