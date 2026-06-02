<!-- Archetype: RULES -->

# Architecture Approval Gate

The agent may identify, document, and recommend architectural changes.

The agent must not treat architectural changes as approved implementation scope.

## Allowed

The agent may:

- detect that a concept may deserve a separate module, bounded context, aggregate, or domain capability
- propose extracting or splitting responsibilities
- compare multiple solution models
- recommend the model that best satisfies confirmed AC, stated priorities, and domain constraints
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

## Confirmation procedure

When the gate is triggered, present the following before generating the final spec:

- recommended model and why
- what will be implemented if approved
- what stays out of scope
- decisions the human must make now
- decisions that can be deferred
- unresolved open questions that still matter

Then ask for confirmation using exactly this menu:

```
Do you want me to use this direction for the final spec?

1. Yes, use this model.
2. No, revise the models.
3. Analyze the options again with different priorities.
4. I want to provide my own direction.
```

## After the user responds

The user may:

- approve the recommended model (option 1) — proceed to implementation planning
- reject and request revision (option 2) — revise models and re-run Steps 7–10
- request re-analysis with different priorities (option 3) — return to Step 9 with the new priority context
- provide their own direction (option 4) — treat the user's direction as the approved model and document it in the decision record

If the user changes priorities or provides new information, update the analysis or recommendation before generating the final spec.

## Post-approval required steps

After option 1 is selected or the user provides their own direction (option 4):

1. Run the artifact reconciliation pass (Step 12 of the workflow) before generating any post-decision artifacts. Follow `instructions/workflows/artifact-reconciliation.instructions.md`.
2. The reconciliation pass must produce a visible report ending with `CLEAR` before `implementation-plan.md` generation begins.
3. Run the implementation readiness check (Step 13 of the workflow) and produce a visible readiness declaration.

`implementation-plan.md` generation is blocked until:
- the reconciliation report shows `CLEAR`, and
- the readiness declaration shows `Ready` or `Ready after minor clarification`

## Hard stop

The agent must not generate final implementation tasks until one of:

- option 1 is selected, or
- the user provides their own direction (option 4)

Until confirmation is given, the implementation plan must be limited to safe, non-structural steps or explicitly marked as blocked.

After confirmation is given, the reconciliation pass and readiness check must complete before implementation tasks are generated.

## Core rule

The agent can recommend architecture changes, but cannot approve them.
