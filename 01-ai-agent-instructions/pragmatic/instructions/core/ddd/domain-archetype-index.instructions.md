<!-- Archetype: KNOWLEDGE -->
<!-- Role: INDEX -->

# Domain Archetype Index

Check this file against every requirement to detect when domain archetype reasoning may be needed.

Match on the **behavioral signal** column first. Synonyms are a secondary hint — domain language varies too widely to rely on names alone. A concept named "work order" may be Ordering; a concept named "booking" may be a Reservation, an Appointment, or an Order, depending on what it commits to.

**If no archetype fits, do not force one.** Archetypes are proven patterns and discovery prompts, not a closed vocabulary. A concept that matches none of the entries below may be exactly the right design for this domain.

## Usage rules

For every proposed archetype, explain:

- which requirement behavior (not name) suggests this archetype
- which existing system behavior supports it
- what problem the archetype framing helps solve
- what simpler model was considered
- why the simpler model is or is not enough

Do not introduce an archetype only because the name appears in the requirement, the pattern is common in DDD, or the model would look more complete.

If the archetype is only a future possibility, mark it as Potential future model.

---

## Domain language signals

| Archetype | Key behavioral signal | Domain may call it |
|:---|:---|:---|
| Party / Actor / Participant | Someone who performs, receives, pays, owns, or is audited in a transaction; the same entity may play multiple roles simultaneously | user, account, employee, customer, contact, member, owner, payer, recipient, counterparty |
| Identity / Profile | An entity that exists independently of any transaction; stable attributes persist across all contexts and relationships | profile, record, master record, customer record, employee file, contact card |
| Tenant / Organisation Unit | A logical partition of the system; data and rules differ between partitions and are invisible across them | company, branch, workspace, environment, department, division, namespace, unit |
| Role / Permission | A named set of capabilities assignable to and revocable from a Party; a Party may hold multiple roles simultaneously | role, permission, access right, scope, security profile, access group |
| Resource | Something that can be claimed, blocked, or maintained; has identity, lifecycle, and optionally a fixed maximum capacity — not a computed availability state | asset, item, seat, slot, vehicle, room, machine, licence, capacity unit |
| Location / Position | Where something physically or logically is; the location itself determines which operations and rules apply | bin, shelf, rack, zone, position, address, sector, cell, bay, slot |
| Inventory / Stock | Quantity of something at a location at a point in time; changes only through recorded movements — never a mutable field | stock, inventory, quantity on hand, on-hand count, batch, lot, stock level |
| Movement / Transfer | A formal recorded transition of something between locations or owners; the act of moving is itself a first-class domain fact | transfer, movement, shipment, withdrawal, receipt, issue, dispatch, pick, put-away |
| Route / Carrier | How something gets from origin to destination; carrier, path, schedule, and constraints all matter | route, carrier, delivery, transport order, shipping instruction, leg |
| Reservation / Allocation | Claims a resource for a period; can expire, be cancelled, or consumed; price is not committed at this stage | booking, claim, hold, assignment, enrollment, registration, permit, ticket, scheduled use |
| Appointment / Slot | A specific time block assigned to a specific party and a specific provider; both time and person assignment are essential | appointment, visit, meeting, booked slot, scheduled session |
| Availability / Capacity | Computed answer to "can this be used now"; derived from all current claims, blocks, and lifecycle state — never a field on the resource | calendar, schedule, occupancy, utilization, headcount, slot count, open capacity |
| Block / Hold | Prevents use of a resource; system- or admin-initiated; no external actor is waiting for notification if removed | freeze, lock, suspension, restriction, embargo, ban, quarantine, exclusion, blackout |
| Schedule / Calendar | A recurring or time-patterned plan for when things happen; has recurrence rules and exceptions | schedule, calendar, timetable, roster, shift plan, rotation, rota |
| Policy / Rule | Configurable business behavior governing what is allowed; owned and versioned separately from the entities it governs | configuration, setting, profile, preference, template, limit, threshold, ceiling, constraint |
| Quota / Entitlement | How much of something a Party may consume; the quota is a consumable state that decreases with use — not just a rule | quota, allocation, entitlement, allowance, pool, budget unit, rate limit, cap |
| Accounting | Tracks flows of quantifiable value with full immutable history; the model is Account → Entry → Transaction | wallet, credit, ledger, ledger entry, transaction log, journal entry, points, tokens, balance, credit trail |
| Pricing | A dynamic function computing a price from context inputs at the time of use; not a static field on a product | fee, rate, charge, tariff, quote, estimate, cost, surcharge, discount |
| Ordering | Binding commitment that locks a price, commits capacity, and initiates an execution process; price is snapshotted at commitment time | purchase order, work order, service order, job order, confirmed booking, treatment authorisation, service request |
| Membership / Subscription | Ongoing relationship granting access or benefits over time; has tiers, renewal mechanics, and cancellation | membership, subscription, plan, licence, access pass, tier |
| Contract / Agreement | Bilateral commitment with negotiated terms; both parties have obligations and rights; breach has defined consequences | agreement, contract, framework agreement, SLA, master agreement, arrangement |
| Catalogue / Reference Data | Descriptive data used by other domains; does not own operational state or lifecycle | library, registry, directory, lookup, master data, reference list, code table |
| Template / Blueprint | Reusable structure that defines the shape of instances without being an instance itself | template, pattern, form, schema, blueprint, starter configuration, boilerplate |
| Batch / Group / Bundle | Collection of entities treated as one unit for processing, billing, or tracking; the batch has its own lifecycle and aggregate properties | batch, pack, group, bundle, set, campaign, run |
| Versioned Entity / Revision | An entity whose state at specific moments must be preserved as immutable history and potentially restored | version, revision, edition, release, snapshot, draft |
| Notification / Communication | Business communication triggered by a domain event; has recipient, channel, template, and idempotency requirements | alert, message, email, reminder, digest, event receipt, notification |
| Fault / Incident / Defect | Something that went wrong and requires detection, assignment, and resolution; the fault persists as a record after resolution | fault, incident, defect, complaint, report, ticket, issue, outage, deviation |
| Rating / Score / Assessment | Judgment about an entity produced at a point in time by a rater or algorithm; has provenance, methodology, and a validity period | rating, score, assessment, grade, result, evaluation, appraisal |
| Approval / Decision | Formal act of authorising or rejecting something; the authorising act is a first-class domain object with its own actor, rationale, and audit trail | approval, decision, authorisation, consent, sign-off, rejection, clearance |
| Workflow / Process | Coordinates multiple domain actions in sequence or parallel; owns state between steps and handles compensation on failure | flow, pipeline, process, approval chain, onboarding, review cycle |
| Plan / Execution / Delta | Compares intended state against actual state; tracks divergence over time | budget vs actuals, forecast vs reality, scheduled vs completed, planned vs delivered |
| Scenario / Simulation | Hypothetical version of a state used for analysis or planning; carries no commitment and does not need to be executed | scenario, simulation, variant, forecast, what-if, alternative plan |
| Audit / Compliance Record | Immutable record of who did what when; kept for accountability or regulatory purposes | audit log, event trail, activity log, change history, compliance record, journal |
| Document / Formal Record | Formal artefact capturing a state at a point in time with legal or operational standing; has signatories and archival requirements | invoice, certificate, report, receipt, confirmation, declaration, attestation |
| Territory / Jurisdiction | Geographic or logical area that determines which rules, assignments, or parties apply; a location may fall in multiple territories simultaneously | zone, region, territory, district, jurisdiction, sales region, catchment area |

