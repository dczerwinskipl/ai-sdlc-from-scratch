<!-- Archetype: RULES -->

# Artifact Lifecycle

This file is the single source of truth for artifact classification, frontmatter schema, status vocabulary, per-artifact rules, and validation constraints.

It is always loaded. Every workflow phase and every future subagent must follow these rules regardless of which phase it operates in.

---

## Frontmatter Schema

Every artifact written to `docs/spec/` must begin with this YAML block:

```yaml
---
artifact: domain-discovery | solution-options | spec | decision | implementation-plan | context-map | c4-diagram | extraction-notes | test-plan | reconciliation-report
status: draft | reviewed | approved | superseded | historical
source-of-truth: true | false
requires-approval: true | false
approved-by: ~
related-spec: docs/spec/<feature>/spec.md
related-decision: docs/spec/<feature>/decision.md
---
```

An artifact file without frontmatter is non-compliant. Do not write a file to `docs/spec/` without this block.

---

## Status Vocabulary

| Status | Meaning |
|---|---|
| `draft` | Being written. Not ready for review or implementation. |
| `reviewed` | Reviewed but not yet formally approved. |
| `approved` | Consistent and ready to act on. Human direction confirmed. |
| `superseded` | Replaced by a newer artifact for the same feature. |
| `historical` | Kept for reference. No longer current. Must not be treated as source-of-truth. |

---

## Artifact Classification

| Class | Artifacts | Source-of-truth | Requires-approval |
|---|---|---|---|
| Working / Analysis | `domain-discovery`, `solution-options` | Always false | false |
| Final | `decision`, `spec`, `context-map`, `c4-diagram` | True only when approved and reconciled | true |
| Implementation | `implementation-plan` | False for architecture; authoritative task breakdown when approved | true |
| Reference / Historical | `extraction-notes` | Always false | false |
| Derived | `test-plan` | Always false | false |
| Process output | `reconciliation-report` | Always false | false |

---

## Per-Artifact Rules

### domain-discovery.md

- `source-of-truth`: always `false`
- `requires-approval`: `false`
- Status on creation: `draft`
- Status after `decision.md` approved: remains `draft` or `reviewed`; `source-of-truth` stays `false`
- Must not contain: architecture decisions, selected model descriptions, module interface definitions, implementation approaches. Discovery findings are evidence, not approved architecture.

### solution-options.md

- `source-of-truth`: always `false`
- `requires-approval`: `false`
- Status on creation: `draft`
- Status after `decision.md` approved: must change to `historical`
- Must not contain: exact method signatures, interface definitions with parameter types, endpoint paths, repository method names or rename operations, handler names, call sequences, task breakdowns. Use capability-level descriptions. See `instructions/workflows/solution-option-analysis.instructions.md` — Must not include.

### decision.md

- `source-of-truth`: `true` only when `status: approved`
- `requires-approval`: `true`
- Status on creation: `draft`; changes to `approved` only when human explicitly confirms via the architecture gate
- Wins against: all other artifacts — if any final artifact contradicts `decision.md`, `decision.md` is authoritative
- Must not contain: method signatures, interface definitions, repository method names, endpoint paths, handler names or call sequences, task breakdowns. It may mention module names, domain concepts, integration contract names, and capability descriptions. Exact signatures and steps belong in `implementation-plan.md`.

### spec.md

- `source-of-truth`: `true` only when `status: approved` AND reconciliation pass confirms no contradiction with `decision.md`
- `requires-approval`: `true`
- Status on creation: `draft`
- Must not be written before `decision.md` is approved when architectural impact exists
- Must not contain: pre-decision model descriptions, unselected model analysis, cross-module call descriptions that contradict `decision.md`, content from `solution-options.md` copied without reconciliation

### context-map.md

- `source-of-truth`: `true` only when `status: approved` AND aligned with `decision.md`
- `requires-approval`: `true`
- Status on creation: `draft`
- Must not be generated before `decision.md` is approved
- Must not contain: method signatures, pre-decision dependency directions, contracts eliminated by `decision.md`

### c4-diagram (c4.md)

Same rules as `context-map.md`.

### implementation-plan.md

- `source-of-truth`: `false` for architectural decisions (`decision.md` is the authority) and for requirements (`spec.md` is the authority). Treat as the authoritative task breakdown once `status: approved`.
- `requires-approval`: `true`
- Must not be generated before reconciliation pass completes without contradictions
- May contain: ordered tasks, method signatures, interface definitions, endpoint paths, slice boundaries, migration steps, test strategy. This is the correct home for implementation-level detail.

### extraction-notes.md (when created)

- `source-of-truth`: always `false`
- `requires-approval`: `false`
- Status on creation: `draft` or `reviewed`
- Note: implementation-level patterns (compensation structures, retry logic, circuit breakers, outbox design) that apply to the current implementation belong in `implementation-plan.md`. Extraction notes are forward-looking reference material only.

### test-plan.md (when created)

- `source-of-truth`: always `false`
- `requires-approval`: `false`
- Add `derived-from: docs/spec/<feature>/spec.md` to frontmatter
- `derived-from` is not required for other artifact types yet — defer until test plan generation becomes common and spec versions diverge

### reconciliation-report.md (when written to disk)

- `source-of-truth`: always `false`
- `requires-approval`: `false`
- Status on creation: `draft`; update to `reviewed` once the report is confirmed complete
- File name: `reconciliation-report.md` in the feature folder
- Writing to disk is optional — the report may be produced inline in the conversation only. Write it to disk when a persistent record of the reconciliation result is useful (e.g., for review, audit, or handoff to another agent).
- Must contain: the structured report table, contradictions list, required-before-implementation list, and the final BLOCKED / CLEAR status per `instructions/workflows/artifact-reconciliation.instructions.md`

---

## Status Transition Rules

| Artifact | Transition | Trigger |
|---|---|---|
| `solution-options.md` | `draft` → `historical` | `decision.md` reaches `status: approved` |
| `domain-discovery.md` | no status change | `decision.md` approved; `source-of-truth` stays `false` |
| `decision.md` | `draft` → `approved` | Human explicit confirmation via architecture gate |
| `spec.md` | `draft` → `approved` | Human review AND reconciliation pass clean |
| `context-map.md` | `draft` → `approved` | Reconciliation pass confirms alignment with `decision.md` |
| `c4.md` | `draft` → `approved` | Same as `context-map.md` |
| `implementation-plan.md` | `draft` → `approved` | Human review after implementation readiness check passes |

---

## Validation Rules

The following frontmatter combinations are contradictory and must not appear on architecture-impacting artifacts:

| Combination | Why it is invalid |
|---|---|
| `requires-approval: false` + `status: approved` + `source-of-truth: true` | If no approval was required, the artifact cannot be both approved and authoritative. Apply to: `context-map`, `c4-diagram`, `decision`, `spec`. |
| `source-of-truth: true` + `status: draft` | A draft artifact is not ready to act on. `source-of-truth: true` requires `status: approved`. |
| `source-of-truth: true` + `status: historical` | A historical artifact is no longer current. It must have `source-of-truth: false`. |
| `source-of-truth: true` on `domain-discovery` or `solution-options` | These are always analysis artifacts. `source-of-truth` must always be `false`. |

An artifact with a known inconsistency must not be marked `approved`. Mark it `historical` with `source-of-truth: false` and create a corrected version if needed.
