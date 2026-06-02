---
artifact: context-map
status: draft
source-of-truth: false
requires-approval: true
approved-by: ~
related-spec: docs/spec/room-maintenance/spec.md
related-decision: docs/spec/room-maintenance/decision.md
---

# Hotel Booking — Module Map & Integration Flows

First context map for this system. Shows all modules and their relationships after the Room Maintenance feature is delivered (Slice 2 target state).

---

## Legend

| Abbreviation | Meaning |
|---|---|
| U | Upstream — defines and owns the contract |
| D | Downstream — depends on and conforms to the upstream contract |
| C-S | Customer-Supplier — upstream controls the contract, downstream adapts |
| OHS | Open Host Service — stable protocol designed for multiple consumers |
| Conformist | Downstream uses the upstream model as-is, no translation |

---

## Module Map

```mermaid
graph TD
    RM[RoomManagement]
    AV["Availability [NEW]"]
    RES["Reservations [EXTENDED]"]

    RM -.->|"[D] C-S [U] — Conformist"| AV
    RES -->|"[D] C-S / OHS [U] — Conformist"| AV
```

Solid arrow — synchronous request/response.
Dashed arrow — push-based state synchronization (write-time only, no query-time call).

---

## Module Responsibilities

| Module | Responsibility |
|---|---|
| RoomManagement | Owns room descriptive data (name, capacity) and operational status (Active, Inactive) |
| Availability [NEW] | Single authority for room availability; owns maintenance blocks, reservation claims, and local room status projection |
| Reservations [EXTENDED] | Owns reservation intent and lifecycle (Pending, Confirmed, Cancelled) |

---

## Integration Notes

| Relationship | Strategic Pattern | Downstream | Tactical Pattern | Interface |
|---|---|---|---|---|
| RoomManagement → Availability | Customer-Supplier (RoomManagement: U, Availability: D) | Conformist | Push-Based State Sync | `IAvailabilityService` (RegisterRoom, SetRoomInactive) |
| Reservations → Availability | Customer-Supplier / OHS (Availability: U, Reservations: D) | Conformist | Synchronous request/response | `IAvailabilityService` (IsAvailableForPeriod, RegisterClaim, ReleaseClaim, ReplaceClaimPeriod) |

**OHS selection rationale (Reservations → Availability):** Availability has a high autonomy requirement (single authority, extraction-ready), and the hotel domain has confirmed future consumers beyond Reservations (owner-use blocks, short-term rental). The interface is designed with primitive types and idempotent operations to survive service extraction without interface changes.

**Removed relationship:** Reservations → RoomManagement dependency for availability and existence checks is eliminated. Availability's local room projection answers these queries.

---

## Integration Flows

### Room Management

#### Room Created (push to Availability)

```mermaid
sequenceDiagram
    actor Staff as Hotel Staff
    participant RM as RoomManagement
    participant AV as Availability [NEW]

    Staff->>RM: Create room
    RM->>AV: Register room (push room state)
    AV-->>RM: Registered
    RM-->>Staff: Room created
```

Note: RoomManagement pushes at write time. Availability stores the room in its local projection. No query-time dependency from Availability to RoomManagement.

#### Room Deactivated (push to Availability)

```mermaid
sequenceDiagram
    actor Staff as Hotel Staff
    participant RM as RoomManagement
    participant AV as Availability [NEW]

    Staff->>RM: Deactivate room
    RM->>AV: Push room inactive (push room state)
    AV-->>RM: Updated
    RM-->>Staff: Room deactivated
```

Note: From this point, Availability.IsAvailableForPeriod returns "room inactive" for any period query on this room.

---

### Maintenance Management

#### Create Maintenance Period

```mermaid
sequenceDiagram
    actor Staff as Hotel Staff
    participant AV as Availability [NEW]

    Staff->>AV: Plan maintenance (room, period)
    alt room not found or inactive
        AV-->>Staff: Room unavailable
    else conflicting reservation claims or maintenance block
        AV-->>Staff: Conflict — active reservations must be resolved first
    else no conflicts
        AV-->>Staff: Maintenance planned
    end
```

Note: In the Slice 2 target state, Availability checks its own claim store for conflicting reservation claims. No call to Reservations is needed. During Slice 1, the application layer temporarily queries Reservations directly for this check — eliminated in Slice 2.

#### Modify Maintenance Period

```mermaid
sequenceDiagram
    actor Staff as Hotel Staff
    participant AV as Availability [NEW]

    Staff->>AV: Modify maintenance period (new period)
    alt maintenance not in Planned state
        AV-->>Staff: Not allowed — maintenance not Planned
    else new period conflicts with reservations or other maintenance
        AV-->>Staff: Conflict — resolve before modifying
    else no conflicts
        AV-->>Staff: Period updated
    end
```

#### Complete Maintenance (Early End Date)

```mermaid
sequenceDiagram
    actor Staff as Hotel Staff
    participant AV as Availability [NEW]

    Staff->>AV: Complete maintenance (actual end date)
    alt actual end date after planned end date
        AV-->>Staff: Invalid — actual end date must be on or before planned end date
    else valid
        AV-->>Staff: Maintenance completed
    end
```

Note: Room becomes available from actual end date + 1 day onward. If actual end date equals planned end date, the full maintenance window is consumed. If earlier, the remaining planned window is unblocked.

#### Cancel Maintenance

```mermaid
sequenceDiagram
    actor Staff as Hotel Staff
    participant AV as Availability [NEW]

    Staff->>AV: Cancel maintenance
    alt maintenance not in Planned state
        AV-->>Staff: Not allowed — maintenance not Planned
    else
        AV-->>Staff: Maintenance cancelled
    end
```

---

### Reservations

#### Create Reservation

```mermaid
sequenceDiagram
    actor Staff as Hotel Staff
    participant RES as Reservations [EXTENDED]
    participant AV as Availability [NEW]

    Staff->>RES: Create reservation (room, period)
    RES->>AV: Register reservation claim (room, period)
    alt room not found, inactive, maintenance block, or conflicting claim
        AV-->>RES: Unavailable
        RES-->>Staff: Rejected
    else claim registered
        AV-->>RES: Claim registered
        RES-->>Staff: Reservation created
    end
```

Note: Claim registration and reservation persistence occur within the same database transaction. If persistence fails after claim registration, the transaction rolls back and the claim is not persisted.

#### Change Reservation Period

```mermaid
sequenceDiagram
    actor Staff as Hotel Staff
    participant RES as Reservations [EXTENDED]
    participant AV as Availability [NEW]

    Staff->>RES: Change reservation period (new period)
    RES->>AV: Replace claim period (new period)
    alt new period unavailable
        AV-->>RES: Unavailable
        RES-->>Staff: Rejected — original period retained
    else period replaced
        AV-->>RES: Claim period replaced
        RES-->>Staff: Period updated (status reverts to Pending if was Confirmed)
    end
```

Note: ReplaceClaimPeriod is atomic — Availability releases the old period claim and registers the new period claim in a single operation. If the new period is unavailable, the old claim is preserved.

#### Cancel Reservation

```mermaid
sequenceDiagram
    actor Staff as Hotel Staff
    participant RES as Reservations [EXTENDED]
    participant AV as Availability [NEW]

    Staff->>RES: Cancel reservation
    RES->>AV: Release claim
    AV-->>RES: Released
    RES-->>Staff: Reservation cancelled
```

Note: Claim release and reservation status update occur within the same database transaction.
