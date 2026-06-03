# Pragmatic - Spec Writer Showcase

This folder is the `pragmatic/` variant from [Post 01 - AI Agent Instructions](../README.md).

It contains a Spec Writer workflow built on top of a booking system codebase. The setup is intentionally more complex than what you would build for a real project - the goal is to showcase what happens when you keep adding structure past the point where it stops paying off.

---

## The idea

`advanced/` has one fixed workflow: every request goes through the same 14-step pipeline. That works, but it applies full DDD analysis to a field addition and a module redesign alike.

`pragmatic/` adds an orchestration layer: the Spec Writer classifies each incoming request against seven signals and routes it to the smallest sufficient workflow. Simple request, simple workflow. Complex request, full discovery. One entry point, three strategies.

The name is intentional. Pragmatic here means: match the process to the problem, not the other way around. A checklist for a photo field. A domain spec for a business rule change. Cross-domain discovery for ownership questions. Not everything needs the full pipeline.

---

## What is added over `advanced/`

**Triage** - before any spec work begins, the Spec Writer evaluates seven signals: ambiguity, source-of-truth, ownership, blast radius, reversibility, cross-domain impact, and domain analysis required. The result is a classification: LOW, MEDIUM, or COMPLEX.

**Strategy routing** - each classification maps to a dedicated workflow:

| Level | Strategy | When |
|---|---|---|
| LOW | Checklist | No domain analysis required. Pattern is known and self-contained. |
| MEDIUM | Domain spec | Existing business rules or invariants must be analyzed. Change contained within one domain. |
| COMPLEX | Cross-domain discovery | Two or more domains must coordinate, or ownership and source-of-truth are unclear. |

**Escalation chain** - each strategy re-checks its own entry conditions before producing output. A checklist strategy that surfaces hidden domain rules hands off to domain-spec mid-execution. Domain-spec hands off to cross-domain if ownership becomes unclear. Escalation is explicit and one-way.

**Reviewer** - eight specialized sub-reviewers (architecture, security, performance, data integrity, test coverage, spec compliance, code quality, acceptance criteria) orchestrated by a single `/reviewer` command. The main reviewer selects which sub-reviewers to run based on the change type; each sub-reviewer writes its findings to a dedicated file under `.review/`.

---

## What the Spec Writer entry point does

The `/spec-writer` command is a facade. It does not run a workflow directly. It:

1. Reads the request
2. Evaluates the seven signals
3. Produces a structured classification report
4. Selects and executes the appropriate internal strategy - in the same session, without asking the user to run another command

The user sends one prompt. The agent decides how much analysis the request actually needs.

---

## Structure

Instruction loading is controlled by two manifests - one for the cross-domain strategy (full workflow with conditional loading), one for the domain strategy. Simpler strategies have no manifest; they are small enough to load directly.

Tool adapters (`.claude/`, `.github/`, `AGENTS.md`) reference the manifests instead of maintaining their own lists.

---

## Example artifacts

Three pre-generated spec artifacts are included - one for each complexity level:

| Feature | Classification | Strategy used | Location |
|---|---|---|---|
| Room photo | LOW | Checklist | `docs/spec/room-photo/` |
| Reservation confirmation | MEDIUM | Domain spec | `docs/spec/reservation-confirmation/` |
| Room maintenance | COMPLEX | Cross-domain discovery | `docs/spec/room-maintenance/` |

See the [parent README](../README.md#example-prompts-for-testing) for the prompts used to generate them.

---

## Copilot usage

The setup includes:

- `.github/copilot-instructions.md`
- `.github/agents/spec-writer.agent.md`
- `AGENTS.md`

Use Copilot with prompts like:

```txt
Act as Spec Writer. Classify this request and prepare a solution spec.
Do not generate implementation tasks until the direction is approved.
```

## Claude Code usage

The setup includes a subagent and a command wrapper:

- `.claude/agents/spec-writer.md`
- `.claude/commands/spec-writer.md`

Use one of:

```txt
/spec-writer Add maintenance mode for rooms.
```

```txt
Use the spec-writer agent to analyze this feature.
```

---

## Important rule

The Spec Writer may propose architecture changes, but must not approve them.

Any change to module boundaries, bounded contexts, aggregate ownership, data ownership, integration contracts, public contracts, transaction boundaries, consistency model, or feature scope requires explicit human confirmation before implementation tasks are generated.

---

## Project-specific context

The booking system context lives in:

```txt
instructions/project/booking/domain-context.instructions.md
instructions/project/booking/known-decisions.instructions.md
```

These are example files specific to this showcase. They are not generic templates.
