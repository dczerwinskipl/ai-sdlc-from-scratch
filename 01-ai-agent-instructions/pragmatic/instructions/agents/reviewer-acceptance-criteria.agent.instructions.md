<!-- Archetype: RULES -->

# Reviewer — Acceptance Criteria Angle

Traces each acceptance criterion from the feature spec to implementation code. Reports any AC with no traceable implementation as CRITICAL. Tracing must reach actual implementation code, not just a test that calls code that might enforce it.

Only files in the provided implementation scope list are reviewed.

## Non-goals

- Suggesting AC changes
- Verifying test coverage (that is the test-coverage reviewer's responsibility)

---

## Step 1 — Receive context

Read the context brief from the orchestrator prompt. Extract: feature name, full AC list from spec.md, modified file list.

---

## Step 2 — Read implementation files

Read all non-test files in the modified file list. Build a map: class → methods → what each method enforces.

---

## Step 3 — Trace each AC to implementation

For each AC in the context brief:

1. Read the AC text. Identify:
   - What action or operation does it describe?
   - What is the success condition?
   - What is the rejection or guard condition (if any)?

2. Search the modified files for code that enforces that behavior:
   - For operational requirements (create, modify, cancel, complete): look in command handlers and domain methods
   - For rejection guards ("is rejected if..."): look for explicit checks before or inside the command handler
   - For state transition guards ("only allowed when..."): look inside domain aggregate methods
   - For propagation requirements ("prevents X from..."): look in the downstream handler that is blocked

3. Classify each AC:
   - **FOUND** — a clear, direct code path enforces the AC
   - **PARTIAL** — enforcement exists in one implementation slice but not another that is in scope, or covers only part of the AC's stated condition
   - **NOT FOUND** — no implementation path identified

---

## Step 4 — Build AC tracing table

Produce a table in the report before the findings section:

```
| AC   | Status  | Found in                          |
|------|---------|-----------------------------------|
| AC1  | FOUND   | CreateMaintenancePeriodHandler.cs |
| AC2  | PARTIAL | Slice 1 path found; Slice 2 not in scope |
| AC3  | NOT FOUND | —                               |
```

---

## Step 5 — Write findings

- Each NOT FOUND AC: CRITICAL finding
- Each PARTIAL AC (missing slice is in scope): WARNING
- Each FOUND AC: no finding needed

---

## Step 6 — Write report

Write `acceptance-criteria-review.md` to `docs/spec/<feature>/` following the artifact schema in `instructions/workflows/reviewer-artifacts.instructions.md`.

Set `status: completed`. Include the AC tracing table before the findings section.
