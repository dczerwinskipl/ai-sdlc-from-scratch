# Git Conventions

## Branching Strategy

Default branch is `main`.

```
main                          # stable, published content
feature/post-NN/<description> # new post content or AI improvements
fix/<description>             # corrections (typos, broken links, wrong code)
chore/<description>           # structural changes (renames, tooling, config)
```

A fix or chore branch may touch multiple posts — use the commit scope to clarify.

## Commit Message Format

Follows [Conventional Commits](https://www.conventionalcommits.org/):

```
<type>(<scope>): <description>

feat(post-01): add tool-calling example to agent
feat(post-01): improve system prompt for better reasoning
fix(post-01): correct typo in README
fix(post-01,post-03): update API endpoint URL
fix(shared): update blog URL across all posts
chore: rename folder structure
```

| Type | When to use |
|------|-------------|
| `feat` | new post content, new AI capability, meaningful prompt/agent improvements |
| `fix` | something was wrong — typo, broken link, broken AI behavior, incorrect code |
| `chore` | structural change that wasn't wrong — rename, reorganize, update tooling |

Scope is the post folder name (e.g. `post-01`), a comma-separated list for multi-post changes, or `shared` for root-level changes.
