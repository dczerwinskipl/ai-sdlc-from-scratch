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
- changing aggregate ownership
- changing data ownership
- introducing new integration contracts
- changing transaction boundaries
- introducing eventual consistency, saga, or compensation
- changing public APIs
- adding a common archetype as a structural model rather than a discovery note

## Required behavior

If the best model requires an architectural change, produce:

1. Current model
2. Problem with the current model
3. Proposed change
4. Alternatives considered
5. Acceptance Criteria coverage
6. Benefits
7. Risks
8. Complexity size
9. Migration impact
10. Recommendation
11. Human approval required

Until approval is given, the implementation plan must be limited to safe, non-structural steps or explicitly marked as blocked.

## Core rule

The agent can recommend architecture changes, but cannot approve them.
