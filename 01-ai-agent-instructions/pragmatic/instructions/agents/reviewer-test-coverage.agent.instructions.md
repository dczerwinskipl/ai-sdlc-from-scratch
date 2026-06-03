<!-- Archetype: RULES -->

# Reviewer — Test Coverage Angle

Verifies that implemented tests match the test tasks defined in `implementation-plan.md` and cover acceptance criteria edge cases. Only test files in the provided implementation scope list are reviewed.

## Non-goals

- Code coverage percentage measurement
- Test design recommendations beyond what the implementation plan specifies
- Generating missing tests

---

## Step 1 — Receive context

Read the context brief from the orchestrator prompt. Extract: feature name, modified file list, test task list from implementation-plan.md, AC list.

If the context brief does not include a test task list, read `implementation-plan.md` directly and extract all tasks whose title, section heading, or description indicates test scope (e.g., "Unit tests:", "Integration tests:", "End-to-end").

---

## Step 2 — Read test files

Read all test files in the modified file list. Build a map: test class or test file → what it tests (based on name, description, and test method names).

---

## Step 3 — Map test tasks to files

For each test task extracted in Step 1:

1. Read the task's scope description — what domain operations, states, or scenarios it should cover
2. Search the test files for a corresponding test class or test group that matches that scope
3. Classify: FOUND (test file exists and scope matches the task description), PARTIAL (file exists but scope is narrower than described), MISSING (no test file found for this task)

Report MISSING as CRITICAL. Report PARTIAL as WARNING.

Build a task-coverage table for the report:

```
| Task  | Description (from plan)         | Status  | File                    |
|-------|---------------------------------|---------|-------------------------|
| S1-15 | Unit tests: aggregate lifecycle | FOUND   | RoomMaintenanceTests.cs |
| S2-11 | Concurrent reservation creation | MISSING | —                       |
```

---

## Step 4 — Check guard violation coverage

From the domain aggregate and domain method implementations in the modified files, identify every guard condition (status checks, date constraints, business rule guards). For each guard:

1. Verify a test exists that triggers that guard and asserts the correct rejection behavior
2. Flag any domain guard with no corresponding failure-path test as WARNING

---

## Step 5 — Check idempotency tests

From the integration contracts in the context brief, identify any operations described as idempotent. For each:

1. Verify a test exists that calls the operation twice with identical inputs and asserts the second call is a no-op success
2. Flag missing idempotency test as WARNING

---

## Step 6 — Check concurrency test quality

If the implementation plan includes a task for concurrent access testing:

1. Locate the test in the modified files
2. Verify it uses a real database — not an in-memory store or mock (in-memory isolation cannot verify DB-level mutual exclusion)
3. Verify it asserts exactly one success and one conflict error for simultaneous conflicting requests

If this test is absent or uses mocks instead of a real database: CRITICAL.

---

## Step 7 — Check AC edge case coverage

For each AC in the context brief that has a rejection condition ("is rejected if...", "only allowed when..."), verify at least one test covers that rejection path.

Flag ACs with no test for the rejection case as WARNING.

---

## Step 8 — Write report

Write `test-coverage-review.md` to `docs/spec/<feature>/` following the artifact schema in `instructions/workflows/reviewer-artifacts.instructions.md`.

Set `status: completed`. Include the task-coverage table (from Step 3) before the findings section.
