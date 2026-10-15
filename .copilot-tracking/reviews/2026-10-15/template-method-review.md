<!-- markdownlint-disable-file -->
# Review: template-method

Plan: `.copilot-tracking/plans/2026-10-15/template-method-plan.instructions.md`
Changes: `.copilot-tracking/changes/2026-10-15/template-method-changes.md`
Reviewer: RPI Agent (Phase 4)
Date: 2026-10-15
Artifact completion: 100%
Outcome: Conformant with justified divergence

## Executive outcome

The shared report algorithm now lives in `ReportExporter`; the subclasses provide accumulation and writing. Both S23 switches remain in their original format-specific classes, and the golden master is unchanged. The Release build succeeds with warnings as errors, and all 45 tests pass.

## Most recent findings

| ID | Severity | Finding | Impact and disposition |
| --- | --- | --- | --- |
| RV-1 | Low | `ReportTotals` exposes mutable totals to subclass hooks. | This gives the template subclasses the minimal shared accumulator needed to preserve current aggregation and S23 behavior. Accepted. |
| RV-2 | Low | CSV and JSON still contain separate product-type switches. | These are the S23 smell and are intentionally preserved; Template Method removes only the shared S22 workflow. Accepted. |
| RV-3 | Medium | The baseline diff generator assumes scalar command output is an array and emits whitespace-only lines. | Frozen tooling was not edited. The process-local shim generated the artifact and generated whitespace was normalized. |
| RV-4 | Info | Root README already contained the Template Method link. | Verified and left unchanged. |

## Request fulfillment

| Request | Result | Evidence |
| --- | --- | --- |
| Copy and verify baseline | Complete | 41 tests passed before edits; copy commit `c812e07` |
| Extract fixed report algorithm | Complete | `ReportExporter.Export` owns filtering, traversal, accumulation dispatch, and writing dispatch |
| Preserve S23 hooks and output | Complete | Both format-specific switches remain; golden master passes byte-for-byte |
| Complete docs, diff, map, and index | Complete | README compares Strategy, diff matches six source/test paths, map Done, and root link exists |

## Validation

* Unchanged copy: 41/41 tests passed.
* Template Method-specific tests: 4/4 passed.
* Full solution: 45/45 tests passed.
* Release build with warnings as errors: 0 warnings, 0 errors.
* Golden-master test file hash matches baseline; golden master passes.
* `get_errors`: no errors found.
* S22 absent; S23 preserved in both exporters; DIFF whitespace clean; `git diff --check` passed.

## Remaining work

The reviewed Template Method changes are committed as `ca45636`; the unchanged copy is `c812e07`; local tag `template-method-v1` points to `ca45636f581e14d3a096fd7475635648871291c5`. The progress row is Done. Continue with Chain of Responsibility. Do not push.