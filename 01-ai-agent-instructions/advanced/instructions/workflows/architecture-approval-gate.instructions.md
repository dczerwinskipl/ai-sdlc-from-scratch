<!-- Type: guardrail -->

# Architecture Approval Gate

The agent may identify, document, and recommend architectural changes.

The agent must not treat architectural changes as approved implementation scope.

## Allowed

The agent may:

- detect that a concept may deserve a separate module, bounded context, aggregate, or domain capability
- propose extracting or splitting responsibilities
- compare multiple solution models
- recommend the simplest sufficient model
- document benefits, risks, trade-offs, and migration path
- mark the recommendation as requiring human approval

## Not allowed without human approval

The agent must not:

- create implementation tasks that introduce a new module or bounded context
- move entities or behavior between modules
- change aggregate ownership
- change data ownership
- introduce new integration contracts
- introduce new public APIs
- change transaction boundaries
- introduce eventual consistency, saga, compensation, or integration events
- present a proposed architecture as already decided

## Human approval is required before

- creating a new module
- creating a new bounded context
- moving behavior between modules
- changing aggregate or data ownership
- introducing new integration contracts
- changing transaction boundaries
- changing the consistency model
- introducing eventual consistency, saga, or compensation
- changing public APIs
- introducing domain events or integration events
- adding a common archetype as a structural model rather than a discovery note
- expanding scope beyond explicit requirements

## When the gate is triggered

When an architectural change is involved, follow the direction questions, recommendation, and confirmation steps defined in `instructions/workflows/spec-writer-flow.instructions.md` (Steps 9–11).

## Alternative user responses

The workflow must support more than simple approval.

The user may:

- approve the recommended model
- select a different model
- ask for a hybrid model
- provide a fix to one or more models
- change priorities
- ask the Spec Writer to re-analyze the options
- answer previously open questions
- reject all models

If the user changes priorities or provides new information, update the analysis or recommendation before generating the final spec.

## Core rule

The agent can recommend architecture changes, but cannot approve them.
