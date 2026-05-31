<!-- Archetype: KNOWLEDGE -->

# Domain Archetypes

Use archetypes as discovery aids, not as implementation instructions.

The agent may use archetypes to ask better questions and detect missing concepts.

The agent must not force an archetype when requirements do not justify it.

## Usage rules

For every proposed archetype, explain:

- which requirement suggests this archetype
- which existing system behavior supports it
- what problem it solves
- what simpler model was considered
- why the simpler model is or is not enough

Do not introduce an archetype only because:

- the name appears in the requirement
- the archetype is common in DDD
- the model would look more complete
- the archetype may be useful in the future
- the current code looks too simple

If the archetype is only a future possibility, mark it as Potential future model, not as the recommended design.

## Common archetypes

Use this list as a light heuristic, not as a mandatory catalogue.

### Party / Actor / Participant

Represents people, organizations, accounts, employees, customers, guests, requesters, payers, owners, contacts, or external systems involved in an action.

Check whether the model must distinguish:

- who performs an action
- who benefits from it
- who pays
- who receives communication
- who owns the relationship
- who is audited

**Identity vs role:** The same subject may act as employee, customer, and payer simultaneously. Check whether the model conflates identity (who the entity is) with role (how it participates in this context). Conflating identity with role makes it impossible to model the same entity acting in multiple roles, across multiple tenants, or with changing responsibilities over time.

**Structural relationships:** Check whether parties relate to each other hierarchically (employee → department → holding, customer → group → parent company). If so, check whether rules, permissions, billing, or reporting depend on structural position. Hierarchy may affect scope of access, aggregated billing, or inherited policies.

Do not introduce a generic Party model if a simple display name is enough.

### Resource

Represents something that can be used, booked, allocated, configured, or maintained.

Check whether the resource has:

- identity
- descriptive attributes
- capacity
- lifecycle
- maintenance
- ownership

**Do not model availability as a field on the resource.** Availability is a computed result derived from the current reservation set, plans, blocks, and capacity rules — not a flag or status stored on the resource itself. See the Availability archetype.

### Reservation / Allocation

Represents a claim on a resource for a period, quantity, or condition.

Check:

- what is reserved
- for whom
- by whom
- for what time range
- whether the claim is temporary or confirmed
- whether it can expire, be cancelled, released, or consumed

### Availability / Capacity

Availability is a derived, dynamic model — not a property stored on a resource.

Availability is computed from the algebra of reservations, capacity limits, plans, and blocks at a point in time or over a time range.

Check:

- what inputs determine availability (reservations, plans, blocks, capacity rules)
- who owns each input
- whether availability is computed on request or materialized
- whether blocks can overlap and how overlaps are resolved
- whether availability has a separate lifecycle from reservation and resource
- whether different actors see different availability views based on permissions

Do not model availability as a status field on the resource. A resource may be Active and still unavailable for a specific time range.

### Block / Hold

Represents temporary or permanent prevention of usage.

Check:

- source of the block (system, admin, maintenance, external rule)
- reason
- time range
- release rules
- priority relative to reservations and other blocks
- visibility
- notification impact

### Policy / Rule

Represents configurable business behavior.

Check:

- whether the rule is static or configurable
- who owns the rule
- scope of the rule (global, per tenant, per resource type, per customer segment)
- whether it varies by customer, location, business line, resource type, or other scope

**Configurable logic:** When business users must define or change rules without developer involvement, a flag or enum is not enough. Check whether the requirement implies a decision engine, a scoring model, a rule tree, or a custom algebra that operates on domain objects. These models require separate ownership, versioning, and testability from the domain objects they govern.

### Accounting

Represents the flow of any quantifiable value through the system.

Use this archetype for money, physical quantities, loyalty points, storage quotas, credits, or any other value that must be tracked with full history and without retroactive modification.

The core model is: **Account → Entry → Transaction**

- An Account holds the current position (balance) for one party and one value type.
- An Entry is an immutable record of a single change to an Account (debit or credit), with reason, timestamp, and reference.
- A Transaction is the unit of consistency — it groups two or more Entries that must balance or satisfy a defined invariant.

Check:

