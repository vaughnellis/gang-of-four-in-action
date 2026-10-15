<!-- markdownlint-disable-file -->
# Review: strategy

Plan: `.copilot-tracking/plans/2026-10-15/strategy-plan.instructions.md`
Changes: `.copilot-tracking/changes/2026-10-15/strategy-changes.md`
Reviewer: RPI Agent (Phase 4)
Date: 2026-10-15
Artifact completion: 100%
Outcome: Conformant with justified divergence

## Executive outcome

S21 is refactored into three interchangeable shipping-cost strategies selected through an injected registry. The existing costs, unknown-method handling, AU/NZ surcharge, and shared transcript are preserved. The Release build succeeds with warnings as errors and all 48 tests pass.

## Most recent findings

| ID | Severity | Finding | Impact and disposition |
| --- | --- | --- | --- |
| RV-1 | Medium | The baseline diff generator assumes native `git diff --numstat` output is always an array, so it fails on a one-file difference in both installed PowerShell editions. | This affects all remaining pattern tasks. Baseline tooling is frozen. A process-local shim generated the normal artifact and whitespace-only lines were normalized; the limitation is documented in both pilot records and the template. No tool file changed. |
| RV-2 | Low | Tests need access to internal strategy participants and to inject a test-only strategy. | `InternalsVisibleTo` is scoped to `CornerStore.Tests`; production types remain internal. The injected registry proves pricing can use a new implementation without code changes. Accepted. |
| RV-3 | Low | The strategy registry is created inside pricing by default but can be replaced by composition code. | This preserves the current `PricingService(PromoEngine)` call site while enabling test and future composition. Default method registration remains explicit and unknown methods still return null. Accepted. |
| RV-4 | Info | Root README already contained the Strategy folder link. | Verified and left unchanged. |
| RV-5 | Info | Strategy's prompt dependency required applying the Facade notes before the run-all step that applies both pilots' notes. | Facade guidance was applied before Strategy; this change record incorporates Strategy's lock-as-is recommendation after its review. Sequencing deviation is recorded in `.copilot-tracking/plans/logs/2026-10-15/strategy-log.md`. |

## Request fulfillment

| Request | Result | Evidence |
| --- | --- | --- |
| Create and verify unchanged copy | Complete | 41 tests passed before edits; copy commit `da5088a` |
| Replace S21 switch with extensible strategies | Complete | `IShippingCostStrategy`, three default implementations, and injected registry |
| Preserve adjacent smells and behavior | Complete | S21 absent; S01 and S09 remain; golden-master file hash matches baseline |
| Add Strategy tests and README | Complete | Seven tests cover all methods in both regions and a test-only strategy; README includes diagram, State comparison, overkill guidance, and exact changed-file list |
| Generate diff, update map/index, record pilot recommendation | Complete with justified divergence | Diff matches seven source/test paths; map Done; root link already existed; template recommendation is to lock as is |

## Validation

* Unchanged copy: 41/41 tests passed.
* Strategy-specific tests: 7/7 passed.
* Full Strategy solution: 48/48 tests passed.
* Release build with warnings as errors: 0 warnings, 0 errors.
* Golden-master test file hash matches baseline; golden master passes.
* `get_errors`: no errors found.
* Root README link exists; `DIFF.md` has no trailing whitespace; `git diff --check` passed.

## Remaining work

Commit the reviewed Strategy changes locally, update the run-all row with both commit hashes, create `strategy-v1`, and continue with Batch A Observer. Do not push.