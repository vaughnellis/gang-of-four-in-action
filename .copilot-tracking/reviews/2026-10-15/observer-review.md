<!-- markdownlint-disable-file -->
# Review: observer

Plan: `.copilot-tracking/plans/2026-10-15/observer-plan.instructions.md`
Changes: `.copilot-tracking/changes/2026-10-15/observer-changes.md`
Reviewer: RPI Agent (Phase 4)
Date: 2026-10-15
Artifact completion: 100%
Outcome: Conformant with justified divergence

## Executive outcome

S19 status-change reactions now use an ordered observer list. All 44 tests pass, including the shared golden master, and the Release build succeeds with warnings as errors. Only the S19 direct reaction path changed; order transitions and other smells remain intact.

## Most recent findings

| ID | Severity | Finding | Impact and disposition |
| --- | --- | --- | --- |
| RV-1 | Medium | `OrderService` still references `InventoryService` for cancellation/refund stock restoration. | This is not the S19 status-change callback: the inventory lookup formerly inside `RecordStatusChange` now belongs to `InventoryOrderStatusObserver`. The restoration path is unchanged and its S20-adjacent behavior remains intact. Accepted. |
| RV-2 | Low | Observer registration order is significant because transcript output is ordered. | `StoreConsole` registers inventory, notification, loyalty, and analytics observers in the original side-effect order. Golden master confirms byte-for-byte behavior. Accepted. |
| RV-3 | Medium | The baseline diff generator fails on scalar native command output in both installed PowerShell editions. | Frozen tooling was not changed. The process-local compatibility shim generated `DIFF.md`; generated whitespace-only lines were normalized. This is the same documented pilot limitation. |
| RV-4 | Info | Root README already contained the Observer folder link. | Verified and left unchanged. |

## Request fulfillment

| Request | Result | Evidence |
| --- | --- | --- |
| Copy and verify the baseline | Complete | 41 tests passed before edits; copy commit `8470705` |
| Publish valid status changes to observers | Complete | `OrderService.NotifyObservers` dispatches after successful transitions |
| Preserve built-in reactions and support new observers | Complete | Four composed observers preserve transcript; test-only observer registers through `Subscribe` |
| Suppress notifications on invalid transitions | Complete | `ObserverTests` verifies invalid `Ship` transition leaves observers untouched |
| Complete README, diff, map, and brief | Complete | README covers Observer versus Mediator, overuse, diagram, and exact file list; map is Done; task brief exists |

## Validation

* Unchanged copy: 41/41 tests passed.
* Observer-specific tests: 3/3 passed.
* Full Observer solution: 44/44 tests passed.
* Release build with warnings as errors: 0 warnings, 0 errors.
* Golden-master test file hash matches baseline; golden master passes.
* `get_errors`: no errors found.
* Root README link exists; generated diff is whitespace-clean; `git diff --check` passed.

## Remaining work

Commit the reviewed Observer changes locally, update the run-all row with both hashes, create `observer-v1`, and continue with Factory Method. Do not push.