# From Vibe Coding to AI Workflow

Source code and materials for the [From Vibe Coding to AI Workflow](https://dczerwinski.pl) series at dczerwinski.pl — a practical guide to building a structured AI-assisted development workflow, one practice at a time.

## What this series is

Most teams start with a single prompt. It works until it doesn't — until the codebase grows, architectural decisions accumulate silently, and each AI response drifts a little further from where the system should be going.

This series documents the path from that starting point to a workflow that is structured, auditable, and recoverable when the model gets it wrong.

## Repository structure

```
/app/                          — Reference booking system, evolves across all lessons
/01-ai-agent-instructions/     — Lesson: structuring AI agent context (post #3)
```

## The reference app

The booking system in `app/` is intentionally small and contains deliberate design flaws. Not because the author can't write clean code — but because clean code would hide the problem.

In a real project, the drift from a modular monolith toward a tightly-coupled ball of mud happens gradually. God entities grow one field at a time. Temporal coupling accumulates through shared infrastructure. Cohesion drops incrementally. By the time it's obvious, it's expensive to reverse — and in a large codebase you may not notice it happening at all, feature by feature.

The app is small enough that the drift shows up fast, and the before/after is visible within a single lesson. That's the point.

## Posts

| # | Title | Material |
|---|-------|----------|
| 1 | [Why a single prompt is not enough](https://dczerwinski.pl/en/blog/1/why-a-single-prompt-is-not-enough) | — |
| 2 | [What controls an agent](https://dczerwinski.pl/en/blog/2/what-controls-an-agent) | — |
| 3 | Agent instructions (coming soon) | [`01-ai-agent-instructions/`](01-ai-agent-instructions/) |

## Working with this repo

Open each lesson folder as a separate project in your editor — not this root folder. Each folder has its own AI context and agent configuration; opening the parent mixes them.

```
code 01-ai-agent-instructions/advanced/
```

## Git flow

Branches follow this pattern:

```
main                           — stable, published content
feature/post-NN/<description>  — new post content or AI improvements
fix/<description>              — corrections (typos, broken links, wrong code)
chore/<description>            — structural changes (renames, tooling, config)
```

Commits follow [Conventional Commits](https://www.conventionalcommits.org/). Full conventions: [`.ai/git.md`](.ai/git.md)

## AI-Assisted Development

Content and code in this repository were developed with AI assistance. All materials have been written, reviewed, and are authored by [Dominik Czerwiński](https://dczerwinski.pl), who retains full responsibility for the work.

## License

Apache 2.0 — see [LICENSE](LICENSE).
