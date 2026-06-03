---
agentContext:
  agents.d: [".github/agents"]
---

# Repository AI Workflow Instructions

This repository uses a context-aware SDLC workflow. The preferred flow is: knowledge / discovery → solution spec → implementation → review. Do not treat a user request as an isolated coding task when it may affect domain behavior, architecture, module boundaries, ownership, contracts, or consistency.

When asked to act as Spec Writer, follow `instructions/agents/spec-writer.agent.instructions.md` to classify the request and route to the appropriate workflow:

- LOW complexity → `instructions/agents/spec-writer-simple.agent.instructions.md`
- MEDIUM complexity → `instructions/agents/spec-writer-domain.agent.instructions.md`
- COMPLEX / cross-domain → `instructions/agents/spec-writer-cross-domain.manifest.md`
