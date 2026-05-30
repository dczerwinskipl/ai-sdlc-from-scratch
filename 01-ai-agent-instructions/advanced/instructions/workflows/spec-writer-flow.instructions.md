<!-- Type: workflow -->

# Spec Writer Flow

## Goal

Turn collected knowledge and draft requirements into a reviewable solution spec.

The output does not need to be a final implementation artifact yet. At this stage, the main goal is correct reasoning, not perfect document formatting.

Keep the process proportional to the task. Not every feature needs all steps at full depth.

## Inputs

The Spec Writer receives:

- collected knowledge or discovery notes
- draft requirements
- draft acceptance criteria
- existing system context
- known behaviors
- known architectural decisions
- project instructions
- DDD and architecture instructions

## Step 1 — Normalize requirements

Extract and organize:

- confirmed requirements
- draft requirements
- confirmed acceptance criteria
- draft acceptance criteria
- assumptions
- open questions
- out-of-scope items

If acceptance criteria are missing or weak, create draft acceptance criteria from available knowledge and clearly mark them as draft.

Do not finalize AC until Step 2 is complete.

Minimum output:
- organized list of requirements (confirmed vs draft)
- organized list of AC (confirmed vs draft)
- initial list of open questions and assumptions to classify in Step 2

## Step 2 — Identify open questions and classify assumptions

Before proceeding, classify open questions and assumptions.

Follow `instructions/core/reasoning/open-questions-and-assumptions.instructions.md`.

Blocking open questions must be surfaced before domain discovery and model selection.

If a blocking question cannot be answered yet, document the unresolved status and the risk of proceeding without it.

Do not convert a blocking open question into a silent assumption.

Minimum output:
- each open question classified as Blocking / Non-blocking / Implementation detail
- each assumption classified as Low-risk / High-impact / Temporary / Unsafe
- for Blocking questions: why it blocks, what it affects, risk of proceeding without it

## Step 3 — Scan known system context for impacted behavior

Analyze the requirements against known system context.

The goal is to detect whether the requested change touches existing behavior, rules, flows, integrations, or constraints that were not explicitly mentioned.

The Spec Writer must not assume the request is isolated.

Review available knowledge and project context for:

- existing business flows related to the requested change
- existing rules or invariants that may be affected
- existing side effects triggered by similar actions
- existing integrations or downstream consumers
- existing notifications, audit, reporting, billing, permissions, or compliance behavior
- existing state transitions or lifecycle rules
- existing data ownership and source-of-truth assumptions
- existing public contracts or APIs
- existing operational constraints
- known architectural decisions
- known exceptions, shortcuts, or technical debt

For each relevant context item, document:

- what existing behavior or rule was found
- why it may matter for the requested change
- whether it creates an AC, open question, architectural concern, test case, or out-of-scope note

The Spec Writer must not:

- invent project-specific behavior not present in knowledge
- treat absence of information as proof that no related behavior exists
- ignore side effects only because the user did not mention them
- copy all known context into the spec
- expand scope without marking it as requirement impact or open question

Minimum output:
- list of impacted existing behaviors or rules (with why each matters for this change)
- any new AC, open questions, or out-of-scope items surfaced during context scan

## Step 4 — Domain discovery

Identify affected domain concepts.

Analyze:

- language used in requirements
- nouns that may be real domain concepts
- lifecycle differences
- ownership of data
- aggregate and module boundaries
- actor distinctions
- whether common archetypes help ask better questions

Use domain archetypes as discovery aids, not as mandatory implementation patterns.

The Spec Writer must check for high-impact semantic ambiguity that may affect AC, domain model, or architecture.
Follow `instructions/core/reasoning/high-impact-semantics.instructions.md`.

Minimum output:
- list of affected domain concepts with initial lifecycle and ownership notes
- any detected semantic ambiguities (classified per open-questions-and-assumptions)
- any domain extraction candidates flagged for Step 6

## Step 5 — Early termination for local changes

Before option analysis, assess whether the change is:

- purely local (inside one module, no ownership or lifecycle change)
- does not affect public contracts, integration points, or domain invariants
- does not introduce or change domain concepts

If all three conditions are met, the Spec Writer may skip Steps 6–12 and produce a direct implementation-ready spec.

The Spec Writer must document why option analysis was skipped.

If any condition is uncertain, do not skip — continue with Steps 6–12.

Minimum output:
- explicit statement: option analysis skipped / option analysis proceeding
- if skipped: documented reason why all three conditions are met
- if proceeding: which condition was uncertain and why

## Step 6 — Detect architectural impact

Explicitly decide whether the change affects architecture.

Flag an architectural concern when the requirement affects:

- module boundaries
- bounded contexts
- aggregate or data ownership
- transaction boundaries
- consistency model
- integration or public contracts
- domain or integration events
- cross-module communication
- lifecycle ownership

When aggregate or lifecycle boundaries are in scope, apply `instructions/core/ddd/aggregate-lifecycle.instructions.md`.

