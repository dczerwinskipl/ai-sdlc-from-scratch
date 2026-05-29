---
description: Run the context-aware Spec Writer workflow for the current feature or provided requirements.
argument-hint: "[requirements or path to knowledge/spec file]"
---

Run the Spec Writer workflow for the current request.

Use these instruction files:

- `instructions/agents/spec-writer.agent.instructions.md`
- `instructions/workflows/spec-writer-flow.instructions.md`
- `instructions/workflows/solution-option-analysis.instructions.md`
- `instructions/workflows/architecture-approval-gate.instructions.md`
- `instructions/core/ddd/domain-discovery.instructions.md`
- `instructions/core/ddd/aggregate-lifecycle.instructions.md`
- `instructions/core/ddd/domain-archetypes.instructions.md`
- `instructions/project/booking/domain-context.instructions.md`

Input:

$ARGUMENTS

Required behavior:

1. Normalize requirements and acceptance criteria.
2. Scan known system context for impacted behavior.
3. Perform domain discovery.
4. Detect architectural impact.
5. If needed, propose exactly three solution models.
6. Compare models against acceptance criteria.
7. Recommend the simplest sufficient model.
8. Ask for human direction confirmation when required.
9. Do not generate final implementation tasks before approval when architecture or scope changes.
