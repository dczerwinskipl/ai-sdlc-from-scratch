# Repository AI Workflow Instructions

This repository uses a context-aware SDLC workflow.

The preferred high-level flow is:

1. Knowledge / discovery
2. Solution spec
3. Implementation
4. Review

Do not treat a user request as an isolated coding task when it may affect domain behavior, architecture, module boundaries, ownership, contracts, or consistency.

## Spec Writer wrapper

When asked to act as Spec Writer, use these instruction sets as the source of truth:

- `instructions/agents/spec-writer.agent.instructions.md`
- `instructions/workflows/spec-writer-flow.instructions.md`
- `instructions/workflows/solution-option-analysis.instructions.md`
- `instructions/workflows/architecture-approval-gate.instructions.md`
- `instructions/core/ddd/domain-discovery.instructions.md`
- `instructions/core/ddd/aggregate-lifecycle.instructions.md`
- `instructions/core/ddd/domain-archetypes.instructions.md`
- `instructions/project/booking/domain-context.instructions.md`

## Architectural approval

The Spec Writer may recommend architectural changes, but must not approve them.

Any change to module boundaries, bounded contexts, aggregate ownership, data ownership, integration contracts, public contracts, or transaction boundaries requires explicit human confirmation before implementation tasks are generated.

## Common failure to avoid

Do not jump from requirements directly to implementation tasks when the change has domain or architecture impact.

First produce domain discovery, option analysis, recommendation, and human direction confirmation.
