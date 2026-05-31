# Spec Writer Instruction System — Refactor Proposal

## Overall Assessment

**Usable but needs cleanup.**

The instruction system has a strong core: genuine phase separation in the workflow, conditional loading logic, an explicit approval gate, and clean reusable reasoning instructions. The example spec demonstrates the workflow works.

The structural problem is specific and fixable: the canonical loading logic in `spec-writer.agent.instructions.md` is bypassed by every adapter. All four tool-facing files maintain independent hardcoded instruction lists that ignore the conditional loading rules entirely. This is not a minor inconsistency — it means the carefully designed conditional loading has never been enforced by any real tool invocation. As the instruction set grows, these four lists will drift independently.

Three instruction files are also orphaned (never listed in any loading rule or adapter). One example artifact contradicts the decision that was recorded in the same feature. And no generated artifact carries a status header despite the workflow requiring one.

The fix is not a rewrite. It is making the manifest canonical and the adapters thin.

---

## Current State Map

| Path | Current Role | Source of Truth or Adapter | Risk / Comment |
| :--- | :--- | :--- | :--- |
| `instructions/agents/spec-writer.agent.instructions.md` | Canonical role definition; file loading rules | **Source of Truth** | Loading rules are bypassed by every adapter. Conditional logic is never enforced. |
| `instructions/workflows/spec-writer-flow.instructions.md` | Full 12-step workflow | **Source of Truth** | Core. Well-structured. |
| `instructions/workflows/solution-option-analysis.instructions.md` | Option analysis sub-workflow | **Source of Truth** | Core. References `tshirt-sizing.instructions.md` inline but that file is never loaded. |
| `instructions/workflows/architecture-approval-gate.instructions.md` | Approval guardrail | **Source of Truth** | Core. Clean. |
| `instructions/workflows/human-direction-confirmation.instructions.md` | Confirmation checkpoint procedure | **Source of Truth** | Core. Correctly referenced by both flow and gate. |
| `instructions/core/reasoning/open-questions-and-assumptions.instructions.md` | Question/assumption classification | **Source of Truth (reusable)** | Well-defined. Always loaded. |
| `instructions/core/reasoning/high-impact-semantics.instructions.md` | Semantic ambiguity detection | **Source of Truth (reusable)** | Present in all adapters but **missing from `spec-writer.agent.instructions.md` loading rules**. Source of truth doesn't list it. |
| `instructions/core/ddd/domain-discovery.instructions.md` | Domain discovery reasoning | **Source of Truth (reusable)** | **Orphaned. Not listed in any adapter or loading rule.** Referenced nowhere. |
| `instructions/core/ddd/aggregate-lifecycle.instructions.md` | Aggregate lifecycle analysis | **Source of Truth (reusable)** | Conditionally loaded in agent; always loaded in all adapters. Condition is bypassed. |
| `instructions/core/ddd/domain-archetypes.instructions.md` | Archetype detection | **Source of Truth (reusable)** | Same bypass issue. |
| `instructions/core/estimation/tshirt-sizing.instructions.md` | Estimation sizing guide | **Source of Truth (reusable)** | **Orphaned. Referenced inside `solution-option-analysis.instructions.md` but not loaded by any adapter or loading rule.** |
| `instructions/project/booking/domain-context.instructions.md` | Project-specific domain facts | **Source of Truth (project-specific)** | Always loaded. Good separation from generic instructions. |
| `instructions/project/booking/known-decisions.instructions.md` | Documented shortcuts and trade-offs | **Source of Truth (project-specific)** | **Orphaned. Not in any loading rule or adapter.** Yet referenced by the example spec's decision record. |
| `.claude/agents/spec-writer.md` | Claude subagent definition | **Adapter (Claude-specific)** | Contains full hardcoded instruction list. Bypasses conditional loading. |
| `.claude/commands/spec-writer.md` | Claude slash command | **Adapter (Claude-specific)** | Duplicate of agent adapter. Same full hardcoded list. |
| `.github/copilot-instructions.md` | GitHub Copilot always-loaded context | **Adapter (GitHub-specific)** | Third copy of instruction list. |
| `.github/instructions/spec-writer.instructions.md` | Copilot context for `docs/spec/**` | **Adapter (GitHub-specific)** | Fourth copy of instruction list. `applyTo` scope is good. |
| `AGENTS.md` | Tool-agnostic agent instructions | **Ambiguous: adapter + partial source of truth** | Contains core rules (approval gate summary, implementation handoff) that overlap with canonical files. Should be a thin adapter only. |
| `README.md` | Documentation | **Documentation** | Lists all instruction files in a code block. Fifth maintenance point for the instruction list. |
| `docs/spec/room-maintenance/domain-discovery.md` | Generated domain discovery artifact | **Generated (example)** | No status header. High quality. |
| `docs/spec/room-maintenance/solution-options.md` | Generated solution options | **Generated (example)** | No status header. Flags Model 2's bidirectional dependency correctly. |
| `docs/spec/room-maintenance/spec.md` | Generated feature spec | **Generated (example)** | No status header. Section 5 describes Model 2 impact as "New" — but Model 3 was selected. Artifact was not updated after the decision changed. |
| `docs/spec/room-maintenance/decision.md` | Decision record | **Generated (approved)** | No status header. Contains interface signatures, API endpoint paths, and repository refactoring details — implementation plan territory, not decision record territory. |

