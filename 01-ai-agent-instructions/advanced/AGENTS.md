# AI Agent Instructions

This repository uses a context-aware Spec Writer workflow.

When operating as a planning, specification, or architecture-aware agent, follow these instructions:

- `instructions/agents/spec-writer.agent.instructions.md`
- `instructions/workflows/spec-writer-flow.instructions.md`
- `instructions/workflows/solution-option-analysis.instructions.md`
- `instructions/workflows/architecture-approval-gate.instructions.md`
- `instructions/core/reasoning/open-questions-and-assumptions.instructions.md`
- `instructions/core/reasoning/high-impact-semantics.instructions.md`
- `instructions/core/ddd/domain-discovery.instructions.md`
- `instructions/core/ddd/aggregate-lifecycle.instructions.md`
- `instructions/core/ddd/domain-archetypes.instructions.md`
- `instructions/project/booking/domain-context.instructions.md`

## Core rule

The agent may detect, document, and recommend architectural changes.

The agent must not approve architectural changes or generate final implementation tasks for them without explicit human confirmation.

## Required checkpoint

Stop and ask for human direction confirmation when the recommendation triggers the architecture approval gate.

See the full trigger list in `instructions/workflows/architecture-approval-gate.instructions.md`.
