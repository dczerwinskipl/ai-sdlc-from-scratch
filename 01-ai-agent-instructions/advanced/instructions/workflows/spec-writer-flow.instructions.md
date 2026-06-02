<!-- Archetype: WORKFLOW -->

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

**Actor context check:** When the feature introduces new externally visible functionality (new API endpoints, new user interactions, new external integrations), check whether the actors who will use this capability are documented in project knowledge files (domain context, known decisions, or provided knowledge). If actor information is missing, add to the open questions list for Step 2. Skip this check when the change is purely internal — no new external surfaces, no new user interactions.

Minimum output:
- organized list of requirements (confirmed vs draft)
- organized list of AC (confirmed vs draft)
- initial list of open questions and assumptions to classify in Step 2
- actor context flag: documented / missing / not applicable (internal change)

## Step 2 — Identify open questions and classify assumptions

Before proceeding, classify open questions and assumptions.

Follow `instructions/core/reasoning/analysis-standards.instructions.md`.

Blocking open questions must be surfaced before domain discovery and model selection.

If a blocking question cannot be answered yet, document the unresolved status and the risk of proceeding without it.

Do not convert a blocking open question into a silent assumption.

**Actor and channel questions:** When actor context was flagged as missing in Step 1, classify the actor question here:
- **Blocking** — when the feature's scope, AC, or access rules depend on knowing which actors are involved, or when different actor types would have meaningfully different workflows or permissions
- **Non-blocking** — when the same actors as existing functionality clearly apply, with no new access or workflow differences
- **Skip** — when the change is purely internal and no actor information is needed for AC or diagrams

When classifying actor questions, also ask: are there alternative channels or output formats relevant to this feature (e.g., will it be accessed via mobile, webhook, batch, or a different user role)? If yes, document as a non-blocking question unless it directly affects AC or model selection.

Minimum output:
- each open question classified as Blocking / Non-blocking / Implementation detail
- each assumption classified as Low-risk / High-impact / Temporary / Unsafe
- for Blocking questions: why it blocks, what it affects, risk of proceeding without it
- actor context: confirmed actors list, or explicit note that actor question is deferred / not applicable

## Step 3 — Scan known system context for impacted behavior

Analyze the requirements against known system context.

The goal is to detect whether the requested change touches existing behavior, rules, flows, integrations, or constraints that were not explicitly mentioned.

The Spec Writer must not assume the request is isolated.

Review available knowledge and project context for:

- existing actors and their roles in affected flows (who initiates the action, who receives the outcome, who is notified)
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
- whether a structural problem being addressed in this feature (ownership mismatch, lifecycle confusion, boundary inconsistency) also exists in other modules

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
- describe module interfaces, contracts, or implementation approaches that depend on a model not yet selected

Minimum output:
- list of impacted existing behaviors or rules (with why each matters for this change)
- any new AC, open questions, or out-of-scope items surfaced during context scan
- adjacent improvement candidates: for each structural problem found in other modules that mirrors what is being fixed here, list: location, problem description, incremental cost estimate (t-shirt delta vs. the selected model), and recommendation (fix now / defer / track as tech debt). May be empty. Do not expand scope silently — present candidates for explicit user decision at Step 11.

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
Follow `instructions/core/reasoning/analysis-standards.instructions.md`.

Minimum output:
- list of affected domain concepts with initial lifecycle and ownership notes
- any detected semantic ambiguities (classified per `instructions/core/reasoning/analysis-standards.instructions.md`)
- any domain extraction candidates flagged for Step 6

## Step 5 — Early termination for local changes

Before option analysis, assess whether the change is:

- purely local (inside one module, no ownership or lifecycle change)
- does not affect public contracts, integration points, or domain invariants
- does not introduce or change domain concepts

If all three conditions are met, the Spec Writer may skip Steps 6–14 and produce a direct implementation-ready spec.

The Spec Writer must document why option analysis was skipped.

If any condition is uncertain, do not skip — continue with Steps 6–14.

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

When aggregate or lifecycle boundaries are in scope, apply `instructions/core/ddd/aggregate-boundaries.instructions.md`.

