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

Do not introduce a generic Party model if a simple display name is enough.

### Resource

Represents something that can be used, booked, allocated, configured, or maintained.

Check whether the resource has:

- identity
- descriptive attributes
- availability
- capacity
- lifecycle
- maintenance
- ownership

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

Represents whether a resource can be used.

Check:

- what affects availability
- who owns availability state
- whether availability is calculated or stored
- whether blocks can overlap
- whether availability has separate lifecycle from reservation

### Block / Hold

Represents temporary or permanent prevention of usage.

Check:

- source of the block
- reason
- time range
- release rules
- priority
- visibility
- notification impact

### Policy / Rule

Represents configurable business behavior.

Check:

- whether the rule is static or configurable
- who owns the rule
- scope of the rule
- whether it varies by customer, location, business line, resource type, or other scope

### Notification / Communication

Represents business communication triggered by events.

Check:

- triggering event
- recipient
- channel
- template
- idempotency
- audit or history

### Catalogue / Reference Data

Represents descriptive data used by other domains.

Check:

- whether other modules should reference it by identity
- whether descriptive data should be snapshotted
- whether it owns lifecycle or only reference attributes

### Workflow / Process

Represents coordination across multiple domain actions.

Check:

- whether the process owns state
- whether it is orchestration, choreography, or simple application service flow
- whether failure compensation is required
