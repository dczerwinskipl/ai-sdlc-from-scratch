<!-- Archetype: MANIFEST -->

# Spec Writer — Instruction Manifest

This file is the single source of truth for which instruction files the Spec Writer loads
and under what conditions.

All tool adapters (`.claude`, `.github`, `AGENTS.md`) must reference this file instead of
maintaining their own instruction lists.

---

## Always load

- `instructions/agents/spec-writer.agent.instructions.md`
- `instructions/workflows/spec-writer-flow.instructions.md`
- `instructions/workflows/architecture-gate.instructions.md`
- `instructions/core/reasoning/analysis-standards.instructions.md`

## Always load (this project)

- `instructions/project/booking/domain-context.instructions.md`
- `instructions/project/booking/known-decisions.instructions.md`

## Load when starting Step 4 (Domain Discovery)

- `instructions/core/ddd/domain-discovery.instructions.md`
- `instructions/core/ddd/domain-archetypes.instructions.md`

Load additionally when multiple candidate aggregates or lifecycle overlap is found:

- `instructions/core/ddd/aggregate-lifecycle.instructions.md`

## Load when Step 6 (Detect architectural impact) confirms a concern exists

- `instructions/workflows/solution-option-analysis.instructions.md`

## Forbidden combinations

- Do not load `solution-option-analysis.instructions.md` before Step 6 (Detect architectural impact)
  confirms that an architectural concern exists.
- Do not load project files from another project folder when operating in this project.

## Files that are adapters only (do not load as instructions)

- `.claude/agents/spec-writer.md`
- `.claude/commands/spec-writer.md`
- `.github/copilot-instructions.md`
- `.github/agents/spec-writer.agent.md`
- `AGENTS.md`

## Files that are generated artifacts (do not treat as instructions)

- `docs/spec/**/*.md`

Artifact notes:
- `implementation-plan.md` is a Phase 9 output. It is generated only after `decision.md` is approved. It is not a loaded instruction file.
