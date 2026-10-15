<!-- markdownlint-disable-file -->
# Changes: visitor

Plan: `.copilot-tracking/plans/2026-10-16/visitor-plan.instructions.md`
Critique: `.copilot-tracking/plans/critiques/2026-10-16/visitor-critique.md`
Review: `.copilot-tracking/reviews/2026-10-16/visitor-review.md`
Implementation date: 2026-10-16

## Summary

Replaced S23 product type switches with `ILineItemVisitor` dispatch. `TaxVisitor`, `WeightVisitor` and `ReportTotalsVisitor` hold each operation's product-specific behavior. The product classes now dispatch through `Accept`; the calculators and both report exporters use visitors. S22's separate report workflows and the approved transcript remain unchanged.

The `CornerStore.csproj` change grants `CornerStore.Tests` internal visibility so focused tests can exercise internal visitor participants without making them public.

## Files added or modified

* `patterns/behavioral/visitor/src/CornerStore/Catalog/ILineItemVisitor.cs`
* `patterns/behavioral/visitor/src/CornerStore/Catalog/Product.cs`
* `patterns/behavioral/visitor/src/CornerStore/Catalog/PhysicalProduct.cs`
* `patterns/behavioral/visitor/src/CornerStore/Catalog/DigitalProduct.cs`
* `patterns/behavioral/visitor/src/CornerStore/Catalog/GiftCardProduct.cs`
* `patterns/behavioral/visitor/src/CornerStore/CornerStore.csproj`
* `patterns/behavioral/visitor/src/CornerStore/Pricing/TaxCalculator.cs`
* `patterns/behavioral/visitor/src/CornerStore/Pricing/WeightCalculator.cs`
* `patterns/behavioral/visitor/src/CornerStore/Pricing/TaxVisitor.cs`
* `patterns/behavioral/visitor/src/CornerStore/Pricing/WeightVisitor.cs`
* `patterns/behavioral/visitor/src/CornerStore/Reporting/CsvReportExporter.cs`
* `patterns/behavioral/visitor/src/CornerStore/Reporting/JsonReportExporter.cs`
* `patterns/behavioral/visitor/src/CornerStore/Reporting/ReportTotalsVisitor.cs`
* `patterns/behavioral/visitor/tests/CornerStore.Tests/VisitorTests.cs`
* `patterns/behavioral/visitor/README.md`
* `patterns/behavioral/visitor/DIFF.md`
* `GangOfFour.slnx`
* `docs/pattern-map.md`
* `prompts/patterns/behavioral-visitor.md`

The root solution update was made by the required copy generator, consistent with prior pattern copy commits. The root README already contained the Visitor link and was left unchanged. `DIFF.md` contains exactly the 14 source and test paths listed under Files changed in the pattern README.

## Validation

| Check | Result |
| --- | --- |
| Unchanged-copy suite | 41 passed, 0 failed |
| Final Release solution tests | 44 passed, 0 failed |
| Release build with warnings as errors | Succeeded, 0 warnings and 0 errors |
| Golden master | Passed in the full suite; `GoldenMasterTests.cs` SHA-256 matches baseline |
| Focused Visitor tests | 3 tests passed, covering tax dispatch, quantity-based weight, and totals aggregation |
| S23 and other smell markers | S23 removed; all other marker IDs and counts match baseline |
| Root README link | Present and verified |
| `DIFF.md` generation | Two successive runs produced identical SHA-256 `1CF7DB3732B90BBB8E730160ECCA6E184237ACE176686DCFDD3A47436EA36CE5` |
| README and diff path list | 14 matching paths |
| Whitespace | `git diff --check` clean |

The generated diff reports 164 additions and 62 removals across 14 files.

## Release summary

Unchanged-copy commit: `d71dc3d`. Refactor commit: `2b4f7f1`. Local tag: `visitor-v1` at `2b4f7f1`. No push was made.
