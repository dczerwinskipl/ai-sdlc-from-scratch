<!-- Archetype: RULES -->

# Reviewer — Security Angle

Reviews the implementation for security vulnerabilities and missing security controls. Only files in the provided implementation scope list are reviewed.

## Non-goals

- Authorization policy design — flag absence of guards only, do not redesign who can access what
- Cryptography review — flag known-weak algorithms only
- Penetration testing or threat modelling

---

## Step 1 — Receive context

Read the context brief from the orchestrator prompt. Extract: feature name, spec artifact path, modified file list.

---

## Step 2 — Read modified files

Read each file in the modified file list. Focus on: HTTP endpoint handlers and controllers, command handlers, repository implementations, files containing SQL queries or ORM query construction, files containing log statements.

---

## Step 3 — Check input validation

For each command handler or endpoint in scope:
- All input parameters validated before use (null checks, range guards, format validation)?
- Is `endDate` defaulted to `startDate` when absent (AC1 requirement — business rule, not just validation)?
- Is `roomId` validated as a non-empty GUID before any database call?

Flag missing validation as WARNING. Flag missing validation on a parameter that could silently corrupt domain state as CRITICAL.

---

## Step 4 — Check injection risks

For each repository or data access method in scope:
- All queries parameterized? No string interpolation or concatenation into SQL.
- [TODO: identify ORM/query library (EF Core, Dapper, raw ADO.NET) and confirm parameterization pattern]

Flag raw string interpolation in SQL as CRITICAL.

---

## Step 5 — Check authorization

For each HTTP endpoint in scope:
- Authorization guard present (e.g., `[Authorize]`, policy-based check)?
- All write endpoints (create, modify, cancel, complete) require authorization?
- [TODO: confirm auth mechanism and required policy names for maintenance management endpoints]

Flag missing authorization on any write endpoint as CRITICAL. Flag missing on read endpoints as WARNING.

---

## Step 6 — Check sensitive data in logs

For each log statement in scope:
- Structured logging used — no string interpolation with user-supplied values?
- Room IDs, reservation IDs, or guest-related data logged only as parameters, not inline string parts?
- No sensitive data (credentials, payment data) anywhere in logs?

Flag string-interpolated log messages containing external IDs as WARNING.

---

## Step 7 — Check error exposure

For each exception handler or error mapping in scope:
- API responses never include stack traces or internal exception messages?
- Domain errors mapped to HTTP status codes, not surfaced as raw `500 Internal Server Error` with exception detail?

Flag stack trace exposure in HTTP responses as CRITICAL.

---

## Step 8 — Write report

Write `security-review.md` to `docs/spec/<feature>/` following the artifact schema in `instructions/workflows/reviewer-artifacts.instructions.md`.

Set `status: completed`. List findings in CRITICAL → WARNING → INFO order. Set `scope` to the list of files reviewed.
