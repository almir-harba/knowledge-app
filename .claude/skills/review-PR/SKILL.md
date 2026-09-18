---
name: review-PR
description: Reviews pull requests for code quality, correctness, maintainability, security, and adherence to project conventions. Use when reviewing a PR or when the user asks for a PR review.
---

# Review PR

When reviewing a PR:

1.Run git diff main...HEAD to see all changes on the branch.
2. Review changed files and understand the context of each change.
3. Check for:
- Bugs and incorrect behavior
- Missing error handling
- Security issues
- Performance problems
- Code duplication
- Maintainability and readability
- Project conventions and patterns
- Missing or inadequate tests
4. Prioritize findings by severity:
Critical — security issues, data loss, breaking production behavior
Major — bugs or problems that should be fixed before merging
Minor — improvements that are useful but not blocking
5. Report only actionable findings and include the affected file/line when possible.
6. End with a concise summary of the overall review.
7. If reviewing an actual GitHub PR (i.e. a PR number/URL, not just a local diff), post the review as a comment on that PR using `gh pr comment <number> --body-file <file>` (write the review to a temp file first). If `gh` is not on PATH, check `C:\Program Files\GitHub CLI\gh.exe`. If `gh` is not installed or not authenticated, tell the user instead of failing silently.

**Tech:** claude-skill
