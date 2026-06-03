<!-- Archetype: WORKFLOW -->

# Reviewer Flow

---

## Step 0 — Entry gate

Determine the review mode before any other step. The mode controls which angles run and whether spec artifacts are required.

### Mode detection

**Spec-driven mode** — triggered when the input is a feature name that resolves to a spec folder at `docs/spec/<feature>/`.

**Generic mode** — triggered when the input is a file path, directory path (e.g., `src/Modules/Availability/`), or `.` (whole project). No spec folder is expected or required.

If the input is ambiguous (could be either), check whether `docs/spec/<input>/` exists. If it does, use spec-driven mode. If it does not, use generic mode.

---

### Spec-driven mode — entry checks

**Hard blocks (stop and report, do not proceed):**

| Condition | Message |
|---|---|
| No file list provided | "A list of files to review is required. Provide the output of the implementer agent (list of modified files) or specify paths manually." |
| `docs/spec/<feature>/` does not exist | "No spec folder found at docs/spec/<feature>/. Use a path-based input for a generic review." |
| `decision.md` absent | "decision.md is missing. Architecture has not been decided — review is premature." |
| `decision.md` status is not `approved` | "decision.md is not approved (current: `<status>`). Approve the architecture decision before reviewing implementation." |
| `spec.md` absent | "spec.md is missing. Requirements are not defined — acceptance criteria and spec compliance angles cannot run." |
| `spec.md` status is `draft` | "spec.md is still a draft. Finalize requirements before running a spec-driven review." |
| `implementation-plan.md` absent | "implementation-plan.md is missing. Test coverage and data integrity angles cannot run without a task breakdown." |
| `implementation-plan.md` status is `draft` | "implementation-plan.md is not approved. Approve the implementation plan before reviewing." |

**Warnings (inform but proceed):**

| Condition | Note to include in summary |
|---|---|
| `spec.md` status is `reviewed` but not `approved` | "spec.md is reviewed but not formally approved. AC tracing and spec compliance findings should be treated as provisional." |
| `context-map.md` or `c4.md` absent | "Architecture diagram artifacts not present — architecture review will rely on spec.md and decision.md only." |

---

### Generic mode — entry checks

**Hard blocks:**

| Condition | Message |
|---|---|
| No file list and no path provided | "Provide a file list or a directory path to review (e.g., `src/Modules/` or `.` for the whole project)." |

**No spec artifact checks apply in generic mode.** Inform the user at the start of the review:

```
Running in generic mode — no spec artifacts loaded.
The following angles are skipped (require spec): acceptance-criteria, spec-compliance.
The following angles run in generic form (no spec comparison): architecture, test-coverage, data-integrity.
Full coverage is only available in spec-driven mode.
```

---

### Angle selection (both modes)

If `--angles <list>` is provided in the arguments, run only the specified angles. Otherwise run all angles available for the current mode.

**Minimum output:** confirmed mode (spec-driven / generic), confirmed angle list, list of blocked angles with reason (if any).

---

## Step 1 — Parse input

Extract from the provided arguments:
- Feature name or path (determines mode per Step 0)
- File list (required in both modes; may be derived from a path glob in generic mode)
- Optional `--angles` override

**Minimum output:** mode, target (feature name or path), resolved file list, active angles.

---

## Step 2 — Load spec artifacts

**Spec-driven mode only.** Read all present artifacts from `docs/spec/<feature>/`:
- `decision.md` — approved architecture; authoritative
- `spec.md` — requirements, acceptance criteria, module structure, integration contracts
- `implementation-plan.md` — task breakdown, slice boundaries, temporary items, test tasks

Follow `instructions/workflows/artifact-lifecycle.instructions.md` for the authority hierarchy.

**Generic mode:** skip this step. Proceed to Step 3 with no spec data.

**Minimum output:** list of artifacts loaded (spec-driven), or "generic mode — no spec loaded."

---

## Step 3 — Prepare context brief

Produce a structured context brief passed to each subagent in its prompt.

**Spec-driven mode** — include:
- Feature name
- Full requirements list (IDs + text) from `spec.md`
- Full acceptance criteria list (IDs + text) from `spec.md`
- Module list and ownership summary from `spec.md`
- Integration contract method signatures from `spec.md`
- Slice boundaries and explicitly marked temporary items from `implementation-plan.md`
- Test task list (IDs + scope descriptions) from `implementation-plan.md`
- The resolved file list

**Generic mode** — include:
- Review target (path or description)
- The resolved file list
- Note: "No spec data available. Run generic checks only."

The context brief is passed inline to each subagent prompt. It is not written to disk.

**Minimum output:** context brief (inline).

---

## Step 4 — Spawn parallel review subagents

Launch all active angles simultaneously. Pass the context brief to each.

**Full angle set (spec-driven mode):**

| Subagent | Artifact | Requires spec |
|---|---|---|
| `reviewer-security` | `security-review.md` | No |
| `reviewer-architecture` | `architecture-review.md` | Partial (uses decision.md if available) |
| `reviewer-code-quality` | `code-quality-review.md` | No |
| `reviewer-performance` | `performance-review.md` | No |
| `reviewer-test-coverage` | `test-coverage-review.md` | Partial (uses implementation-plan.md if available) |
| `reviewer-acceptance-criteria` | `acceptance-criteria-review.md` | **Yes — skip in generic mode** |
| `reviewer-spec-compliance` | `spec-compliance-review.md` | **Yes — skip in generic mode** |
| `reviewer-data-integrity` | `data-integrity-review.md` | Partial (uses implementation-plan.md if available) |

**Generic mode:** spawn all except `reviewer-acceptance-criteria` and `reviewer-spec-compliance`.

**Angle override (`--angles`):** spawn only the specified subagents, regardless of mode. If a spec-dependent angle is requested in generic mode, block it and inform the user.

In spec-driven mode, save artifacts to `docs/spec/<feature>/`.
In generic mode, save artifacts to `<review-target>/.review/` (create if absent) or prompt the user for an output path.

**Minimum output:** all active review artifacts written with `status: completed`.

---

## Step 5 — Collect and summarize findings

After all subagents complete, compile a consolidated finding summary:
- List all CRITICAL findings grouped by angle and file
- List all WARNING findings grouped by angle and file
- Note skipped angles and why (generic mode, angle override, or missing spec)
- Note angles that produced no findings

Present this summary before the gate.

**Minimum output:** consolidated findings summary (inline).

---

## Step 6 — Trigger human confirmation gate

Follow `instructions/workflows/reviewer-gate.instructions.md`.

Present critical findings first. Then present warnings. Ask for human confirmation.

---

## Step 7 — Update report statuses

After human confirmation:
- For each approved report: set `status: approved`, `approved-by: <name>`, `updated: <date>`
- For warnings deferred: append `## Deferred findings` section before setting `status: approved`

Update each file individually.

**Minimum output:** all approved reports updated with correct frontmatter.
