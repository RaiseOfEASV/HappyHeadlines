# .github

This directory contains GitHub-specific configuration for the HappyHeadlines repository.

## Rulesets

Branch protection rules are defined as JSON files under [`rulesets/`](./rulesets/) and applied via the GitHub CLI. They are stored here as the source of truth and can be reapplied at any time.

### `rulesets/protect-master.json`

Protects the `master` branch with the following rules:

| Rule | Description |
|------|-------------|
| `deletion` | Prevents `master` from being deleted |
| `non_fast_forward` | Blocks force pushes |
| `pull_request` | Requires a PR with 1 approving review before merging; dismisses stale reviews on new pushes |

Direct pushes to `master` are blocked for everyone, including admins (`bypass_actors` is empty).

### Applying the ruleset

Requires the [GitHub CLI](https://cli.github.com/) (`gh`) and **Admin** (personal repo) or **Owner** (org repo) permissions.

**Org repo:**
```bash
gh api \
  --method POST \
  -H "Accept: application/vnd.github+json" \
  -H "X-GitHub-Api-Version: 2022-11-28" \
  /repos/RaiseOfEASV/HappyHeadlines/rulesets \
  --input .github/rulesets/protect-master.json
```
### Verifying the ruleset is active

```bash
gh api \
  -H "Accept: application/vnd.github+json" \
  -H "X-GitHub-Api-Version: 2022-11-28" \
  /repos/RaiseOfEASV/HappyHeadlines/rulesets
```

