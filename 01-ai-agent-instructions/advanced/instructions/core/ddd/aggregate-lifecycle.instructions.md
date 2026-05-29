# Aggregate and Lifecycle Analysis

Aggregate boundaries must be based on consistency rules and lifecycle ownership, not object containment.

## Lifecycle analysis

For each candidate aggregate or entity, identify:

- creation command
- state transitions
- invariants
- terminal states
- who can change it
- whether it can exist independently
- whether it must be transactionally consistent with another aggregate

## Extraction signal

If two concepts have different lifecycles, they should usually not be modeled as one aggregate unless there is a strong invariant requiring immediate consistency.

## Warning signs

Flag a concern when:

- one aggregate contains state from another lifecycle
- an entity is added only because a use case needs it
- a value object starts carrying identity or lifecycle
- an aggregate becomes a workflow coordinator
- an aggregate requires data from unrelated modules to validate basic rules
- a descriptive concept is treated as locally owned only because it is needed for display

## Required recommendation format

When proposing aggregate changes, include:

- current aggregate
- mixed responsibility
- proposed aggregate ownership
- consistency requirement
- transactional boundary
- events or application service coordination required
- whether human approval is required
