<!-- Archetype: WORKFLOW -->

# Solution Option Analysis

When a requirement may change the domain model or architecture, prepare option analysis before producing implementation tasks.

## Required input

First establish:

- confirmed acceptance criteria
- draft acceptance criteria
- open questions and their classification
- existing system behaviors
- affected domain concepts
- affected modules
- known architectural constraints

If acceptance criteria are missing or too weak, create draft acceptance criteria from available knowledge and mark them as draft.

## Infrastructure pattern recognition

Before proposing solution models, check whether any flagged architectural concern involves a generic infrastructure pattern. Generic infrastructure concerns have established open-source solutions that must be evaluated before designing custom implementations.

Infrastructure concerns include (not limited to):

- Authentication and session management
- OAuth / OIDC integration
- Event bus and in-process message dispatch
- Job scheduling and background processing
- Caching and distributed state
- API documentation and schema generation

For each infrastructure concern in scope, list candidate options using this evaluation format:

| Field | Content |
|---|---|
| Option | Library name and version line, or "Custom implementation" |
| License | SPDX identifier + commercial terms if relevant (e.g., `Apache 2.0`, `MIT`, `commercial above X RPS`) |
| Adoption cost | T-shirt size for integration effort in this codebase |
| Enables | Capabilities unlocked (future scenarios, patterns, extraction readiness) |
| Blocks | Patterns or decisions that become harder after adopting it |

Always include "Custom implementation" as one candidate. Include library candidates in the model description when they materially reduce the complexity sizing (e.g., building from scratch = XL vs. using an established library = L).

When a library introduces any architectural paradigm not yet in use in this system — meaning it changes how modules communicate, introduces new infrastructure components, or alters how state is managed or replicated — flag it explicitly:
> This option introduces [paradigm name] — an approach not currently used in this system. Confirm it as an architecture decision (Step 9 direction question) before selecting this option.

## Required options

Propose at least two models that represent meaningfully different trade-offs. Propose three when three meaningfully different trade-offs exist. Propose more than three when additional architecturally distinct options apply — do not cap at three when more real alternatives exist. Each additional model must represent a distinct trade-off, not a variation on an existing model. Use the names below as defaults when they fit. Replace them with names that better reflect the actual trade-off being made. Do not force a third model when only two trade-offs exist.

### 1. Minimal Change Model

The smallest safe implementation that can satisfy the current requirements.

This model may be acceptable for limited scope, but must clearly document domain or maintenance risks.

**Model intent:**

| Field | Content |
|---|---|
| Proposed because | Fastest path to satisfying current AC without structural change |
| Starting assumption | The feature is self-contained and unlikely to grow in ways that make the shortcut costly |
| Optimizes for | Delivery speed, low regression risk |
| Protects | Existing structure, minimal migration effort |
| Sacrifices | May preserve known ownership or lifecycle issues |
| Keeps open | All future structural options (no structural commitment made) |
| Makes harder | Adding domain rules, ownership clarity, or lifecycle independence later without revisiting this decision |
| Good fit when | Scope is small, future extension is unlikely, shortcuts are acceptable |
| Bad fit when | The feature will grow or the shortcut will become permanent |

### 2. Incremental Domain Model

A model that improves domain boundaries without requiring full restructuring.

This is often the preferred recommendation when it satisfies the AC and avoids the main ownership mistake.

**Model intent:**

| Field | Content |
|---|---|
| Proposed because | Fixes the most important domain or ownership issue without full restructuring |
| Starting assumption | One specific boundary fix removes the most significant risk without needing the full target architecture |
| Optimizes for | Domain correctness, future maintainability |
| Protects | Bounded context integrity, lifecycle ownership |
| Sacrifices | More effort than minimal change |
| Keeps open | The remaining path to the target domain model |
| Makes harder | Undoing the boundary fix if the assumption about scope turns out to be wrong |
| Good fit when | One domain boundary fix makes the design clearly better |
| Bad fit when | The full restructuring is needed anyway, making incremental a poor middle step |

### 3. Target Domain Model

The cleanest long-term model if cost, migration, and regression risk are acceptable.

This model must not be recommended only because it looks architecturally pure.

**Model intent:**

