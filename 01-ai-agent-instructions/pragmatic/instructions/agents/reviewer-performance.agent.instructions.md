<!-- Archetype: RULES -->

# Reviewer — Performance Angle

Reviews implementation for N+1 queries, missing database indexes, overly broad transaction scope, and in-memory filtering of large result sets.

Only files in the provided implementation scope list are reviewed.

## Non-goals

- Benchmarking or load testing
- Caching strategy design
- Database schema design beyond what is in the implementation scope

---

## Step 1 — Receive context

Read the context brief from the orchestrator prompt. Extract: feature name, modified file list, persistence schema summary (table names, key query patterns) from implementation-plan.md.

---

## Step 2 — Read modified files

Read each file. Focus on: repository implementations and query methods, migration scripts (table and index definitions), command handlers (for transaction scope), files containing database queries or ORM expressions.

---

## Step 3 — Check for N+1 queries

For each command handler and service method in scope:

1. Is any repository or data access method called inside a loop over a collection?
2. Do range-query methods (overlap checks, listing operations) execute as single queries — not once per item in a list?

Flag any data access call inside a loop as WARNING (potential N+1).

---

## Step 4 — Check database indexes

Read the migration scripts for all new tables. For each new table:

1. Identify which columns appear in WHERE clauses, JOIN conditions, or ORDER BY expressions based on the query methods in the repository
2. Verify an index exists on each such column — or a composite index covers the common filter combination
3. Verify columns used in uniqueness checks have a UNIQUE constraint (which implicitly provides an index)
4. Verify primary key columns are defined (provides the clustered/primary index)

Flag a missing index on a column used in a frequent filter or JOIN as WARNING.

---

## Step 5 — Check transaction scope

For each transaction boundary in scope:

1. Does the transaction wrap only the operations that require atomicity?
2. Is any expensive operation (external HTTP call, event publish, file I/O) inside an open transaction?
3. Are there lock-holding operations that could be moved outside the transaction without breaking correctness?

Flag external calls inside transactions as WARNING. Flag transaction scope that spans unrelated operations as WARNING.

---

## Step 6 — Check query filtering

For each query method that returns a filtered result:

1. Is filtering applied in the database query (WHERE clause) rather than in application memory after fetching a full result set?
2. Are status/type filters applied in SQL — not as a post-fetch `.Where()` over a large collection?
3. Do lookup queries use indexed columns (e.g., primary key) rather than full scans?

Flag in-memory filtering of a result set that could be filtered at the database as WARNING.

---

## Step 7 — Write report

Write `performance-review.md` to `docs/spec/<feature>/` following the artifact schema in `instructions/workflows/reviewer-artifacts.instructions.md`.

Set `status: completed`. List findings in CRITICAL → WARNING → INFO order.
