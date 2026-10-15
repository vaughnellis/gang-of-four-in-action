<!-- markdownlint-disable-file -->
# Changes: composite

## Summary

Created and verified the unchanged Structural Composite copy in commit
`e840c73`. An attempted `ICatalogItem` refactor supports nested bundles, but
that changes a fixed golden-master transcript line. The refactor edits remain
uncommitted in the Composite folder; no tag was created.

## Current in-progress files

* `patterns/structural/composite/src/CornerStore/Catalog/ICatalogItem.cs`
* `patterns/structural/composite/src/CornerStore/Catalog/Product.cs`
* `patterns/structural/composite/src/CornerStore/Catalog/Bundle.cs`
* `patterns/structural/composite/src/CornerStore/Catalog/Catalog.cs`
* `patterns/structural/composite/src/CornerStore/Cart/CartService.cs`
* `patterns/structural/composite/tests/CornerStore.Tests/CatalogScenarioTests.cs`
* `patterns/structural/composite/tests/CornerStore.Tests/CompositeTests.cs`

## Validation

* Unchanged copy: 41 tests passed.
* Refactored attempt compiled; 43 of 44 tests passed.
* `GoldenMasterTests` failed at line 46: expected
  `rejected: BUN-RUN-KIT is a bundle and bundles cannot nest`, actual
  `added BUN-RUN-KIT to BUN-GIFT`.

## Blocker

The S08 map target asks for nested bundles, while the fixed scenario and
approved transcript require this exact nesting attempt to remain rejected.
Enabling nesting changes settled behavior; preserving the transcript leaves
the targeted behavior unchanged. Do not edit the scenario or approved
transcript. The Composite refactor, README, diff, map update, second commit,
and tag are waiting for scope direction.
