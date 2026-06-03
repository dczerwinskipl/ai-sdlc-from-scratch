---
artifact: checklist
status: draft
source-of-truth: false
requires-approval: false
---

# Room Photo — Implementation Checklist

## Scope

Add a `photo` field (base64-encoded JPG string) to `Room`. Validate on create: JPG format only, max 1024×768 resolution, max 2MB. Return on `GetRoom`. No other behavior changes.

**Owner:** `RoomManagement`
**Source-of-truth:** `Room` entity

---

## Implementation

- [ ] `Domain/Room.cs` — Add `Photo` property (`string?`). Update `Create()` to accept an optional `photo` parameter and assign it.
- [ ] `UseCases/AddRoom/AddRoomRequest.cs` — Add `Photo` property (`string?`).
- [ ] `UseCases/AddRoom/AddRoomCommand.cs` — Add `Photo` property (`string?`).
- [ ] `UseCases/AddRoom/AddRoomValidator.cs` — Add three guards:
  - Format: decoded bytes start with `FF D8 FF` (JPEG magic bytes)
  - Size: decoded byte length ≤ 2,097,152 (2MB)
  - Resolution: decoded image width ≤ 1024 and height ≤ 768
  - **Note:** resolution check requires image decoding — verify `SixLabors.ImageSharp` or equivalent is available in the project.
- [ ] `UseCases/AddRoom/AddRoomHandler.cs` — Pass `command.Photo` to `Room.Create()`.
- [ ] `UseCases/GetRoom/GetRoomResponse.cs` — Add `Photo` property (`string?`).
- [ ] `UseCases/GetRoom/GetRoomHandler.cs` — Map `room.Photo` to `GetRoomResponse`.

**Assumption to confirm:** Is `Photo` optional (nullable) at creation, or required?

---

## Tests

- [ ] Unit — Validator rejects non-JPG base64 (provide valid PNG as base64)
- [ ] Unit — Validator rejects photo exceeding 2MB (decoded size > 2,097,152 bytes)
- [ ] Unit — Validator rejects photo exceeding 1024×768 (e.g. 1025×100)
- [ ] Unit — Validator accepts valid JPG within all constraints
- [ ] Unit — Validator accepts null photo *(confirm only if photo is optional)*
- [ ] Integration — `AddRoom` stores photo; `GetRoom` returns it unchanged
- [ ] Integration — `AddRoom` returns validation error on invalid photo
