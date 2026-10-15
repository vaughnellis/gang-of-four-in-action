<!-- markdownlint-disable-file -->
# Changes: decorator

Plan: `.copilot-tracking/plans/2026-10-15/decorator-plan.instructions.md`
Details: `.copilot-tracking/details/2026-10-15/decorator-details.md`
Planning log: `.copilot-tracking/plans/logs/2026-10-15/decorator-log.md`
Critique: `.copilot-tracking/plans/critiques/2026-10-15/decorator-critique.md`
Review: `.copilot-tracking/reviews/2026-10-15/decorator-review.md`
Artifact completion: 100%
Implementation date: 2026-10-15

## Summary

Replaced the S09 pricing if-chain with `IAddOnPriceComponent`, a zero-cost base, and gift-wrap and insurance decorators. Insurance continues to use the discounted subtotal as its basis. Cart booleans remain as the input/undo selection state; cost calculation is now decorator composition. All 45 tests pass and the Release build has zero warnings.

## Files added or modified

* `patterns/structural/decorator/src/CornerStore/Pricing/IAddOnPriceComponent.cs`
* `patterns/structural/decorator/src/CornerStore/Pricing/BaseAddOnPriceComponent.cs`
* `patterns/structural/decorator/src/CornerStore/Pricing/GiftWrapPriceDecorator.cs`
* `patterns/structural/decorator/src/CornerStore/Pricing/InsurancePriceDecorator.cs`
* `patterns/structural/decorator/src/CornerStore/Pricing/PricingService.cs`
* `patterns/structural/decorator/src/CornerStore/Cart/Cart.cs`
* `patterns/structural/decorator/src/CornerStore/StoreConsole.cs`
* `patterns/structural/decorator/src/CornerStore/CornerStore.csproj`
* `patterns/structural/decorator/tests/CornerStore.Tests/DecoratorTests.cs`
* `patterns/structural/decorator/README.md`
* `patterns/structural/decorator/DIFF.md`
* `docs/pattern-map.md`
* `prompts/patterns/structural-decorator.md`

The root README already linked Decorator and was left unchanged. The generated `DIFF.md` includes exactly the nine source/test paths listed in the pattern README.

## Validation

| Check | Result |
| --- | --- |
| Unchanged copy tests | 41 passed, 0 failed |
| Decorator-specific tests | 4 passed, 0 failed |
| Full solution tests | 45 passed, 0 failed |
| Release build with `-warnaserror` | Succeeded, 0 warnings and 0 errors |
| GoldenMasterTests copy | Hash matches baseline |
| S09 marker | Absent |
| S01 and S21 markers | Preserved |
| Editor diagnostics | No errors found |
| Root README link | Present and verified |
| `DIFF.md` summary | Exactly the nine README source/test paths |
| Diff whitespace and `git diff --check` | Clean |

## Release summary

Unchanged-copy commit: `b5c83ac`. The refactor commit and local `decorator-v1` tag are recorded in run-all progress after review.