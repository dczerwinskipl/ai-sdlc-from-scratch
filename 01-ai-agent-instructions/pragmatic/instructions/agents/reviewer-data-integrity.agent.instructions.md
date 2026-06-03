<!-- Archetype: RULES -->

# Reviewer — Data Integrity Angle

Verifies transaction boundaries, concurrent access correctness, idempotency guarantees, migration safety, and compensation call structure.

Only files in the provided implementation scope list are reviewed.

## Non-goals

- Database performance (that is the performance reviewer's responsibility)
- Testing for race conditions (that is the test-coverage reviewer's responsibility)

---

## Step 1 — Receive context

Read the context brief from the orchestrator prompt. Extract: feature name, modified file list, integration contracts (to identify operations that must be atomic), persistence schema (to identify tables with uniqueness invariants), slice definitions.

---

## Step 2 — Read modified files

Read all files. Focus on: command handlers that perform multiple writes, implementations of idempotent operations defined in the integration contracts, migration scripts, transaction management patterns.

---

## Step 3 — Check transaction boundaries

Read the integration contracts from the context brief. For each operation that the spec describes as requiring atomicity (operations that modify multiple stores or must succeed or fail together):

1. Locate the command handler in the modified files
2. Verify both writes are wrapped in a single database transaction
3. Verify no operation runs outside the transaction boundary (e.g., a write called after `SaveChanges`/commit)

Flag any pair of logically atomic writes running as separate operations as CRITICAL.

---

## Step 4 — Check concurrent access

For operations the spec or implementation plan describes as requiring mutual exclusion (preventing two concurrent requests from both succeeding for the same resource and period):

1. Locate the implementation of the conflict check + write operation
2. Verify the check and the write are in the same transaction
3. Verify the transaction isolation level or locking mechanism is sufficient to prevent two concurrent operations from both passing the conflict check — `READ COMMITTED` alone is not sufficient for check-then-insert patterns
4. [TODO: verify the actual isolation level or lock type used in this codebase]

Flag a check-then-insert pattern without sufficient isolation as CRITICAL.

For replace-style operations (deactivate old + insert new): verify both steps are in the same transaction.

---

## Step 5 — Check idempotency implementation

From the integration contracts in the context brief, identify all operations described as idempotent. For each:

1. Locate the implementation in the modified files
2. Verify the method checks for an existing matching record before inserting and returns success (no-op) on duplicate
3. Verify delete/deactivate operations succeed silently when the target is already deactivated or not found

Flag missing idempotency on any operation described as idempotent in the spec as CRITICAL.

For push operations (write-time state synchronization between modules): verify they use upsert semantics, not plain insert.

---

## Step 6 — Check uniqueness constraints

Read the persistence schema from the context brief. For each table where the spec or implementation plan states a one-to-one invariant between two entities (e.g., one claim per reservation):

1. Locate the migration script in the modified files
2. Verify a UNIQUE constraint is defined on the column that enforces that invariant at the database level
3. Application-level enforcement alone is insufficient — the DB constraint is the safety net

Flag a missing uniqueness constraint on a column that enforces a one-to-one invariant as CRITICAL.

---

## Step 7 — Check migration safety

For each new table's seeding or migration script in the modified files:

1. Is the seed script idempotent — does it use upsert semantics rather than plain INSERT?
2. Does it handle a non-empty target table safely (re-runnable on deployment failure)?
3. Are non-nullable columns defined with appropriate NOT NULL constraints?

Flag a non-idempotent seed script as WARNING. Flag missing NOT NULL on a column the domain treats as required as WARNING.

---

## Step 8 — Check compensation call structure

For operations that modify multiple stores atomically, the implementation plan may specify compensation calls (reverse operations called in the failure path, dormant in a monolith but load-bearing after service extraction):

1. Verify compensation calls are present in the failure path of any such handler
2. Verify they are marked with a comment explaining they are currently dormant and when they become load-bearing
3. [TODO: confirm the comment convention used for dormant compensation calls in this codebase]

Flag missing compensation call structure as WARNING.

---

## Step 9 — Write report

Write `data-integrity-review.md` to `docs/spec/<feature>/` following the artifact schema in `instructions/workflows/reviewer-artifacts.instructions.md`.

Set `status: completed`. List findings in CRITICAL → WARNING → INFO order.
