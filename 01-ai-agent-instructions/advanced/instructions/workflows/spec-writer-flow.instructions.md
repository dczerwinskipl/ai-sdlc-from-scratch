# Spec Writer Flow

## Goal

Turn collected knowledge and draft requirements into a reviewable solution spec.

The output does not need to be a final implementation artifact yet. At this stage, the main goal is correct reasoning, not perfect document formatting.

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

## Step 1 - Normalize requirements

Extract and organize:

- confirmed requirements
- draft requirements
- confirmed acceptance criteria
- draft acceptance criteria
- assumptions
- open questions
- out-of-scope items

If acceptance criteria are missing or weak, create draft acceptance criteria from the available knowledge and clearly mark them as draft.

## Step 2 - Scan known system context for impacted behavior

Analyze the requirements against known system context.

The goal is to detect whether the requested change touches existing behavior, rules, flows, integrations, or constraints that were not explicitly mentioned in the request.

The Spec Writer must not assume the request is isolated.

Review the available knowledge and project context for:

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
- whether it creates an acceptance criterion, open question, architectural concern, test case, or out-of-scope note

The Spec Writer must not:

- invent project-specific behavior that is not present in knowledge
- treat absence of information as proof that no related behavior exists
- ignore side effects only because the user did not mention them
- copy all known context into the spec
- expand the scope without marking it as requirement impact or open question

## Step 3 - Domain discovery

Identify affected domain concepts.

Analyze:

- language used in requirements
- nouns that may be real domain concepts
- lifecycle differences
- ownership of data
- aggregate boundaries
- module boundaries
- actor, requester, guest, customer, owner, payer, contact, and external system distinctions
- resource, availability, reservation, block, policy, and notification concepts
- whether common archetypes help ask better questions

Use domain archetypes as discovery aids, not as mandatory implementation patterns.

## Step 4 - Detect architectural impact

Explicitly decide whether the change affects architecture.

Flag an architectural concern when the requirement affects:

- module boundaries
- bounded contexts
- aggregate ownership
- data ownership
- transaction boundaries
- consistency model
- integration contracts
- public APIs
- domain events or integration events
- cross-module communication
- lifecycle ownership

## Step 5 - Propose solution models

If the change has architectural or domain impact, propose exactly three solution models:

1. Minimal Change Model
2. Incremental Domain Model
3. Target Domain Model

Each model must include:

- description
- what changes
- what stays unchanged
- acceptance criteria coverage
- benefits
- risks
- complexity size
- confidence
- implementation complexity
- domain risk
- operational risk
- migration impact

Use t-shirt sizing: XS, S, M, L, XL, XXL.

## Step 6 - Compare models against acceptance criteria

Compare each model against confirmed and draft acceptance criteria.

Architecture must be selected against requirements, not taste.

Use simple coverage labels:

- Full
- Partial
- No
- Unknown

## Step 7 - Recommend the simplest sufficient model

Recommend the simplest model that:

- satisfies confirmed acceptance criteria
- does not violate known domain invariants
- does not hide lifecycle differences
- does not assign ownership to the wrong module
- does not create unacceptable maintenance risk

The recommendation must explain:

- why this model
- why not the simpler model
- why not the more complete model
- main risks
- required approval
- what is blocked until approval

## Step 8 - Ask for human direction confirmation

Before producing implementation tasks, ask for human confirmation when the recommended solution affects architecture, domain model, or scope.

Present the decision in a compact form:

- recommended model
- why this model
- rejected alternatives
- main risks
- complexity size
- what will be implemented if approved
- what will stay out of scope

The Spec Writer must stop at the confirmation checkpoint when human approval is required.
It must not generate final implementation tasks in the same response unless the direction has already been approved.

Human confirmation is required when the recommendation changes or introduces:

- module boundaries
- bounded contexts
- aggregate ownership
- data ownership
- public contracts
- integration contracts
- transaction boundaries
- consistency model
- domain events or integration events
- feature scope beyond explicit requirements
- a new common archetype as part of the model

Human confirmation is not required for local implementation details that do not affect architecture, scope, contracts, or ownership.

## Step 9 - Generate implementation plan only after approval

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

## Expected output shape

The Spec Writer should produce a reviewable solution spec with the following sections:

1. Requirements and Acceptance Criteria
2. Existing System Impact
3. Domain Discovery
4. Architectural Impact
5. Solution Options
6. Acceptance Criteria Coverage
7. Recommendation
8. Human Direction Confirmation
9. Implementation Plan Status
