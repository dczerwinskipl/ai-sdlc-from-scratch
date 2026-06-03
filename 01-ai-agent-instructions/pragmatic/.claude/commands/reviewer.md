---
description: Reviews implementation from multiple angles in parallel (security, architecture, code quality, performance, test coverage, acceptance criteria, spec compliance, data integrity). Supports two modes — spec-driven (full review against spec artifacts) and generic (any codebase path, no spec required). Requires human confirmation before approving any report.
argument-hint: "<feature-name> [file ...] | <path/> [file ...] | . [file ...] [--angles security,architecture,...]"
---

You are running the Reviewer workflow.

Follow `instructions/agents/reviewer.manifest.md` for the complete list of instruction files to load and under what conditions.

Input:

$ARGUMENTS

## Modes

**Spec-driven** — pass a feature name that matches `docs/spec/<feature>/`:
```
/reviewer room-maintenance src/Modules/Availability/Commands/CreateMaintenancePeriod.cs ...
```
Runs all 8 angles. Requires `decision.md`, `spec.md`, and `implementation-plan.md` to be in approved state.

**Generic** — pass a path or `.` for the whole project:
```
/reviewer src/Modules/Availability/
/reviewer .
```
Runs 6 angles (security, architecture, code-quality, performance, test-coverage, data-integrity). Skips acceptance-criteria and spec-compliance — no spec to compare against.

**Angle override** — run only specific angles in either mode:
```
/reviewer room-maintenance [files] --angles security,data-integrity
```

## Output

Artifacts are written to:
- Spec-driven: `docs/spec/<feature>/`
- Generic: `<path>/.review/` (created if absent)

| Artifact | Angle | Mode |
|---|---|---|
| `security-review.md` | Security | Both |
| `architecture-review.md` | Architecture | Both |
| `code-quality-review.md` | Code quality + observability | Both |
| `performance-review.md` | Performance | Both |
| `test-coverage-review.md` | Test coverage | Both |
| `data-integrity-review.md` | Data integrity | Both |
| `acceptance-criteria-review.md` | AC tracing | Spec-driven only |
| `spec-compliance-review.md` | Spec compliance | Spec-driven only |

All reports start with `status: completed`. Updated to `status: approved` only after human confirmation via the gate in `instructions/workflows/reviewer-gate.instructions.md`.

For report schema and status vocabulary follow `instructions/workflows/reviewer-artifacts.instructions.md`.
