<!-- Archetype: KNOWLEDGE -->

# Booking System: Known Decisions

This file documents intentional shortcuts, accepted trade-offs, and API stability decisions for the booking system.

These are deliberate choices with known limitations — not accidents or oversights.

---

## Documented MVP Decisions

### Availability check embedded in repository

Overlap detection lives inside `IReservationRepository.TryAdd` and `TryChangePeriod` rather than in a separate Availability module or service.

**Reason:** Time pressure. We wanted a working implementation fast. A dedicated Availability module was considered but deferred.

**Known limitation:** Availability rules cannot be changed or tested independently of the repository.

### Room active/inactive status used directly

Reservations read room status via `IRoomReader.IsActive` rather than through a dedicated Availability or scheduling layer.

**Reason:** Time pressure. The simplest model that worked for MVP. A richer availability model (time-bounded blocks, maintenance windows) was considered but deferred.

**Known limitation:** There is no way to temporarily block a room without fully deactivating it.

---

## API Stability

There are currently no external consumers of this API.

Breaking changes to endpoints, request shapes, and response shapes are acceptable without versioning or backward compatibility obligations.
