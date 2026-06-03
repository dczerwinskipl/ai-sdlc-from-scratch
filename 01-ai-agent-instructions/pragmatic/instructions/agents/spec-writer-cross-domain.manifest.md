<!-- Archetype: MANIFEST -->

# Spec Writer Cross-Domain — Instruction Manifest

This file is the single source of truth for which instruction files the Spec Writer Cross-Domain agent loads
and under what conditions.

All tool adapters (`.claude`, `.github`, `AGENTS.md`) must reference this file instead of
maintaining their own instruction lists.

---

## Always load

- `instructions/agents/spec-writer-cross-domain.agent.instructions.md`
- `instructions/workflows/spec-writer-cross-domain-flow.instructions.md`
- `instructions/workflows/architecture-gate.instructions.md`
- `instructions/workflows/artifact-lifecycle.instructions.md`
- `instructions/core/reasoning/analysis-standards.instructions.md`
- `instructions/core/ddd/domain-archetype-index.instructions.md`

## Always load (this project)

- `instructions/project/booking/domain-context.instructions.md`
- `instructions/project/booking/known-decisions.instructions.md`

## Load when starting Step 4 (Domain Discovery)

- `instructions/core/ddd/domain-discovery.instructions.md`

Archetype group files are loaded dynamically by the agent during Step 4, following the
instructions in `domain-archetype-index.instructions.md` (always loaded). The index
identifies which groups are relevant and instructs the agent to load them in parallel.
Do not pre-load all group files here.

Load additionally when multiple candidate aggregates or lifecycle overlap is found:

- `instructions/core/ddd/aggregate-boundaries.instructions.md`

## Load when Step 6 (Detect architectural impact) confirms a concern exists

- `instructions/workflows/solution-option-analysis.instructions.md`
- `instructions/core/ddd/context-map.instructions.md`

## Load when Step 14 requires c4.md generation

Load only when Step 14 confirms the approved model changes module boundaries, bounded
context boundaries, or cross-module contracts.

- `instructions/core/ddd/c4-diagrams.instructions.md`

## Load when starting Step 12 (Artifact Reconciliation Pass)

- `instructions/workflows/artifact-reconciliation.instructions.md`

## Forbidden combinations

- Do not load `solution-option-analysis.instructions.md` before Step 6 (Detect architectural impact)
  confirms that an architectural concern exists.
- Do not load project files from another project folder when operating in this project.

## Files that are adapters only (do not load as instructions)

- `.claude/agents/spec-writer-cross-domain.md`
- `.claude/commands/spec-writer-cross-domain.md`
- `.github/copilot-instructions.md`
- `.github/agents/spec-writer-cross-domain.agent.md`
- `AGENTS.md`
- `instructions/README.md`  (human documentation — not an instruction file)

## Files that are generated artifacts (do not treat as instructions)

- `docs/**/*.md`

Artifact notes:
- `implementation-plan.md` is a Step 14 output. It is generated only after `decision.md` is approved, the reconciliation pass produces `CLEAR`, and the readiness check passes. It is not a loaded instruction file.
- `reconciliation-report.md` is a Step 12 output. Writing it to disk is optional — it may be produced inline in the conversation only. It is not a loaded instruction file.
