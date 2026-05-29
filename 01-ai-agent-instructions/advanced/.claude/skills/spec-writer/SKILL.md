---
description: Creates a context-aware solution spec from knowledge, requirements, and draft acceptance criteria. Use for feature specification, DDD discovery, architecture option analysis, and pre-implementation planning.
---

# Spec Writer Skill

You are running the Spec Writer workflow.

Read and follow these repository instruction files:

- `instructions/agents/spec-writer.agent.instructions.md`
- `instructions/workflows/spec-writer-flow.instructions.md`
- `instructions/workflows/solution-option-analysis.instructions.md`
- `instructions/workflows/architecture-approval-gate.instructions.md`
- `instructions/core/ddd/domain-discovery.instructions.md`
- `instructions/core/ddd/aggregate-lifecycle.instructions.md`
- `instructions/core/ddd/domain-archetypes.instructions.md`
- `instructions/project/booking/domain-context.instructions.md`

## Workflow

1. Normalize requirements and acceptance criteria.
2. Scan known system context for impacted behavior.
3. Perform domain discovery.
4. Detect architectural impact.
5. If needed, propose exactly three solution models:
   - Minimal Change Model
   - Incremental Domain Model
   - Target Domain Model
6. Compare models against acceptance criteria.
7. Recommend the simplest sufficient model.
8. Ask for human direction confirmation when required.
9. Generate implementation planning only after approval.

## Hard stop

If the recommended direction changes architecture, domain model, ownership, contracts, transaction boundaries, consistency model, or scope, stop and ask for human confirmation.

Do not generate final implementation tasks until the direction has been approved.
