<!-- Archetype: KNOWLEDGE -->

# Resources & Location — Domain Archetypes

---

### Resource

Represents something that can be used, booked, allocated, configured, or maintained. Has identity and a lifecycle independent of any specific use.

Check:

- does it have identity (can two instances be distinguished)?
- does it have a fixed maximum capacity (a physical upper limit — seats in a hall, volume of a tank)?
- does it have a maintenance or operational lifecycle (active, inactive, under maintenance)?
- does it have an owner?
- can it be claimed or blocked by more than one type of actor?

**Maximum capacity vs available capacity:** a resource may carry its physical maximum as a static attribute (a room seats 50). Available capacity at any moment is computed from reservations, blocks, and lifecycle state — not stored as a field.

Do not model availability as a field on the Resource.

Do not model location as a field on the Resource when location determines operational rules or when the Resource moves between locations.

**Examples:**

- Car rental fleet: each vehicle has a unique identifier, seat count, and lifecycle (available, rented, in service, retired). Availability is derived from active rentals and service blocks — not stored on the vehicle.
- Laboratory: each analytical instrument has a calibration lifecycle and capacity limits per run. The instrument does not store "available" — availability is computed from the booking calendar and maintenance blocks.

---

### Location / Position

Represents where something physically or logically exists. The location itself determines which operations are valid, which rules apply, and which parties are responsible.

Check:

- does moving something between locations trigger business rules?
- does the location determine who is responsible or which pricing applies?
- is there a hierarchy of locations (building → floor → zone → shelf → bin)?
- do compliance or safety rules depend on what is co-located?
- can the same physical thing exist in different logical locations simultaneously?

Do not model location as an attribute of the entity when location has its own rules, hierarchy, or lifecycle.

**Examples:**

- High-bay warehouse: each bin position has an address. The automated system can only reach bins on specific aisles. Moving a pallet to a different bin changes which crane handles it — a rule derived from the location, not the pallet.
- Hospital pharmacy: each shelf slot has temperature requirements and access restrictions. Dispensing rules depend on which location the medication is retrieved from.

---

### Inventory / Stock

Represents the quantity of something at a specific location at a specific point in time. Quantity changes only through recorded movements — never modified directly.

Check:

- who owns the definitive answer to "how much is here right now"?
- must every change be traceable to a movement event?
- can quantity go negative, and is that a business error or a permitted state?
- are quantities tracked per batch or lot, or only in aggregate?
- must the quantity at any past point in time be reconstructable?

Do not model inventory as a mutable quantity field. A mutable field cannot be audited, reconstructed after an error, or corrected without data loss.

**Examples:**

- Blood bank: each unit of blood is tracked by type and location. Quantity changes only through formal receipt and dispensing events. Reconstructing inventory at any past moment is required for traceability regulations.
- Restaurant chain: ingredients are tracked per kitchen location. A stock transfer between kitchens is a formal movement event that decrements one location and increments another.

---

### Movement / Transfer

Represents a formal, recorded transition of something from one location or owner to another. Both the source, the destination, and the act of transfer are first-class domain facts.

Check:

- is the movement itself auditable and traceable?
- can a movement be reversed or cancelled after it is recorded?
- does a movement trigger downstream events (inventory update, billing, notification)?
- is partial movement possible?
- are there approval or authorisation requirements?

Do not model movement as a status change on the thing being moved. The movement is an independent event; the thing's location is a consequence.

**Examples:**

- Medical device supply: when surgical instruments move from central sterilisation to an operating theatre, a Transfer record is created with instrument, source, destination, staff member, and timestamp. This is the audit trail required for infection traceability.
- Grain trading: when a grain lot moves from a storage silo to a ship's hold, a Movement record is created against both parties' inventory accounts and triggers invoice generation.

---

### Route / Carrier

Represents how something gets from an origin to a destination: which carrier, which path, what schedule, and what constraints apply.

Check:

- are there multiple possible routes with different cost, time, or compliance profiles?
- does the carrier have its own capacity, schedule, and reliability characteristics?
- is tracking of in-progress movement required?
- do regulatory or customs requirements vary by route?
- can the route change after movement starts?

**Examples:**

- Pharmaceutical distribution: each shipment has a defined route with cold chain requirements per leg. If a leg is delayed past the threshold, the route is flagged and the carrier's SLA breach is logged.
- E-commerce fulfilment: carrier and service level are locked at despatch. If the carrier fails to collect, an alternative route is assigned and the original carrier's failure is recorded.
