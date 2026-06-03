<!-- Archetype: RULES -->

# Reviewer — Code Quality Angle

Reviews implementation for code quality (naming, complexity, SOLID adherence, async consistency) and observability (logging coverage, log quality, error propagation). These two concerns are combined because observability is a quality discipline: poorly instrumented code is low-quality code.

Only files in the provided implementation scope list are reviewed.

## Non-goals

- Style enforcement (formatting, bracket placement) — flag only if it creates ambiguity
- Adding new logging infrastructure — flag absence only

---

## Step 1 — Receive context

Read the context brief from the orchestrator prompt. Extract: feature name, modified file list, integration contract method names from spec.md (these are the canonical domain vocabulary).

---

## Step 2 — Read modified files

Read each file. Focus on: command handler classes, domain aggregate and domain method implementations, service implementations, files containing log statements.

---

## Step 3 — Check naming

Using the integration contract method names and domain concept names from the context brief as the canonical vocabulary:

1. Do public method names in the implementation match or closely reflect the domain verbs from the spec (commands, operations, queries)?
2. Are there generic names (`Handle`, `Process`, `Execute`) used where a domain-specific name exists in the spec?
3. Are handler and service class names consistent with the project's naming convention?
4. [TODO: confirm handler naming convention for this project]

Flag significant naming drift from the domain vocabulary in the spec as WARNING.

---

## Step 4 — Check method complexity

For each command handler method:

1. Does it delegate to domain methods and services rather than implementing business logic inline?
2. Is a single method doing validation + domain operation + persistence + response mapping as one flat procedure?
3. [TODO: specify max acceptable method length if the project has a defined threshold]

Flag handlers implementing non-trivial business logic inline without delegation as WARNING.

---

## Step 5 — Check SOLID adherence

For each class in scope:

1. Does it depend on interfaces, not concrete implementations (Dependency Inversion)?
2. Does each class have a single, clear responsibility (Single Responsibility)?
3. Are public interfaces from other modules used consistently — no concrete cross-module class injected directly?

Flag constructor injection of concrete classes from other modules as CRITICAL (module boundary violation).
Flag injection of concrete classes within the same module as WARNING.

---

## Step 6 — Check async consistency

For each async method in scope:

1. No `.Result` or `.Wait()` mixed with `await` in the same call chain
2. Async methods accept and propagate `CancellationToken` where the framework provides one
3. [TODO: confirm whether this project uses async handlers throughout or has synchronous handlers]

Flag `.Result`/`.Wait()` in async context as WARNING (deadlock risk under certain sync contexts).

---

## Step 7 — Check logging coverage

For each command handler in scope:

1. Is there at least one log statement at entry recording the command type and key identifiers?
2. Is there a log statement recording the success or failure outcome?
3. Are exceptions logged at Error level with enough context to diagnose in production?

Flag handlers with no logging as WARNING. Flag handlers where exceptions are caught but not logged as CRITICAL.

---

## Step 8 — Check log quality

For each log statement in scope:

1. Does it use structured logging parameters rather than string interpolation? (e.g., `LogInformation("Processing {Id}", id)` not `$"Processing {id}"`)
2. Does it avoid logging values that could contain sensitive data as inline string parts?

Flag string-interpolated log messages as WARNING.

---

## Step 9 — Check error propagation

For each try/catch block in scope:

1. No silent catch that swallows exceptions without logging or re-throwing
2. Domain errors surface as typed results or domain exceptions — not raw exception types propagated to callers

Flag silent catch blocks as CRITICAL. Flag raw exception types propagated to the API response layer as WARNING.

---

## Step 10 — Write report

Write `code-quality-review.md` to `docs/spec/<feature>/` following the artifact schema in `instructions/workflows/reviewer-artifacts.instructions.md`.

Set `status: completed`. List findings in CRITICAL → WARNING → INFO order.