Minimum output:
- explicit statement: architectural concern flagged / not flagged
- for each flagged concern: what is affected and why it matters

## Step 7 — Propose solution models

If an architectural concern was flagged in Step 6, propose at least two solution models that represent meaningfully different trade-offs. Propose more when more distinct alternatives exist — see `instructions/workflows/solution-option-analysis.instructions.md` for the full option set including advanced models.

Use the Minimal Change / Incremental Domain / Target Domain framing as defaults when it fits. Replace the names when it does not. Do not force a third model when only two trade-offs exist.

Each model must include a model intent block — see the format in `instructions/workflows/solution-option-analysis.instructions.md`.

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

When a model introduces any architectural paradigm or library not currently in the system, ask the paradigm-level question first — do you want to adopt this approach? — then ask the library-level question only after the paradigm is confirmed. A paradigm is any approach that changes how modules communicate, introduces new infrastructure components, or alters how state is managed. Do not combine paradigm acceptance and library selection into one question. The user may accept a paradigm and still reject the first library proposed in favor of a different implementation.

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

Steps 10 and 11 describe what changes and why — not how it is implemented. Do not include code samples, class definitions, method signatures, DTO structures, or call sequences before Step 14. Describe capabilities and structural decisions in plain language.

Minimum output:
- recommended model with decision basis
- explicit rejection rationale for each non-selected model

## Step 11 — Ask for simple confirmation

Present:

- recommended model and why
- what will be implemented if approved
- what stays out of scope
- decisions the human must make now
- decisions that can be deferred
- unresolved open questions that matter
- adjacent improvement candidates accepted or deferred (when surfaced in Step 3)

Do not include code samples, class definitions, method signatures, DTO structures, or call sequences in the confirmation summary. See Step 10 — the same rule applies here.

Then follow the confirmation procedure defined in `instructions/workflows/architecture-gate.instructions.md`.

The Spec Writer must stop here when human approval is required.

It must not generate final implementation tasks in the same response unless the direction has already been approved.

For the full list of conditions that require human approval before proceeding, see `instructions/workflows/architecture-gate.instructions.md`.

Minimum output:
- presented summary of recommended model, scope, deferred items, and unresolved questions
- confirmation prompt presented per `instructions/workflows/architecture-gate.instructions.md`

## Step 12 — Artifact Reconciliation Pass

After `decision.md` reaches `status: approved`, run the artifact reconciliation pass before generating any post-decision artifacts.

Follow `instructions/workflows/artifact-reconciliation.instructions.md` for the full checklist, output format, and blocking conditions.

The pass must:

1. Check all existing final artifacts (`spec.md`, `context-map.md`, `c4.md`, `implementation-plan.md`) for contradictions with `decision.md`.
2. Update working artifact statuses: mark `solution-options.md` as `status: historical`; verify `domain-discovery.md` has `source-of-truth: false`; verify `extraction-notes.md` has frontmatter.
3. Produce a visible reconciliation report in the conversation.
4. End with `Implementation plan generation: BLOCKED` or `CLEAR`.

The workflow must not proceed to Step 13 while the report shows `BLOCKED`.

Minimum output:
- reconciliation report produced per `instructions/workflows/artifact-reconciliation.instructions.md`
- explicit BLOCKED or CLEAR status

## Step 13 — Implementation Readiness Check

Evaluate whether all conditions for implementation are met. Produce a visible readiness declaration.

Implementation must not start unless all of the following are true:

1. `decision.md` is either not required (local change, no architectural impact) or has `status: approved` and `source-of-truth: true`
2. `spec.md` has `status: approved` and `source-of-truth: true`
3. The reconciliation report from Step 12 shows `CLEAR`
4. All blocking open questions are resolved or explicitly accepted as non-blocking
5. If `implementation-plan.md` exists for this feature, it has `status: approved`

If `spec.md` is `historical` or `source-of-truth: false`, implementation is blocked regardless of other artifact states.

Produce a visible readiness declaration using one of:

