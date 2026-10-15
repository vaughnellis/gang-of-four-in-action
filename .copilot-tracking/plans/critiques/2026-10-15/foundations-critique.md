<!-- markdownlint-disable-file -->
# Plan critique: foundations

Plan: `.copilot-tracking/plans/2026-10-15/foundations-plan.instructions.md`
Critic scope: the plan against `prompts/01-foundations.md`, the project brief's Decisions and Global boundaries, and the repo instructions.

## Verdict

Proceed, with four decisions needed from you. Each has a default that I will apply if you reply "go" without changes. Nothing in the plan breaks a Decision or a boundary.

## Coverage check

| Item | Covered | Notes |
| --- | --- | --- |
| FR-001 to FR-008 | Yes | Traced in the details file |
| AC-001 | Yes, evidence already gathered | Empty `.slnx` build, restore and test exit 0 in a probe |
| AC-002 | Yes | Parse the YAML and assert steps |
| AC-003 | Partly | See C-4 |
| AC-004 | Yes | Row count and ID check in P02-S5 |
| AC-005 | Yes | Boundary checks in P04-S1 |
| D-01, D-06, D-07, D-08 | Yes | Pinned SDK, `.slnx`, central packages, build settings, xUnit plus Verify |
| Global boundaries | Yes | No edits to `prompts/`, instructions, baseline content or expected transcript |

## Findings

### C-1 xUnit v2 or v3 (decision needed)

The brief names "xUnit" and "Verify for xUnit" without a major version. Both lines are current on NuGet and each pairs with its own Verify package.

* v3: `xunit.v3` 4.0.2, `Verify.XunitV3` 33.3.3. Test projects become executables, which the baseline task must account for.
* v2: `xunit` 2.9.3, `Verify.Xunit` 31.12.5. No project changes, but it is the older line.

Default: v3. This choice affects how the baseline task writes its test project, so it is better settled now.

### C-2 LICENSE holder (decision needed)

MIT needs a copyright holder and the brief gives none. The workspace is not a git repository, so there is no author to read. I will not invent a name. Default: wait for your answer before writing `LICENSE`; the rest of P01 proceeds.

### C-3 Line endings (decision needed)

The golden master compares bytes. A Windows checkout with `core.autocrlf` can turn `scenarios/expected/baseline.txt` into CRLF while Linux CI writes LF, which fails the comparison in a way that is hard to diagnose later. The brief's file list has no `.gitattributes`. Adding one is small and additive. Default: include it (PA-1), with `end_of_line = lf` in `.editorconfig` as well.

### C-4 AC-003 wording (low risk)

AC-003 says every section 4 folder exists. Section 4 also lists `baseline/src/CornerStore/` and `baseline/tests/CornerStore.Tests/`, which hold projects and cannot be empty placeholders. FR-005 lists only `baseline/`. The plan follows FR-005 and treats the deeper folders as baseline task output. If you would rather have those folders now, say so and I will add `.gitkeep` files there; the baseline task would then replace them.

### C-5 README frontmatter (low risk)

hve-core markdown rules require `title` and `description` frontmatter, including on the root README. GitHub shows that as a small table above the README content. If you prefer a clean README, I can start it with an H1 and skip frontmatter, which deviates from the hve-core rule. Default: follow hve-core.

### C-6 Warnings as errors and the empty solution (resolved)

Risk: an empty solution could fail under `TreatWarningsAsErrors` because NuGet warns "Unable to find a project to restore". The probe showed exit 0 for restore, build and test. The plan re-verifies with the exact CI flags in P01-S6 and P03-S4. Residual risk: CI runner SDK differs from local; mitigated by `global-json-file`.

### C-7 `.editorconfig` can break the build later (resolved in plan)

Analyzer severities in `.editorconfig` would be promoted to errors by `TreatWarningsAsErrors` once projects exist. PD-4 keeps the file to formatting and naming, with no diagnostic severities. Later tasks should not add severities without checking the baseline compiles.

### C-8 Supply chain in CI (accepted)

Major-tag action references are readable but mutable. SHA pinning is stronger and fits a repo that will also teach good practice, but it needs Dependabot to stay current. The plan accepts major tags and sets `permissions: contents: read`. Say if you want SHA pins.

### C-9 Dead links in the README index (accepted)

The pattern index links to 23 folders that do not exist yet, as FR-008 requires. They resolve as pattern tasks land. The plan does not add placeholders.

### C-10 Plan path convention (informational)

`prompts/README.md` and the RPI Agent name the plan file differently (DL-1). No action in this task; the change record will propose a fix because `prompts/` is off limits.

## Gaps not addressed by the brief, left out on purpose

* `.markdownlint.json` and markdown lint in CI
* Dependabot configuration
* `TargetFramework` in `Directory.Build.props` (PD-3)
* Any `CONTRIBUTING.md` or issue templates

## Decisions needed

| ID | Question | Default |
| --- | --- | --- |
| C-1 | xUnit v2 or v3 | v3 |
| C-2 | Copyright holder name for LICENSE | None; wait |
| C-3 | Include `.gitattributes` | Yes |
| C-5 | README frontmatter or plain H1 | Frontmatter |
