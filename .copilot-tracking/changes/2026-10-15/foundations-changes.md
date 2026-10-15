<!-- markdownlint-disable-file -->
# Changes: foundations

Plan: `.copilot-tracking/plans/2026-10-15/foundations-plan.instructions.md`
Details: `.copilot-tracking/details/2026-10-15/foundations-details.md`
Planning log: `.copilot-tracking/plans/logs/2026-10-15/foundations-log.md`
Review log: `.copilot-tracking/reviews/2026-10-15/foundations-plan-review.md`
Implementation date: 2026-10-15

## Summary

Created the empty repository skeleton: build settings, umbrella solution, folder layout, docs stubs, CI workflow and root README. No C# projects. `dotnet restore`, `build` and `test` on `GangOfFour.slnx` with the CI flags all exit 0.

## Added

Phase P01, root files and build settings:

* `global.json`: SDK 10.0.302, `rollForward` `latestFeature`, no prerelease
* `Directory.Build.props`: `Nullable`, `TreatWarningsAsErrors`, `ImplicitUsings`, `LangVersion` latest; no `TargetFramework`
* `Directory.Packages.props`: Central Package Management with `xunit.v3` 4.0.2, `xunit.runner.visualstudio` 4.0.1, `Microsoft.NET.Test.Sdk` 18.11.0, `Verify.XunitV3` 33.3.3, `Microsoft.Extensions.TimeProvider.Testing` 10.11.0
* `GangOfFour.slnx`: five empty solution folders
* `.gitignore`: output of `dotnet new gitignore`, normalised to LF, plus `*.received.*` and `*.received/`
* `.editorconfig`: formatting only, no analyzer severities
* `.gitattributes`: LF for text files, common binary types marked binary, `*.received.*` not diffed
* `LICENSE`: MIT, 2026, Vaughn Ellis Ramos

Phase P02, folder layout and docs stubs:

* `.gitkeep` in `baseline/`, `patterns/creational/`, `patterns/structural/`, `patterns/behavioral/`, `capstone/`, `scenarios/expected/` and `tools/`
* `docs/smells-catalog.md`: intro and empty table header
* `docs/pattern-map.md`: 5, 7 and 11 rows with the Status column set to Not started
* `docs/comparisons.md`: four headings only

Phase P03, CI workflow and root README:

* `.github/workflows/ci.yml`: push and pull request triggers, `contents: read`, ubuntu-latest, `actions/checkout@v4`, `actions/setup-dotnet@v4` with `global-json-file`, then restore, build (Release, no restore) and test (Release, no build) on `GangOfFour.slnx`
* `README.md`: plain H1, purpose, status note, folder tree, how to run, 23-row pattern index with folder links, Built with HVE Core, license

## Modified

* None by the agent. `.github/copilot-instructions.md` shows one uncommitted added line (plain H1 and no frontmatter rule). That edit is the user's, not the agent's.
* Tracking artifacts only: plan checklist ticked and PD-1 and PD-7 updated, planning log extended (DL-6 resolved, DL-7, user decisions, follow-on).

## Removed

* None.

## Deviations from the plan

| Item | Plan | Actual | Reason |
| --- | --- | --- | --- |
| Docs frontmatter | PD-7: `title` and `description` on docs and README | Plain H1, no frontmatter on README and all three docs | User decision for the README; repo rule in `.github/copilot-instructions.md` covers the rest (log DL-7) |
| Line endings | Not specified | All created files converted from CRLF to LF | `.editorconfig` and `.gitattributes` require LF; the file tool wrote CRLF on Windows |
| README Status section | Not in plan | Added a short Status section saying folder links resolve as tasks land | Mitigates dead links (critique C-9) |

## Validation

| Check | Result |
| --- | --- |
| `dotnet --version` | 10.0.302 |
| `dotnet restore GangOfFour.slnx` | exit 0, one expected warning "Unable to find a project to restore" |
| `dotnet build GangOfFour.slnx --no-restore -c Release` | exit 0 |
| `dotnet test GangOfFour.slnx --no-build -c Release` | exit 0 |
| CI YAML parse | valid; triggers, permissions and the five steps match FR-007; no `continue-on-error` |
| Pattern map against project brief | 23 rows each, tuples identical, 23 "Not started" |
| README index against pattern map | 23 folder links, set equal |
| Em dashes or en dashes in created files | none |
| `*.csproj`, `*.cs`, `*.sln` files | none |
| `scenarios/expected/` | `.gitkeep` only |
| `git diff -- prompts .github/instructions` | empty |
| CR characters in created files | none |
| Tracked paths not ignored | `.copilot-tracking/`, `scenarios/expected/baseline.txt`, `GangOfFour.slnx`, `global.json` |

## Release summary

Files are untracked in git. The agent did not commit, push or tag. The human commits, pushes and confirms CI is green on GitHub.
