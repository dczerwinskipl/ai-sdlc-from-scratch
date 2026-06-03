# Post 01 - AI Agent Instructions

Source for the blog post at [dczerwinski.pl](https://dczerwinski.pl) <!-- TODO: add post link --> - coming soon.

The question this post explores: what difference does it actually make to give an AI agent a well-structured set of instructions versus a basic context file with some project notes?

Three variants are included - `vibe/`, `advanced/`, and `pragmatic/` - all targeting the same booking system codebase.

---

## How to test

Open the inner folder (`vibe/`, `advanced/`, or `pragmatic/`) as the project root in Claude Code or VS Code - not this folder. Each variant has its own instruction set and agent configuration. Opening the parent folder mixes agents from different examples and defeats the point.

---

## Vibe vs Advanced vs More Advanced

| | `vibe/` | `advanced/` | `pragmatic/` |
|---|---|---|---|
| **Core file** | `CLAUDE.md` | `instructions/` tree + adapters | `instructions/` tree + triage orchestrator |
| **Setup cost** | Minutes | Hours | Days |
| **Entry point** | Chat | `/spec-writer` | `/spec-writer` (facade) |
| **Workflow** | None — agent decides | Fixed 12-step pipeline | Dynamic — triage classifies, routes to LOW / MEDIUM / COMPLEX strategy |
| **Context cost** | All at once | Manifest-driven, conditional | Strategy-driven — checklist only for LOW, DDD analysis for MEDIUM, full cross-domain discovery for COMPLEX |
| **Architecture gate** | None | Explicit | Explicit |
| **Reviewer** | None | None | 8 specialized sub-reviewers, orchestrated |
| **Multi-tool** | Claude Code only | Claude Code + Copilot + Codex | Claude Code + Copilot + Codex |

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

### More Advanced

An orchestration layer on top of the advanced setup. The Spec Writer becomes a facade: it evaluates incoming requests against a set of signals, classifies the complexity, and routes to the appropriate internal strategy - without the user having to choose.

Key additions over the advanced setup:

- **Triage** - each request is evaluated against seven signals (ambiguity, source-of-truth, ownership, blast radius, reversibility, cross-domain impact, domain analysis). The result is a classification: LOW, MEDIUM, or COMPLEX.
- **Strategy routing** - LOW routes to a checklist-only workflow; MEDIUM runs domain spec with DDD analysis; COMPLEX runs full cross-domain discovery before any spec is produced. The entry point is always the same command.
- **Escalation chain** - each strategy re-checks its own entry conditions and escalates if assumptions fail. A checklist strategy that surfaces hidden domain rules hands off to the domain-spec strategy mid-execution.
- **Reviewer** - eight specialized sub-reviewers (architecture, security, performance, data integrity, test coverage, spec compliance, code quality, acceptance criteria) orchestrated by a single `/reviewer` command.

The cost is real: every request pays for triage before any output is produced. For a team where most requests are MEDIUM, the routing adds overhead with no routing benefit. It pays off when the variance in incoming requests is high enough that sending everything through the same heavy workflow would be wasteful.

---

## Example session artifacts

Each variant contains pre-generated artifacts from real sessions. You can browse them before running anything yourself.

### Room maintenance - COMPLEX

The `docs/spec/room-maintenance/` folder inside each variant. Used the room-maintenance prompt below. The domain discovery step surfaced a gap in the original requirements: cancellation handling and the status lifecycle were missing. That correction was fed back after the proposal, not before — showing how structured spec work makes gaps visible at the point where they are cheapest to fix.

In `pragmatic/`, the triage classifies this as COMPLEX and runs the cross-domain discovery strategy.

### Reservation confirmation - MEDIUM

The `docs/spec/reservation-confirmation/` folder inside `pragmatic/`. Changes the conflict detection rule so only Confirmed reservations block new ones, and adds cascade cancellation on confirmation.

In `pragmatic/`, the triage classifies this as MEDIUM: existing business rules must be analyzed (status lifecycle, conflict detection logic, cascade behavior), but the change is fully contained within the `Reservations` module. The domain-spec strategy runs; no cross-domain discovery is needed.

### Room photo - LOW

The `docs/spec/room-photo/` folder inside `pragmatic/`. Adds a base64-encoded JPG field to `Room` with format, size, and resolution validation.

In `pragmatic/`, the triage classifies this as LOW: business meaning is clear, source-of-truth is a single entity, no new business logic is introduced. The checklist strategy produces seven implementation steps and seven tests. No domain discovery, no solution options, no architecture gate.

---

**Is the orchestration always worth it?**

The room-photo checklist could have been written by a developer in five minutes without any agent. The reservation-confirmation spec required understanding the existing status lifecycle — the triage at least prevented the agent from jumping into implementation and missing the cascade behavior.

The setup in `pragmatic/` makes sense when the complexity distribution of incoming requests is genuinely varied. When most requests fall in the same band, the classification step produces no real routing decision — it is overhead without benefit. That tradeoff is worth testing before committing to this level of structure.

---

## Example prompts for testing

Each variant contains expected output artifacts for the examples above. Before running a prompt, either delete those files or ask for a different feature entirely.

### Room maintenance (COMPLEX)

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

### Reservation confirmation (MEDIUM)

```
Change how reservation conflict detection works. Right now any active reservation
blocks new ones for the same room and period. I want only Confirmed reservations
to block new bookings. Pending reservations should not prevent other guests from
booking the same room and period.

When a reservation is confirmed, check for conflicts with existing Confirmed
reservations and fail if one is found. Also automatically cancel all other
Pending reservations for the same room that overlap with the confirmed period.

Please analyze and prepare a solution spec.
```

### Room photo (LOW)

```
Add a photo to Room. Store it as a base64 JPG string. Validate on upload:
JPEG format only, max 1024x768 resolution, max 2MB. Return the photo when
fetching room details.
```

### Invocation by tool and variant

| Tool | `vibe/` | `advanced/` | `pragmatic/` |
|---|---|---|---|
| **Claude Code** | Paste the prompt directly in chat | Use `/spec-writer` command | Use `/spec-writer` command |
| **GitHub Copilot** | - | Select the `spec-writer` agent, then paste the prompt | Select the `spec-writer` agent, then paste the prompt |

The vibe setup has no command or agent - you just talk to the model. In `pragmatic/`, all three prompts go through the same `/spec-writer` entry point - the triage decides which strategy runs.

---

## Example session stats (advanced variant, no token optimization)

| Metric | Value |
|--------|-------|
| Total cost | ~$2.69 |
| Spec output | 1,463 lines across 6 artifacts |
| claude-sonnet-4-6 | 1.1k input / 82.1k output / 2.2m cache read / 212k cache write |
| claude-haiku-4-5 | 492 input / 15 output (subagent tasks) |

This is a showcase session. The outputs are intentionally rich — the goal was to demonstrate what structured instructions can produce, not to minimize cost or token usage. In a real project you would optimize: fewer output tokens, more targeted documents, generating only what you actually need. That tradeoff is the subject of the next post.
