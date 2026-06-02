---
description: Creates a context-aware solution spec from knowledge, requirements, and draft acceptance criteria. Use for feature specification, DDD discovery, architecture option analysis, and pre-implementation planning.
argument-hint: "[requirements or path to knowledge/spec file]"
---

You are running the Spec Writer workflow.

Follow `instructions/agents/spec-writer.manifest.md` for the complete list of instruction files to load and under what conditions.

Input:

$ARGUMENTS

## Output

Save all generated artifacts to `docs/spec/[feature-name]/`, where `[feature-name]` is a kebab-case slug derived from the feature being specified.

| File | Created at |
|---|---|
| `domain-discovery.md` | Step 5 |
| `solution-options.md` | Step 8 (when architectural concern flagged) |
| `decision.md` | Step 9 — draft; approved after human confirmation |
| `reconciliation-report.md` | Step 12 — optional; may be produced inline only |
| `spec.md` | Step 14 (after approval and reconciliation) |
| `context-map.md` | Step 14 (after approval, only when BC or module boundaries change) |
| `c4.md` | Step 14 (same condition as context-map.md) |
| `implementation-plan.md` | Step 14 (after approval and reconciliation) |

Create the directory if it does not exist. Do not write files until the corresponding workflow step is complete.

For artifact frontmatter rules, status vocabulary, and source-of-truth constraints, follow `instructions/workflows/artifact-lifecycle.instructions.md`.

## Pre-implementation checklist

Before generating `implementation-plan.md`, verify all of the following. Do not proceed until each item is confirmed.

- [ ] `decision.md` is `status: approved` and `source-of-truth: true`
- [ ] Step 12 (Artifact Reconciliation Pass) completed — report ends with `CLEAR`
- [ ] Step 13 (Implementation Readiness Check) shows `Ready` or `Ready after minor clarification`
- [ ] `spec.md` is `status: approved` and `source-of-truth: true`
- [ ] No final artifact carries content that contradicts `decision.md`
- [ ] `solution-options.md` is marked `status: historical`

If any item is unchecked, state which step is blocked and why before continuing.

## Hard stop

If the recommended direction changes architecture, domain model, ownership, contracts, transaction boundaries, consistency model, or scope, stop and ask for human confirmation.

Do not generate final implementation tasks until the direction has been approved, the reconciliation pass produces `CLEAR`, and the readiness check passes.
