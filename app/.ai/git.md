# Git Conventions

Same branching strategy as the root repo — see `../../.ai/git.md` for the full reference.

## Branches

```
main                    # stable
feature/app/<desc>      # new features or improvements
fix/app/<desc>          # bug fixes
chore/app/<desc>        # structural or tooling changes
```

## Commit Scope

Use `app` as the scope for all commits originating from this folder:

```
feat(app): add room availability check to create reservation flow
fix(app): correct period overlap logic in ReservationPeriod
chore(app): add InternalsVisibleTo for test project
```
