<!-- markdownlint-disable-file -->
# Baseline Review

Plan: `.copilot-tracking/plans/2026-10-15/baseline-plan.instructions.md`
Change record: `.copilot-tracking/changes/2026-10-15/baseline-changes.md`
Reviewer: GitHub Copilot
Review date: 2026-10-15
Outcome: Conformant with justified divergence

## Request fulfillment

| Request | Result | Evidence |
| --- | --- | --- |
| Create run-all progress first and resume the baseline at P05 | Complete | `.copilot-tracking/run-all-progress.md` was created before baseline edits; P01-P04 were preserved and P05 onward completed |
| Keep S17 as direct peer references without coordinator/events | Complete | Four widget classes hold peer references and directly call peer refresh methods; no mediator or event system exists |
| Keep S18 undo as built | Complete | `StoreConsole` still copies public cart values before add/remove and restores them on undo |
| Complete the baseline and stop at its run-all checkpoint | Ready | All P01-P11 acceptance criteria pass; commit and `baseline-v1` tag follow this review |

## Acceptance criteria

| Criterion | Result | Evidence |
| --- | --- | --- |
| AC-001 | Pass | Full Release build succeeds with warnings as errors; 41 tests pass |
| AC-002 | Pass | `DeterminismTests` compares two complete runs; golden master matches byte-for-byte |
| AC-003 | Pass | Source scan finds exactly S01-S23 and no other marker IDs |
| AC-004 | Pass | Every planted marker matches its mapped smell; no target pattern is applied at a smell site |
| AC-005 | Pass | All smell sites are exercised by the 75-step scenario and listed in the catalog |
| AC-006 | Pass | No forbidden time, GUID, sleep, delay or hash APIs; random scoring uses the scenario seed |
| AC-007 | Pass | `scratch-check` copied, built and passed the golden master under PowerShell 7.6.7 and Windows PowerShell 5.1; both diff runs reported no differences |
| AC-008 | Pass | 55 C# source files and 2,212 lines; largest feature folder has 9 files |

FR-001 to FR-014 are fulfilled by the baseline projects, deterministic app, 23 marked smells, full trading-day scenario and approved plain-string golden master. FR-015 to FR-018 are fulfilled by the completed smell catalog, cross-edition copy/diff tools and CI freshness check.

## Validation

* `dotnet restore GangOfFour.slnx`: succeeded
* `dotnet build GangOfFour.slnx --no-restore -c Release -warnaserror`: succeeded, 0 warnings and 0 errors
* `dotnet test GangOfFour.slnx --no-build -c Release`: 41 passed, 0 failed
* Golden-master mismatch probe reported the first differing line, then passed after restoring the approved transcript
* `baseline.txt` is 14,887 bytes, UTF-8 without BOM, LF only, with a final LF
* `git diff --check`: succeeded
* No scratch folder or received transcript remains

## Accepted divergences

* Checkout computes the quote before `OrderValidator.Validate` because the planned validator signature accepts a `Quote`. Validation still precedes payment and all checkout state changes; computing the quote has no business side effects.
* PowerShell scripts require version 5.1 rather than 7.0 to honor settled PD-12. Both scripts were executed under Windows PowerShell 5.1 and PowerShell 7.6.7.
* PSScriptAnalyzer is not installed in the environment. Both scripts were parsed and exercised by successful runs in both supported editions.

No unresolved findings remain. Commit `79da1bac67bc277d66de86bd7610ba9a84c6ce16` is tagged `baseline-v1`. Do not push. The run-all checkpoint now awaits human review before pattern copies begin.