### Source-of-truth ambiguity summary

| What | Where it currently lives | Problem |
| :--- | :--- | :--- |
| Spec Writer role definition | `spec-writer.agent.instructions.md` | Correct. Single location. |
| Spec Writer workflow | `spec-writer-flow.instructions.md` | Correct. Single location. |
| File loading rules | `spec-writer.agent.instructions.md` (conditional) AND four adapters (unconditional, hardcoded) | **Duplicated and contradictory.** Adapters override the conditional logic. |
| Approval gate triggers | `architecture-approval-gate.instructions.md` (authoritative) + `AGENTS.md` (partial summary) | AGENTS.md creates a secondary, abbreviated version. |
| Project-specific context | `domain-context.instructions.md` + `known-decisions.instructions.md` | `known-decisions.instructions.md` is never loaded by any rule. |
| Claude usage | `.claude/agents/spec-writer.md` + `.claude/commands/spec-writer.md` | Two Claude files. Neither references the loading rules in the canonical agent file. |
| GitHub/Copilot usage | `.github/copilot-instructions.md` + `.github/instructions/spec-writer.instructions.md` | Two GitHub files with different scoping. Neither references the canonical loading rules. |

---

## Recommended Spec Writer Model

The Spec Writer should run a single unified workflow with nine explicit phases. Phases have mandatory sequence — no phase may start before the previous phase's required output exists. The approval gate is a hard stop between Phase 7 and Phase 8.

The Spec Writer loads a core always-loaded set plus phase-specific files loaded at the relevant phase. Project-specific files are always loaded because they are always relevant to the scope of analysis.

The critical design rule: **the Spec Writer is one agent, but the workflow must be structured so that any phase can later be extracted into a separate agent without rewriting the phase's instructions.** Each phase file should define its own inputs, outputs, and gates.

---

## Phase-by-Phase Design

| Phase | Purpose | Inputs | Outputs | Allowed Decisions | Must Not Do | Instructions to Load |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **1. Input Intake** | Accept and validate input material; confirm what is provided vs missing | Raw user input, knowledge files, prior docs | Intake summary: what was provided, what is missing, which phase to start | Flag missing inputs; ask for them if blocking | Proceed with analysis; make any requirement or domain judgment | Always-loaded core only |
| **2. Requirements Normalization** | Extract, separate, and label requirements and AC; surface initial open questions | Intake summary, raw requirements, existing knowledge | Confirmed vs draft requirements; confirmed vs draft AC; initial open questions list | Mark items as confirmed/draft; draft missing AC from available knowledge | Finalize AC without completing Step 3; convert blocking questions to silent assumptions | `open-questions-and-assumptions.instructions.md` |
| **3. Context Scan** | Detect existing behaviors, rules, integrations, and constraints that the change may affect | Requirements list, project context, domain context, known decisions | List of impacted existing behaviors; new open questions or AC surfaced; no-impact confirmation | Flag new open questions; flag new AC candidates; flag out-of-scope items | Describe module interfaces or implementation details before a model is selected; invent behavior not present in knowledge | `domain-context.instructions.md`, `known-decisions.instructions.md` |
| **4. Domain Discovery** | Identify affected domain concepts; classify semantic ambiguities; flag extraction candidates | Requirements, context scan output | Domain concepts with lifecycle/ownership notes; semantic ambiguities classified; extraction candidates flagged | Flag extraction candidates; classify ambiguities; recommend no structural action | Decide on module extraction; propose solution models; describe implementation | `domain-discovery.instructions.md`, `domain-archetypes.instructions.md`, `high-impact-semantics.instructions.md`; load `aggregate-lifecycle.instructions.md` when multiple candidate aggregates found |
| **5. Architecture Impact Detection** | Explicitly decide if the change has architectural impact; determine whether to continue or fast-exit | Domain discovery output, requirements | Explicit flag: architectural concern / no concern; if no concern, justification for fast-exit | Declare architectural impact or absence; fast-exit to Phase 8 when all three local-change conditions are met | Silently skip; treat uncertainty as no-concern | `architecture-approval-gate.instructions.md` |
| **6. Solution Option Analysis** | Propose 2–3 meaningfully different models; evaluate each against AC and trade-off dimensions | Domain discovery, impacted behaviors, flagged concerns, AC | Model definitions with intent blocks; AC coverage table; trade-off table | Propose models; name trade-offs; flag agent-introduced risks | Recommend a model; treat any model as decided; generate implementation tasks | `solution-option-analysis.instructions.md`, `tshirt-sizing.instructions.md` |
| **7. Human Direction Confirmation** | Ask direction questions; recommend a model; get explicit approval before proceeding | Models, AC coverage, trade-off table, user priority answers | Recommended model with decision basis; explicit confirmation or rejection | Recommend one model; document rejection rationale for others | Generate final spec or any implementation task before confirmation is given; treat recommendation as approval | `human-direction-confirmation.instructions.md`, `architecture-approval-gate.instructions.md` |
| **8. Final Spec Draft** | Produce consolidated spec, domain discovery, and decision record as approved artifacts | Approved direction, all prior phase outputs | `spec.md` (status: approved), `domain-discovery.md` (status: approved), `decision.md` (status: approved); `solution-options.md` updated to reflect selection | Write finalized requirements, AC, domain model, and decision record | Include implementation tasks; include interface signatures or API paths in decision record; mix pre-decision analysis with post-decision conclusions | All always-loaded files |
| **9. Implementation Readiness Check** | Confirm what is needed before implementation can start; list safe preparatory tasks; flag blockers | Approved decision record, final spec | Preparatory task list; remaining open questions; explicit implementation-blocked or implementation-ready status | Generate safe preparatory tasks (characterization tests, documentation tasks) | Generate structural implementation tasks before planning phase begins | `architecture-approval-gate.instructions.md` |

