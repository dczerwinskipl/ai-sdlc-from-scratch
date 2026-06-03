<!-- Archetype: RULES + WORKFLOW -->

# Spec Writer Simple

Handles pre-classified LOW complexity requests: changes that only expose, map, or display existing known behavior.

## Role

Produce a minimal implementation checklist and test checklist. Review the result against the request. Do not perform domain discovery. Do not produce full feature specs.

## Scope guard

Before producing any checklist, confirm all three conditions:

1. Business meaning of the affected concept is documented and uncontested.
2. Source-of-truth is a single known module or system.
3. The change does not add new business logic — it only exposes, maps, or displays what already exists.

If any condition cannot be confirmed: escalate to `/spec-writer-cross-domain`. State which condition failed and why. Do not attempt to resolve the ambiguity within this workflow.

## Escalation rule

If a hidden assumption surfaces at any step that changes the scope of the change, STOP.

Report:
- The assumption found
- Which signal it affects (business meaning / source-of-truth / ownership / blast radius / reversibility / cross-domain)
- Whether to escalate to MEDIUM (`/spec-writer-domain`) or COMPLEX (`/spec-writer-cross-domain`)

Do not continue the checklist after escalating.

## Non-goals

MUST NOT:

- Produce a feature spec or full acceptance criteria document
- Perform domain discovery
- Propose new architectural patterns or module dependencies
- Expand scope beyond what is confirmed in Step 1

---

## Inputs

- Required: the user's request
- Required: confirmation that the request is pre-classified as LOW (or arrived via `/spec-writer`)
- Optional: reference to the module, file, or system that owns the behavior

## Outputs

- Scope confirmation (in conversation)
- `docs/spec/<feature-slug>/checklist.md` — implementation and test checklist written to disk
- Review result (in conversation)
- Escalation notice (if triggered)

---

## Workflow

### Step 1 — Confirm scope

State explicitly:

- **Business meaning:** what this concept or behavior is, in plain language (quote from known project documentation or context if available)
- **Source-of-truth:** which module, service, or data store owns this behavior or data
- **Owner:** which module or team is responsible

If any of these cannot be stated from available context: escalate to `/spec-writer-cross-domain`. Do not proceed past this step.

### Step 2 — Implementation checklist

Produce an ordered checklist of implementation steps.

Each item must:
- Name the specific code location or layer to change (e.g., query handler, read model, API endpoint)
- Describe the change in one line
- Flag any item that requires reading an existing contract or schema before proceeding

Do not include speculative steps or steps conditional on decisions not yet made.

[TODO: If the project uses a standard layer structure (e.g., CQRS handlers, repository pattern, specific naming conventions), reference that structure so the checklist uses the correct terminology and file locations.]

### Step 3 — Test checklist

Produce a minimal test checklist.

Each item must:
- Name what is being verified
- Name the test type (unit, integration, contract, end-to-end)
- Note any dependency on fixtures or data setup

Keep tests proportional: LOW changes do not require full test plans. Cover each changed code path. One passing assertion per path is the target.

### Step 4 — Write artifact and review

Write the implementation and test checklist to `docs/spec/<feature-slug>/checklist.md`. Derive the feature slug from the request (e.g. "add photo to room" → `room-photo`).

Then present the checklist in conversation and ask the user to confirm:

> Does this checklist correctly represent the change you need? Is any step missing or incorrect?

If the user confirms: the artifact is ready.

If the user identifies a missing step or unexpected complexity: evaluate whether the request has grown beyond LOW scope. Update the file if the checklist changes. If scope has grown, escalate.

### Step 5 — Escalation (if triggered)

When escalating:

1. Name the assumption or complexity found
2. State which signal it affects
3. Recommend the next command: `/spec-writer-domain` (MEDIUM) or `/spec-writer-cross-domain` (COMPLEX)
4. Stop. Do not continue the checklist.
