# Instructions - Architecture Notes

This folder contains instruction sets for the `pragmatic/` variant: the three-level spec-writer routing system and the full cross-domain Spec Writer agent. The files are written for AI consumption - kept minimal and directive. This README is the human layer: it explains why each file exists and why it has its current shape.

---

## Workflow routing

Not every request needs the full Spec Writer process. Start with `/spec-writer` — it classifies the request and routes you to the right workflow.

| Level | Definition | Command |
|---|---|---|
| LOW | CRUD or display with no domain analysis required. Standard input guards (format, size, range, type) are fine. May touch multiple modules if the same known technical pattern applies independently in each. | `/spec-writer-simple` |
| MEDIUM | Existing business rules or invariants must be analyzed to implement correctly, within one business domain. May touch multiple modules technically if no cross-domain business coordination is required. | `/spec-writer-domain` |
| COMPLEX | Two or more business domains must coordinate on a rule, contract, or shared behavior — or ownership and source-of-truth are unclear across domains. | `/spec-writer-cross-domain` |

**When uncertain, run `/spec-writer`.** It evaluates seven signals (ambiguity, source-of-truth, ownership, blast radius, reversibility, cross-domain impact, domain analysis) and tells you which command to use next. When classification cannot be safely determined, it defaults to COMPLEX.

**Escalation is explicit and one-way.** Simple and Domain each define a scope guard and escalation rules. When hidden complexity surfaces, the agent stops, names what it found, and recommends `/spec-writer-cross-domain`. It does not silently absorb the complexity.

### Instruction files

| File | Archetype | Purpose |
|---|---|---|
| [`agents/spec-writer.agent.instructions.md`](agents/spec-writer.agent.instructions.md) | RULES + WORKFLOW | Classification rules, signal evaluation table, output format |
| [`agents/spec-writer-simple.agent.instructions.md`](agents/spec-writer-simple.agent.instructions.md) | RULES + WORKFLOW | LOW scope guard, checklist workflow, escalation rule |
| [`agents/spec-writer-domain.agent.instructions.md`](agents/spec-writer-domain.agent.instructions.md) | RULES + WORKFLOW | MEDIUM scope guard, spec + plan workflow, escalation rule |

These files use the compact shape: RULES and WORKFLOW are combined in one file because they have a single change reason and no shared instruction dependencies.

---

## Archetype system

Every file is tagged with one archetype in its first line (`<!-- Archetype: X -->`). The tag signals the file's single responsibility:

| Archetype | Role | Changes when |
|---|---|---|
| `MANIFEST` | Routing index — tells the agent what to load and when | A file is added, removed, or its load condition changes |
| `GUARDRAIL` | Behavioral boundary — what the agent may and must not do autonomously | A permission boundary or approval procedure changes |
| `CONTRACT` | Artifact schema, status vocabulary, and validation rules | Frontmatter fields, status values, or validation constraints change |
| `RULES` | Methodological constraints and analytical criteria | A discovery method, reasoning standard, or selection criterion changes |
| `WORKFLOW` | Process pipeline — ordered steps to execute a task | The task process changes |
| `KNOWLEDGE` | Domain facts and project-specific knowledge | The business, codebase, or known decisions evolve |

These are file roles, not a theory. `GUARDRAIL` and `CONTRACT` are not subcategories of `RULES` — they have different change triggers and different consumers. A `GUARDRAIL` file changes when a permission boundary changes. A `CONTRACT` file changes when a schema field or validation rule changes. A `RULES` file changes when a reasoning method changes. Mixing these into one archetype makes it harder to know why a file exists and when to update it.