- **Ready** — all conditions met; implementation can proceed
- **Ready after minor clarification** — no blocking contradictions; one or more non-blocking questions remain; list them
- **Blocked — open question** — a blocking question is unresolved; name it
- **Blocked — artifact inconsistency** — a final artifact contradicts `decision.md`; name the artifact and the contradiction
- **Blocked — reconciliation incomplete** — Step 12 produced `BLOCKED`; list required corrections

Minimum output:
- implementation readiness declaration with status and reason
- list of any remaining items if not fully ready

## Step 14 — Generate implementation plan only after approval

If the selected model requires human approval, do not generate final implementation tasks yet.

Allowed before approval:

- safe preparatory tasks
- documentation tasks
- characterization test suggestions
- open questions
- decision summary

Allowed after approval:

- `implementation-plan.md` — ordered implementation tasks, interface definitions, API paths, slice boundaries, test strategy
- `context-map.md` — bounded context map (when the change affects BC or module boundaries, ownership, or cross-module contracts)
- `c4.md` — C4 container-level diagram of affected modules (same condition as context map)
- persistence changes
- API or contract changes
- test plan

When the final spec is generated, include a decision record:

- selected model and who confirmed it
- decision basis
- alternatives considered and why they were rejected
- remaining assumptions and open questions
- any modifications made after the original option analysis
- accepted trade-offs: what the approved model deliberately sacrifices
- deferred decisions: what was identified but intentionally left for a later iteration

### Architectural diagrams

Generate `context-map.md` and `c4.md` when the approved model changes bounded context boundaries, module boundaries, aggregate ownership, or cross-module contracts.

Do not generate for local changes (Step 5 early termination) or when the change is fully contained within one module without touching its public contracts.

**`context-map.md`**
- Follow `instructions/core/ddd/context-map.instructions.md` for diagram format, pattern labeling, and integration flows.
- When generating the first context-map for a system, show all bounded contexts.
- When generating for a feature that extends an existing system with a prior context-map artifact, show only relationships that are new or changed. Reference the prior artifact for unchanged relationships by path. Do not repeat the full module map.
- Highlight contexts affected by this change.
- When the feature introduces new cross-module contracts, include an Integration Flows section with per-use-case sequence diagrams. Use "Module Map & Integration Flows" as the document title in that case.
- Use Mermaid diagram format.

**`c4.md`**
- Follow `instructions/core/ddd/c4-diagrams.instructions.md` for level selection, required structural elements, Mermaid syntax, and validation checklist.
- Generate at C2 level as the minimum. Add C3 only when the change reorganizes internal module structure.
- Show new or changed cross-module contracts and integration points.

### Artifact boundary rules

**`decision.md`** must not contain:
- method signatures or interface definitions (e.g., `TryRegisterBlock(roomId, period, blockId) → Result`)
- repository method names or rename operations (e.g., `TryAdd → Add`, `TryChangePeriod removed`)
- endpoint paths
- handler names or call sequences (e.g., "Updated `CreateRoomHandler`: calls `RegisterRoom` after persist")
- task breakdowns or implementation steps

It may mention module names, domain concepts, integration contract names, and existing component names when needed to explain the architectural decision. The capability a contract provides is appropriate; its exact method signatures are not. Implementation specifics belong in `implementation-plan.md`.

**`spec.md`** — the Existing System Impact section must describe the post-decision impact of the selected model only. Pre-decision model analysis belongs in `solution-options.md`. Do not carry pre-decision impact speculation, unselected model artifacts, or implementation interface predictions into `spec.md`.

Minimum output (before approval):
- list of safe preparatory tasks or explicit "blocked" status for implementation tasks
- decision summary with pending confirmation noted

Minimum output (after approval):
- complete implementation plan with all required tasks
- decision record with all required fields

## Spec artifact status markers

Every artifact written to `docs/spec/` must include a YAML frontmatter block.

Follow `instructions/workflows/artifact-lifecycle.instructions.md` for:
- the complete frontmatter schema
- status vocabulary
- artifact classification (working / final / implementation / reference)
- per-artifact source-of-truth and requires-approval rules
- status transition rules (e.g., when solution-options.md becomes historical)
- validation rules (contradictory frontmatter combinations)

## Expected conversation output shape

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
11. Reconciliation Report
12. Implementation Readiness Declaration
13. Implementation Plan
