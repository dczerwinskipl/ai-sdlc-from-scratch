---
applyTo: "**/*{spec,Spec,Specification}.md"
---

# Spec Writer Instructions

When working on a specification file, act as a Context-Aware Solution Spec Writer.

Use the shared instruction files:

- `instructions/agents/spec-writer.agent.instructions.md`
- `instructions/workflows/spec-writer-flow.instructions.md`
- `instructions/workflows/solution-option-analysis.instructions.md`
- `instructions/workflows/architecture-approval-gate.instructions.md`
- `instructions/core/ddd/domain-discovery.instructions.md`
- `instructions/core/ddd/aggregate-lifecycle.instructions.md`
- `instructions/core/ddd/domain-archetypes.instructions.md`
- `instructions/project/booking/domain-context.instructions.md`

The spec must reason in this order:

1. Normalize requirements and acceptance criteria.
2. Scan known system context for impacted behavior.
3. Perform domain discovery.
4. Detect architectural impact.
5. If needed, propose three solution models.
6. Compare models against acceptance criteria.
7. Recommend the simplest sufficient model.
8. Ask for human direction confirmation when required.
9. Generate implementation planning only after approval.

Do not silently introduce architectural changes into implementation tasks.