Tool adapters (`.claude/`, `.github/`, `AGENTS.md`) use their own format and are not tagged. See the [manifest's adapter section](agents/spec-writer-cross-domain.manifest.md#files-that-are-adapters-only-do-not-load-as-instructions) for context.

The reason for the tag: a file that mixes two archetypes (e.g., contains both rules and workflow steps) introduces competing sources of truth. The LLM has to reconcile them instead of following one clear signal.

**Documented exception:** `domain-archetype-index.instructions.md` carries two tags — `<!-- Archetype: KNOWLEDGE -->` and `<!-- Role: INDEX -->`. It is KNOWLEDGE because it contains the signal table and confusion pairs. It is also an INDEX because it maps archetypes to group files and instructs the agent to load them in parallel. The routing is inseparable from the knowledge it routes, so splitting the file would gain nothing. The dual tag makes the hybrid role explicit rather than hiding it.

**Note on token cost:** The `<!-- Archetype: X -->` tags exist for human navigation and as part of the showcase — they make the file classification visible when reading the instruction set. The agent does not benefit from them: it already knows a file's role from its content and load condition in the manifest. In a production setup, remove them. Every HTML comment in every instruction file is wasted context on every load.

---

## Instruction authoring conventions

Use RFC-style modal verbs consistently. The distinction matters because the agent treats them differently.

| Verb | Meaning |
|---|---|
| `MUST` | Required. The agent is not allowed to skip or override this. |
| `MUST NOT` | Forbidden. No exceptions unless explicitly stated. |
| `SHOULD` | Preferred. The agent may deviate when there is a clear reason. |
| `MAY` | Optional. The agent can choose whether to apply this. |

Avoid vague phrasing:

- "consider" → say what the agent must check and what to do with the result
- "be careful" → name the specific risk and the required response
- "where appropriate" → define the condition explicitly

Every guardrail should define what the agent must do when it triggers, not just what it must not do.

Weak:
> Do not make architecture decisions.

Stronger:
> The agent MUST NOT approve architecture decisions. When an architecture decision is required, the agent MUST present options, recommend one, and follow the approval procedure in `instructions/workflows/architecture-gate.instructions.md`.

---

## Always-load budget

**[Always load](agents/spec-writer-cross-domain.manifest.md#always-load)** — Every file in this section costs a file read on every session, regardless of what the user asks for. Keep it small.

A file SHOULD NOT be always loaded if it contains:

- long catalogues or reference tables used only during one workflow step
- detailed examples or project-specific material
- large checklists consulted only in specific conditions

Such files SHOULD be loaded conditionally by workflow step or explicit need. The manifest's conditional-load sections exist for this reason. `domain-archetype-index.instructions.md` is always loaded; the full catalogue it points to is not.

---

## `agents/`

### [`spec-writer-cross-domain.manifest.md`](agents/spec-writer-cross-domain.manifest.md) - `MANIFEST`

The only file every cross-domain adapter is allowed to reference. When you add a new instruction file to the cross-domain workflow, you register it here. When a file becomes obsolete, you remove it here. No adapter maintains its own instruction list.

**[Always load](agents/spec-writer-cross-domain.manifest.md#always-load)** - Every file in this section costs a file read on every session, regardless of what the user asks for. Keep it small.

**[Conditional load sections](agents/spec-writer-cross-domain.manifest.md#load-when-starting-step-4-domain-discovery)** - Files here are loaded only when a specific workflow step confirms they're needed. This is the lazy-loading pattern: don't pay the context cost until you know the feature requires it. `aggregate-boundaries.instructions.md` is the clearest example - loaded only when domain discovery finds multiple aggregate candidates.

**[Forbidden combinations](agents/spec-writer-cross-domain.manifest.md#forbidden-combinations)** - Explicit prohibition against loading `solution-option-analysis.instructions.md` before Step 6 confirms an architectural concern. Without this rule, an aggressive agent might load option analysis for every feature, including trivial ones.

**[Adapter comment](agents/spec-writer-cross-domain.manifest.md#files-that-are-adapters-only-do-not-load-as-instructions)** - The adapter files (`.claude/agents/`, `.claude/commands/`, `.github/copilot-instructions.md`, `.github/agents/`, `AGENTS.md`) look like duplication but aren't. Each one targets a different runtime (Claude Code CLI, GitHub Copilot, Codex-compatible hosts). They all point to this manifest. Changing a rule in one instruction file propagates to all runtimes without touching any adapter - that's the point.

---

### [`spec-writer-cross-domain.agent.instructions.md`](agents/spec-writer-cross-domain.agent.instructions.md) - `RULES`

Defines what the agent *is* and what it's *not allowed to do*. Deliberately not a workflow file - it contains no steps, no pipeline. The distinction matters: an agent should know its constraints before it knows its process.

**[Decision levels](agents/spec-writer-cross-domain.agent.instructions.md#decision-levels)** - Four levels (Requirement → Domain → Architecture → Implementation) exist because LLMs tend to collapse them. A common failure mode is answering an unresolved architectural question with an implementation-level choice ("we'll just use a flag"). The four-level classification forces the agent to name the level before resolving it. Implementation decisions are safe to make autonomously. The other three require escalation or documentation.

**[Non-goals](agents/spec-writer-cross-domain.agent.instructions.md#non-goals)** - Negative constraints are more durable than positive ones. "The agent must not implement code" is clearer than "the agent should only specify." The last non-goal ("create a process so heavy that it is unusable for normal feature work") is the meta-constraint: the instruction set should not become a burden.

---

## `workflows/`

### [`spec-writer-cross-domain-flow.instructions.md`](workflows/spec-writer-cross-domain-flow.instructions.md) - `WORKFLOW`

The main pipeline. Fourteen steps, always loaded, always executed in order. The most important structural decisions:

**[Step 5 - Early termination](workflows/spec-writer-cross-domain-flow.instructions.md#step-5--early-termination-for-local-changes)** - If all three conditions are met (local change, no contract impact, no new domain concepts), the agent skips Steps 6–14 and goes straight to an implementation-ready spec. This exists because not every feature needs option analysis. A missing early exit causes the agent to run full architectural analysis on a one-line bug fix.

**[Step 6 vs Step 7](workflows/spec-writer-cross-domain-flow.instructions.md#step-6--detect-architectural-impact)** - These are deliberately separate. Step 6 decides whether an architectural concern exists. Step 7 proposes solution models *only if* Step 6 flagged a concern. Collapsing them into one step causes the agent to propose models even when no architectural decision is needed.

**[Steps 12–14 guard](workflows/spec-writer-cross-domain-flow.instructions.md#step-12--artifact-reconciliation-pass)** - The implementation pipeline has three sequential gates. Step 12 runs the artifact reconciliation pass — verifying all existing artifacts are consistent with `decision.md` and producing a visible BLOCKED / CLEAR report. Step 13 is the implementation readiness check — all conditions (approved `decision.md`, approved `spec.md`, clean reconciliation, no blocking open questions) must be true before proceeding. Step 14 generates the implementation plan only after Step 13 declares Ready. Without these gates, an agent that has recommended a model might generate tasks before the human has confirmed the direction, or before artifact contradictions are resolved.

**[Artifact status markers](workflows/spec-writer-cross-domain-flow.instructions.md#spec-artifact-status-markers)** - Every generated file gets YAML frontmatter with `status`, `source-of-truth`, and `requires-approval`. `historical` is the key status: a spec that was superseded must be explicitly marked so the agent doesn't treat an old document as current context.

---

### [`artifact-lifecycle.instructions.md`](workflows/artifact-lifecycle.instructions.md) - `CONTRACT`

Always loaded. Single source of truth for artifact classification, frontmatter schema, status vocabulary, per-artifact source-of-truth and approval rules, status transition triggers, and validation constraints.

**[Frontmatter schema](workflows/artifact-lifecycle.instructions.md#frontmatter-schema)** - Every file written to `docs/spec/` must begin with a YAML block declaring `artifact`, `status`, `source-of-truth`, `requires-approval`, and related artifact paths. A file without this block is non-compliant.

**[Authority hierarchy](workflows/artifact-lifecycle.instructions.md#authority-hierarchy)** - When two approved artifacts contradict each other, `decision.md` wins against all others. `spec.md` wins against derived artifacts. `implementation-plan.md` is authoritative only for task breakdown, not for architecture decisions. If a contradiction cannot be resolved using this hierarchy, the agent must stop and report rather than choose silently.

**[Validation rules](workflows/artifact-lifecycle.instructions.md#validation-rules)** - Specific frontmatter combinations are forbidden: `source-of-truth: true` with `status: draft`, `source-of-truth: true` with `status: historical`, `source-of-truth: true` on `domain-discovery` or `solution-options`. These rules exist because a draft or historical artifact that claims to be authoritative would give the agent a contradictory signal without any visible error.

**[Status transitions](workflows/artifact-lifecycle.instructions.md#status-transition-rules)** - Defines when each artifact transitions: `solution-options.md` becomes `historical` when `decision.md` is approved; `decision.md` becomes `approved` only on explicit human confirmation via the architecture gate. These transitions are not optional cleanup — they are part of the reconciliation pass.

---

### [`artifact-reconciliation.instructions.md`](workflows/artifact-reconciliation.instructions.md) - `RULES`

Loaded at Step 12. Defines the mandatory reconciliation pass that runs after `decision.md` reaches `status: approved` and before any implementation plan is generated.

**[Per-artifact checks](workflows/artifact-reconciliation.instructions.md#what-to-check-per-artifact)** - Each present artifact is checked for specific contradiction signals against `decision.md`: cross-module call directions, contract names, module ownership statements, component relationships. The check is concrete, not generic — the file lists what counts as a contradiction for each artifact type.

**[Working artifact status updates](workflows/artifact-reconciliation.instructions.md#working-artifact-status-updates)** - Part of the pass, not optional cleanup. `solution-options.md` must be marked `historical`. `domain-discovery.md` must have `source-of-truth: false`. Missing frontmatter on `extraction-notes.md` is a reconciliation error.

**[BLOCKED / CLEAR output](workflows/artifact-reconciliation.instructions.md#reconciliation-report)** - The pass must produce a visible structured report in the conversation before the workflow continues. The report ends with `Implementation plan generation: BLOCKED` or `CLEAR`. A clean pass does not mean all artifacts are perfect — it means none of them contradict the approved decision.

---

### [`solution-option-analysis.instructions.md`](workflows/solution-option-analysis.instructions.md) - `WORKFLOW`

Loaded conditionally (Step 6 must confirm an architectural concern). Describes how to construct and evaluate solution models.

**[Three default model names](workflows/solution-option-analysis.instructions.md#required-options)** - Minimal Change / Incremental Domain / Target Domain. These are starting names, not mandatory categories. The instruction says to replace them when they don't fit. The names exist because "Option A / Option B / Option C" forces the evaluator to stay abstract, while names like "Minimal Change" carry a direction signal that helps the human understand the trade-off without reading every field.

**[Model intent table](workflows/solution-option-analysis.instructions.md#1-minimal-change-model)** - Each model carries a structured intent block (Proposed because / Optimizes for / Sacrifices / Good fit when / Bad fit when). This exists to prevent option analysis from becoming a list of trade-offs with no recommendation signal. The "Bad fit when" field is the most important: it forces the author to describe the condition under which this model should *not* be chosen, which is where most architectural mistakes live.

**[Agent-introduced concerns](workflows/solution-option-analysis.instructions.md#evaluation-fields)** - This field is mandatory for every model at every size (including XS/S). An agent proposing a solution can accidentally introduce circular dependencies, bidirectional coupling, or integration inversions that didn't exist in the original system. Making this field required prevents the agent from burying new coupling inside an otherwise clean recommendation.

**[Complexity Sizing section](workflows/solution-option-analysis.instructions.md#complexity-sizing)** - Lives inside this file rather than a separate document because it has a single consumer and the table belongs next to the process that uses it.

**[Recommendation rule](workflows/solution-option-analysis.instructions.md#recommendation-rule)** - The ordering (confirmed AC → stated priorities → domain invariants → lowest cost) is intentional. Cost is the last tiebreaker, not the first. An agent that uses cost as the primary criterion will recommend the simplest model even when it violates a domain invariant.

---

### [`architecture-gate.instructions.md`](workflows/architecture-gate.instructions.md) - `RULES`

Single file for the entire guardrail: what the agent cannot do autonomously, and the exact procedure for getting human approval when it hits that boundary. Both concerns belong together because they describe the same decision point.

**[Not allowed list](workflows/architecture-gate.instructions.md#not-allowed-without-human-approval)** - The specific items (new module, moved behavior, new integration contract, saga) are there because each one represents a category of irreversible or hard-to-reverse change. An agent that introduces a saga pattern without human approval has committed the codebase to an eventual-consistency model that may be very expensive to undo.

**[Confirmation menu](workflows/architecture-gate.instructions.md#confirmation-procedure)** - Four options, not two (yes/no). Option 3 ("analyze with different priorities") and option 4 ("I want to provide my own direction") are the important ones. They allow the human to change the input to the analysis rather than just approving or rejecting the output. A binary approval gate pressures the human to accept a flawed recommendation or restart from scratch.

---

## `core/reasoning/`

### [`analysis-standards.instructions.md`](core/reasoning/analysis-standards.instructions.md) - `RULES`

One file for one cognitive task: how to reason about requirements that are ambiguous or incomplete. Covers both question classification and semantic ambiguity detection because both happen in the same phase of analysis and inform the same decision - whether to proceed or surface a blocker first.

**[Open question classification](core/reasoning/analysis-standards.instructions.md#open-question-classification)** - Three levels (Blocking / Non-blocking / Implementation detail). The Blocking level is the load-bearing one: a blocking question must be surfaced *before* domain discovery and model selection. An agent that silently converts a blocking question into an assumption will produce a spec built on an unverified foundation.

**[Unsafe assumption](core/reasoning/analysis-standards.instructions.md#assumption-risk-classification)** - The "Unsafe" category exists specifically for assumptions that should have been open questions. The list of triggers (scope, AC, lifecycle, ownership, contracts, billing, security) is deliberately long and redundant - it's a checklist, not a definition. The goal is to catch the assumption before it becomes a design decision.

**[Semantic ambiguity categories](core/reasoning/analysis-standards.instructions.md#semantics-to-check-when-relevant)** - The categories (Time, Identity vs display, Reference vs snapshot, Ownership, etc.) cover the most common sources of requirement ambiguity that silently affect domain model or architecture decisions. "Do not check all of these for every feature" is an explicit instruction - the list is a menu, not a mandatory scan.

---

## `core/ddd/`

### [`domain-discovery.instructions.md`](core/ddd/domain-discovery.instructions.md) - `RULES`

**[10 discovery questions](core/ddd/domain-discovery.instructions.md#discovery-questions)** - Each question probes a different signal for whether a concept deserves to be a first-class domain object. They're questions, not patterns, because applying patterns ("this looks like a Policy") leads to forcing archetypes onto concepts that don't need them. Questions lead to evidence; patterns lead to architecture for its own sake.

**[Simplest local alternative](core/ddd/domain-discovery.instructions.md#simplest-local-alternative)** - Before proposing extraction of any concept into a new aggregate, module, or bounded context, the agent must first describe what the implementation looks like if the concept stays where it is. This step exists to prevent the default reflex of "this sounds like a domain concept, therefore new module." The local model has to fail a concrete test before extraction is proposed.

**[Do not extract only because](core/ddd/domain-discovery.instructions.md#do-not-extract-only-because)** - The negative list (large class, many columns, technically complex, name sounds like a domain concept, code would look cleaner) is there because these are the most common justifications for premature extraction. Each one is a symptom of engineering aesthetics, not domain evidence.

---

### [`domain-archetype-index.instructions.md`](core/ddd/domain-archetype-index.instructions.md) - `KNOWLEDGE`

Always-loaded index. Contains usage rules, the behavioral signal table (match on behavior, not name), confusion pairs for choosing between similar archetypes, and a routing table mapping each archetype to its group file. The index instructs the agent to load only the relevant group files in parallel during Step 4.

### `archetypes/` — seven group files - `KNOWLEDGE`

Loaded dynamically during Step 4 following instructions in the index. Each file covers a thematic group of archetypes with full descriptions, decision criteria, and conceptual examples drawn from multiple industries. Never loaded in full — only the groups matching the current feature are loaded.

| File | Archetypes |
|---|---|
| `identity-organisation.instructions.md` | Party, Identity, Tenant, Role |
| `resources-location.instructions.md` | Resource, Location, Inventory, Movement, Route |
| `time-claims.instructions.md` | Reservation, Appointment, Availability, Block, Schedule |
| `value-commitments.instructions.md` | Policy, Quota, Accounting, Pricing, Ordering, Membership, Contract |
| `structure-meta.instructions.md` | Catalogue, Template, Batch, Versioned Entity |
| `process-decisions.instructions.md` | Workflow, Approval, Plan/Execution/Delta, Scenario |
| `quality-accountability.instructions.md` | Notification, Fault, Rating, Audit, Document, Territory |

---

### [`context-map.instructions.md`](core/ddd/context-map.instructions.md) - `RULES`

Loaded when Step 6 confirms an architectural concern. Defines integration patterns between bounded contexts — who defines the contract, how the downstream adapts, which Mermaid diagram syntax to use.

**[Pattern selection](core/ddd/context-map.instructions.md#strategic-patterns--who-defines-the-contract)** - Covers Shared Kernel, Open Host Service, Customer-Supplier, Conformist, Anti-Corruption Layer, and Published Language. The instruction is explicit: do not apply patterns by name — derive which fits from the selection criteria. Naming follows from evidence, not familiarity with pattern vocabulary.

**Why separate from `c4-diagrams.instructions.md`** - Context maps answer "who owns the interface and how does the downstream adapt." C4 diagrams answer "what are the structural components and how do they relate." Different questions, different consumers: context maps guide model selection during option analysis; C4 is generated after the decision is approved.

---

### [`c4-diagrams.instructions.md`](core/ddd/c4-diagrams.instructions.md) - `RULES`

Loaded only when Step 14 confirms the approved model changes module boundaries, bounded context boundaries, or cross-module contracts. Not loaded for local changes.

**[Level selection](core/ddd/c4-diagrams.instructions.md#level-selection)** - C2 (Container) is the minimum level. C3 (Component) is added only when the change reorganizes internal module structure. The file maps each C4 level to its Mermaid diagram type, required structural elements, and what each level must and must not show. Level mixing is prohibited.

**[Validation checklist](core/ddd/c4-diagrams.instructions.md#validation-checklist)** - Applied before the diagram is finalized to catch common errors: missing boundary declarations, unlabeled relationships, relationships that contradict `decision.md`.

---

### [`aggregate-boundaries.instructions.md`](core/ddd/aggregate-boundaries.instructions.md) - `RULES`

Loaded only when domain discovery has already identified multiple candidate aggregates or lifecycle overlap. It answers one question: given two or more candidates, where should the boundary be?

**[Consistency boundary analysis](core/ddd/aggregate-boundaries.instructions.md#consistency-boundary-analysis)** - The key question is which invariants must be enforced within a single transaction. This avoids the common failure mode of drawing boundaries by team ownership, by name similarity, or by "it felt natural." Two concepts belong in the same aggregate only when a business rule spans both and must never be violated, even temporarily.

**[Root access boundary](core/ddd/aggregate-boundaries.instructions.md#root-access-boundary)** - The root is the only entry point. If an inner entity needs to be addressed by its own ID from outside, or if commands consistently modify only one nested entity, the inner entity does not belong inside.

**[Size and contention signals](core/ddd/aggregate-boundaries.instructions.md#size-and-contention-signals)** - An aggregate is loaded as a whole. Unbounded collections (a collection that grows without a natural cap), high concurrency contention, and commands that consistently touch only a subset of the aggregate's state are runtime signals that the boundary is too coarse. The section includes the time-bounded aggregate pattern: instead of one long-lived aggregate accumulating infinite history, design short-lived aggregates that cover a bounded period and finalize cleanly.

**[Domain event signal](core/ddd/aggregate-boundaries.instructions.md#domain-event-signal)** - If an aggregate raises structurally unrelated domain events, it is likely coordinating multiple independent responsibilities.

**[Warning signs](core/ddd/aggregate-boundaries.instructions.md#warning-signs)** - An expanded checklist of smell patterns covering lifecycle violations, contention, partial-load patterns, and domain event misalignment.

---

## `project/booking/`

These two files are separated deliberately. Domain facts change when the business adds rules. Decisions change when the team accepts a new trade-off. Mixing them creates a file that's neither a reliable fact sheet nor a reliable decision log.

### [`domain-context.instructions.md`](project/booking/domain-context.instructions.md) - `KNOWLEDGE`

Module-level facts: what each module owns, what state transitions exist, what the cross-module contract is. No shortcuts, no trade-off language - those belong in `known-decisions.instructions.md`. If a fact here becomes intentionally wrong (e.g., deactivation becomes reversible), it moves to `known-decisions` as a documented decision, not a quiet edit here.

### [`known-decisions.instructions.md`](project/booking/known-decisions.instructions.md) - `KNOWLEDGE`

Intentional shortcuts with known limitations. Each entry has a Reason and a Known limitation. The Reason field exists so the agent knows *why* the shortcut was acceptable, which tells it whether the shortcut is still acceptable under a new feature request. The Known limitation field exists so the agent doesn't recommend building on top of the shortcut without surfacing the constraint.

The two current shortcuts (availability check embedded in repository, room status used directly) share the same reason: time pressure at MVP. This is honest documentation, not engineering pride. An agent reading this knows the architecture has a known debt and can flag it when a new feature makes that debt relevant.
