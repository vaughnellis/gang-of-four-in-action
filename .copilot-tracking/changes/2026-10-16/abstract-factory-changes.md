<!-- markdownlint-disable-file -->
# Changes: abstract-factory

Plan: `.copilot-tracking/plans/2026-10-16/abstract-factory-plan.instructions.md`
Critique: `.copilot-tracking/plans/critiques/2026-10-16/abstract-factory-critique.md`
Review: `.copilot-tracking/reviews/2026-10-16/abstract-factory-review.md`
Implementation date: 2026-10-16

## Summary

Replaced scattered regional tax, shipping-surcharge and currency branches with AU, NZ and fallback family factories. Pricing uses one regional pricing product for both tax and shipping. The fallback factory preserves the prior behavior for unrecognized region codes. The S23 line-item tax switch remains unchanged.

The `CornerStore.csproj` change adds `InternalsVisibleTo` for `CornerStore.Tests`, allowing focused tests to verify internal factory participants without widening the production API.

## Files added or modified

* `patterns/creational/abstract-factory/src/CornerStore/CornerStore.csproj`
* `patterns/creational/abstract-factory/src/CornerStore/Pricing/AustraliaRegionFactory.cs`
* `patterns/creational/abstract-factory/src/CornerStore/Pricing/DefaultRegionFactory.cs`
* `patterns/creational/abstract-factory/src/CornerStore/Pricing/IMoneyFormatter.cs`
* `patterns/creational/abstract-factory/src/CornerStore/Pricing/IRegionFactory.cs`
* `patterns/creational/abstract-factory/src/CornerStore/Pricing/IRegionalPricing.cs`
* `patterns/creational/abstract-factory/src/CornerStore/Pricing/MoneyFormatter.cs`
* `patterns/creational/abstract-factory/src/CornerStore/Pricing/NewZealandRegionFactory.cs`
* `patterns/creational/abstract-factory/src/CornerStore/Pricing/PricingService.cs`
* `patterns/creational/abstract-factory/src/CornerStore/Pricing/RegionalMoneyFormatter.cs`
* `patterns/creational/abstract-factory/src/CornerStore/Pricing/RegionalPricing.cs`
* `patterns/creational/abstract-factory/src/CornerStore/Pricing/RegionFactoryProvider.cs`
* `patterns/creational/abstract-factory/src/CornerStore/Pricing/ShippingCalculator.cs` (removed)
* `patterns/creational/abstract-factory/src/CornerStore/Pricing/TaxCalculator.cs`
* `patterns/creational/abstract-factory/tests/CornerStore.Tests/AbstractFactoryTests.cs`
* `patterns/creational/abstract-factory/README.md`
* `patterns/creational/abstract-factory/DIFF.md`
* `docs/pattern-map.md`
* `prompts/patterns/creational-abstract-factory.md`

The root README already contained the Abstract Factory link and was left unchanged. `DIFF.md` covers the 15 source and test paths listed in the README.

## Validation

| Check | Result |
| --- | --- |
| Unchanged copy tests | 41 passed, 0 failed |
| Full Release solution tests | 44 passed, 0 failed |
| Release build with `-warnaserror` | Succeeded, 0 warnings and 0 errors |
| Golden master | Passed; `GoldenMasterTests.cs` SHA-256 matches baseline |
| Abstract Factory tests | 3 focused tests passed within the 44-test suite |
| S01 markers | Removed |
| S23 marker | Preserved in `TaxCalculator` |
| Root README link | Present and verified |
| `DIFF.md` generation | Repeated generation produced identical SHA-256 `7DB99C447870954473F88823DC88D98BC7283138EA1538AEDFED9BDFF07BCF80` |
| README and diff path list | 15 matching paths |
| Whitespace | `git diff --check` clean |

## Release summary

Unchanged-copy commit: `ff3f207`. Refactor commit: pending. Local tag: pending.
