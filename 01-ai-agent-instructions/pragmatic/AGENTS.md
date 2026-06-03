# AI Agent Instructions

This repository contains a context-aware Spec Writer workflow that turns collected knowledge, requirements, and draft acceptance criteria into a reviewable solution spec.

For Spec Writer tasks, follow `instructions/agents/spec-writer.agent.instructions.md` to classify the request and route to the appropriate workflow:

- LOW complexity → `instructions/agents/spec-writer-simple.agent.instructions.md`
- MEDIUM complexity → `instructions/agents/spec-writer-domain.agent.instructions.md`
- COMPLEX / cross-domain → `instructions/agents/spec-writer-cross-domain.manifest.md`
