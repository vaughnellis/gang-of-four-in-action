<!-- markdownlint-disable-file -->
# Planning log: foundations

Plan: `.copilot-tracking/plans/2026-10-15/foundations-plan.instructions.md`

## Discrepancy log

| ID | Item | Resolution |
| --- | --- | --- |
| DL-1 | `prompts/README.md` shows plan path `.copilot-tracking/plans/<date>/<task slug>-plan.md`; RPI Agent convention is `<task>-plan.instructions.md` | Used the RPI Agent convention. `prompts/` is off limits; propose aligning the README or the agent in the change record |
| DL-2 | AC-003 says every folder in brief section 4 exists, but section 4 includes `baseline/src/CornerStore/` and `baseline/tests/CornerStore.Tests/` | Treated FR-005 as the authoritative list. Those two belong to the baseline task, and FR-005 lists only `baseline/` |
| DL-3 | Brief says "xUnit" and "Verify for xUnit" without naming v2 or v3 | Chose v3, pending user confirmation (critique C-1) |
| DL-4 | Brief does not mention `.gitattributes`; golden master is a byte comparison and the author works on Windows | Proposed as PA-1, pending approval |
| DL-5 | hve-core markdown rules want frontmatter on docs and root README; GitHub renders frontmatter as a table on the README | Followed hve-core; flagged in critique C-5 |
| DL-6 | Workspace is not a git repository | Resolved: the user initialised git and committed the starter files before implementation |
| DL-7 | PD-7 put frontmatter on docs, but `.github/copilot-instructions.md` (user edit, uncommitted at implementation time) says markdown files use a plain H1 and no frontmatter outside `prompts/` and `.copilot-tracking/` | Followed the repo rule: root README and `docs/*.md` use a plain H1 and no frontmatter. The hve-core markdown rule is superseded for this repo |

## User decisions applied at implementation

| ID | Decision | Effect |
| --- | --- | --- |
| C-1 | xUnit v3 with `Verify.XunitV3` | Package pins as planned |
| C-2 | LICENSE holder Vaughn Ellis Ramos, 2026 | `LICENSE` written |
| C-3 | Add `.gitattributes` forcing LF, binary files marked binary | PA-1 approved |
| C-5 | Plain H1 for the root README, no frontmatter | Applied to README and, per DL-7, to docs |
| C-4 / DL-2 | Follow FR-005 | `baseline/src` and `baseline/tests` stay with the baseline task |
| C-10 / DL-1 | Leave `prompts/` untouched; record the naming as follow-on | See follow-on below. The human edits `prompts/README.md` |

## Research performed

* Read `prompts/00-project-brief.md`, `prompts/01-foundations.md`, `prompts/README.md`, `.github/copilot-instructions.md` and both scoped instruction files.
* Probed an empty `.slnx` with `Directory.Build.props` and `Directory.Packages.props` in a temp directory (since deleted): restore, build and test exit 0.
* Queried NuGet for latest stable versions and the xUnit dependency ranges of both Verify packages.
* No Decisions D-01 to D-12 were re-researched.

## Implementation paths considered

| Topic | Selected | Alternatives |
| --- | --- | --- |
| xUnit line | v3 | v2 (`xunit` 2.9.3 with `Verify.Xunit` 31.12.5) |
| SDK pin | exact 10.0.302, `latestFeature` roll forward | 10.0.100 floor with `latestFeature`; `latestPatch` only |
| `.gitignore` source | `dotnet new gitignore` plus additions | hand-written minimal file |
| Action pinning | major version tags | full commit SHA pins |
| TFM location | per project (baseline task) | central in `Directory.Build.props` |

## Suggested follow-on work

* Baseline task: set `net10.0` in each project, mark test projects as xUnit v3 executables, add the Verify shared-path configuration.
* Add `.markdownlint.json` and a markdown lint step to CI if the hve-core rules should be enforced on this repo.
* Add Dependabot for NuGet and GitHub Actions once projects exist.
* Align `prompts/README.md` plan path with the RPI Agent convention (human edit). Correct naming: the RPI Agent writes plans to `.copilot-tracking/plans/<date>/<task>-plan.instructions.md`, details to `.copilot-tracking/details/<date>/<task>-details.md`, changes to `.copilot-tracking/changes/<date>/<task>-changes.md` and reviews to `.copilot-tracking/reviews/<date>/<task>-plan-review.md`, whereas `prompts/README.md` shows `<task slug>-plan.md`.
* Decide whether to SHA-pin `actions/checkout` and `actions/setup-dotnet` in `ci.yml`.
* Add a status note or placeholder folders so README index links do not 404 (README Status section covers this for now).
