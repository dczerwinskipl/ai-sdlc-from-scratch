<!-- Archetype: RULES + WORKFLOW -->

# Spec Writer Domain

Handles pre-classified MEDIUM complexity requests: changes that extend one existing module or capability with known ownership and known source-of-truth.

## Role

Produce a small feature spec, acceptance criteria, implementation plan, and test plan. Review the result against the request. Escalate if the change turns out to require COMPLEX treatment.

## Scope guard

Before producing any spec, confirm all three conditions:

1. The business concept being extended is documented and understood.
2. A single module or team owns this capability.
3. The change does not affect cross-module contracts, domain events, bounded context boundaries, or data ownership.

If any condition cannot be confirmed: escalate to `/spec-writer-cross-domain`. State which condition failed and why. Do not attempt to resolve the ambiguity within this workflow.

## Escalation rule

If the change is found to touch multiple domains, contested ownership, source-of-truth ambiguity, or architecture boundaries at any step: STOP and escalate to `/spec-writer-cross-domain`.

Report:
- What was found
- Which boundary was crossed (ownership / source-of-truth / contract / cross-domain / architecture)

Do not continue the plan after escalating.

## Non-goals

MUST NOT:

- Perform full domain discovery (that is COMPLEX work, handled by `/spec-writer-cross-domain`)
- Propose new bounded contexts, aggregates, or module reorganizations
- Evaluate multiple architectural models
- Approve architecture decisions
- Silently expand scope beyond the named module

---

## Inputs

- Required: the user's request
- Required: confirmation that the request is pre-classified as MEDIUM (or arrived via `/spec-writer`)
- Optional: module name, owning team, relevant existing files, contracts, or interfaces

## Outputs

- Scope confirmation (in conversation)
- `docs/spec/<feature-slug>/spec.md` — feature description and acceptance criteria
- `docs/spec/<feature-slug>/implementation-plan.md` — ordered task list and test plan
- Escalation notice (if triggered)

---

## Workflow

### Step 1 — Confirm scope

State explicitly:

- **Business concept:** what is being extended, in plain language
- **Module owner:** which module or team owns this capability
- **Source-of-truth:** which data store or system holds the authoritative state for the affected data
- **Boundary:** what will NOT change — module boundaries, contracts, events, ownership

If any item cannot be confirmed from available context: escalate to `/spec-writer-cross-domain`. Do not proceed past this step.

### Step 2 — Feature spec

Produce a small feature spec:

- What the feature does (one paragraph, capability description — not implementation approach)
- What it does not do (explicit out-of-scope list)
- Which existing behavior or module it extends
- Which existing contracts or interfaces it relies on (read-only reference — do not propose changes to them here)

Do not describe implementation approach in this step.

### Step 3 — Acceptance criteria

Produce acceptance criteria.

Each criterion must:
- Describe an observable behavior, not internal state
- Reference the actor and the expected outcome
- Be scoped to the named module only

Mark each criterion as:
- **Confirmed** — directly derivable from the request
- **Draft** — inferred from context, requires user confirmation

Do not finalize acceptance criteria until the user reviews them.

### Step 4 — Implementation plan

Produce an ordered implementation task list.

Each task must:
- Name the layer and location (e.g., command handler, repository method, read model, API endpoint)
- Describe the change in one sentence
- Note any dependency on another task

Keep the plan proportional: MEDIUM changes should be completable within one or two working sessions. Do not produce an exhaustive breakdown.

[TODO: If this project has a standard slice structure or task template (e.g., vertical slices, CQRS task grouping), reference that structure so the task list uses the correct terminology and reflects the actual implementation pattern.]

### Step 5 — Test plan

Produce a test plan.

For each acceptance criterion, name:
- Test type (unit, integration, end-to-end)
- What behavior is being verified
- Any required fixtures or data dependencies

### Step 6 — Write artifacts and review

Write the artifacts to disk. Derive the feature slug from the request (e.g. "confirmation cascade" → `reservation-confirmation`).

- `docs/spec/<feature-slug>/spec.md` — feature description and acceptance criteria from Steps 2–3
- `docs/spec/<feature-slug>/implementation-plan.md` — task list and test plan from Steps 4–5

Then present both artifacts in conversation and ask the user to confirm:

> Does this plan correctly represent the change you need? Is any acceptance criterion missing? Do any implementation tasks touch outside the named module?

If the user confirms: the artifacts are ready.

If the user identifies scope growth (new module touched, contract change needed, ownership question raised): update the files to reflect corrections, then escalate to `/spec-writer-cross-domain`.

If the user requests changes that remain within MEDIUM scope: update the files and re-present.

### Step 7 — Escalation (if triggered)

When escalating:

1. Name what was found
2. State which boundary was crossed (ownership / source-of-truth / contract / cross-domain / architecture)
3. Recommend: `/spec-writer-cross-domain`
4. Stop. Do not continue the plan.
