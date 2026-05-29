# Solution Option Analysis

When a requirement may change the domain model or architecture, the Spec Writer must prepare option analysis before producing implementation tasks.

## Required input

First establish:

- confirmed acceptance criteria
- draft acceptance criteria
- open questions
- existing system behaviors
- affected domain concepts
- affected modules
- known architectural constraints

If acceptance criteria are missing or too weak, create draft acceptance criteria from available knowledge and mark them as draft.

## Required options

Propose exactly three models:

### 1. Minimal Change Model

The smallest safe implementation that can satisfy the current requirements.

This model may be acceptable for limited scope, but must clearly document domain or maintenance risks.

### 2. Incremental Domain Model

A model that improves domain boundaries without requiring full restructuring.

This is often the preferred recommendation when it satisfies the acceptance criteria and avoids the main ownership mistake.

### 3. Target Domain Model

The cleanest long-term model if cost, migration, and regression risk are acceptable.

This model must not be recommended only because it looks architecturally pure.

## Required evaluation

For each model include:

- description
- what changes
- what stays unchanged
- acceptance criteria coverage
- benefits
- risks
- implementation complexity
- domain risk
- operational risk
- t-shirt size
- confidence
- migration impact
- future maintenance impact
- decision scope

## T-shirt sizing

Use:

- XS
- S
- M
- L
- XL
- XXL

Size represents relative implementation complexity, architectural impact, uncertainty, and regression scope.
It is not a time estimate.

Guidance:

- XS: trivial local change, no domain decision, persistence change, or contract change
- S: small local change inside one module, known pattern, low uncertainty
- M: moderate feature or rule change, one main module, some persistence, contract, or test impact
- L: significant domain change, affects lifecycle, aggregate boundaries, module boundaries, or multiple behaviors
- XL: architectural change, multiple modules, ownership changes, new contracts, migration, broad regression scope
- XXL: too large for one implementation slice, must be split before implementation

If a model is XL or XXL, propose smaller implementation slices.

## Recommendation rule

Recommend the simplest model that:

- satisfies all confirmed acceptance criteria
- does not violate known domain invariants
- does not hide lifecycle differences
- does not assign ownership to the wrong module
- does not create unacceptable future maintenance risk

The recommendation must not be based only on the smallest size.

A model may be low effort but high domain risk.
A model may be higher effort but lower long-term domain risk.

## Required human decision

If the recommended model changes architecture, domain ownership, contracts, or scope, ask for human direction confirmation before implementation planning.
