## Spec artifact checklist (complete if this PR touches `docs/spec/` or `instructions/`)

- [ ] All new spec artifacts have status frontmatter (`artifact`, `status`, `source-of-truth`, `requires-approval`)
- [ ] `decision.md` contains no method signatures, endpoint paths, or task lists
- [ ] `spec.md` reflects post-decision system impact only (no pre-decision model speculation)
- [ ] `solution-options.md` is marked `source-of-truth: false`
- [ ] If a new file was added to `instructions/`, it is listed in `instructions/agents/spec-writer.manifest.md`
- [ ] If an adapter file was added, renamed, or removed, the manifest's "Files that are adapters only" list was updated
- [ ] Adapter files (`.claude/`, `.github/`, `AGENTS.md`) were not updated with instruction lists
