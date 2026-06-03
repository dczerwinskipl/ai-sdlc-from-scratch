<!-- Archetype: CONTRACT -->

# Reviewer Artifacts

This file defines the schema, status vocabulary, and format rules for all review artifacts.

All review artifacts are written to `docs/spec/<feature>/`.

---

## File naming

```
<angle>-review.md
```

Valid angles: `security`, `architecture`, `code-quality`, `performance`, `test-coverage`, `acceptance-criteria`, `spec-compliance`, `data-integrity`

---

## Frontmatter schema

Every review artifact must begin with this YAML block:

```yaml
---
artifact: review
angle: security | architecture | code-quality | performance | test-coverage | acceptance-criteria | spec-compliance | data-integrity
status: draft | completed | approved
created: YYYY-MM-DD
updated: YYYY-MM-DD
approved-by: ~
feature: <feature-name>
scope: [list of reviewed files]
---
```

A review file without this frontmatter block is non-compliant.

---

## Status vocabulary

| Status | Meaning |
|---|---|
| `draft` | Review in progress. Subagent has not finished writing. |
| `completed` | All findings written. Awaiting human confirmation via the gate. |
| `approved` | Human confirmed findings. `approved-by` is set to the approver's name. |

Subagents set `status: completed` when they finish writing.
The orchestrator sets `status: approved` only after the gate in `reviewer-gate.instructions.md` is cleared.

---

## Finding format

Each finding follows this structure:

```markdown
### [CRITICAL|WARNING|INFO] <Title>

**File:** `path/to/file.cs` (line N — or "N/A" for file-level findings)
**Details:** What was found and why it matters.
**Recommendation:** Specific action, or explicit note that deferring is acceptable.
```

---

## Report structure

```markdown
---
[frontmatter]
---

# <Angle> Review — <Feature>

## Summary

| Level | Count |
|---|---|
| CRITICAL | N |
| WARNING | N |
| INFO | N |

## Findings

[findings in CRITICAL → WARNING → INFO order]

## Deferred findings

[Added by orchestrator after human selects deferred items — omit section if empty]
```

---

## Validation rules

| Combination | Invalid because |
|---|---|
| `status: approved` with `approved-by: ~` | Approval requires a named approver |
| `status: draft` as final subagent output | Subagents must write `status: completed` when done |
| Missing `scope` field | Every report must record which files were reviewed |
| `status: approved` without gate confirmation | Self-approval is forbidden per `reviewer-gate.instructions.md` |
