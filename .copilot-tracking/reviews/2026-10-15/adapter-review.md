<!-- markdownlint-disable-file -->
# Review: adapter

Plan: `.copilot-tracking/plans/2026-10-15/adapter-plan.instructions.md`
Changes: `.copilot-tracking/changes/2026-10-15/adapter-changes.md`
Reviewer: RPI Agent (Phase 4)
Date: 2026-10-15
Artifact completion: 100%
Outcome: Conformant with justified divergence

## Executive outcome

S06 is isolated behind `IShippingProvider` and `ShipFastAdapter`. The adapter preserves cents rounding, safely escapes XML values, invokes the unchanged fake client, and extracts the tracking number. All 43 tests pass and the Release build succeeds with warnings as errors.

## Most recent findings

| ID | Severity | Finding | Impact and disposition |
| --- | --- | --- | --- |
| RV-1 | Low | `IShippingProvider.BookShipment` receives the domain `Order` object. | The provider is internal and this keeps caller-side mapping small; the adapter translates the vendor protocol. Accepted for this teaching example. |
| RV-2 | Medium | The baseline diff generator fails on scalar native output and emits whitespace-only diff lines. | Frozen tooling was not changed. The process-local compatibility shim generated the artifact, and only generated whitespace was normalized. The issue remains documented in the pilot notes. |
| RV-3 | Info | Root README already contained the Adapter link. | Verified and left unchanged. |

## Request fulfillment

| Request | Result | Evidence |
| --- | --- | --- |
| Copy and verify baseline | Complete | 41 tests passed before edits; copy commit `e3af51e` |
| Adapt the ShipFast protocol behind an interface | Complete | `StoreConsole` depends on `IShippingProvider`; `ShipFastAdapter` handles the XML/cents protocol |
| Verify escaping and tracking output | Complete | Two adapter tests cover special XML characters, cents output, and tracking ID |
| Complete docs, diff, map, and index | Complete | README distinguishes Decorator; diff matches five source/test paths; map Done; index link already existed |

## Validation

* Unchanged copy: 41/41 tests passed.
* Adapter-specific tests: 2/2 passed.
* Full solution: 43/43 tests passed.
* Release build with warnings as errors: 0 warnings, 0 errors.
* Golden-master test file hash matches baseline; golden master passes.
* `get_errors`: no errors found.
* Root README link exists; DIFF whitespace clean; `git diff --check` passed.

## Remaining work

Commit the reviewed Adapter changes locally, update progress with both hashes, create `adapter-v1`, and continue with Decorator. Do not push.