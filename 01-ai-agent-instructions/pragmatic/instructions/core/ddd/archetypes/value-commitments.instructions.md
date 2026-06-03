<!-- Archetype: KNOWLEDGE -->

# Value & Commitments — Domain Archetypes

---

### Policy / Rule

Represents configurable business behavior governing what is allowed. Owned and versioned separately from the entities it governs.

Check:

- is the rule static or configurable by business users without engineering involvement?
- who owns the rule and what is its scope (global, per tenant, per product type)?
- does the rule vary by context, channel, or time?
- does the requirement imply a decision engine, rule tree, or scoring model?

**Examples:**

- E-commerce returns: return windows differ by product category. The merchandise team updates these policies seasonally without engineering involvement.
- Lending: loan eligibility criteria change with market conditions. A credit application is evaluated against the Policy current at the time of application.

---

### Quota / Entitlement

Represents how much of a resource or capability a Party is allowed to consume. The quota is a consumable state — not just a rule about limits.

Check:

- what is being limited (API calls, storage, seats, messages)?
- who sets the limit and who tracks consumption?
- what happens at exhaustion (hard block, soft cap, overage billing)?
- can unused quota carry forward?
- does the quota reset on a schedule?

Do not model quota as a field on the Party. A quota has its own scope, reset schedule, and carry-forward rules.

**Examples:**

- Cloud platform: each plan includes a monthly API call quota. When exhausted, requests are rate-limited. The quota resets at the start of each billing period. Unused quota does not carry forward.
- Software licensing: a company purchases 50 named-user licences. Attempting to activate a 51st user is blocked until a seat is released or more licences are purchased.

---

### Accounting

Tracks flows of quantifiable value with full immutable history. The core model is Account → Entry → Transaction.

- Account: holds the current position for one party and one value type.
- Entry: immutable record of a single change (debit or credit), with reason, timestamp, and reference.
- Transaction: groups two or more Entries that must balance or satisfy an invariant.

Check:

- what value is being tracked (money, points, storage, physical quantities)?
- who holds an account (per party, per resource, per tenant)?
- what events trigger entries?
- must historical position at any past point be reconstructable?
- are entries immutable once recorded?
- does the balance rule involve expiry, weighting, or caps?

Do not model value flows as mutable fields. Mutable balances cannot be audited, reconstructed, or corrected without data loss.

**Examples:**

- Loyalty programme: each purchase earns points credited as an immutable entry. A redemption creates a debit entry. The balance is the sum of all entries — never a stored field. Correcting an error means adding a compensating entry.
- Project billing: time entries are recorded as immutable facts. Invoicing reads the sum of unbilled entries. Reversing an incorrect entry creates a correcting entry with a reference to the original.

---

### Pricing

Price is a dynamic function of context — not a static attribute of a product or resource.

Check:

- what inputs determine the price (time, quantity, customer segment, channel, contract terms)?
- are price components additive, multiplicative, or conditional?
- who can define or override price components?
- must the price be committed (snapshotted) at a specific moment?
- must historical prices be auditable?

Do not store price as a field on a product or resource. A single field cannot capture discounts, surcharges, or time-bounded promotions without becoming a hidden policy engine.

**Examples:**

- Hotel: the rate depends on room category, dates, length of stay, channel, and corporate rate. No single field holds the price — computed at search time and snapshotted at booking.
- Freight: the price depends on weight, dimensions, origin, destination, service level, and surcharges. Computed at booking and locked at that point.

---

### Ordering

Represents a formal commitment to deliver goods or services at a specified price. Locks a price, commits capacity, and initiates an execution process.

Check:

- what does the order commit to?
- what price is locked, and when?
- what execution process does it initiate?
- what is its lifecycle (Draft → Confirmed → In Progress → Fulfilled / Cancelled)?
- is partial fulfilment allowed?
- what cancellation and amendment rules apply?

Do not confuse with Reservation. A Reservation claims availability without committing a price. An Order snapshots the price at commitment time.

**Examples:**

- Field service company: a customer requests a repair. A "work order" is created, assigning a technician and locking the agreed call-out rate. The work order initiates dispatch. This is Ordering — the domain calls it a work order.
- Hospital treatment authorisation: an insurer authorises a specific procedure, locking which coverage applies at that moment and triggering care coordination. This is Ordering — no physical product is sold.

---

### Membership / Subscription

Represents an ongoing relationship granting access, capabilities, or benefits over time.

Check:

- does it have tiers with different benefit levels?
- does it renew automatically or require explicit action?
- can it be paused, downgraded, or cancelled mid-period?
- is there a grace period after expiry?
- are benefits consumed (N uses per month) or unlimited within the tier?
- does pricing change at renewal?

**Examples:**

- Gym chain: members pay monthly. Benefits depend on tier. A paused membership freezes billing but preserves history and allows reactivation.
- SaaS tool: companies subscribe to a plan. The plan determines seats, data retention, and export limits. Upgrading mid-cycle requires prorating the difference.

---

### Contract / Agreement

Represents a bilateral commitment with negotiated terms. Both parties have obligations and rights. Breach has defined consequences.

Check:

- were terms negotiated (not just accepted from a standard offering)?
- do both parties have obligations?
- are there penalties or remedies for breach?
- does it have a fixed duration with renewal or termination conditions?
- must the exact agreed terms be preserved for legal purposes?
- can it be amended, and does amendment require both parties' consent?

**Examples:**

- Staffing agency: each enterprise client has an individually negotiated agreement specifying billing rate, payment terms, and liability cap. Terms differ between clients.
- Supply chain: a manufacturer and supplier agree on delivery quantities, quality standards, pricing formula, and penalty clauses. Both parties have binding obligations.
