<!-- markdownlint-disable-file -->
# Review: composite

Plan: `.copilot-tracking/plans/2026-10-16/composite-plan.instructions.md`
Changes: `.copilot-tracking/changes/2026-10-16/composite-changes.md`
Reviewer: RPI Agent (Phase 4)
Date: 2026-10-16
Outcome: Blocked

## Request fulfillment

| Request | Result | Evidence |
| --- | --- | --- |
| Generate and verify unchanged copy | Complete | Copy commit `e840c73`; 41 tests passed |
| Refactor S08 while preserving golden master | Blocked | Enabling nesting changes the approved scenario result |
| Complete README, diff, map, refactor commit, and tag | Waiting | Cannot finish without resolving the conflicting behavior requirements |

## Validation

* Unchanged-copy Release suite: 41/41 tests passed.
* Composite attempt compiled and its focused tests passed.
* Golden master: failed at line 46 because the nested-bundle rejection changed.
* No refactor commit or tag was created.

## Blocking finding

The pattern-map target calls out that bundles cannot nest, but the trading-day
scenario deliberately records a rejection when `BUN-GIFT` receives
`BUN-RUN-KIT`. The approved transcript is immutable for this task. Enabling
nested bundles changes that settled result; keeping the result means the
targeted behavior is not refactored. Further pattern work is paused at this
run-all checkpoint.
