<!-- markdownlint-disable-file -->
# Review: foundations

Plan: `.copilot-tracking/plans/2026-10-15/foundations-plan.instructions.md`
Changes: `.copilot-tracking/changes/2026-10-15/foundations-changes.md`
Reviewer: RPI Agent (Phase 4)
Date: 2026-10-15

## Overall status

Complete. All user requests and acceptance criteria are met. One check remains outside the agent's reach: CI green on GitHub, which the human confirms after pushing.

## User request fulfilment

| ID | Request | Status | Evidence |
| --- | --- | --- | --- |
| UR-1 | Planning, implementation and review | Complete | Plan, details, log, critique, changes log and this review exist |
| UR-2 | Decisions D-01 to D-12 settled, not re-researched | Complete | Only open questions were xUnit line, license holder and similar; none re-opened a Decision |
| UR-3 | Respect Global and task boundaries | Complete | No `.csproj` or `.cs`, `prompts/` and `.github/instructions/` unchanged, `scenarios/expected/` holds `.gitkeep` only, no push or tag |
| UR-4 | P01 to P03 as the starting point | Complete | P01 to P03 kept as written, P04 added for verification |
| UR-5 | Stop after planning | Complete | Stopped; implementation began only after the user's reply |
| UR-6 | Stop at the brief's Stop point | Pending hand-back | Handed back in the final message |
| UR-7 | FR-001 to FR-008, AC-001 to AC-005 | Complete | Table below |

## Requirements and acceptance criteria

| ID | Status | Evidence |
| --- | --- | --- |
| FR-001 | Complete | `README.md`, `LICENSE`, `.gitignore`, `.editorconfig`, `global.json` exist |
| FR-002 | Complete | `Directory.Build.props` sets the four properties |
| FR-003 | Complete | `Directory.Packages.props` with CPM and five versions |
| FR-004 | Complete | `GangOfFour.slnx` has the five solution folders |
| FR-005 | Complete | Seven `.gitkeep` files plus `docs/` |
| FR-006 | Complete | Three docs created; map has 23 rows with Status Not started |
| FR-007 | Complete | `ci.yml` parsed and checked |
| FR-008 | Complete | README has all five required parts |
| AC-001 | Met | Build exits 0 locally |
| AC-002 | Met | Valid YAML, steps match |
| AC-003 | Met per FR-005 | `baseline/src` and `baseline/tests` belong to the baseline task (user confirmed) |
| AC-004 | Met | 23 rows, tuples identical to the brief |
| AC-005 | Met | No C# or solution projects |

## Findings

| ID | Severity | Finding | Evidence | Impact | Recommended action |
| --- | --- | --- | --- | --- | --- |
| RV-1 | Medium | README pattern index links point to folders that do not exist yet | 23 links to `patterns/<category>/<slug>/`; only the category folders exist | Visitors get 404s on GitHub until pattern tasks land | Kept as the brief requires; the README Status section states it. Revisit if the repo is made public before patterns exist |
| RV-2 | Medium | Docs frontmatter conflict between hve-core markdown rules and the repo rule | hve-core requires `title` and `description`; `.github/copilot-instructions.md` forbids frontmatter outside `prompts/` and `.copilot-tracking/` | A future markdownlint or frontmatter validation step would fail on the docs | Followed the repo rule. If lint is added, configure it to match the repo rule |
| RV-3 | Low | `.github/copilot-instructions.md` has an uncommitted edit that the agent did not make | `git diff` shows one added line | The rule behind RV-2 is not in the commit history yet | Human commits it with the skeleton |
| RV-4 | Low | `dotnet restore` prints "Unable to find a project to restore" on the empty solution | Local restore output | Cosmetic; exit code 0 and CI still passes | Disappears when the baseline task adds projects |
| RV-5 | Low | CI actions are pinned to major tags, not commit SHAs | `ci.yml` | Supply-chain risk is small with `contents: read`, but tags can move | Optional follow-on, see Discover |
| RV-6 | Info | CI has not run on GitHub | No remote push by the agent | AC-002 is verified by parsing only | Human pushes and confirms green |
| RV-7 | Info | Created files were CRLF from the file tool, now LF | Checked, no CR remains | None after fix | None |

No blocking findings.

## Validation outputs

Restore, build and test with the CI flags all exit 0 after the final change (see the changes log, Validation table).

## Follow-up recommendations

See Phase 5 Discover in the final message and the planning log follow-on section.
