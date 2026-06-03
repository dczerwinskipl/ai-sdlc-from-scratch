<!-- Archetype: GUARDRAIL -->

# Reviewer Confirmation Gate

The Reviewer must stop and wait for human confirmation after all review reports are produced. It must not self-approve any report.

---

## Phase 1 — Critical findings

If any report contains a CRITICAL finding, present all of them grouped by angle before proceeding:

```
## Critical findings requiring acknowledgement

### [angle-name]
- [CRITICAL] <title> — `<file>` (line N)
  <one-line description>

Do you want to:
1. Acknowledge all critical findings and proceed to warnings
2. Stop — I will fix critical findings before approving
```

If the human chooses option 2: do not update any report status. All reports remain `status: completed`. The human re-runs the reviewer after fixes.

If there are no CRITICAL findings, skip Phase 1.

---

## Phase 2 — Warning findings

Present all WARNING findings grouped by angle:

```
## Warnings — confirm or defer

### [angle-name]
- [WARNING] <title> — `<file>` (line N)
  <one-line description>

These are non-blocking. Choose:
1. Accept all warnings — approve all reports
2. Accept with exceptions — I will specify which to defer
3. Defer all warnings — note them as deferred, approve reports
```

---

## After confirmation

When the human approves (Phase 2 option 1 or 3):

- Set `status: approved` on all confirmed reports
- Set `approved-by: <human name>` in each report's frontmatter
- Set `updated: <today's date>` in each report's frontmatter

When warnings are deferred (option 2 or 3):
- Append a `## Deferred findings` section to the relevant report listing each deferred item with a `DEFERRED` label
- Then set `status: approved`

---

## Hard stop

The Reviewer must not set `status: approved` on any report without an explicit human confirmation in this conversation.

A confirmation given in a previous conversation does not carry over. Each reviewer run requires its own gate pass.
