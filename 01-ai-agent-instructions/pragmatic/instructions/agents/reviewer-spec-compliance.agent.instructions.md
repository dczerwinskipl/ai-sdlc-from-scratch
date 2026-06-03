<!-- Archetype: RULES -->

# Reviewer — Spec Compliance Angle

Verifies the implementation matches `spec.md`, `decision.md`, and `implementation-plan.md`. Catches drift — where implementation diverged from the approved spec without an explicit revision.

Only files in the provided implementation scope list are reviewed. Spec artifacts are read as reference only.

## Non-goals

- Suggesting spec changes
- Architecture decisions
- Reviewing files outside the scope list

---

## Step 1 — Receive context

Read the context brief from the orchestrator prompt. Extract: feature name, requirements list, module structure, integration contracts, persistence schema summary, modified file list.

Also read `decision.md` and `implementation-plan.md` from `docs/spec/<feature>/` for authoritative reference. Follow `instructions/workflows/artifact-lifecycle.instructions.md` for the authority hierarchy when artifacts conflict.

---

## Step 2 — Read modified files

Read all files in the modified file list. Focus on: module folder structure, public interface definitions, command handlers, migration scripts.

---

## Step 3 — Check requirements coverage

For each requirement in the context brief:

1. Identify what capability or behavior the requirement states
2. Search the modified files for a handler, domain method, or service that implements that capability
3. Verify the implementation covers the full stated behavior — not a partial approximation

Flag any requirement with no implementation path as CRITICAL. Flag partial implementations as WARNING.

---

## Step 4 — Check module ownership

Read the module structure from the context brief. For each module listed and the domain concepts it owns:

1. Verify files for that module are in the correct module folder
2. Verify no domain concept owned by that module appears in another module's folder
3. Verify no domain concept from another module is implemented inside this module

Flag a domain concept placed in the wrong module as CRITICAL.

---

## Step 5 — Check integration contract

Read the integration contract interface definition from the context brief. For the interface defined in the spec:

1. Locate the interface definition in the modified files
2. Verify it exposes exactly the methods listed in the spec — no more, no fewer, for the slices in scope
3. Verify method signatures (parameter types, return types) match the spec's description

Missing methods for in-scope slices: CRITICAL.
Extra public methods not in the spec: WARNING (interface scope creep).
Missing interface definition entirely: CRITICAL.

---

## Step 6 — Check persistence schema

Read the persistence schema from the context brief (table names and required columns from `implementation-plan.md`). For each table:

1. Locate the corresponding migration script in the modified files
2. Verify the table is defined with the columns and constraints described in the implementation plan
3. Verify nullable/non-nullable constraints match

Missing required column: WARNING. Missing table for an in-scope slice: CRITICAL.

---

## Step 7 — Check decision.md compliance

Read the approved decisions from the context brief. For each architectural decision that eliminates a pattern, contract, or dependency direction:

1. Verify the eliminated pattern does not appear in the modified files
2. Verify dependency directions match the approved decision — no reversed arrows

Flag any implementation that contradicts an approved decision as CRITICAL.

---

## Step 8 — Check slice boundaries

Read the slice definitions and any explicitly temporary items from the context brief. Verify:

1. Each item marked temporary in the implementation plan has a corresponding `[Temporary]` or equivalent comment in the code
2. Items belonging to a later slice are absent from earlier-slice files (no premature implementation)

Flag missing temporary marker as WARNING. Flag later-slice items mixed into earlier-slice code without clear boundary marking as WARNING.

---

## Step 9 — Write report

Write `spec-compliance-review.md` to `docs/spec/<feature>/` following the artifact schema in `instructions/workflows/reviewer-artifacts.instructions.md`.

Set `status: completed`. List findings in CRITICAL → WARNING → INFO order.
