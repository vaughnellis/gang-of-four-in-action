---
description: "Implementation plan for the foundations task: repository skeleton, build settings, umbrella solution, docs stubs and CI"
applyTo: ".copilot-tracking/changes/2026-10-15/foundations-changes.md"
---
<!-- markdownlint-disable-file -->

# Plan: foundations

Task slug: `foundations`
Date: 2026-10-15
Difficulty: Medium (many small files, no code, one real unknown that was probed and resolved)
Execution model: direct implementation in the agent context, one phase at a time, no subagents

Related files:

* Details: `.copilot-tracking/details/2026-10-15/foundations-details.md`
* Planning log: `.copilot-tracking/plans/logs/2026-10-15/foundations-log.md`
* Critique: `.copilot-tracking/plans/critiques/2026-10-15/foundations-critique.md`

## User Requests

| ID | Request | Source |
| --- | --- | --- |
| UR-1 | Take the task through planning, implementation and review | Chat request |
| UR-2 | Treat project brief Decisions D-01 to D-12 as settled, no re-research | Chat request |
| UR-3 | Respect project brief Global boundaries and this brief's Boundaries | Chat request |
| UR-4 | Use P01 to P03 as the starting point for the plan | Chat request |
| UR-5 | Stop after planning so the plan and critique can be read before files are created | Chat request |
| UR-6 | Stop at the brief's Stop point so the human can commit | Chat request |
| UR-7 | FR-001 to FR-008 and AC-001 to AC-005 from `prompts/01-foundations.md` | Task brief |

## Objectives

* Produce the empty skeleton every later task builds on, with no C# projects.
* Make `dotnet build GangOfFour.slnx` and the CI workflow succeed with zero projects.
* Leave later tasks nothing to guess: package versions, folder names, docs stubs and README index are all in place.

## Context summary

Instructions and rules applied:

* `.github/copilot-instructions.md`: no em dashes in docs, determinism rules, no push or tag, hands off `prompts/`.
* `.github/instructions/patterns.instructions.md`: applies to `patterns/**`; only `.gitkeep` files are created there, which it does not restrict.
* `.github/instructions/baseline.instructions.md`: applies to `baseline/**`; only a `.gitkeep` is created there.
* hve-core `markdown.instructions.md` and `writing-style.instructions.md`: docs and root README need `title` and `description` frontmatter and no H1 when `title` is present; asterisk list markers; no bolded-prefix list items; no em dashes.
* Project brief sections 4 (structure), 6 (pattern map), 10 (Global boundaries).

Environment facts found during research:

* Local SDKs: 8.0.302, 9.0.316, 10.0.302. The workspace is not yet a git repository.
* An empty `.slnx` with empty `<Folder>` elements, plus `Directory.Build.props` and `Directory.Packages.props`, was probed in a temp directory: `restore`, `build` and `test` all exit 0. They print one NuGet warning ("Unable to find a project to restore") that does not fail the build.
* Latest stable packages (NuGet flat container, 2026-10-15): `xunit.v3` 4.0.2, `xunit` 2.9.3, `xunit.runner.visualstudio` 4.0.1, `Microsoft.NET.Test.Sdk` 18.11.0, `Verify.XunitV3` 33.3.3 (requires `xunit.v3.extensibility.core` 4.0.2), `Verify.Xunit` 31.12.5 (requires xUnit 2.9.3), `Microsoft.Extensions.TimeProvider.Testing` 10.11.0.

## Decisions made in this plan

| ID | Decision | Reason |
| --- | --- | --- |
| PD-1 | Use xUnit v3 (`xunit.v3` 4.0.2 with `Verify.XunitV3` 33.3.3) | Current line; the two versions are mutually compatible. v2 is the maintenance line. Confirmed by the user |
| PD-2 | `global.json`: `version` 10.0.302, `rollForward` `latestFeature`, `allowPrerelease` false | Pins to the installed SDK, CI installs the same one through `global-json-file` |
| PD-3 | `Directory.Build.props` does not set `TargetFramework` | FR-002 lists only D-07 settings; the baseline task sets `net10.0` per project |
| PD-4 | `.editorconfig` covers formatting and naming only, with no analyzer severities | Avoids new warnings that warnings-as-errors would turn into build failures |
| PD-5 | `.gitignore` is generated with `dotnet new gitignore`, then extended | Standard .NET file as FR-001 asks; additions below |
| PD-6 | CI uses ubuntu-latest, `actions/checkout` and `actions/setup-dotnet` at major tags, `permissions: contents: read` | Smallest workflow that meets FR-007; SHA pinning noted as an option in the critique |
| PD-7 | Docs and root README carry `title` and `description` frontmatter | Required by the hve-core markdown instructions. SUPERSEDED: the user chose a plain H1 and no frontmatter for the root README, and the repo rule in `.github/copilot-instructions.md` says the same for all markdown outside `prompts/` and `.copilot-tracking/`, so the docs use a plain H1 too (log DL-7) |

