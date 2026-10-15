<!-- markdownlint-disable-file -->
# Plan critique: baseline

Plan: `.copilot-tracking/plans/2026-10-15/baseline-plan.instructions.md`
Critic scope: the plan against `prompts/02-baseline.md`, the project brief's Decisions and Global boundaries, the foundations decisions and the repo instructions.
Revision: round 2, 2026-10-15, after the user answered C-1 to C-6 and C-13. The briefs were re-read; D-08, FR-013 and FR-014 now describe a plain golden master comparison.

## Verdict

PASS. Proceed to P01. All seven open decisions are answered and applied. The plan, details and log match the updated briefs, no Decision or Global boundary is broken, and nothing found blocks P01. Four new findings (C-14 to C-17) are informational; C-14 (`pwsh` not found) must be resolved before P11, not before P01.

## Coverage check

| Item | Covered | Notes |
| --- | --- | --- |
| FR-001 to FR-004 | Yes | P01 |
| FR-005 to FR-011 | Yes | P02 to P09, smell table in the details file |
| FR-012 to FR-014 | Yes | Scenario grows per phase, P10 completes and approves. P10 implements the plain comparison, received file and approval variable as FR-013 and FR-014 word them |
| FR-015 to FR-018 | Yes | P11 |
| AC-001 | Yes | Every phase stop runs the CI commands with zero warnings |
| AC-002 | Yes | P10 `DeterminismTests` and a manual double run |
| AC-003 | Yes | Marker search in the details file, run at every stop and at review |
| AC-004 | Yes | Per-phase pattern-leak confirmation, smell table with the shape per site |
| AC-005 | Yes | FR-012 coverage matrix, catalog entries name their steps |
| AC-006 | Yes | Banned API search, plus the conventions that avoid them |
| AC-007 | Yes, with C-14 | Run under `pwsh` and `powershell.exe`; blocked until `pwsh` is found |
| AC-008 | Yes | Budget is a ceiling (C-6) |
| D-01 to D-12 | Yes | D-08 now matches the plan: plain xUnit v3 comparison, no snapshot library; D-09 shapes the conventions |
| Global boundaries | Yes | No edits to `prompts/`, instructions or `patterns/` (except the scratch check); no push, tag or release |
| UR-7 and UR-8 | Yes | Every phase ends with a stop and the smell marker table |

## Findings

### C-1 Verify build-time fee check (resolved: Verify dropped)

Original finding: Verify 33.3.3 fails the build with SC021 until the repo declares its fee status. Resolution: the user dropped Verify and updated D-08, FR-013 and FR-014. `Verify.XunitV3` leaves `Directory.Packages.props` in P01-S1 and no fee status is declared (PD-3). Options B, C and D from round 1 no longer apply, and the yearly exemption renewal (old C-10) disappears.

### C-2 Approved transcript name (resolved: `baseline.txt`)

Original finding: D-08 and the briefs said `baseline.txt` but Verify can only read `baseline.verified.txt`. Resolution: with Verify gone the approved file is `scenarios/expected/baseline.txt` and the received file is `baseline.received.txt` (PD-4). The briefs and `.github/copilot-instructions.md` already agree, so the human has no wording fix to make.

### C-3 Phase order (resolved: 11 phases, stop after each)

The brief's order cannot compile: checkout needs pricing, payments, inventory, orders and notifications, and the cart widgets need pricing, promos and shipping. The plan uses 11 dependency-ordered phases that keep every smell assignment, except S05 (moved to P01 because `Log` is the transcript writer) and the S23 tax and weight sites (planted with pricing). Accepted by the user, with a stop after every phase.

### C-4 PowerShell 7 (resolved in plan; new blocker found, see C-14)

The user accepted cross-edition scripts, tested under both `pwsh` and `powershell.exe` in P11. The user says PowerShell 7 is now installed, but it could not be found on this machine (C-14).

### C-5 Scenario grows per phase (resolved: accepted)

Each phase adds its own steps so a stop can show real output. Only P10 approves `baseline.txt` (PD-8).

### C-6 Skimmability budget (resolved: ceiling, not a target)

The budget in PD-13 (60 files, about 2,800 lines, 9 files per folder, 40 lines per method unless a smell needs more) is a ceiling. Smaller is better. Pressure points remain P07 and P08; if a feature approaches the ceiling it is simplified and the reason recorded per AC-008.

### C-7 Static `Config` and `Log` versus parallel tests (resolved in plan)

S05 makes the app non-reentrant. xUnit runs test classes in parallel, and pattern tasks add test classes. The plan disables parallelisation at the assembly level (PD-7) and has the runner reset the statics on every `Run`. Alternative of thread-local statics would hide the smell.

### C-8 Order-sensitive refactors (informational)

Several seams have an observable order that a pattern refactor must keep: checkout steps, widget refresh order after a change, validation order, transition guard messages. Per the golden master, a refactor that reorders them fails the transcript, which is the point. The details file records each fixed order so that the pattern briefs can say so. `Random` is created per use from `seed + customerNumber` so that restructuring validation cannot shift a score.

