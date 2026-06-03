<!-- Archetype: KNOWLEDGE -->

# Quality & Accountability — Domain Archetypes

---

### Notification / Communication

Represents business communication triggered by domain events.

Check:

- what domain event triggers this notification?
- who is the recipient (role, specific person, external system)?
- which channel (email, SMS, push, webhook, in-app)?
- is the template configurable or fixed?
- must the same event not produce duplicate notifications (idempotency)?
- must there be a delivery audit trail?

**Examples:**

- Insurance claims: when a claim is approved, the policyholder receives an email, the payment system receives a webhook, and the broker receives an in-app notification. Three notifications, three channels, one triggering event. Each must be delivered exactly once.
- Supply chain: when a shipment departs, notifications go to the consignee (email), the customs broker (EDI), and the internal tracking system (event). Failure to deliver the customs notification triggers an escalation.

---

### Fault / Incident / Defect

Represents something that went wrong and requires detection, classification, assignment, and resolution. The fault is a domain object independent of the process that resolves it; it persists as a record after resolution.

Check:

- who detects it (automated monitoring, inspection, user report)?
- who is responsible for resolution?
- what is the severity and business impact?
- is there an SLA for resolution time?
- can it be escalated, reassigned, or split?
- must the root cause be documented?
- does resolution trigger further actions (compensation, notification, regulatory reporting)?

**Examples:**

- Food manufacturing: a batch fails a contamination test. A defect record is created capturing batch ID, test result, inspector, and timestamp. A recall process is initiated. The defect record exists permanently, used for regulatory reporting after the recall is complete.
- Financial services: a payment fails to settle. An incident record is created with severity, affected accounts, and timeline. After settlement, the incident record is retained as evidence for reconciliation and dispute resolution.

---

### Rating / Score / Assessment

Represents a judgment about an entity produced at a specific point in time by a rater (person, committee, or algorithm). Has provenance, methodology, and potentially a validity period.

Check:

- who produces the rating (individual, committee, automated model)?
- is the methodology fixed or configurable?
- can the subject dispute the rating?
- does the rating expire or require periodic renewal?
- is the rating an input to other decisions (eligibility, pricing, access)?
- must historical ratings be preserved after a new rating is issued?

Do not model rating as a mutable field on the rated entity. Ratings have history, methodology, and provenance that must be captured separately.

**Examples:**

- Credit scoring: a credit score is computed using a defined model. Scores are valid for 90 days. The score at the time of a loan decision is preserved on the credit file. Future score changes do not affect historical decisions.
- Supplier qualification: a procurement team rates each supplier quarterly on quality, delivery reliability, and financial stability. Improvement in rating unlocks higher-value contracts; deterioration triggers a corrective action plan.

---

### Approval / Decision

See `process-decisions.instructions.md`.

---

### Audit / Compliance Record

Represents an immutable record of who did what when, kept for accountability, security, or regulatory purposes.

Check:

- is retention required by regulation, and for how long?
- must the record be tamper-proof?
- must it be queryable by actor, time range, action type, or subject?
- who has access (administrators only, external auditors)?
- is there a concept of a compliance report derived from these records?

**Examples:**

- Pharmaceutical manufacturing: every step in drug production must be logged — who performed it, on what equipment, at what time, and with what batch of materials. The audit trail is submitted to regulators as part of batch release. Any gap invalidates the batch.
- Financial trading: every order placed, modified, or cancelled must be logged with trader identity, system timestamp, and order parameters. The audit trail is held for seven years under financial regulation.

---

### Document / Formal Record

Represents a formal artefact capturing a state at a specific point in time with legal or operational standing. Has signatories, versions, and archival requirements.

Check:

- does it require signatures (electronic or physical)?
- is it sent to external parties who will rely on its content?
- must the exact content be preserved for legal or audit purposes?
- does it have a lifecycle (draft → issued → archived → voided)?
- is it versioned and does each version need to be independently accessible?

**Examples:**

- Professional certification: when a technician completes a qualification, a certificate is issued. It cannot be retroactively altered. If revoked, a separate revocation document is issued; the original is not modified.
- International trade: a bill of lading is issued when goods are loaded. It is a legal document required for customs clearance. The content at issuance is legally binding; any amendment requires a new document referencing the original.

---

### Territory / Jurisdiction

Represents a geographic or logical area that determines which rules, pricing, assignments, or parties apply to entities within it. A location may fall in multiple territories simultaneously.

Check:

- do rules differ between territories (tax rates, compliance requirements, permitted products)?
- are entities or tasks assigned to parties based on which territory they fall in?
- can a location fall in multiple territories simultaneously?
- does territory assignment change over time, and must historical assignment be preserved?

**Examples:**

- Tax calculation: a delivery address falls simultaneously in a national, state, and municipal jurisdiction. Each layer applies a different rate. When jurisdictional boundaries are redrawn, historical orders must be calculated using the jurisdiction applicable at the time of sale.
- Sales force management: when a lead is created, its address determines which territory it falls in and which executive owns it. Territory boundaries are reviewed annually. Leads created before a boundary change retain their original assignment.
