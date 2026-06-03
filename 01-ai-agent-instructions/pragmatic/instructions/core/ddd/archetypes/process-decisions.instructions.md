<!-- Archetype: KNOWLEDGE -->

# Process & Decisions — Domain Archetypes

---

### Workflow / Process

Represents coordination across multiple domain actions with owned state between steps.

Check:

- does the process own state that persists between steps?
- is it orchestration (one coordinator knows all steps), choreography (each step reacts to events), or a simple call sequence?
- is failure compensation required?
- does the process have cycles, parallel paths, or convergence points?

When a process has complex branching or convergence, model it as a state graph: states, transitions, triggering events, guards, and the entity responsible for advancing.

**Examples:**

- Loan application: Credit Check → Document Collection → Underwriting → Approval → Disbursement. Some steps run in parallel. If underwriting rejects, the process terminates. The process owns state at each step.
- Employee onboarding: IT account creation, access card provisioning, payroll registration, and compliance training can proceed in parallel after the offer is accepted. Some steps have dependencies. The process tracks completion of each step independently.

---

### Approval / Decision

Represents a formal act of authorising or rejecting a proposed action or artefact. The act of authorising is itself a first-class domain object with its own actor, rationale, and audit trail.

Check:

- who is authorised to approve (a role, a specific person, a quorum)?
- can approval be delegated?
- can an approval be revoked after it is given?
- does rejection trigger a revision cycle?
- is there a timeout or escalation if no decision is made?
- does approval trigger downstream processes?

Do not collapse approval into a status field on the thing being approved. Approval has its own actors, timeline, and rationale.

**Examples:**

- Capital expenditure: a purchase request above a threshold requires a department head approval then a CFO approval. Each approval is a distinct decision record: who approved, when, with what authority, and with what comment.
- Clinical protocol deviation: a nurse deviating from a clinical protocol requires physician authorisation before proceeding. The authorisation is a formal domain event kept permanently in the patient record.

---

### Plan / Execution / Delta

Represents the relationship between what was intended and what actually happened. Core model: Plan → Execution → Delta.

- Plan: the intended state at a future point (budget, capacity, schedule, forecast).
- Execution: what actually happened (actual spend, actual occupancy, actual deliveries).
- Delta: the computed difference between Plan and Execution.

Check:

- what is being planned (schedule, budget, capacity, route, quantity)?
- what execution events update the actual state?
- must the delta be computed on request or materialised continuously?
- are plan revisions allowed, and must historical plan versions be preserved?
- must the system support what-if comparisons?

Do not model plan vs execution as a boolean flag. A flag cannot represent partial completion, late delivery, over-delivery, or divergence trends.

**Examples:**

- Construction project: a project has planned cost, milestone dates, and resource allocations. As work is completed, actuals are recorded. The delta drives variance reports and early warning alerts. Revised plans are stored alongside originals.
- Manufacturing production: a production run has planned output, cycle time, and material consumption. The delta reveals yield rates and scrap levels, feeding into the next production plan.

---

### Scenario / Simulation

Represents a hypothetical version of a state used for analysis or planning. Carries no commitment and does not need to be executed.

Check:

- can multiple scenarios exist in parallel for comparison?
- are scenarios derived from real data with hypothetical changes, or built from scratch?
- can a scenario be promoted to become the plan?
- are results computed on demand or pre-materialised?
- must scenarios be preserved for audit?

**Examples:**

- Financial planning: a finance team builds three budget scenarios (base, optimistic, pessimistic). At board sign-off, one is promoted to the official budget. The others are retained for comparison throughout the year.
- Capacity planning: a logistics team simulates adding a new distribution centre using historical order data. If the simulation shows sufficient improvement, the decision to invest is taken and the scenario becomes the capital expenditure plan.
