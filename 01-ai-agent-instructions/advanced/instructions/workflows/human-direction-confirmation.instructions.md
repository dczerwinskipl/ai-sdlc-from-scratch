<!-- Type: workflow -->

# Human Direction Confirmation

This file defines the confirmation procedure used when the Spec Writer has recommended a model and needs human approval before generating the final spec.

This procedure is the canonical source for the confirmation checkpoint. Both `spec-writer-flow.instructions.md` (Step 11) and `architecture-approval-gate.instructions.md` reference this file.

## When to use

Use this procedure after recommending a model in any of these cases:

- the recommendation changes or introduces module or bounded context boundaries
- the recommendation changes aggregate or data ownership
- the recommendation changes public or integration contracts
- the recommendation changes transaction or consistency boundaries
- the recommendation introduces domain or integration events
- the recommendation expands scope beyond explicit requirements

See the full trigger list in `instructions/workflows/architecture-approval-gate.instructions.md`.

## What to present

Before asking for confirmation, present:

- recommended model and why
- what will be implemented if approved
- what stays out of scope
- decisions the human must make now
- decisions that can be deferred
- unresolved open questions that still matter

## Confirmation prompt

Ask for confirmation using exactly this menu:

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

## Hard stop

The Spec Writer must not generate final implementation tasks until one of:

- option 1 is selected, or
- the user provides their own direction (option 4)

Until confirmation is given, the implementation plan must be limited to safe, non-structural steps or explicitly marked as blocked.