---

## Future Agent Split Strategy

| Future Agent | Maps To Phase | Responsibility | May Decide | Must Not Decide | Add Now / Later / Not Yet |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **Explorer / Context Collector** | Phase 3 | Read repository files, scan known context, surface impacted behaviors | What files to read; which behaviors are relevant | Whether impact is architectural; domain model | **Not Yet** |
| **Requirements Analyst** | Phases 1–2 | Normalize requirements; classify questions and assumptions; draft missing AC | Confirmed vs draft labels; question classifications | Domain model; architectural impact | **Not Yet** |
| **Domain Discovery Agent** | Phase 4 | Identify domain concepts; detect semantic ambiguities; flag extraction candidates | Extraction candidate flags; ambiguity classifications | Module extraction decisions; solution models | **Later** |
| **Architecture Option Analyst** | Phase 6 | Produce option analysis; compare models against AC; evaluate trade-offs | Model proposals; AC coverage tables; trade-off labels | Recommend a model; approve anything | **Later** |
| **Human Direction Facilitator** | Phase 7 | Manage the confirmation checkpoint; present the menu; route user response | Nothing — pure routing/presentation | Architecture decisions | **Not Yet** |
| **ADR Writer** | Phase 8 (decision.md only) | Write a clean architecture decision record from the approved direction | ADR structure and format | Architecture decisions themselves | **Later** |
| **Final Spec Writer** | Phase 8 | Produce `spec.md` from normalized requirements and approved direction | Spec artifact format and status | Anything not already decided | **Not Yet** |
| **Implementation Planner** | Phase 9 + beyond | Generate ordered implementation tasks from approved `decision.md` and `spec.md` | Task ordering; slice boundaries | Architecture decisions; scope | **Later** |
| **Architecture Reviewer** | Outside current scope | Review proposed architecture for consistency, coupling, and completeness | Flags and risks | Architecture decisions | **Not Yet** |
| **Instruction Reviewer** | Outside current scope | Audit instruction files for drift, orphans, and duplication | Review findings | Instruction changes | **Not Yet** |
| **C4 Diagram Agent** | Phase 8 supplement | Generate C4-style module diagrams from decision record | Diagram structure | Architecture decisions | **Not Yet** |
| **Code Reviewer** | Outside current scope | Review implementation against spec and decision record | Review findings | Anything in the spec domain | **Not Yet** |

---

## Recommended Directory Structure

### Recommendation: Keep the current shape. Fix the manifest. Harden the adapter boundary.

The current `instructions/agents/` + `instructions/workflows/` + `instructions/core/` + `instructions/project/` structure is a good hybrid. The two changes needed are: (1) add a `spec-writer.manifest.md` as the authoritative loading list, and (2) distinguish generated artifacts from source artifacts.

### Target Structure

```
instructions/
  agents/
    spec-writer.agent.instructions.md   ← role definition, responsibilities, non-goals
    spec-writer.manifest.md             ← NEW: canonical loading list (always/conditional/forbidden)

  workflows/
    spec-writer-flow.instructions.md
    solution-option-analysis.instructions.md
    architecture-approval-gate.instructions.md
    human-direction-confirmation.instructions.md

  core/
    reasoning/
      open-questions-and-assumptions.instructions.md
      high-impact-semantics.instructions.md
    ddd/
      domain-discovery.instructions.md
      aggregate-lifecycle.instructions.md
      domain-archetypes.instructions.md
    estimation/
      tshirt-sizing.instructions.md

  project/
    booking/
      domain-context.instructions.md
      known-decisions.instructions.md

.claude/
  agents/
    spec-writer.md        ← thin: name, tools, model, reference to manifest
  commands/
    spec-writer.md        ← thin: argument hint, output paths, reference to manifest

.github/
  copilot-instructions.md               ← thin: workflow overview, reference to manifest
  instructions/
    spec-writer.instructions.md         ← thin: applyTo scope, reference to manifest

AGENTS.md                               ← entry point only: role description + manifest reference, no behavioral rules
README.md                               ← documentation only; no instruction list

docs/
  spec/
    room-maintenance/                   ← generated artifacts (status-tagged)
      domain-discovery.md
      solution-options.md
      spec.md
      decision.md
```

