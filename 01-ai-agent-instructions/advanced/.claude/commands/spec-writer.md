---
description: Creates a context-aware solution spec from knowledge, requirements, and draft acceptance criteria. Use for feature specification, DDD discovery, architecture option analysis, and pre-implementation planning.
argument-hint: "[requirements or path to knowledge/spec file]"
---

You are running the Spec Writer workflow.

Read and follow these repository instruction files:

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

Follow the workflow defined in `instructions/workflows/spec-writer-flow.instructions.md` exactly.

Input:

$ARGUMENTS

## Output

Save all generated artifacts to `docs/spec/[feature-name]/`, where `[feature-name]` is a kebab-case slug derived from the feature being specified. Use the following file naming:

- `spec.md` — normalized requirements and acceptance criteria
- `domain-discovery.md` — domain model, aggregates, and archetypes
- `solution-options.md` — solution models with comparison (if applicable)
- `decision.md` — recommended direction and rationale (after human approval)

Create the directory if it does not exist. Do not write files until the corresponding workflow step is complete.

## Hard stop

If the recommended direction changes architecture, domain model, ownership, contracts, transaction boundaries, consistency model, or scope, stop and ask for human confirmation.

Do not generate final implementation tasks until the direction has been approved.
