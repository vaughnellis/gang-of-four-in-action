# Task: release-v1

**Task slug:** `release-v1`
**Shared context:** `prompts/00-project-brief.md` (read it first)
**Depends on:** `readmes` committed, and CI green on `main`.

## Goal

Prepare the repository for its first public release. No code or README rewrites: the `readmes` task already wrote the documentation. This task hardens CI, adds a changelog, and does a final accuracy pass.

## Requirements

- **FR-001** Read the root `README.md` and confirm it is accurate and complete for v1.0.0. Fix factual errors only; do not restyle it.
- **FR-002** Pin every action in `.github/workflows/` to a full commit SHA, with the version as a trailing comment (for example `actions/checkout@<sha> # v4.2.2`).
- **FR-003** Add `.github/dependabot.yml` with weekly updates for `nuget` (respecting Central Package Management in `Directory.Packages.props`) and `github-actions`.
- **FR-004** Add `CHANGELOG.md` with a `1.0.0` entry summarising what the repo contains: the baseline, the 23 pattern solutions, the capstone, the comparisons guide and the CI checks.
- **FR-005** Check every relative link in `README.md`, `docs/*.md`, `baseline/README.md`, `capstone/README.md` and `patterns/**/README.md`, and fix any that do not resolve. Leave the `Blog post: coming soon <!-- blog-link -->` lines as they are.

## Acceptance criteria

- **AC-001** No text in the root `README.md` describes the repo as incomplete or refers to future pattern folders.
- **AC-002** All 23 pattern links and every relative link checked in FR-005 resolve.
- **AC-003** No workflow references an action by tag or branch only.
- **AC-004** `dependabot.yml` is valid and covers both ecosystems.
- **AC-005** `dotnet build GangOfFour.slnx` and `dotnet test GangOfFour.slnx` pass, using the commands in `ci.yml`.
- **AC-006** No changes under `baseline/`, `patterns/`, `capstone/`, `scenarios/`, `prompts/`, `.github/copilot-instructions.md` or `.github/instructions/`, except README link fixes from FR-005 and factual fixes from FR-001.
- **AC-007** Prose is plain and direct, with no em dashes.

## Boundaries

- Do not create tags or GitHub releases. The human tags `v1.0.0` and publishes the release.
- Do not commit or push. Stop so the human can commit.

## Suggested phases

1. **P01** Root README accuracy check (FR-001).
2. **P02** CI hardening: pinned actions and Dependabot (FR-002, FR-003).
3. **P03** Changelog and link check (FR-004, FR-005), then build and test.

## Stop point

After review, stop so the human can commit, push, tag `v1.0.0` and publish the release.