### Per-folder rules

| Folder | What belongs | What must not belong | Who uses it | Source of truth or adapter |
| :--- | :--- | :--- | :--- | :--- |
| `instructions/agents/` | Role definitions; agent manifests | Workflow logic; reasoning rules; project context | All tool adapters reference this | **Source of Truth** |
| `instructions/workflows/` | Phase sequences; sub-workflow procedures; guardrails | Role definitions; project-specific facts; reasoning heuristics | Loaded per manifest | **Source of Truth** |
| `instructions/core/reasoning/` | Tool-agnostic reasoning heuristics reusable across agents | Project facts; workflow steps; tool-specific behavior | Loaded conditionally per manifest | **Source of Truth (reusable)** |
| `instructions/core/ddd/` | DDD discovery guides reusable across agents | Project domain facts; workflow procedures; decision records | Loaded conditionally per manifest | **Source of Truth (reusable)** |
| `instructions/core/estimation/` | Sizing and estimation guides | Project facts; workflow steps | Loaded per manifest when option analysis runs | **Source of Truth (reusable)** |
| `instructions/project/<name>/` | Project-specific domain context; known decisions; documented shortcuts | Generic reasoning rules; workflow logic; DDD theory | Always loaded for this project | **Source of Truth (project-specific)** |
| `.claude/` | Claude-specific frontmatter, thin behavior wrappers | Instruction logic; duplicate loading lists | Claude Code only | **Adapter** |
| `.github/` | GitHub Copilot `applyTo` scoping, workflow summaries | Instruction logic; duplicate loading lists | GitHub Copilot only | **Adapter** |
| `AGENTS.md` | Tool-agnostic entry point; manifest reference | Instruction logic; approval gate rules | Agent-aware tools without other config | **Adapter** |
| `docs/spec/` | Generated spec artifacts; status-tagged | Instruction files; reusable rules; examples-as-instructions | Spec Writer writes; Implementation Planner reads | **Generated artifacts** |

---

## Manifest and Routing Design

### Canonical manifest: `instructions/agents/spec-writer.manifest.md`

```markdown
<!-- Type: manifest -->

# Spec Writer — Instruction Manifest

This file is the single source of truth for which instruction files the Spec Writer loads
and under what conditions.

All tool adapters (.claude, .github, AGENTS.md) must reference this file instead of
maintaining their own instruction lists.

---

## Always load

- `instructions/agents/spec-writer.agent.instructions.md`
- `instructions/workflows/spec-writer-flow.instructions.md`
- `instructions/workflows/architecture-approval-gate.instructions.md`
- `instructions/workflows/human-direction-confirmation.instructions.md`
- `instructions/core/reasoning/open-questions-and-assumptions.instructions.md`
- `instructions/core/reasoning/high-impact-semantics.instructions.md`

## Always load (this project)

- `instructions/project/booking/domain-context.instructions.md`
- `instructions/project/booking/known-decisions.instructions.md`

## Load when starting Phase 4 (Domain Discovery)

- `instructions/core/ddd/domain-discovery.instructions.md`
- `instructions/core/ddd/domain-archetypes.instructions.md`

Load additionally when multiple candidate aggregates or lifecycle overlap is found:

- `instructions/core/ddd/aggregate-lifecycle.instructions.md`

## Load when starting Phase 6 (Solution Option Analysis)

- `instructions/workflows/solution-option-analysis.instructions.md`
- `instructions/core/estimation/tshirt-sizing.instructions.md`

## Forbidden combinations

- Do not load `solution-option-analysis.instructions.md` before Phase 5 confirms an
  architectural concern exists.
- Do not load `tshirt-sizing.instructions.md` outside of option analysis context.
- Do not load project files from another project folder when operating in this project.

## Files that are adapters only (do not load as instructions)

- `.claude/agents/spec-writer.md`
- `.claude/commands/spec-writer.md`
- `.github/copilot-instructions.md`
- `.github/instructions/spec-writer.instructions.md`
- `AGENTS.md`

## Files that are generated artifacts (do not treat as instructions)

- `docs/spec/**/*.md`
```

### Adapter rule: what each adapter contains after this change

| Adapter | Before | After |
| :--- | :--- | :--- |
| `.claude/agents/spec-writer.md` | Full instruction list hardcoded | Name, tools, model, one-liner: "Follow `instructions/agents/spec-writer.manifest.md`" |
| `.claude/commands/spec-writer.md` | Full instruction list hardcoded | Argument hint, output path rules, one-liner: "Follow `instructions/agents/spec-writer.manifest.md`" |
| `.github/copilot-instructions.md` | Full instruction list hardcoded | Workflow overview (2–3 sentences), one-liner: "Follow `instructions/agents/spec-writer.manifest.md`" |
| `.github/instructions/spec-writer.instructions.md` | Full instruction list hardcoded | `applyTo` scope, one-liner: "Follow `instructions/agents/spec-writer.manifest.md`" |
| `AGENTS.md` | Core rules + partial list | Role description for human readers + manifest reference only. No behavioral rules. |
| `README.md` | Instruction list in a code block | Remove instruction list entirely; describe workflow purpose and usage only |

