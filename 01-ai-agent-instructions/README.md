# Post 01 - AI Agent Instructions

Source for the blog post at [dczerwinski.pl](https://dczerwinski.pl) <!-- TODO: add post link --> - coming soon.

The question this post explores: what difference does it actually make to give an AI agent a well-structured set of instructions versus a basic context file with some project notes?

Two variants are included - `vibe/` and `advanced/` - both targeting the same booking system codebase.

---

## How to test

Open the inner folder (`vibe/` or `advanced/`) as the project root in Claude Code or VS Code - not this folder. Each variant has its own instruction set and agent configuration. Opening the parent folder mixes agents from different examples and defeats the point.

---

## Vibe vs Advanced

| | `vibe/` | `advanced/` |
|---|---|---|
| **Core file** | `CLAUDE.md` | `instructions/` tree + adapters |
| **Setup cost** | Minutes | Hours |
| **What the agent knows** | Commands, architecture, module layout, test conventions | Everything in vibe + DDD principles, decision levels, lazy-loaded domain context, known shortcuts |
| **Workflow** | None - agent decides | Spec Writer pipeline (12 steps) |
| **Architecture gate** | None | Explicit - agent cannot approve its own architectural proposals |
| **Multi-tool** | Claude Code only | Claude Code + GitHub Copilot + Codex/AGENTS-compatible hosts |
| **Instruction loading** | All at once | Manifest-driven, conditional - context cost paid only when needed |
| **Maintenance** | Edit one file | Edit one manifest, files update across all tools |

### Vibe

A single `CLAUDE.md` with enough context for the agent to function: how to build and run the project, module layout, cross-module communication, test conventions.

This is what most people write naturally. It works well for focused tasks - implement this handler, fix this bug, write tests for this use case.

What it lacks: no framework for decisions that cross module boundaries or introduce new domain concepts. The agent will make those decisions, but without guidance on how.

### Advanced

A structured Spec Writer workflow on top of the same project context. Intentionally over-engineered as a showcase of what structured AI instructions can look like - manifest-driven loading, decision levels, architecture gates, conditional context. It is not a production-ready template; it is a reference for the post.

Key additions over the vibe setup:

- **Manifest** - one canonical file that lists what to load and when. All tool adapters (Claude Code, Copilot, AGENTS.md) reference it instead of maintaining their own lists.
- **Decision levels** - the agent is taught to distinguish Requirement / Domain / Architecture / Implementation decisions. Implementation decisions are safe to make. The other three require escalation or explicit documentation.
- **Architecture gate** - the agent can propose architecture changes, but cannot approve them. Any change to module boundaries, aggregate ownership, integration contracts, or consistency model requires explicit human confirmation before tasks are generated.
- **Conditional loading** - domain discovery instructions, aggregate boundary analysis, and solution option evaluation are loaded only when the workflow step determines they are needed. Not every feature needs full option analysis.
- **Solution option analysis** - when an architectural concern is detected, the agent produces 2-3 named models, evaluates each against acceptance criteria and domain invariants, and recommends one. Cost is the last tiebreaker, not the first.

The point is not that the advanced setup produces better output every time. For simple tasks it is slower and noisier. The point is that for tasks where the wrong architecture is expensive to undo, having the agent surface its uncertainty and require human confirmation is worth the overhead.

---

## Example session artifacts

The `docs/spec/room-maintenance/` folder inside each variant contains artifacts from a real session using the prompt below. You can browse them to see what structured instructions produce before running anything yourself.

The session used the provided prompt, then continued with deliberate clarification rounds. Specifically: after the model produced its solution proposal, the domain discovery output was reviewed and the model's assumptions about maintenance status were found to be incomplete — cancellation handling and the status lifecycle were missing from the original spec. That correction was fed back after the proposal, not before. This is intentional: a structured spec makes gaps visible at the point where they are cheapest to fix, before any implementation tasks are generated.

### Example session stats (advanced variant, no token optimization)

| Metric | Value |
|--------|-------|
| Total cost | ~$2.69 |
| Spec output | 1,463 lines across 6 artifacts |
| claude-sonnet-4-6 | 1.1k input / 82.1k output / 2.2m cache read / 212k cache write |
| claude-haiku-4-5 | 492 input / 15 output (subagent tasks) |

This is a showcase session. The outputs are intentionally rich — the goal was to demonstrate what structured instructions can produce, not to minimize cost or token usage. In a real project you would optimize: fewer output tokens, more targeted documents, generating only what you actually need. That tradeoff is the subject of the next post.

---

## Example prompt for testing

Both variants contain expected output artifacts for the room maintenance feature in `docs/spec/room-maintenance/`. Before running the prompt below, either delete those files or ask for a different feature entirely.

The prompt is the same regardless of which variant or tool you use - the difference is only in how you invoke it.

```
I want to add maintenance mode for rooms.

Maintenance should be planned per room, either for one date or for a date range.
During that period the room should be blocked for reservations, but only for that
period. So if the room is under maintenance today, but next week there is no
maintenance planned anymore, it should still be possible to reserve it for next week.

We also need to know what happened with the maintenance. It can be planned,
completed, or cancelled. Cancelled maintenance should stop blocking the room.
If maintenance finishes earlier than planned, the room should stop being blocked
from the actual completion time.

Please analyze this against the current system and prepare a solution spec.
Do not generate implementation tasks until the direction is approved.
```

### Invocation by tool and variant

| Tool | `vibe/` | `advanced/` |
|---|---|---|
| **Claude Code** | Paste the prompt directly in chat | Use `/spec-writer` command |
| **GitHub Copilot** | - | Select the `spec-writer` agent, then paste the prompt |

The vibe setup has no command or agent - you just talk to the model. Comparing the two outputs on the same prompt is the point of the exercise.
