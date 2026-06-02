# Advanced - Spec Writer Showcase

This folder is the `advanced/` variant from [Post 01 - AI Agent Instructions](../README.md).

It contains a Spec Writer workflow built on top of a booking system codebase. The setup is intentionally more complex than what you would write for a real project - the goal is to showcase different types of instructions, not to provide a production-ready template.

---

## Assumptions and design choices

The instruction set was built with a specific set of constraints in mind:

- **Multi-tool from the start** - the same instructions need to work with Claude Code, GitHub Copilot, and Codex/AGENTS-compatible hosts. That is why there is a manifest instead of each adapter maintaining its own list.
- **Context cost matters** - not every feature needs full DDD analysis. Instructions are split into always-load and conditional sections. The manifest controls what gets loaded and when.
- **Architecture changes are expensive to undo** - the agent can propose changes to module boundaries or aggregate ownership, but cannot approve them. A human must confirm before implementation tasks are generated.
- **Decision levels are explicit** - the agent is taught to distinguish Requirement / Domain / Architecture / Implementation decisions, because collapsing them into implementation-level answers is a common failure mode.
- **Known shortcuts are documented** - the booking system has intentional shortcuts from MVP. They are recorded with their reason and known limitations so the agent knows when they are relevant.

---

## What the Spec Writer does

The Spec Writer turns collected knowledge, requirements, and draft acceptance criteria into a reviewable solution spec.

It is not an implementer.

Its main job is to:

- normalize requirements and acceptance criteria
- scan known system behavior for impact
- perform DDD-oriented domain discovery
- detect architectural concerns
- propose two or three solution models when needed
- compare models against acceptance criteria
- recommend based on stated priorities, domain invariants, and cost - in that order
- ask for human direction confirmation before architecture changes are turned into implementation tasks

---

## Structure

The instruction loading rules are maintained in a single canonical location: `instructions/agents/spec-writer.manifest.md`.

All tool adapters (`.claude/`, `.github/`, `AGENTS.md`) reference the manifest instead of maintaining their own lists.

---

## Copilot usage

The setup includes:

- `.github/copilot-instructions.md`
- `.github/agents/spec-writer.agent.md`
- `AGENTS.md`

Use Copilot with prompts like:

```txt
Act as Spec Writer. Use the repository instructions and create a solution spec from docs/knowledge/<feature>.md.
```

or:

```txt
Use the Spec Writer flow for this feature. Do not generate implementation tasks until the direction is approved.
```

## Claude Code usage

The setup includes a subagent and a command wrapper:

- `.claude/agents/spec-writer.md`
- `.claude/commands/spec-writer.md`

Use one of:

```txt
Use the spec-writer agent to analyze this feature.
```

```txt
/spec-writer docs/knowledge/<feature>.md
```

or:

```txt
/spec-writer Add maintenance mode for rooms based on current knowledge.
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
