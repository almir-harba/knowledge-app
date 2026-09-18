---
name: fix-PR-comments
description: Resolves code review findings/comments on a pull request. Use when addressing reviewer feedback, fixing issues flagged in a PR review, or when the user asks to work through PR comments.
---

# Fix PR Comments

When resolving PR findings:

1. Run gh pr view --comments (or fetch review comments via the platform's API/UI) to list all open findings on the PR
2. For each finding:
- Read the comment and the surrounding code context (git diff, file at that line)
- Determine if it's a required fix, a suggestion, or a question needing a reply
- Make the code change if applicable
- Re-run relevant tests/linters to confirm the fix doesn't break anything
3. Group related findings together if multiple comments point to the same root cause
4. Write a reply for each resolved comment summarizing what changed, e.g.:

## Resolved
- [Finding]: [what was changed and why]

5. For findings that are disagreed with or deferred, leave a clear reply explaining the reasoning instead of silently skipping it
6. Push the changes and mark conversations as resolved where the platform supports it

**Tech:** claude-skill