---

## Artifact Boundary Design

| Artifact | Purpose | Status | Source of Truth? | Requires Approval? | Must Contain | Must Not Contain |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| `domain-discovery.md` | Analysis artifact: domain concepts, lifecycle notes, extraction candidates, semantic ambiguities | `draft` → `reviewed` → `historical` | **No. Always `source-of-truth: false`.** Informs decisions; is never a decision. Candidates and extraction signals in this file are proposals, not accepted changes. | No | Concepts, lifecycle, archetype mapping, extraction signals, semantic ambiguities | Approved decisions; solution models; implementation details; API paths |
| `solution-options.md` | Analysis artifact: option models with intent blocks, AC coverage, trade-off comparison | `draft` → `reviewed` → `historical` | **No. Always `source-of-truth: false`.** | No | Models, AC coverage, trade-offs, rejection rationale | Approved direction; confirmed implementation details; interface signatures |
| `architecture-review.md` | Analysis artifact: architectural review findings | `draft` → `reviewed` | **No. Always `source-of-truth: false`.** Input for decisions; never a decision itself. | No | Risks, inconsistencies, coupling concerns, recommendations | Implementation details; approved decisions |
| `spec.md` | Source of truth for requirements, AC, scope, and post-decision system impact | `draft` → `approved` | **Yes — for requirements, AC, and scope only.** Not source of truth for architecture; that is `decision.md` or ADR. | Yes | Confirmed requirements, confirmed AC, remaining open questions, post-decision system impact | Pre-decision model speculation; architecture decisions; implementation-level interface names |
| `decision.md` | Source of truth for architecture decisions | `approved` → `superseded` when replaced | **Yes — for architecture decisions only** after human approval | Yes | Selected model, decision basis, rejection rationale, accepted trade-offs, deferred decisions, module names, domain concepts, integration contract names when needed to explain the decision | Implementation-level API signatures; repository method names; endpoint paths; task breakdowns — those belong in `implementation-plan.md` |
| `implementation-plan.md` | Source of truth for the approved implementation scope | `draft` → `approved` | **Yes — only when approved.** Draft is not source of truth. | Yes (plan generated post-approval) | Ordered tasks, interface definitions, API paths, slice boundaries, test strategy | Architecture decisions; requirements |
| `test-plan.md` | Derived test strategy; not source of truth | `draft` → `reviewed` | **No. Always `source-of-truth: false`.** Derived from `spec.md`. | No | AC-to-test mapping, test types, edge cases; `derived-from: docs/spec/<feature>/spec.md` in frontmatter | Implementation tasks; architecture decisions; requirements not in spec.md |
| ADR files | Source of truth for architectural decisions that outlive a single feature | `accepted` or `superseded` | Yes | Yes | Decision, context, consequences, alternatives considered | Implementation specifics; temporary analysis |

### Artifact status frontmatter convention

Use YAML frontmatter at the top of every generated artifact:

```yaml
---
artifact: domain-discovery | solution-options | spec | decision | implementation-plan | test-plan
status: draft | reviewed | approved | superseded | historical
source-of-truth: true | false
requires-approval: true | false
approved-by: ~
related-spec: docs/spec/<feature>/spec.md
related-decision: docs/spec/<feature>/decision.md
---
```

Status vocabulary:
- `draft` — being written; not ready for review
- `reviewed` — reviewed but not yet formally approved
- `approved` — human direction confirmed; consistent and ready to act on
- `superseded` — replaced by a later artifact in the same feature
- `historical` — kept for reference; no longer tied to active work; must not be treated as current

Do not add `in-progress` or `pending-implementation`. If there is no process that reads those statuses programmatically, they add noise without value. Use `approved` for a confirmed artifact and `historical` for an artifact that is no longer current.

An artifact with a known inconsistency must not be marked `approved`. Mark it `historical` with `source-of-truth: false` and create a corrected artifact if needed.

---

## Core Design Principles

These three principles must be reflected in the manifest, the workflow instructions, and the adapter files. They are not implementation details — they are the invariants the whole system depends on.

### Principle 1 — A manifest routes instructions. It must not duplicate workflow behavior.

The manifest lists which files to load and under what conditions. It must not contain rules about how to write a decision record, when to ask the human, or how to evaluate a model. Those rules belong in workflow and reasoning instruction files. If a rule appears in both the manifest and a workflow file, delete it from the manifest.

### Principle 2 — Analysis artifacts are never source of truth.

`domain-discovery.md`, `solution-options.md`, and `architecture-review.md` are inputs to decisions. They must always carry `source-of-truth: false`. An extraction candidate in `domain-discovery.md` is a proposal, not an approved domain change. A model in `solution-options.md` is an option, not a decision. Marking any of these as source of truth risks an agent treating analysis as an accepted architectural change.