| Field | Content |
|---|---|
| Proposed because | Establishes the correct long-term model |
| Starting assumption | The strategic value of the clean model justifies the higher cost now |
| Optimizes for | Extensibility, architectural consistency, long-term maintenance |
| Protects | All domain boundaries, clear ownership, future variant support |
| Sacrifices | Higher implementation and migration cost, broader regression scope |
| Keeps open | All future capabilities that depend on clean boundaries and clear ownership |
| Makes harder | Rolling back or simplifying after the full restructuring is done |
| Good fit when | Strategic feature, upcoming related use cases, long-term foundation needed |
| Bad fit when | Scope is a one-off, cost is not justified, architectural purity drives the choice |

### Advanced Model (optional, scope-dependent)

An advanced model applies a paradigm shift — any architectural approach not currently
in use in the system that fundamentally changes how state is managed, how modules
communicate, or what infrastructure is required.

**Include an advanced model when:**

- The feature's domain characteristics make the paradigm a natural fit
- Adopting the paradigm now prevents a known expensive migration later
- The user's stated direction questions suggest future scenarios that the current model family cannot support cleanly

**Do not include an advanced model when:**

- The change is XS or S (local, no structural impact)
- The paradigm would require infrastructure changes (new broker, new store, schema migration) that are not proportional to the feature scope
- The paradigm is architecturally plausible but has no concrete advantage for the requirements at hand

**Additional model intent fields for advanced models:**

| Field | Content |
|---|---|
| Paradigm introduced | Name of the paradigm and what it changes about the current architecture |
| Infrastructure cost | New infrastructure required (broker, store, tooling) and t-shirt size |
| Lock-in risk | What becomes harder to undo if this paradigm is adopted |
| Enables | Future capabilities that become easier or possible |

Label advanced models clearly: `### Advanced Model — [Paradigm Name]`

## Must not include

Option analysis describes trade-offs, not implementation blueprints. Each model description must not include:

- exact method signatures (e.g., `IServiceName.DoOperation(resourceId, period, claimId) → Result`)
- interface definitions with parameter types (e.g., `Task<bool> IsAffectedBy(Guid resourceId, DateTimeOffset start, DateTimeOffset end, CancellationToken ct)`)
- endpoint paths (e.g., `POST /api/resources/{resourceId}/operations`)
- repository method names or rename operations (e.g., `TryAdd → Add`, `TryUpdate removed`)
- handler names or application service call sequences
- task breakdowns or implementation steps

Use capability-level descriptions instead. State what a module must be able to do, not how it does it.

Good:
- `[ModuleName]` must provide a capability to register a claim on `[ResourceName]` for a given period, rejecting conflicts atomically.
- `[ModuleName]` must be able to release a claim when the owning operation is cancelled.

Avoid:
- `IModuleNameService.RegisterClaim(resourceId, period, claimId, claimType) → Result`
- `CancelHandler` calls `IModuleNameService.ReleaseClaim(claimId)`

Exact method names and call sequences belong in `implementation-plan.md`, produced only after `decision.md` is approved.

## Evaluation fields

For each model include:

- description
- what changes
- what stays unchanged
- acceptance criteria coverage
- benefits
- risks
- implementation complexity (t-shirt size — see Complexity Sizing section in this file)
- domain risk
- agent-introduced concerns: new coupling, circular dependencies, integration inversions, or architectural problems this model introduces that are not present in the current system (must be listed explicitly; a model that introduces circular module dependency or bidirectional coupling cannot be recommended without explicitly flagging this as High domain risk)
- operational risk
- migration impact
- future maintenance impact
- decision scope

Include fields that materially affect the decision. For small-scope changes (XS/S size), limit to AC coverage, benefits, risks, and implementation complexity. Skip fields that would be identical or empty for all models.

Agent-introduced concerns must always be listed explicitly regardless of scope size. Do not skip this field for XS/S changes.

### Required checks for agent-introduced concerns

When a model involves cross-module integration or a new module with multiple consumers, run these checks explicitly:

**Single source of truth test:** Is there exactly one module that can answer the domain query (e.g., "what is the current state of this entity?", "can this actor perform this action?") without querying other modules at read time? If no — flag as High domain risk. Multiple modules owning overlapping state cannot enforce mutual exclusion and will produce inconsistent answers under concurrent access.

**Module boundary test:** Does any module in this model implement another module's own interface and inject that implementation back into the defining module? If yes — flag as High domain risk. This creates bidirectional coupling at runtime even when interfaces appear unidirectional. The defining module can never be extracted without creating a circular network dependency. See `instructions/core/ddd/context-map.instructions.md` — Module Boundary Rule.

