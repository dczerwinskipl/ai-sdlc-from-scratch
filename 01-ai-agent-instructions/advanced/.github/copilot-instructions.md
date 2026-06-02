---
agentContext:
  agents.d: [".github/agents"]
---

# Repository AI Workflow Instructions

This repository uses a context-aware SDLC workflow. The preferred flow is: knowledge / discovery → solution spec → implementation → review. Do not treat a user request as an isolated coding task when it may affect domain behavior, architecture, module boundaries, ownership, contracts, or consistency.

When asked to act as Spec Writer, follow `instructions/agents/spec-writer.manifest.md` for the complete list of instruction files to load and under what conditions.
