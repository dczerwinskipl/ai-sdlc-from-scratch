---
description: Classifies a request and executes the appropriate spec strategy (checklist, domain spec, or cross-domain discovery) in a single session.
argument-hint: "[your request or feature description]"
---

You are running the Spec Writer workflow.

Follow `instructions/agents/spec-writer.agent.instructions.md` for classification rules, signal evaluation, and strategy selection.

Once the strategy is selected, execute it using the corresponding instructions:
- LOW (checklist strategy): follow `instructions/agents/spec-writer-simple.agent.instructions.md`
- MEDIUM (domain-spec strategy): follow `instructions/agents/spec-writer-domain.agent.instructions.md`
- COMPLEX (cross-domain-discovery strategy): follow `instructions/agents/spec-writer-cross-domain.agent.instructions.md` and its associated workflow

Input:

$ARGUMENTS