**Module extraction test:** For each module in the proposed model, ask: if this module were deployed as a separate process, would replacing its public contracts with remote calls (HTTP/gRPC) create any circular call path? If yes — flag as High domain risk and identify which dependency direction must be reversed. Interface-level circularity is visible; service-level circularity from provider patterns is hidden and only becomes apparent at extraction.

**Context map pattern selection:** For each new or changed cross-module dependency, identify which integration pattern applies and state the reason. Use `instructions/core/ddd/context-map.instructions.md`. Unstated integration patterns are a maintenance risk — the next person to modify the integration will not know what constraints were intended.

## Complexity Sizing

Use t-shirt sizing to represent relative implementation complexity across solution models.

Size represents relative implementation complexity, architectural impact, uncertainty, and regression scope. It is not a time estimate.

| Size | Meaning |
|---|---|
| XS | Trivial local change. No domain decision, persistence change, or contract change. |
| S | Small local change inside one module. Known pattern, low uncertainty. |
| M | Moderate feature or rule change. One main module, some persistence, contract, or test impact. |
| L | Significant domain change. Affects lifecycle, aggregate boundaries, or multiple behaviors. |
| XL | Architectural change. Multiple modules, ownership changes, new contracts, migration, broad regression scope. |
| XXL | Too large for one implementation slice. Must be split before implementation. |

Usage rules:
- Assign a size to each solution model in option analysis.
- If a model is XL or XXL, propose smaller implementation slices.
- For small-scope changes (XS/S), simplify the evaluation — focus on AC coverage, benefits, risks, and complexity. Skip fields that would be identical or empty for all models.
- Do not use size as a tie-breaker between models that have different domain risk profiles. A smaller model with higher domain risk is not automatically better.

## Trade-off dimensions

Evaluate models across dimensions that materially affect the decision.

Mark a dimension as "not relevant" if it clearly does not apply — do not force a full table for every task.

Relevant dimensions to consider when they matter:

- implementation cost
- migration cost
- future maintainability
- domain correctness
- architectural consistency
- coupling and cohesion
- reversibility
- test scope and regression risk
- observability
- operational complexity
- performance and scalability
- security and compliance
- auditability and data consistency
- integration complexity
- developer experience
- risk of becoming a permanent shortcut
- ability to support future input or output channels
- ability to support future business variants or actors

## Future-scenario probing

Before recommending a model, the Spec Writer must assess whether future direction could change the choice.

Must ask when scope, future channels, business variants, or actors are not fixed:

- Are similar future use cases expected that may affect the same concept from other processes, actors, or integration points?
- Is this feature a one-off addition or the first step toward a more general capability?
- Are additional input or output channels likely?
- Are new business variants, actors, or external systems expected?
- Is the goal fastest delivery, MVP, production hardening, or long-term foundation?
- Are temporary shortcuts acceptable, or should the spec avoid solutions that may become permanent?

Use concrete examples when helpful, but mark them as examples — do not hardcode project-specific scenarios.

## Recommendation rule

Recommend the model that best satisfies, in order:

1. All confirmed acceptance criteria
2. User-stated priorities from direction questions
3. Known domain invariants and architectural constraints
4. Lowest implementation cost among models satisfying 1–3

Do not use cost as a tiebreaker against a stated priority.

If the model satisfying stated priorities is prohibitively costly, surface the conflict explicitly and ask for revised direction before recommending.

The recommendation must document the decision basis explicitly.

## Multi-step selection

When option analysis produces models with multiple independent sub-decisions (paradigm choice, then implementation approach, then library selection), split the decision into sequential direction questions:

1. **Paradigm question:** do you want to adopt [paradigm]?
2. **Conditional on step 1:** which implementation approach? (e.g., in-process vs. broker-based; orchestrated vs. choreographed)
3. **Conditional on step 2:** library selection with evaluation table

Do not present step 2 or step 3 until the previous level is confirmed. Do not pre-select a complete stack and ask for one-shot confirmation.

**Proportionality rule:** apply multi-step selection only when:

- The feature is M or larger in complexity
- The options contain at least one advanced model
- The sub-decisions are genuinely independent (accepting one does not imply the other)

For XS/S features, single-round confirmation is always sufficient. For M features without advanced models, single-round confirmation is preferred unless a library adoption decision is involved.

## Required human decision

If the recommended model changes architecture, domain ownership, contracts, or scope, follow the direction questions, recommendation, and confirmation steps in `instructions/workflows/spec-writer-flow.instructions.md` (Steps 9–11):

1. Ask direction questions before recommending
2. Recommend after the user answers
3. Ask for simple confirmation before generating the final spec