Proposed additions beyond the brief (need approval, default shown):

| ID | Addition | Default |
| --- | --- | --- |
| PA-1 | `.gitattributes` forcing LF, with `scenarios/expected/*.txt` explicitly `eol=lf` | Include. Golden master compares bytes across Windows and Linux CI |
| PA-2 | `.gitignore` entries `*.received.*` (Verify) and nothing that ignores `.copilot-tracking/` | Include |

## Implementation checklist

### Phase P01: Root files and build settings (FR-001 to FR-004) <!-- parallelizable: false -->

* [x] P01-S1 Create `global.json` (PD-2)
* [x] P01-S2 Create `Directory.Build.props` (nullable, warnings as errors, implicit usings, `LangVersion` latest)
* [x] P01-S3 Create `Directory.Packages.props` with Central Package Management and the five package versions (PD-1)
* [x] P01-S4 Create `GangOfFour.slnx` with the five solution folders
* [x] P01-S5 Create `.gitignore` (PD-5, PA-2), `.editorconfig` (PD-4), `LICENSE` (MIT), and `.gitattributes` (PA-1 approved)
* [x] P01-S6 Run `dotnet --version`, `dotnet restore`, `dotnet build` and `dotnet test` against `GangOfFour.slnx`; all must exit 0 (AC-001)

### Phase P02: Folder layout and docs stubs (FR-005, FR-006) <!-- parallelizable: true -->

* [x] P02-S1 Create `.gitkeep` in `baseline/`, `patterns/creational/`, `patterns/structural/`, `patterns/behavioral/`, `capstone/`, `scenarios/expected/`, `tools/`
* [x] P02-S2 Create `docs/smells-catalog.md` stub
* [x] P02-S3 Create `docs/pattern-map.md` with the three category tables and a Status column set to Not started
* [x] P02-S4 Create `docs/comparisons.md` with the four headings only
* [x] P02-S5 Check AC-004: 23 rows, smell IDs S01 to S23, folder names match the project brief

### Phase P03: CI workflow and root README (FR-007, FR-008) <!-- parallelizable: false -->

* [x] P03-S1 Create `.github/workflows/ci.yml` (PD-6)
* [x] P03-S2 Validate the YAML parses and the steps match FR-007 (AC-002)
* [x] P03-S3 Create root `README.md` with purpose, organisation, how to run a solution, the 23-row pattern index with folder links, and "Built with HVE Core"
* [x] P03-S4 Re-run restore, build and test with the same flags CI uses

### Phase P04: Verify and record <!-- parallelizable: false -->

* [x] P04-S1 Check AC-001 to AC-005 and the boundaries: no `.csproj` or `.cs` files, nothing under `prompts/` changed, no git tag or push
* [x] P04-S2 Write the changes log at `.copilot-tracking/changes/2026-10-15/foundations-changes.md`
* [x] P04-S3 Write the review log at `.copilot-tracking/reviews/2026-10-15/foundations-plan-review.md`, then stop at the brief's Stop point

P01 to P03 follow the brief's suggested phases. P04 is added so verification and records are explicit rather than implied.

## Dependencies

* .NET SDK 10.0.302 installed locally (present)
* Network access to NuGet is not needed for an empty solution
* No skills required

## Success criteria

* AC-001 to AC-005 hold.
* Every FR has a file or folder that satisfies it, traced in the details file.
* No file is created under `prompts/`, `.github/instructions/` or `.github/copilot-instructions.md`, and `scenarios/expected/` holds only `.gitkeep`.
* No em dashes in any file written.

## Stop points

1. Now: after planning, for the human to read the plan and critique.
2. After review: hand back for the human to commit, push, and confirm CI is green on GitHub. The agent does not push or tag.
