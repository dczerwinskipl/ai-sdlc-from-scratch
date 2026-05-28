# Repository Context

Source for a blog series on building an AI-powered Software Development Lifecycle (SDLC) from scratch, published at https://dczerwinski.pl. Each post is self-contained and lives in its own subfolder.

## Structure

```
/NN-post-name/    # Each post is an independent mini-repo
/app/             # Final reference implementation (separate from posts)
```

Post folders follow the pattern `NN-short-description` (e.g. `01-introducing-agents`, `10-agent-orchestration`). Each post is fully independent — no shared dependencies or cross-references between posts.

- Root level contains only meta files. No code lives here.
- Each post has its own AI instructions, tooling, and README.
- `/app` is the final implementation, treated as its own independent project.