### C-9 `global.json` edit (resolved: accepted)

`test.runner` is added to a foundations file in P01-S1 (research E-6, PD-2). The user confirmed it. It is called out in the P01 changes log entry.

### C-10 Verify fee renewal (withdrawn)

No longer applies. Verify is gone.

### C-11 `IPaymentProcessor` (low risk)

One interface exists in the baseline, as the product type that S03 will create. A reader could mistake it for the pattern already applied. The marker on `ChargePayment` says the smell is in the creation, and the changes log records the reason (PD-10). Alternative: three unrelated processor classes called with different method names, which would plant an Adapter smell by accident. Default: keep the interface.

### C-12 hve-core C# and PowerShell instructions (partly resolved)

The two C# files were read in round 2 and the conflicts with repo rules are recorded in log DL-11 (XML docs on every member, `LangVersion`, test naming and NSubstitute, `src/` at the root). The PowerShell file is read at P11-S1a.

### C-13 Planning logs are git-ignored (resolved: accepted)

The .NET `.gitignore` has `[Ll]ogs/`, which matches `.copilot-tracking/plans/logs/`. P01-S1 appends `!.copilot-tracking/plans/logs/` (PD-14) and verifies it with `git check-ignore`. The user also asked for `*.received.txt` to be ignored; it replaces the Verify-specific entries. After P01 both planning logs show up as untracked files for the human to commit.

### C-14 `pwsh` not found (new, informational for P01, blocks P11 until resolved)

The user says PowerShell 7 is now installed. In this terminal session `pwsh` is not on PATH, `C:\Program Files\PowerShell\7` does not exist, and no `pwsh*` exists in `WindowsApps`, `Program Files`, `Program Files (x86)` or the machine and user PATH. The install may have gone elsewhere or may not have completed. P11-S1a re-checks and stops to ask if it is still missing. P01 does not need it. Impact if never resolved: AC-007 cannot be run on the edition FR-016 names.

### C-15 Approval variable in pattern copies (new, informational)

`GoldenMasterTests.cs` is identical in every copy, so every pattern solution can also write `baseline.txt` when `GOLDEN_MASTER_APPROVE=1` is set. A stray variable in a shell would silently rewrite the shared approved file and hide a regression. The brief says only the baseline task may approve, and the test file is fixed by FR-013, so the mitigation is procedural: the P10 stop report tells the human to leave the variable unset, and the baseline-v1 review checks that `git diff` on `scenarios/expected/baseline.txt` is empty after the pattern task runs. Not changed in the plan beyond the P10 stop-report note.

### C-16 Shared received file (new, informational)

All copies write `scenarios/expected/baseline.received.txt`. `dotnet test` on the umbrella solution can run several test projects at once, so two failing copies could overwrite each other's received file. The test still fails in every case and the received file is git-ignored, so the cost is a possibly misleading received file, not a wrong pass. Accepted as is.

### C-17 Exact comparison and line endings (new, resolved in plan)

With no snapshot library nothing normalises line endings or a BOM. The plan has the runner write `\n` explicitly (PD-6), the test read and write with `UTF8Encoding(false)` and compare ordinally, and `.gitattributes` already forces LF for `*.txt` (checked). P10-S4 checks the approved file for LF and no BOM and exercises the mismatch path once.

## Gaps not addressed by the brief, left out on purpose

* Markdown lint and Dependabot (still follow-ons from foundations).
* SHA pinning of the two Actions.
* A pre-commit hook or analyzer for the SMELL markers. The search commands in the details file do the job.
* Benchmarks. Flyweight's BenchmarkDotNet comparison belongs to the Flyweight task.

## Decisions

All answered on 2026-10-15.

| ID | Question | Answer |
| --- | --- | --- |
| C-1 | How to handle the Verify fee check | Drop Verify; no fee declaration |
| C-2 | Approved file name | `scenarios/expected/baseline.txt` (plain comparison) |
| C-3 | 11 dependency-ordered phases or the brief's 8 | 11 phases, a stop after every phase |
| C-4 | Cross-edition PowerShell scripts | Accepted; test under `pwsh` and `powershell.exe` in P11 |
| C-5 | Scenario growth | Grow per phase; only the final scenario phase approves |
| C-6 | Size budget | Accepted as a ceiling, not a target |
| C-13 | `.gitignore` negation for logs | Accepted; also ignore `*.received.txt` |
| C-9 | `global.json` runner setting | Accepted, applied in P01 |

## Pass criteria used for this verdict

* Every open decision answered and reflected in the plan, details and log (checked by search: no live Verify dependency remains in the plan or details).
* FR-001 to FR-018 and AC-001 to AC-008 each map to a phase (coverage table).
* No Decision or Global boundary broken (table above).
* No blocker for P01: the edits P01 needs are `global.json`, `Directory.Packages.props` and `.gitignore`, all permitted; the new projects need only `xunit.v3`, `xunit.runner.visualstudio` and `Microsoft.NET.Test.Sdk`, which are pinned.
