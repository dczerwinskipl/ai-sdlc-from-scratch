<!-- Archetype: MANIFEST -->

# Reviewer — Instruction Manifest

This file is the single source of truth for which instruction files the Reviewer loads
and under what conditions.

All adapters (`.claude/agents/reviewer.md`, `.claude/commands/reviewer.md`) must reference this file.

---

## Always load

- `instructions/agents/reviewer.agent.instructions.md`
- `instructions/workflows/reviewer-flow.instructions.md`
- `instructions/workflows/reviewer-gate.instructions.md`
- `instructions/workflows/reviewer-artifacts.instructions.md`
- `instructions/workflows/artifact-lifecycle.instructions.md`
- `instructions/core/reasoning/analysis-standards.instructions.md`

---

## Subagents spawned during Step 4 (parallel review pass)

All subagents launch simultaneously after the context brief is prepared in Step 3. Each subagent adapter lists its own instruction files directly — no separate manifest file. The orchestrator does not directly load subagent instruction files.

| Subagent adapter | Angle | Model |
|---|---|---|
| `.claude/agents/reviewer-security.md` | Security | sonnet |
| `.claude/agents/reviewer-architecture.md` | Architecture + DDD invariants | sonnet |
| `.claude/agents/reviewer-code-quality.md` | Code quality + observability | haiku |
| `.claude/agents/reviewer-performance.md` | Performance + query efficiency | haiku |
| `.claude/agents/reviewer-test-coverage.md` | Test task coverage + edge cases | haiku |
| `.claude/agents/reviewer-acceptance-criteria.md` | AC1–AC11 tracing | haiku |
| `.claude/agents/reviewer-spec-compliance.md` | Spec + decision compliance | sonnet |
| `.claude/agents/reviewer-data-integrity.md` | Transaction + concurrency + migration | sonnet |

---

## Files that are adapters only (do not load as instructions)

- `.claude/agents/reviewer.md`
- `.claude/commands/reviewer.md`
- `.claude/agents/reviewer-*.md`
- `instructions/README.md`

## Files that are generated artifacts (do not treat as instructions)

- `docs/**/*-review.md`
