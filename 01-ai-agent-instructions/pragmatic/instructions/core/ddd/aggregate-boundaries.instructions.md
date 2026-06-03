<!-- Archetype: RULES -->

# Aggregate Boundaries

## Scope

Apply this file when domain discovery has already identified multiple candidate aggregates, lifecycle overlap, or unclear ownership between concepts.

**This file answers:** given two or more candidate aggregates, where should the boundary be?
**Domain discovery answers:** should these concepts be separated at all?

If domain discovery has not yet identified multiple candidates, return to `instructions/core/ddd/domain-discovery.instructions.md` first.

## Lifecycle analysis

For each candidate aggregate or entity, identify:

- creation command - what triggers its existence
- state transitions - what commands or events move it through states
- invariants - what must always be true across its state
- terminal states - what ends its lifecycle
- who can change it - which actors or systems can issue commands
- whether it can exist independently - does it require another aggregate to be valid
- whether it must be transactionally consistent with another aggregate - is there an invariant that spans both

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

## Root access boundary

The aggregate root is the only entry point for all commands and modifications within the aggregate. External code may not reach inside the aggregate to address child entities directly.

Flag a boundary problem when:
- an entity inside the aggregate is addressed by its own ID from outside - it should be its own aggregate root
- a command must load the whole aggregate but modifies only one nested entity consistently - that entity likely does not belong inside
- the aggregate root cannot validate its own invariants without loading data from a different aggregate - the boundary is in the wrong place
- the aggregate exposes internal entities through public collections that callers iterate and modify directly

## Size and contention signals

An aggregate is loaded and persisted as a whole unit. Size problems become visible through performance, contention, and consistency failures - not through code complexity alone.

**Unbounded collections**

If an aggregate contains a collection that grows without a natural upper bound, it will eventually become too expensive to load as a unit for each command. The collection is a candidate for a separate aggregate.

A common failure pattern: an Account aggregate that owns all its historical entries as a child collection. With large entry volumes, executing any command on the Account requires loading the full history, even when only the current balance matters. The invariant (balance must reflect all entries) does not require holding all entries in one aggregate - it requires that each new entry is consistent with the running balance at the time of recording.

**Time-bounded aggregates**

A solution to unbounded growth is designing aggregates with a natural, finite lifespan. Instead of one long-lived aggregate accumulating unbounded history, create short-lived aggregates that cover a bounded period or operation. When the period closes, the aggregate is finalized - it becomes immutable and a new one begins.

Examples of natural boundaries: a billing cycle, a settlement batch, a daily ledger closing, a delivery run, a shift, a maintenance window, an academic semester. The finalization event is a domain concept, not a technical workaround. Ask: does the business already think of this as a closing, settlement, or period end? If yes, the aggregate boundary follows that concept.

**High concurrency and contention**

If multiple actors or processes frequently compete to modify the same aggregate, it is too coarse. Optimistic concurrency conflicts at runtime are a diagnostic signal, not just a performance problem - they indicate that the aggregate is trying to serialize changes that the domain does not actually require to be serialized.

Look for parts of the aggregate that change by different actors or at different rates. Those parts are candidates for extraction into separate aggregates with eventual consistency between them.

**Partial load pattern**

If commands or queries consistently access only a predictable subset of the aggregate's state, the remainder likely does not belong in the same consistency boundary. An aggregate should be fully loaded to execute any command. If that loading is consistently wasteful, the boundary is wrong.

## Domain event signal

If an aggregate raises many structurally different domain events - events that describe unrelated facts about the system - it may have multiple independent responsibilities. Each responsibility is a candidate for its own aggregate.

A single aggregate raising `OrderPlaced`, `PaymentCaptured`, `InventoryReserved`, and `NotificationScheduled` is coordinating four independent concerns. Each of those events likely belongs to a different aggregate or domain service.

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
- a collection inside the aggregate grows without a natural upper bound
- commands on the aggregate consistently modify only a small, predictable subset of its state
- different actors issue commands to different parts of the aggregate independently
- the aggregate is frequently involved in optimistic concurrency conflicts at runtime
- the aggregate raises domain events that describe structurally unrelated facts