- what value is being tracked (money, quantity, points, quota)
- who holds an account (per party, per resource, per tenant, per product)
- what events trigger entries
- whether the system needs to reconstruct position at any point in time
- whether entries must be immutable once recorded
- whether the balance rule is simple (sum) or composite (weighted, capped, expiring)
- whether entries expire, carry forward, or are consumed

Do not model value flows as mutable fields or status updates. Mutable balance fields cannot be audited, reconstructed, or corrected without data loss.

### Pricing

Price is a dynamic function of context, not a static attribute of a product or resource.

Check:

- what inputs determine the price (time, quantity, customer segment, channel, promotional codes, contract terms, resource type)
- whether price components are additive, multiplicative, or conditional
- who is allowed to define or override price components
- whether the final price must be calculated at request time or committed at a specific moment (e.g., order placement)
- whether a price snapshot must be stored to protect historical records (e.g., what the customer was shown at checkout)
- whether price rules must be versioned or audited

Do not treat price as a simple field on a catalogue item. A price field on a product cannot capture discounts, surcharges, customer-specific rates, or time-bounded promotions without becoming a hidden policy engine.

### Catalogue / Reference Data

Represents descriptive data used by other domains.

Check:

- whether other modules should reference it by identity
- whether descriptive data must be snapshotted at a point in time (to protect historical records from future catalogue changes)
- whether it owns lifecycle or only reference attributes

Do not treat price as a stable catalogue attribute. Price is a dynamic function computed by the Pricing archetype, not a field owned by the catalogue.

### Notification / Communication

Represents business communication triggered by events.

Check:

- triggering event
- recipient
- channel
- template
- idempotency
- audit or history

### Ordering

Represents a formal commitment to deliver goods or services at a specified price and under specified conditions.

An Order is not a Reservation. A Reservation claims availability. An Order is a binding commitment that locks a specific price, commits inventory or capacity, and initiates an execution process.

Check:

- what the order commits to (which resources, quantities, service items)
- at what price and under what conditions (snapshot from Pricing — captured at order time)
- what execution process is initiated (fulfilment, delivery, provisioning)
- what lifecycle the order has (Draft → Confirmed → In Progress → Fulfilled / Cancelled)
- whether partial fulfilment is allowed
- what cancellation and amendment rules apply
- whether the order must survive changes to the underlying catalogue, resource, or pricing model

### Workflow / Process

Represents coordination across multiple domain actions.

Check:

- whether the process owns state
- whether it is orchestration, choreography, or simple application service flow
- whether failure compensation is required

**Non-linear processes:** Not all processes are linear sequences. Check whether the process is a dependency network (some steps can proceed in parallel, some require multiple predecessors to complete). Check whether the domain has cycles (a step repeats until a condition is met), branching (conditional paths), or convergence (multiple parallel paths joining at a checkpoint).

**State graphs:** When a process has complex branching or convergence, model it as a state graph rather than a sequential step list. Identify: states, transitions, triggering events, guards, and the entities responsible for advancing the process.

**Influence zones:** Check whether completing a step in one process unblocks or triggers steps in other processes. Model these dependencies explicitly — do not rely on implicit coupling through shared state.

### Plan / Execution / Delta

Represents the relationship between what was planned and what actually happened.

Use this archetype when the system must track whether execution matches a plan, calculate divergence, or simulate alternative scenarios.

The core model is: **Plan → Execution → Delta**

- A Plan represents the intended state at a future point in time (schedule, budget, delivery forecast, capacity allocation).
- An Execution represents what actually happened (actual deliveries, actual spend, actual occupancy).
- A Delta is the computed difference between Plan and Execution at a specific point in time or over a range.

Check:

- what is being planned (schedule, capacity, budget, quantity, route)
- what execution events update the actual state
- whether the delta must be computed on request or materialized
- whether the system must support what-if simulations (alternative plans against the same execution history)
- whether partial execution must be tracked (plan partially fulfilled)
- whether plan revisions are allowed and whether historical plan versions must be preserved
- whether actors need to approve, reject, or negotiate deltas

Do not model plan vs execution as a boolean flag (e.g., `isCompleted`). A flag cannot represent partial completion, late delivery, over-delivery, or divergence trends over time.
