<!-- markdownlint-disable-file -->
# Review: builder

Plan: `.copilot-tracking/plans/2026-10-15/builder-plan.instructions.md`
Changes: `.copilot-tracking/changes/2026-10-15/builder-changes.md`
Reviewer: RPI Agent (Phase 4)
Date: 2026-10-15
Artifact completion: 100%
Outcome: Conformant with justified divergence

## Executive outcome

`OrderService` now uses fluent builders for both Order and Invoice, with named values and required-value checks. Optional fields retain null defaults and the model constructors remain intact. The Release build succeeds with warnings as errors and all 45 tests pass.

## Most recent findings

| ID | Severity | Finding | Impact and disposition |
| --- | --- | --- | --- |
| RV-1 | Low | The model types retain their positional constructors. | Construction call sites are now centralized inside the builders, while the data model and neighboring S20 behavior remain unchanged. This keeps the S02 refactor focused. Accepted. |
| RV-2 | Low | `InvoiceBuilder` takes an `Order` as its source rather than repeating all quote values. | This gives the invoice builder a coherent input and derives totals from the same quote used by the order. Accepted. |
| RV-3 | Medium | The baseline diff generator fails on scalar native output and emits whitespace-only lines. | Frozen tooling was not modified. A local compatibility shim generated the diff, and only generated whitespace was normalized. |
| RV-4 | Info | Root README already contained the Builder link. | Verified and left unchanged. |

## Request fulfillment

| Request | Result | Evidence |
| --- | --- | --- |
| Copy and verify baseline | Complete | 41 tests passed before edits; copy commit `2fcb8e5` |
| Replace positional order/invoice assembly | Complete | `OrderService` uses `OrderBuilder` and `InvoiceBuilder` fluent APIs |
| Validate required values and preserve optional defaults | Complete | Builder tests verify clear failures and null optionals |
| Complete docs, diff, map, and index | Complete | README compares Builder with Factory Method; diff matches five source/test files; map Done; root link already present |

## Validation

* Unchanged copy: 41/41 tests passed.
* Builder-specific tests: 4/4 passed.
* Full solution: 45/45 tests passed.
* Release build with warnings as errors: 0 warnings, 0 errors.
* Golden-master test file hash matches baseline; golden master passes.
* `get_errors`: no errors found.
* Root README link exists; DIFF whitespace clean; `git diff --check` passed.

## Remaining work

Commit the reviewed Builder changes locally, update progress with both hashes, create `builder-v1`, and continue with Template Method. Do not push.