### Principle 3 — Implementation must not start unless the pipeline is clean.

Both conditions must be true before implementation tasks can be generated:

1. `spec.md` has `status: approved` and `source-of-truth: true`
2. `decision.md` is either not required (local change, no architectural impact) or has `status: approved` and `source-of-truth: true`

If `spec.md` is `historical` or `source-of-truth: false`, the spec is not ready. Implementation is blocked regardless of the state of `decision.md`. This prevents a clean `decision.md` from creating a false sense of readiness when the spec has a known inconsistency.

---

## Claude vs GitHub / Copilot Boundary

### Ownership model

| Layer | Owner | Principle |
| :--- | :--- | :--- |
| Instruction logic (roles, workflows, reasoning) | `instructions/` | Tool-agnostic. No tool-specific syntax. |
| Loading rules and manifest | `instructions/agents/spec-writer.manifest.md` | Tool-agnostic. Referenced by all adapters. |
| Claude invocation | `.claude/` | Claude-specific frontmatter only. No instruction logic. |
| GitHub Copilot activation | `.github/` | Copilot-specific `applyTo` only. No instruction logic. |
| Tool-agnostic agent instructions | `AGENTS.md` | Entry point for tools that read this file. References manifest. No logic. |

### Currently flagged files

- `.claude/agents/spec-writer.md`: Contains the complete instruction loading list. Should contain only Claude agent frontmatter + manifest reference.
- `.claude/commands/spec-writer.md`: Same problem. Output path rules are fine to keep — they are tool-specific behavior.
- `.github/copilot-instructions.md`: Contains the complete instruction loading list + "Common failure to avoid" that duplicates `AGENTS.md`. Should contain a workflow summary (2–3 sentences) + manifest reference.
- `.github/instructions/spec-writer.instructions.md`: Contains the complete instruction loading list. Should contain `applyTo` scope + manifest reference.
- `AGENTS.md`: Contains approval gate summary and implementation handoff rules that duplicate canonical files. Should be a thin entry point only.

---

## Problems Found in Current Example Specs

### Problem 1: spec.md was not updated after the decision changed

`spec.md` Section 5 (Existing System Impact) contains a description of Model 2's architecture ("Reverse dependency created: RoomManagement → Reservations… `IReservationConflictChecker`"). Model 3 was selected specifically to avoid this bidirectional dependency. The spec was written mid-process and never reconciled with the selected direction.

**Root cause:** The workflow writes one `spec.md` incrementally, mixing pre-decision analysis with post-decision conclusions. Phase 3 context scan should not predict module interfaces or dependency directions before a model is selected.

**Fix:** `spec.md` is a post-decision document. The "Existing System Impact" section must describe the selected model's actual impact, not a mid-analysis prediction. Rewrite Section 5 to match Model 3.

### Problem 2: decision.md contains implementation-plan content

`decision.md` includes interface method signatures, API endpoint paths, and repository refactoring specifics. These are correct and valuable — but they belong in an `implementation-plan.md`, not a decision record. Decision records should answer: what was decided, why, and what alternatives were rejected.

**Fix:** Define `decision.md` boundary strictly in the manifest and workflow: what/why only. Move implementation specifics to `implementation-plan.md`.

### Problem 3: No status headers on any artifact

All four artifacts lack the status header required by `spec-writer-flow.instructions.md`. The workflow instruction exists; it is not being followed. Prose instructions are insufficient for this convention.

**Fix:** Add frontmatter to all four example artifacts. Add a pre-commit hook for `docs/spec/**`.

### Problem 4: No cross-references between artifacts

`spec.md` does not reference `domain-discovery.md`. `decision.md` does not reference `solution-options.md` by path. With frontmatter, this is a one-line addition per artifact.

---

## Quality and Scalability Risks

| Risk | Why It Matters | Suggested Fix |
| :--- | :--- | :--- |
| **Instruction list duplicated in 5 places** | Any addition or removal requires 5 edits. Already drifted: 3 orphaned files. | Create `spec-writer.manifest.md`. Make adapters reference it with a single line. |
| **Conditional loading bypassed by all adapters** | All adapters load all files unconditionally. Conditional loading design is never enforced. | Manifest enforces conditions. Adapters delegate to manifest. |
| **Three orphaned instruction files** | `domain-discovery.instructions.md`, `tshirt-sizing.instructions.md`, `known-decisions.instructions.md` are never loaded. | Add all three to the manifest. |
| **spec.md mixes pre- and post-decision content** | Pre-decision model speculation stays in spec after a different model is chosen. Misleading for implementation. | Define spec.md as post-decision artifact. Rewrite Section 5. |
| **decision.md accumulates implementation detail** | Decision rationale becomes entangled with implementation specifics. Record becomes stale. | Strict decision record boundary: what/why only. `implementation-plan.md` for how-to-build. |
| **No status headers on generated artifacts** | Workflow requires them; example artifacts don't have them. Nothing enforces the convention. | Add frontmatter to examples. Add lint check for `docs/spec/**`. |
| **AGENTS.md duplicates canonical rules** | Approval gate summary in AGENTS.md can drift from the canonical file. | Make AGENTS.md a thin adapter: one-line role + manifest reference + no logic. |
| **README.md lists instruction files** | README is a fifth maintenance point for the instruction list. | Remove the instruction list from README. Purpose and usage only. |
| **`high-impact-semantics.instructions.md` not in canonical loading rules** | Agent definition's loading rules don't list this file. Adapters all load it. Canonical definition is incomplete. | Add to manifest under always-load. |
| **`known-decisions.instructions.md` never loaded but referenced by example spec** | Decision record cites it as context for architectural choice, but Spec Writer may not have read it. | Add to manifest under always-load (project-specific section). |
| **`solution-options.md` has no source-of-truth boundary** | Nothing forbids treating option analysis as a decided artifact. | Add `source-of-truth: false` to frontmatter. Note in manifest: solution-options.md is never source of truth. |
| **Two Claude adapter files with identical instruction lists** | Same list maintained twice. | After manifest, both reduce to a one-liner. Difference is only frontmatter and output-path behavior. |

