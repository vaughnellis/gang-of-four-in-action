<!-- markdownlint-disable-file -->
# Changes: interpreter

Plan: `.copilot-tracking/plans/2026-10-16/interpreter-plan.instructions.md`
Critique: `.copilot-tracking/plans/critiques/2026-10-16/interpreter-critique.md`
Review: `.copilot-tracking/reviews/2026-10-16/interpreter-review.md`
Implementation date: 2026-10-16

## Summary

Replaced per-evaluation parsing of promo text with a recursive-descent parser that creates an Interpreter expression tree once for each configured promo. The expression nodes evaluate strict spend and quantity thresholds, any-line category equality, customer-region equality, and `AND` conjunctions against a `PromoEvaluationContext`. Original display text and scenario output remain unchanged.

`CornerStore.csproj` now grants internal visibility to `CornerStore.Tests`. This keeps parser and expression participants internal while allowing focused tests to exercise the grammar directly.

## Files added or modified

* `patterns/behavioral/interpreter/src/CornerStore/CornerStore.csproj`
* `patterns/behavioral/interpreter/src/CornerStore/Pricing/PromoEngine.cs`
* `patterns/behavioral/interpreter/src/CornerStore/Pricing/PromoEvaluationContext.cs`
* `patterns/behavioral/interpreter/src/CornerStore/Pricing/PromoExpression.cs`
* `patterns/behavioral/interpreter/src/CornerStore/Pricing/PromoRuleParser.cs`
* `patterns/behavioral/interpreter/tests/CornerStore.Tests/InterpreterTests.cs`
* `patterns/behavioral/interpreter/README.md`
* `patterns/behavioral/interpreter/DIFF.md`
* `GangOfFour.slnx`, updated by the required copy generator to include the app and test projects.
* `docs/pattern-map.md`
* `prompts/patterns/behavioral-interpreter.md`
* `.copilot-tracking/plans/2026-10-16/interpreter-plan.instructions.md`
* `.copilot-tracking/plans/critiques/2026-10-16/interpreter-critique.md`
* `.copilot-tracking/run-all-progress.md`

The root README already contained the Interpreter index link and was not changed. The generated `DIFF.md` covers six source and test files, all six are listed in the pattern README. It contains 487 lines with 348 additions and 78 removals.

## Validation

| Check | Result |
| --- | --- |
| Unchanged-copy Release tests | 41 passed, 0 failed |
| Final Interpreter Release tests | 54 passed, 0 failed |
| Interpreter solution Release build | Succeeded, 0 warnings and 0 errors |
| Golden master | Passed; `GoldenMasterTests.cs` SHA-256 matches baseline |
| S15 and other smell markers | S15 removed; all 61 remaining markers match baseline |
| README and diff paths | All six generated paths are listed in the README |
| `DIFF.md` reproducibility | Two generator runs produced the same SHA-256: `779773702F634FF638FBC7ADA25634C2E1F8D60D8C4A94FC1D0B0DDAF00C221D` |
| Root README link | Existing Interpreter link verified |
| Whitespace | `git diff --check` clean |

The umbrella solution has a previously recorded duplicate-project-name issue in its behavioral folder. The Interpreter-specific solution builds and tests successfully. This pattern task did not change the root solution's naming convention or attempt unrelated solution restructuring.

## Release summary

Unchanged-copy commit: `0cd28fb`. Refactor commit: `71d1cea`. Local tag: `interpreter-v1`. No push was made.
