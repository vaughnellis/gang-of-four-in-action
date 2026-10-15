<!-- markdownlint-disable-file -->
# Changes: composite

## Summary

Refactored S08 with `ICatalogItem` implemented by products and bundles.
Catalog pricing uses the shared `Price` contract, and cart additions delegate
to the selected item. Bundles expand their component products into cart lines
with the existing discount and remainder allocation.

The catalog still rejects nested bundles. This is a justified divergence from
the full Composite flexibility: the approved golden-master transcript records
that rejection, and the project requires every pattern copy to preserve that
behavior. The refactor removes type-specific pricing and cart-expansion paths
without changing the established business rule.

## Files changed

* `patterns/structural/composite/README.md`
* `patterns/structural/composite/DIFF.md`
* `patterns/structural/composite/src/CornerStore/Cart/CartService.cs`
* `patterns/structural/composite/src/CornerStore/Catalog/Bundle.cs`
* `patterns/structural/composite/src/CornerStore/Catalog/Catalog.cs`
* `patterns/structural/composite/src/CornerStore/Catalog/ICatalogItem.cs`
* `patterns/structural/composite/src/CornerStore/Catalog/Product.cs`
* `patterns/structural/composite/tests/CornerStore.Tests/CompositeTests.cs`
* `docs/pattern-map.md`
* Root README link verified; no change was needed.

## Validation

* Unchanged copy commit `e840c73`: 41 tests passed.
* Final Composite Release suite: 44 tests passed, including the golden master.
* `DIFF.md` regenerated twice with matching SHA-256
  `B646357733DC40EF2936F7FBB9687832E794DB46813D76C1FEB163F46E421D2B`.
* No `CornerStore.csproj` change was required or made.

## Commits and tag

* Copy: `e840c73`.
* Refactor: `6e0bdd3`.
* Tag: `composite-v1`.
