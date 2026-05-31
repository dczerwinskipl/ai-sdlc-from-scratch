# SDLC Spec Writer Pack

This pack contains a reusable context-aware Spec Writer workflow.

It is designed to work with both GitHub Copilot and Claude Code.

## What it does

The Spec Writer turns collected knowledge, requirements, and draft acceptance criteria into a reviewable solution spec.

It is not an implementer.

Its main job is to:

- normalize requirements and acceptance criteria
- scan known system behavior for impact
- perform DDD-oriented domain discovery
- detect architectural concerns
- propose two or three solution models when needed
- compare models against acceptance criteria
- recommend based on stated priorities, domain invariants, and cost — in that order
- ask for human direction confirmation before architecture changes are turned into implementation tasks

## Structure

The instruction loading rules are maintained in a single canonical location: `instructions/agents/spec-writer.manifest.md`.

All tool adapters (`.claude/`, `.github/`, `AGENTS.md`) reference the manifest instead of maintaining their own lists.

## Copilot usage

The pack includes:

- `.github/copilot-instructions.md`
- `.github/instructions/spec-writer.instructions.md`
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

The pack includes a subagent and a command wrapper:

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

## Important rule

The Spec Writer may propose architecture changes, but must not approve them.

Any change to module boundaries, bounded contexts, aggregate ownership, data ownership, integration contracts, public contracts, transaction boundaries, consistency model, or feature scope requires explicit human confirmation before implementation tasks are generated.

## Project-specific part

The current project-specific example is:

```txt
instructions/project/booking/domain-context.instructions.md
```

Replace it or add another project folder when reusing this pack for a different domain.

Keep generic DDD instructions separate from project-specific domain context.
