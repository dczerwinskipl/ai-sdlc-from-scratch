## Spec artifact checklist (complete if this PR touches `docs/spec/` or `instructions/`)

- [ ] All new spec artifacts have status frontmatter (`artifact`, `status`, `source-of-truth`, `requires-approval`)
- [ ] `decision.md` contains no method signatures, endpoint paths, repository method names, or task lists (module names and capability descriptions are allowed)
- [ ] `spec.md` reflects post-decision system impact only (no pre-decision model speculation)
- [ ] `solution-options.md` is marked `source-of-truth: false`
- [ ] If a new file was added to the cross-domain workflow in `instructions/`, it is listed in `instructions/agents/spec-writer-cross-domain.manifest.md`
- [ ] If an adapter file was added, renamed, or removed, the manifest's "Files that are adapters only" list was updated
- [ ] Adapter files (`.claude/`, `.github/`, `AGENTS.md`) were not updated with instruction lists
