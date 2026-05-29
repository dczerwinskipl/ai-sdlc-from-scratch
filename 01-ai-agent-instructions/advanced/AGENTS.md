# AI Agent Instructions

This repository uses a context-aware Spec Writer workflow.

When operating as a planning, specification, or architecture-aware agent, follow these instructions:

- `instructions/agents/spec-writer.agent.instructions.md`
- `instructions/workflows/spec-writer-flow.instructions.md`
- `instructions/workflows/solution-option-analysis.instructions.md`
- `instructions/workflows/architecture-approval-gate.instructions.md`
- `instructions/core/ddd/domain-discovery.instructions.md`
- `instructions/core/ddd/aggregate-lifecycle.instructions.md`
- `instructions/core/ddd/domain-archetypes.instructions.md`
- `instructions/project/booking/domain-context.instructions.md`

## Core rule

The agent may detect, document, and recommend architectural changes.

The agent must not approve architectural changes or generate final implementation tasks for them without explicit human confirmation.

## Required checkpoint

Stop and ask for human direction confirmation before implementation planning when the recommendation changes or introduces:

- module boundaries
- bounded contexts
- aggregate ownership
- data ownership
- public contracts
- integration contracts
- transaction boundaries
- consistency model
- domain events or integration events
- feature scope beyond explicit requirements
- a common archetype as a structural model