Minimum output:
- explicit statement: architectural concern flagged / not flagged
- for each flagged concern: what is affected and why it matters

## Step 7 — Propose solution models

If an architectural concern was flagged in Step 6, propose two or three solution models that represent meaningfully different trade-offs.

Use the Minimal Change / Incremental Domain / Target Domain framing as defaults when it fits. Replace the names when it does not. Do not force a third model when only two trade-offs exist.

Each model must include a model intent block explaining:

- why this model was proposed
- starting assumption: what belief about the future this model depends on being correct
- what it optimizes for
- what it protects
- what it intentionally sacrifices
- keeps open: what future directions remain possible if this model is chosen
- makes harder: what future directions become more costly or locked out
- when it is a good fit
- when it is a bad fit

Follow `instructions/workflows/solution-option-analysis.instructions.md` for full evaluation requirements.

Minimum output: follow the field definitions in `instructions/workflows/solution-option-analysis.instructions.md`.

## Step 8 — Compare models against acceptance criteria

Compare each model against confirmed and draft acceptance criteria.

Architecture must be selected against requirements, not taste.

Use coverage labels: Full, Partial, No, Unknown.

Evaluate models across relevant trade-off dimensions. Mark a dimension as "not relevant" if it does not materially affect the decision — do not force a full table for every task.

Minimum output:
- AC coverage table (model × AC item × coverage label)
- trade-off dimension table with relevant dimensions only

## Step 9 — Ask direction questions before recommending

Before selecting a recommendation, ask targeted decision questions when the model choice depends on human priorities or future direction.

Ask about what the user values most, for example:

- delivery speed vs architectural improvement
- minimal scope vs improving architecture as part of the change
- whether future related use cases are expected
- whether additional input or output channels are likely
- whether shortcuts are acceptable
- whether the goal is MVP, production hardening, or long-term foundation

Do not ask vague questions. Ask decision-oriented questions.

Example only — replace with questions appropriate to the specific decision:

> Is your priority to deliver the smallest safe change now, or are you willing to spend more effort to improve the domain boundary as part of this feature?

> Do you expect similar future use cases that may affect the same concept from other processes, actors, or integration points?

Skip this step if the model choice is clearly determined by the AC and no human trade-off applies.

Minimum output:
- targeted direction questions asked (specific to this decision, not generic)
- user's answers, or explicit note that the step is awaiting response

## Step 10 — Recommend a model based on answers

After the user answers direction questions, recommend a model.

The recommendation must include:

- selected model
- why this model fits the stated priorities
- why the simpler model was rejected
- why the more complete model was rejected
- key trade-offs accepted
- risks that remain
- open questions that still matter
- decision basis

Document the decision basis explicitly. For example:

> Decision basis: The user preferred improving the domain boundary as part of this feature but did not want the cost of the full target architecture.

Minimum output:
- recommended model with decision basis
- explicit rejection rationale for each non-selected model

## Step 11 — Ask for simple confirmation

After recommending a model, follow the confirmation procedure defined in `instructions/workflows/human-direction-confirmation.instructions.md`.

Present:

- recommended model and why
- what will be implemented if approved
- what stays out of scope
- decisions the human must make now
- decisions that can be deferred
- unresolved open questions that matter

Ask:

```
Do you want me to use this direction for the final spec?

1. Yes, use this model.
2. No, revise the models.
3. Analyze the options again with different priorities.
4. I want to provide my own direction.
```

The Spec Writer must stop here when human approval is required.

It must not generate final implementation tasks in the same response unless the direction has already been approved.

For the full list of conditions that require human approval before proceeding, see `instructions/workflows/architecture-approval-gate.instructions.md`.

Minimum output:
- presented summary of recommended model, scope, deferred items, and unresolved questions
- confirmation prompt presented per `instructions/workflows/human-direction-confirmation.instructions.md`

## Step 12 — Generate implementation plan only after approval

If the selected model requires human approval, do not generate final implementation tasks yet.

Allowed before approval:

- safe preparatory tasks
- documentation tasks
- characterization test suggestions
- open questions
- decision summary

Allowed after approval:

- implementation plan
- persistence changes
- API or contract changes
- test plan
- implementation tasks

When the final spec is generated, include a decision record:

- selected model and who confirmed it
- decision basis
- alternatives considered and why they were rejected
- remaining assumptions and open questions
- any modifications made after the original option analysis
- accepted trade-offs: what the approved model deliberately sacrifices
- deferred decisions: what was identified but intentionally left for a later iteration

Minimum output (before approval):
- list of safe preparatory tasks or explicit "blocked" status for implementation tasks
- decision summary with pending confirmation noted

Minimum output (after approval):
- complete implementation plan with all required tasks
- decision record with all required fields

## Expected output shape

1. Requirements and Acceptance Criteria
2. Open Questions and Assumptions
3. Existing System Impact
4. Domain Discovery
5. Architectural Impact
6. Solution Options
7. Acceptance Criteria Coverage
8. Recommendation
9. Direction Confirmation
10. Decision Record
11. Implementation Plan Status