---

## Validation and Automation Suggestions

| Rule | Suggested Enforcement | Why |
| :--- | :--- | :--- |
| Every generated artifact under `docs/spec/` must have a status frontmatter block | **Pre-commit hook** or **GitHub workflow** that greps for `^status:` in `docs/spec/**/*.md` | Status convention will not be followed consistently without mechanical enforcement. |
| Adapters must not contain instruction loading lists | **PR template checklist item**: "Did you update only the manifest and not the adapter loading lists?" | Low change frequency; checklist is sufficient. |
| `solution-options.md` must have `source-of-truth: false` in frontmatter | Same **pre-commit hook** as above | Prevents option analysis from being treated as approved decisions. |
| `decision.md` must not contain implementation-level API signatures, repository method names, endpoint paths, or task breakdowns (it may mention module names, domain concepts, and integration contract names when needed to explain the architectural decision) | **Manual review gate via PR template**: "Does decision.md contain method signatures, endpoint paths, or task lists?" | Hard to automate accurately; the line between architectural context and implementation detail requires judgment. |
| Implementation tasks must not appear in `spec.md` | **PR template checklist** | Hard to automate. Low frequency. |
| All files listed in manifest must exist | **CI script** that reads the manifest and checks file existence | Prevents orphaned references in the manifest itself. |
| New instruction files must be added to the manifest | **PR template checklist**: "If you added a file to `instructions/`, did you add it to the manifest?" | Simple mechanical check for low-frequency change. |

---

## Implementation Batches

Three sequential batches. Each is a self-contained commit. Do not combine them.

---

### Batch 1 — Routing and Manifest (load-bearing)

This is the foundation. Nothing else should be done before this is stable.

**Scope:**

1. Create `instructions/agents/spec-writer.manifest.md` as the single source of truth for Spec Writer loading. Content:
   - Always-load section: core workflow files + `high-impact-semantics.instructions.md`
   - Always-load (project) section: `domain-context.instructions.md` + `known-decisions.instructions.md`
   - Phase 4 conditional: `domain-discovery.instructions.md`, `domain-archetypes.instructions.md`, `aggregate-lifecycle.instructions.md`
   - Phase 6 conditional: `solution-option-analysis.instructions.md`, `tshirt-sizing.instructions.md`
   - Forbidden combinations section
   - Adapter-only files section (list .claude, .github, AGENTS.md as adapters, not instructions)
   - Generated artifacts section (`docs/spec/**/*.md`)

2. Reduce `.claude/agents/spec-writer.md` to thin Claude adapter: keep name, tools, model frontmatter; replace instruction list with a single reference to `instructions/agents/spec-writer.manifest.md`.

3. Reduce `.claude/commands/spec-writer.md`: keep argument hint and output path rules (those are tool-specific); replace instruction list with manifest reference.

4. Reduce `.github/copilot-instructions.md`: keep a 2–3 sentence workflow overview; replace instruction list with manifest reference; remove "Common failure to avoid" (it duplicates AGENTS.md).

5. Reduce `.github/instructions/spec-writer.instructions.md`: keep `applyTo` scope; replace instruction list with manifest reference.

6. Reduce `AGENTS.md` to an entry point only. Content: one sentence describing the role for human readers + "For Spec Writer tasks, follow `instructions/agents/spec-writer.manifest.md`." No behavioral rules. No approval gate reminder. Even a single sentence of behavioral logic is a second copy that will drift.

7. Update `README.md`: remove the code block listing instruction files; replace with one sentence pointing to the manifest. README is documentation for humans — not a loading list.

**After this batch, provide:**
- Files changed
- What logic moved to manifest
- What remains tool-specific
- Any ambiguous rule you did not move and why

---

### Batch 2 — Artifact Status Frontmatter

Do this after Batch 1 is committed and stable.

**Scope:**

Add YAML frontmatter to each file in `docs/spec/room-maintenance/` using this convention:

