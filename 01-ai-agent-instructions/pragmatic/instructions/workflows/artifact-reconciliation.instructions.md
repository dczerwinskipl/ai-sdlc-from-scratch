<!-- Archetype: RULES -->

# Artifact Reconciliation Pass

The reconciliation pass is a mandatory workflow step that runs after `decision.md` reaches `status: approved`.

Its purpose is to verify that all existing artifacts for the feature are consistent with the approved decision before implementation planning begins.

The reconciliation pass is not optional. Implementation plan generation is blocked until it produces a clean result.

---

## When It Runs

- Immediately after `decision.md` is marked `status: approved` (Step 12 of the workflow)
- Again if `decision.md` is revised and re-approved at any later point — all prior reconciliation results are invalidated by a decision revision
- Before generating `implementation-plan.md` — always, without exception

---

## What to Check Per Artifact

### spec.md (when present)

Check every statement in Section 5 (Existing System Impact) against `decision.md`:

- Every cross-module call direction must match the dependency map in `decision.md`. If `decision.md` states "Module A has no outbound module dependencies", `spec.md` must not say "Module A calls Module B".
- No contract names that `decision.md` explicitly eliminated may remain (e.g., if `decision.md` replaced `IMaintenanceReader` with a push-based catalogue, `spec.md` must not reference `IMaintenanceReader`).
- Module ownership statements must match `decision.md`. If `decision.md` says Module X owns data Y, `spec.md` must not attribute ownership of Y to a different module.
- Draft requirements that must be confirmed before implementation must be clearly marked as draft and must not block `status: approved`.

Contradiction signal: any sentence in `spec.md` that contradicts a sentence in `decision.md`'s module boundary, dependency map, or ownership sections.

### context-map.md (when present)

- Integration arrow directions must match `decision.md`'s dependency map exactly.
- Contracts named on arrows must match contracts described in `decision.md`. No eliminated contracts.
- The "no outbound dependencies" constraint for any module `decision.md` names as self-contained must be reflected — no outbound arrows from that module.
- The OHS / Customer-Supplier pattern labels must be consistent with `decision.md`'s module relationship descriptions.

### c4.md (when present)

- Component relationships and arrows must match `decision.md`'s module boundary changes.
- No components that were eliminated or merged by `decision.md` may appear.
- No dependency arrows contradicting `decision.md`'s stated direction.

### implementation-plan.md (when present)

- No task may implement an architectural pattern that `decision.md` explicitly rejected.
- Interface names and dependency directions in task descriptions must match `decision.md`.
- No task may introduce a new module or contract that `decision.md` did not approve.

---

## Working Artifact Status Updates

These updates are part of the reconciliation pass — not optional clean-up.

| Artifact | Required action |
|---|---|
| `solution-options.md` | Change `status` to `historical`. |
| `domain-discovery.md` | Verify `source-of-truth: false`. If `true`, flag as a reconciliation error and correct. |
| `extraction-notes.md` | Verify frontmatter block exists with `source-of-truth: false`. If missing, flag as non-compliant. |

---

## Reconciliation Report

The reconciliation pass must produce a visible structured report before the workflow continues. Produce it inline in the conversation.

```
## Reconciliation Report — [feature-name]

decision.md: status: approved ✓

| Artifact             | Present | Aligned with decision.md | Action taken              |
|----------------------|---------|--------------------------|---------------------------|
| spec.md              | yes/no  | YES / NO — [reason]      | [corrected / flagged / —] |
| context-map.md       | yes/no  | YES / NO — [reason]      | [corrected / flagged / —] |
| c4.md                | yes/no  | YES / NO — [reason]      | [corrected / flagged / —] |
| implementation-plan.md | yes/no | YES / NO — [reason]     | [corrected / flagged / —] |
| solution-options.md  | yes/no  | n/a (analysis)           | marked status: historical |
| domain-discovery.md  | yes/no  | n/a (analysis)           | verified source-of-truth: false |
| extraction-notes.md  | yes/no  | n/a (reference)          | frontmatter verified / flagged |

Contradictions found:
- [list each contradiction or "none"]

Required before implementation:
- [list each required correction or "none"]

Implementation plan generation: BLOCKED / CLEAR
```

If there are no contradictions and all working artifacts are correctly classified, the report ends with `CLEAR`. Otherwise it ends with `BLOCKED` and lists what must be resolved.

---

## Blocking Conditions

Implementation plan generation is blocked when any of the following are true:

- Any final artifact (`spec.md`, `context-map.md`, `c4.md`) carries content that contradicts `decision.md`
- Any final artifact has `source-of-truth: true` but has not been reconciled against `decision.md`
- `solution-options.md` has not been marked `status: historical`
- `domain-discovery.md` has `source-of-truth: true`
- `extraction-notes.md` is missing frontmatter
- Any contradiction in the report is marked as unresolved

---

## What a Clean Pass Looks Like

A clean reconciliation pass means:

- All final artifacts present have been checked and contain no statements contradicting `decision.md`
- All working artifacts have correct status and source-of-truth values
- The reconciliation report ends with `Implementation plan generation: CLEAR`
- No required corrections are listed

A clean pass does not mean all artifacts are perfect — it means none of them contradict the approved decision.

---

## Correcting Contradictions

When a contradiction is found in a final artifact:

1. Identify the specific section and statement that contradicts `decision.md`.
2. Correct it to reflect the approved decision. Do not preserve the pre-decision description.
3. Note the correction in the reconciliation report under "Action taken".
4. Do not change `decision.md` to match the artifact — `decision.md` wins.

If a contradiction cannot be resolved without human input (e.g., `decision.md` is ambiguous on a specific point), surface it as a blocking question before proceeding.
