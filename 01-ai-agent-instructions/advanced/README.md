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
- propose three solution models when needed
- compare models against acceptance criteria
- recommend the simplest sufficient model
- ask for human direction confirmation before architecture changes are turned into implementation tasks

## Structure

```txt
instructions/
  agents/
    spec-writer.agent.instructions.md

  workflows/
    spec-writer-flow.instructions.md
    solution-option-analysis.instructions.md
    architecture-approval-gate.instructions.md

  core/
    ddd/
      domain-discovery.instructions.md
      aggregate-lifecycle.instructions.md
      domain-archetypes.instructions.md

  project/
    booking/
      domain-context.instructions.md

.github/
  copilot-instructions.md
  instructions/
    spec-writer.instructions.md

.claude/
  agents/
    spec-writer.md
  skills/
    spec-writer/
      SKILL.md
  commands/
    spec-writer.md

AGENTS.md
```

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

The pack includes both a subagent and a skill/command wrapper:

- `.claude/agents/spec-writer.md`
- `.claude/skills/spec-writer/SKILL.md`
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
