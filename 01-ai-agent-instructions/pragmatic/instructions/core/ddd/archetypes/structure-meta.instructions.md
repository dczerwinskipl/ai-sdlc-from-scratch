<!-- Archetype: KNOWLEDGE -->

# Structure & Meta — Domain Archetypes

---

### Catalogue / Reference Data

Represents descriptive data used by other domains. Does not own operational state or lifecycle.

Check:

- do other modules reference it by identity?
- must descriptive data be snapshotted at a point in time to protect historical records from future catalogue changes?
- does it own lifecycle, or only reference attributes?
- who is the authoritative source?

Do not treat price as a catalogue attribute. Price is a dynamic function computed at time of use.

**Examples:**

- E-commerce: a product catalogue holds descriptions, dimensions, and category assignments. An order references a product by ID. When the description changes, existing orders must still display what the product looked like at purchase — the order captured a snapshot.
- Healthcare: a procedure code catalogue holds standardised codes. Claims reference procedure codes. When a code is deprecated, historical records must remain linked to the original code.

---

### Template / Blueprint

Represents a reusable structure that defines the shape of instances without being an instance itself.

Check:

- can instances be created from it?
- can the template be versioned independently of its instances?
- do instances inherit future changes, or are they independent at creation time?
- can a template be mandatory?
- who can create, modify, or delete templates?

**Examples:**

- Contract management: a legal team maintains standard templates (NDA, service agreement). A new contract starts from a template and becomes independent of it at creation — changes to the template do not retroactively affect existing contracts.
- IT service management: an incident template defines required fields, default severity, escalation path, and notification list per incident category. Each new incident is created from the appropriate template.

---

### Batch / Group / Bundle

Represents a collection of entities treated as a single unit for processing, billing, or tracking. The batch itself has identity, lifecycle, and aggregate properties independent of its members.

Check:

- are members added and removed dynamically, or is membership fixed at creation?
- does the batch have its own lifecycle (open → processing → closed) separate from member lifecycles?
- are aggregate properties tracked on the batch (total count, total value, completion rate)?
- does processing require all members to succeed, or are partial results acceptable?
- can a member belong to multiple batches simultaneously?

Do not model a batch as a status on individual members. The batch is the domain object.

**Examples:**

- Payroll: at the end of each pay period, a payroll batch contains all payment instructions. The batch has its own lifecycle (prepared → submitted → settled → reconciled). The batch is the unit of processing.
- Clinical trials: patient samples are grouped into a processing batch before being sent to the laboratory. If the batch fails quality control, all samples in the batch are flagged for re-collection.

---

### Versioned Entity / Revision

Represents an entity whose state at specific points in time must be preserved as immutable history and potentially restored. Old versions are immutable; new versions are created rather than overwriting.

Check:

- who can create a new version, and under what conditions?
- are old versions readable by consumers?
- can a previous version be promoted back to current?
- do other entities reference specific versions?
- is there a concept of a draft version not yet published or approved?

**Examples:**

- Regulatory document management: each approved version of a safety procedure is immutable. When the procedure is updated, a new version is submitted for approval. Operators must follow the version current at their last certification.
- Pricing catalogue: a published version is immutable. When prices change, a new version is prepared in draft, reviewed, and published on an effective date. Historical orders reference the version current at order placement.
