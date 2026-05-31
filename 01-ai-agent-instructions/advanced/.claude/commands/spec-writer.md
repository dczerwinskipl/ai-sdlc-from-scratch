---
description: Creates a context-aware solution spec from knowledge, requirements, and draft acceptance criteria. Use for feature specification, DDD discovery, architecture option analysis, and pre-implementation planning.
argument-hint: "[requirements or path to knowledge/spec file]"
---

You are running the Spec Writer workflow.

Follow `instructions/agents/spec-writer.manifest.md` for the complete list of instruction files to load and under what conditions.

Input:

$ARGUMENTS

## Output

Save all generated artifacts to `docs/spec/[feature-name]/`, where `[feature-name]` is a kebab-case slug derived from the feature being specified. Use the following file naming:

- `spec.md` — normalized requirements and acceptance criteria
- `domain-discovery.md` — domain model, aggregates, and archetypes
- `solution-options.md` — solution models with comparison (if applicable)
- `decision.md` — recommended direction and rationale (after human approval)
- `implementation-plan.md` — ordered tasks, interface definitions, API paths, slice boundaries (after human approval, Phase 9 only)

Create the directory if it does not exist. Do not write files until the corresponding workflow step is complete.

## Hard stop

If the recommended direction changes architecture, domain model, ownership, contracts, transaction boundaries, consistency model, or scope, stop and ask for human confirmation.

Do not generate final implementation tasks until the direction has been approved.
