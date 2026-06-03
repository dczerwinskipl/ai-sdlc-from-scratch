<!-- Archetype: KNOWLEDGE -->

# Time & Claims — Domain Archetypes

---

### Reservation / Allocation

Represents a claim on a resource for a period, quantity, or condition. Price is not committed at this stage.

Check:

- what is reserved (resource, quantity, slot)?
- for whom and by whom?
- for what time range?
- whether the claim is temporary or confirmed
- whether it can expire, be cancelled, released, or consumed

**Examples:**

- Tennis court booking: a slot is reserved for 90 minutes. The reservation holds the slot but does not commit to a price for racket rental. It expires if payment is not completed within 15 minutes.
- Cargo shipping: container space is allocated to a shipper before the manifest is finalised. The allocation guarantees space but does not lock the freight rate.

---

### Appointment / Slot

Represents a specific time block assigned to a specific party for a specific purpose, at a specific location or with a specific provider. Both the time and the person assignment are essential.

Check:

- is the appointment tied to a specific person (provider), not just a resource?
- is the time window fixed and non-movable without explicit rescheduling?
- does the party need to confirm attendance?
- is there a no-show or late-cancellation policy?
- can the same party have multiple appointments for the same purpose?

Do not use when the specific provider is not part of what is being claimed — that is Reservation.

**Examples:**

- Medical clinic: a patient books a 20-minute consultation with a specific physician on a specific day. The physician's identity matters — cancelling the physician's availability requires rescheduling all their appointments, not just releasing a resource slot.
- Vehicle inspection: a car owner books an MOT test at a specific time. If the inspector is sick, the appointment must be reassigned — the station is still available, but the Appointment must be rescheduled.

---

### Availability / Capacity

Represents whether and when a resource can be used. Computed from all current constraints, claims, and lifecycle state — not a field stored on the resource.

Check:

- who owns the definitive answer to "can this be used for this period / quantity / condition"?
- what independent factors affect availability (reservations, blocks, lifecycle status, capacity rules)?
- must mutual exclusion across claim sources be enforced by one authority?
- must availability survive temporary unavailability of claim sources?
- is availability calculated at query time or maintained as events occur?

Do not model availability as a status field on the resource. A resource may be active and still unavailable for a specific period or quantity.

**Examples:**

- Hotel: room availability is not a field on the room record. It is computed from confirmed reservations, maintenance blocks, operational status, and occupancy rules.
- Conference venue: availability for a specific event is a function of current bookings, setup and teardown windows, exclusive-use blocks, and the hall's maintenance schedule — never a single flag.

---

### Block / Hold

Represents temporary or permanent prevention of usage, initiated by the system or an administrator.

Check:

- what is the source (system rule, admin action, maintenance, external regulation)?
- what is the reason?
- what is the time range?
- what are the release rules?
- what is the priority relative to reservations and other blocks?
- is the block visible to end users?

Do not confuse with Reservation. A Block is system- or admin-initiated; no external actor is waiting for notification if removed.

**Examples:**

- Airline seat management: a block of seats is held for upgrade-eligible passengers until 24 hours before departure. System-initiated, released automatically, no individual customer is waiting.
- Equipment maintenance: a CNC machine is blocked for 3 days during preventive maintenance. Admin-initiated, reason-coded, visible to schedulers. Overlapping reservations must be rescheduled.

---

### Schedule / Calendar

Represents a recurring or time-patterned plan for when things happen. Has recurrence rules, exceptions, and a validity period.

Check:

- does the pattern repeat (daily, weekly, custom rule)?
- can individual occurrences be overridden without changing the underlying pattern?
- does the schedule drive resource allocation, staffing, or availability?
- who maintains the schedule and who is affected by changes?
- does the schedule have a validity period?

**Examples:**

- Public transport: a bus route operates on a weekly schedule with different timetables per day type. Bank holidays override the Sunday schedule. Each departure is an occurrence derived from the pattern; delays are recorded against the occurrence, not the pattern.
- Shift management: a hospital ward operates a rotating shift schedule across three teams. If a nurse calls in sick, the affected shift occurrence is modified without changing the underlying pattern.
