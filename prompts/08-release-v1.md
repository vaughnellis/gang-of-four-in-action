# Task: release-v1

**Task slug:** `release-v1`
**Shared context:** `prompts/00-project-brief.md` (read it first)
**Depends on:** `capstone` committed, and CI green on `main`.

## Goal

Prepare the repository for its first public release. No pattern code changes: this task tidies the landing page, records how the repo was built, and hardens CI.

## Requirements

- **FR-001** Update the root `README.md`:
    - Remove or rewrite any note written before the patterns existed, such as the Status section saying pattern links will 404.
    - Make sure the pattern index lists all 23 patterns with working links, grouped by category, and that the capstone is linked.
    - Add a short **How this was built** section: the repo was planned, implemented and reviewed task by task with Microsoft HVE Core's RPI workflow. Link to `prompts/README.md` and `.copilot-tracking/`, and give the counts from `.copilot-tracking/run-all-progress.md` (tasks completed, review outcomes). Facts only, no marketing language.
- **FR-002** Pin every action in `.github/workflows/` to a full commit SHA, with the version as a trailing comment (for example `actions/checkout@<sha> # v4.2.2`).
- **FR-003** Add `.github/dependabot.yml` with weekly updates for `nuget` (respecting Central Package Management in `Directory.Packages.props`) and `github-actions`.
- **FR-004** Add `CHANGELOG.md` with a `1.0.0` entry summarising what the repo contains: the baseline, the 23 pattern solutions, the capstone, the comparisons guide and the CI checks.
- **FR-005** Check every relative link in `README.md`, `docs/*.md`, `capstone/README.md` and `patterns/**/README.md`, and fix any that do not resolve.

## Acceptance criteria

- **AC-001** No text in the root `README.md` describes the repo as incomplete or refers to future pattern folders.
- **AC-002** All 23 pattern links and every relative link checked in FR-005 resolve.
- **AC-003** No workflow references an action by tag or branch only.
- **AC-004** `dependabot.yml` is valid and covers both ecosystems.
- **AC-005** `dotnet build GangOfFour.slnx` and `dotnet test GangOfFour.slnx` pass, using the commands in `ci.yml`.
- **AC-006** No changes under `baseline/`, `patterns/**/src`, `patterns/**/tests`, `scenarios/`, `prompts/`, `.github/copilot-instructions.md` or `.github/instructions/`. Pattern README link fixes from FR-005 are the only edits allowed inside `patterns/`.
- **AC-007** Prose is plain and direct, with no em dashes.

## Boundaries

- Do not create tags or GitHub releases. The human tags `v1.0.0` and publishes the release.
- Do not commit or push. Stop so the human can commit.

## Suggested phases

1. **P01** README rewrite and How this was built (FR-001).
2. **P02** CI hardening: pinned actions and Dependabot (FR-002, FR-003).
3. **P03** Changelog and link check (FR-004, FR-005), then build and test.

## Stop point

After review, stop so the human can commit, push, tag `v1.0.0` and publish the release.
