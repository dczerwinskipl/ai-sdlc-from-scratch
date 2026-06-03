<!-- Archetype: KNOWLEDGE -->

# Identity & Organisation — Domain Archetypes

---

### Party / Actor / Participant

Represents a person, organisation, account, or external system in the role it plays within a specific transaction or context. The same real-world entity may play different roles simultaneously.

Check:

- who performs an action
- who benefits from it
- who pays
- who receives communication
- who owns the relationship
- who is audited

Check whether parties relate hierarchically (employee → department → holding). If so, check whether rules, permissions, billing, or reporting depend on structural position.

Do not introduce a generic Party model if a simple display name is enough.

**Examples:**

- Insurance: a single policy covers a policyholder (who pays), an insured person (who is covered), and a beneficiary (who receives the payout). These may be three different people. Each is a Party role in the same policy.
- Freight: a shipment involves a shipper, a consignee, a notify party, and a payer. All four roles must be tracked independently for customs, delivery, and billing.

---

### Identity / Profile

Represents the stable, long-lived attributes of an entity that persist across all contexts and transactions. The entity exists independently of any transaction or role it may play.

Check:

- does the entity exist before it participates in any transaction?
- must attributes be preserved across context switches?
- is there a concept of merging or deduplicating records for the same real-world entity?
- do GDPR, data portability, or right-to-erasure requirements apply?
- must historical attribute values be accessible at a specific point in time?

Do not model identity as a by-product of the first transaction. An entity that has never completed a transaction may still need to exist in the system.

**Examples:**

- B2B platform: the same company is both a customer and a supplier. A single Identity record represents the company. Two separate Party roles represent the two relationships.
- Freelancer marketplace: the same person can be a client and a service provider. Using "client account" as the identity breaks down the moment someone switches roles.

---

### Tenant / Organisation Unit

Represents a logical partition of the system where entities, rules, and data belong to one partition and are invisible to others.

Check:

- do rules, configurations, or pricing differ between partitions?
- are entities in one partition invisible or inaccessible to another?
- can a user belong to more than one tenant simultaneously?
- is there a concept of cross-tenant operations or shared reference data?
- does billing happen at the tenant level?

Do not conflate Organisation Unit with a Party hierarchy. An org unit is a system partition; a Party hierarchy describes relationships between participants within a partition.

**Examples:**

- Legal firm management: each law firm is a tenant. Cases, clients, and billing records are fully separated between firms. A lawyer who works for two firms has access to both tenants but their data never merges.
- Hospital network: each hospital is an Organisation Unit. Patients registered at Hospital A are not visible to Hospital B by default.

---

### Role / Permission

Represents a named set of capabilities or access rights assignable to and revocable from a Party, independently of the Party's identity.

Check:

- can the same Party hold multiple roles simultaneously?
- can roles be inherited from a group or parent entity?
- can roles be time-bounded or context-scoped?
- can one Party delegate a role to another?
- does the same role name mean different capabilities in different contexts?

Do not model role as a status field on the Party. Status implies one value at a time; a Party may hold multiple roles simultaneously.

**Examples:**

- Project management tool: a user may be an Owner in one project, an Editor in a second, and a Viewer in a third simultaneously. Role is a relationship between the user and the project, not a property of the user.
- Hospital system: a physician has a general "Doctor" role, but specific procedure authorisations are scoped per department. The system-wide role is insufficient.
