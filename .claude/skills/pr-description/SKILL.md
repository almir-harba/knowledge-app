---
name: pr-description
description: Writes pull request descriptions. Use when creating a PR, writing a PR, or when the user asks to summarize changes for a pull request.
---

# PR Description

When writing a PR description:

1. Determine the base branch (usually `main`; use the PR's actual base if it differs, or the branch's upstream tracking branch if the current branch is itself `main`).
2. Run `git diff <base>...HEAD` to see all changes on this branch.
3. Write a description following this format:

## What
One sentence explaining what this PR does.

## Why
Brief context on why this change is needed

## Changes
- Bullet points of specific changes made
- Group related changes together
- Mention any files deleted or renamed

4. If creating or updating an actual GitHub PR, write the description to a temp file and pass it with `gh pr create --body-file <file>` or `gh pr edit <number> --body-file <file>`. If `gh` is not on PATH, check `C:\Program Files\GitHub CLI\gh.exe`. If `gh` is not installed or not authenticated, print the description for the user to copy instead of failing silently.

**Tech:** claude-skill