```yaml
---
artifact: domain-discovery | solution-options | spec | decision
status: draft | reviewed | approved | superseded | historical
source-of-truth: true | false
requires-approval: true | false
approved-by: ~
related-spec: docs/spec/room-maintenance/spec.md
related-decision: docs/spec/room-maintenance/decision.md
---
```

Values per file:

- `domain-discovery.md`: artifact=domain-discovery, status=historical, source-of-truth=false, requires-approval=false
- `solution-options.md`: artifact=solution-options, status=historical, source-of-truth=false, requires-approval=false
- `spec.md`: artifact=spec, status=historical, source-of-truth=false, requires-approval=false
  — Has a known inconsistency: Section 5 describes Model 2 system impact; Model 3 was selected. An artifact with a known inconsistency must not be `approved`. Mark as `historical` and correct it in Batch 3.
- `decision.md`: artifact=decision, status=approved, source-of-truth=true, requires-approval=true, approved-by=Dominik Czerwiński
  — The decision itself is valid. The implementation detail it contains is a boundary violation to clean up in Batch 3.

Also add the artifact status vocabulary and `derived-from` field convention to `instructions/workflows/spec-writer-flow.instructions.md` (Spec artifact status markers section). Use the simplified vocabulary:
- `draft` — being written; not ready for review
- `reviewed` — reviewed but not yet formally approved
- `approved` — consistent and ready to act on; human direction confirmed
- `superseded` — replaced by a later artifact in the same feature
- `historical` — kept for reference; no longer current; must not be treated as source of truth

For `test-plan.md` (when created): add `source-of-truth: false` and `derived-from: docs/spec/<feature>/spec.md`. Test plans are derived from `spec.md`, not an independent source of truth.

Also add the implementation pipeline guard to `instructions/workflows/spec-writer-flow.instructions.md` (Step 12 / Phase 9 section):

> Implementation must not start unless both conditions are true:
> 1. `spec.md` has `status: approved` and `source-of-truth: true`
> 2. `decision.md` is either not required (local change, no architectural impact) or has `status: approved` and `source-of-truth: true`
>
> If `spec.md` is `historical` or `source-of-truth: false`, implementation is blocked regardless of `decision.md` state.

This rule exists because after Batch 2, the repo will temporarily have `decision.md: approved` and `spec.md: historical`. The guard prevents that state from being read as "ready to implement."

---

### Batch 3 — Artifact Boundary Cleanup

Do this after Batch 2 is committed and stable.

**Scope:**

1. Define `decision.md` boundary explicitly in `instructions/workflows/spec-writer-flow.instructions.md` (Phase 8 / Step 12 section). Rule to add:

   > `decision.md` must not contain implementation-level API signatures, repository method names, endpoint paths, or task breakdowns. It may mention module names, domain concepts, integration contract names, and existing component names when needed to explain the architectural decision. Implementation specifics belong in `implementation-plan.md`.

2. Add `spec.md` post-decision boundary rule to Phase 8:

   > The Existing System Impact section in `spec.md` must describe the post-decision impact of the selected model only. Pre-decision model analysis belongs in `solution-options.md`. Do not carry pre-decision impact speculation, unselected model artifacts, or implementation interface predictions into `spec.md`.

3. Correct `docs/spec/room-maintenance/spec.md` Section 5 (Existing System Impact) to reflect the selected Model 3.

   **Hard constraint:** Do not redesign the room-maintenance solution. Do not re-evaluate the models. Do not change requirements or AC. The only goal is to align Section 5 with the decision already recorded in `decision.md`. The selected model is Model 3 (Availability layer). It is approved and final.

   Specific change: remove the row "Reverse dependency created: RoomManagement → Reservations / `IReservationConflictChecker`". Replace it with the accurate Model 3 impact: Availability module introduced as coordination hub; Reservations calls `IAvailabilityService` instead of `IRoomReader` + repository overlap check; no bidirectional module dependency; `IRoomReader` contract is not extended with a maintenance check method.

   After correction, update the frontmatter: `status=approved, source-of-truth=true`.

4. Formalize `implementation-plan.md` as a distinct artifact:
   - Add it to the Phase 9 output description in `spec-writer-flow.instructions.md`
   - Add it to the output path list in `.claude/commands/spec-writer.md`
   - Note in the manifest (generated artifacts section) that it is a Phase 9 output, not a loaded instruction file

5. Add a PR template at `.github/pull_request_template.md` with a spec artifact checklist:

   ```markdown
   ## Spec artifact checklist (complete if this PR touches docs/spec/ or instructions/)

   - [ ] All new spec artifacts have status frontmatter (artifact, status, source-of-truth, requires-approval)
   - [ ] decision.md contains no method signatures, endpoint paths, or task lists
   - [ ] spec.md reflects post-decision system impact only (no pre-decision model speculation)
   - [ ] solution-options.md is marked source-of-truth: false
   - [ ] If a new file was added to instructions/, it is listed in spec-writer.manifest.md
   - [ ] Adapter files (.claude, .github, AGENTS.md) were not updated with instruction lists
   ```