---

## Confusion pairs

Use these when two archetypes seem to apply to the same concept. Each pair ends with a key question.

**Party vs Identity/Profile**
Party: participation in a specific context (who books, who pays, who receives).
Identity: the entity across all contexts, independent of any transaction.
→ Key question: does this represent how someone participates in a transaction, or who they are regardless of transactions?

**Resource vs Location**
Resource: the thing that is claimed, used, or maintained.
Location: where something is; the location determines which rules apply.
→ Key question: is this thing itself claimed or used, or does it determine rules for what is inside it?

**Inventory vs Accounting**
Both model quantities through immutable entries. Inventory = physical goods at a location. Accounting = abstract value flows (money, points, credits).
→ Key question: is the subject physical and locatable, or abstract and monetary?

**Reservation vs Appointment**
Reservation: claims a resource. The specific provider is not fixed.
Appointment: assigns a specific provider to a specific recipient at a specific time.
→ Key question: is a specific person — not just a resource — part of what is being claimed?

**Reservation vs Ordering**
Reservation: claims availability; price is not committed; can expire.
Ordering: locks a price at commitment time; initiates an execution process.
→ Key question: is the price locked? Does placing it trigger fulfilment?

**Block/Hold vs Reservation**
Reservation: external actor wants to use the resource and expects notification if removed.
Block: system or admin prevents use; no external actor is waiting.
→ Key question: who initiates it, and does that initiator expect to be notified if the claim is removed?

