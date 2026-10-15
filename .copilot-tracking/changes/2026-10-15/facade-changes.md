<!-- markdownlint-disable-file -->
# Changes: facade

Plan: `.copilot-tracking/plans/2026-10-15/facade-plan.instructions.md`
Details: `.copilot-tracking/details/2026-10-15/facade-details.md`
Planning log: `.copilot-tracking/plans/logs/2026-10-15/facade-log.md`
Critique: `.copilot-tracking/plans/critiques/2026-10-15/facade-critique.md`
Review: `.copilot-tracking/reviews/2026-10-15/facade-review.md`
Artifact completion: 100%
Implementation date: 2026-10-15

## Summary

Added `CheckoutFacade.PlaceOrder` and routed the console checkout through it. The existing subsystem order and business behavior are preserved. The Facade solution builds with zero warnings and passes all 44 tests, including the golden master and three facade-specific tests.

## Files added or modified

* `patterns/structural/facade/src/CornerStore/Checkout/CheckoutFacade.cs`
* `patterns/structural/facade/src/CornerStore/StoreConsole.cs`
* `patterns/structural/facade/src/CornerStore/CornerStore.csproj`
* `patterns/structural/facade/tests/CornerStore.Tests/CheckoutFacadeTests.cs`
* `patterns/structural/facade/README.md`
* `patterns/structural/facade/DIFF.md`
* `docs/pattern-map.md`
* `GangOfFour.slnx` (unchanged-copy registration was included in commit `ca016f9`)

The root `README.md` already linked `patterns/structural/facade/` and needed no edit. The generated `DIFF.md` covers exactly the four source/test files listed under Files changed in the pattern README.

## Pilot notes

* The first unchanged-copy build and golden-master gate was clear and fast. Keep this as the first operation in every pattern task.
* The checked-in `tools/diff-baseline.ps1` fails under both Windows PowerShell 5.1 and PowerShell 7 on a single-file diff: native output is scalar, but the script assumes `.Count` and array indexing. Do not fix it in a pattern task because the baseline tooling is frozen. Suggested tooling fix for a later approved baseline/tooling task: normalize native output to an array before counting and indexing, and add a one-file-difference regression check.
* Direct tests for an internal facade needed `InternalsVisibleTo` scoped to `CornerStore.Tests`. Suggested template note: pattern tests may exercise internal participants, but keep the production API internal and use the repository's established test-visibility mechanism.
* The root README already links all pattern folders before they exist. Verify the required link and avoid a redundant README edit when it is already present.
* No Facade-specific edits to `prompts/patterns/_template.md` are needed until both pilots have recorded their notes. Suggested template note: explicitly require checking whether the index link already exists and recording any diff-generator workaround.

## Validation

| Check | Result |
| --- | --- |
| Unchanged copy tests | 41 passed, 0 failed |
| Checkout scenario tests | 3 passed, 0 failed |
| Facade-specific tests | 3 passed, 0 failed |
| Full Facade solution tests | 44 passed, 0 failed |
| Release build with `-warnaserror` | Succeeded, 0 warnings and 0 errors |
| GoldenMasterTests copy | Hash matches baseline |
| S10 marker | Absent |
| S02, S03, S06, S13 markers | Preserved |
| Editor diagnostics | No errors found |
| `git diff --check` | Passed |
| Diff artifact summary | Exactly the four README source/test paths |

## Release summary

Unchanged-copy commit: `ca016f9`. The refactor commit and `facade-v1` tag are recorded in run-all progress after review.