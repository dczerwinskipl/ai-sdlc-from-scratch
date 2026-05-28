# AI SDLC From Scratch

Source code for the blog series on building an AI-powered Software Development Lifecycle from scratch, published at [dczerwinski.pl](https://dczerwinski.pl).

## Posts

| # | Folder | Blog Post |
|---|--------|-----------|
| — | —      | —         |

## Repository Structure

```
/NN-post-name/    # One folder per post — fully self-contained
/app/             # Final reference implementation
```

Each post folder is independent — no shared dependencies or cross-references between posts.

## Working with This Repo

Each post and `/app` is designed to be opened as a **separate project** in your editor. This keeps tooling, dependencies, and AI context isolated and avoids conflicts between posts.

```
# Example: work on post 01
code 01-introducing-agents/
```

## Git Flow

Branches follow this pattern:

```
main                          # stable, published content
feature/post-NN/<description> # new post content or AI improvements
fix/<description>             # corrections (typos, broken links, wrong code)
chore/<description>           # structural changes (renames, tooling, config)
```

Commits follow [Conventional Commits](https://www.conventionalcommits.org/):

```
feat(post-01): add tool-calling example to agent
fix(post-01): correct typo in README
chore: rename folder structure
```

Full conventions: [`.ai/git.md`](.ai/git.md)

## License

MIT