**Policy/Rule vs Quota/Entitlement vs Accounting**
Policy defines the rule ("max 100 per day"). Accounting tracks consumption (each use is an immutable entry). Quota is the current state: how many remain. All three may coexist.
→ Key question: is this the rule, the consumption history, or the remaining balance?

**Membership vs Ordering**
Ordering: one-time commitment, price snapshotted at placement, initiates a fulfilment process.
Membership: ongoing relationship, renews, benefits evolve across tiers.
→ Key question: single transaction with a defined endpoint, or ongoing relationship with renewal?

**Contract vs Ordering**
Ordering: typically unilateral — customer accepts vendor-defined terms.
Contract: bilateral — terms are negotiated; both parties have obligations.
→ Key question: were terms negotiated by both parties, or accepted from a standard offering?

**Template vs Catalogue/Reference Data**
Catalogue: describes things that already exist.
Template: generates things that will be created.
→ Key question: does using this thing create a new instance, or describe an existing one?

**Fault vs Workflow/Process**
Fault: the domain object — what went wrong — persists as a record after resolution.
Workflow: the process that resolves the fault.
→ Key question: does the "thing" need to exist as a record after the process ends?

**Approval vs Workflow**
Workflow: coordinates many steps. Approval: a single authorising act that must be captured as a first-class record.
→ Key question: is the authorising act itself the domain object, or just a step inside a larger process?

**Audit vs Versioned Entity**
Versioned Entity: WHAT the entity was at a point in time.
Audit: WHO caused a change, WHEN, and WHY.
→ Key question: reconstructing past state, or establishing accountability for a specific action?

**Territory vs Location**
Location: where something IS.
Territory: what rules apply given where something is. A location may fall in multiple territories.
→ Key question: is this describing a place, or a set of rules that apply within a boundary?

**Schedule vs Plan/Execution/Delta**
Plan: a single forward-looking intent. Schedule: a repeating pattern of intent with recurrence rules.
→ Key question: one-time intent, or recurring pattern?

**Scenario vs Plan**
Plan: intended for execution; carries commitment. Scenario: hypothetical; no commitment.
→ Key question: is this meant to happen, or to answer "what if it happened?"

**Resource vs Inventory/Stock**
Resource: a specific, identifiable object tracked individually — the system cares which exact instance it is (a machine with a serial number, a vehicle with a registration plate).
Inventory: a quantity of interchangeable, fungible units where individual instances have no identity from the system's perspective (100 units of component X, 50 litres of reagent Y).
→ Key question: does the process require tracking a specific individual instance, or is knowing the available quantity of any instance of this type sufficient?

**Movement/Transfer vs Workflow/Process**
Movement: an atomic business fact — something moved from A to B, recorded as a single domain event. Complete when the fact is recorded.
Workflow: orchestrates multiple steps (pick, pack, load, transit, unload, confirm receipt) that together constitute a relocation, with state and error handling at each step.
→ Key question: is this a single recorded fact of a completed transition, or a multi-step coordinated process that advances through states and may require compensation on failure?

---

## Load matching group files in parallel

After identifying matching archetypes in the table above, load the corresponding group files.
Load all identified files in parallel before proceeding with domain discovery.

| Archetypes in group | File |
|:---|:---|
| Party, Identity, Tenant, Role | `instructions/core/ddd/archetypes/identity-organisation.instructions.md` |
| Resource, Location, Inventory, Movement, Route | `instructions/core/ddd/archetypes/resources-location.instructions.md` |
| Reservation, Appointment, Availability, Block, Schedule | `instructions/core/ddd/archetypes/time-claims.instructions.md` |
| Policy, Quota, Accounting, Pricing, Ordering, Membership, Contract | `instructions/core/ddd/archetypes/value-commitments.instructions.md` |
| Catalogue, Template, Batch, Versioned Entity | `instructions/core/ddd/archetypes/structure-meta.instructions.md` |
| Workflow, Approval, Plan/Execution/Delta, Scenario | `instructions/core/ddd/archetypes/process-decisions.instructions.md` |
| Notification, Fault, Rating, Audit, Document, Territory | `instructions/core/ddd/archetypes/quality-accountability.instructions.md` |

When in doubt about which group covers a concept, load all groups that might apply. The cost of loading one extra file is lower than missing a relevant archetype.

**If no archetype fits:** do not load any group file to force a match. `domain-discovery.instructions.md` is already loaded at Step 4 and covers first-principles discovery for novel concepts. Document the concept on its own terms, name it explicitly, and proceed with discovery without forcing an archetype frame.
