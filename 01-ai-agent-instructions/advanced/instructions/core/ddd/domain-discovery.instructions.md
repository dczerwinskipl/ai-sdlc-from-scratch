<!-- Archetype: RULES -->

# Domain Discovery

Identify domain concepts by behavior, lifecycle, ownership, and language, not by existing class names.

## Discovery questions

For each important noun, process, or rule, ask:

1. Does it have its own lifecycle?
2. Does it have state transitions independent from other concepts?
3. Does it have business rules that can change independently?
4. Does it require separate permissions?
5. Does it have separate users or owners?
6. Does it integrate with other modules as a source of truth?
7. Does it need historical tracking?
8. Does it create events that other parts of the system react to?
9. Would changing this concept frequently force unrelated code changes?
10. Is the current model mixing multiple business capabilities?

## Extraction signals

The Spec Writer must flag a domain extraction concern when:

- the concept has an independent lifecycle
- the concept owns data used by multiple flows
- multiple features need to query or mutate it
- the concept has policies that differ from the current aggregate or module
- the current aggregate starts coordinating unrelated business processes
- changes in this area frequently affect unrelated parts of the code
- stakeholder language differs from the current module language

## Do not extract only because

Do not propose a new module only because:

- a class is large
- a table has many columns
- the feature is technically complex
- the name sounds like a domain concept
- the code would look cleaner
- a pattern could be applied

## Simplest local alternative

Before proposing extraction of a concept into a separate aggregate, module, or bounded context, the Spec Writer must:

1. Describe the simplest local model — what the implementation looks like if the concept stays where it is.
2. Explain why the local model is insufficient or creates unacceptable risk.
3. Propose extraction only when the local model fails at least one of:
   - independent lifecycle
   - independent ownership
   - independent business rules
   - multiple independent consumers
   - separate consistency requirements
   - separate evolution pressure
   - separate language
   - integration boundary
   - known future variants the local model cannot accommodate

## Discovery vs decision

The agent may discover architectural concerns and recommend options.

The agent may include an architecture decision only when:

- the decision is already documented in knowledge or project architecture
- the user explicitly asked for that direction
- the decision is small and local

Mark as Decision Required when the change affects:

- module boundaries
- bounded context boundaries
- aggregate ownership
- data ownership
- transaction model
- integration model
- public contracts

## Negative example

Example only — not a project rule:

Bad: "Maintenance is mentioned in the requirement. Create a Maintenance module."

Good: "Maintenance appears in the requirement. Flag it as a domain extraction candidate. Present it in domain discovery with extraction signals. Propose it as one option in option analysis. Ask for human direction before changing module boundaries."
