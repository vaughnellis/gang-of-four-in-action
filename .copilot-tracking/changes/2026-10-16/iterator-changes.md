<!-- markdownlint-disable-file -->
# Changes: iterator

## Summary

Replaced S16's recursive category walks with an explicit depth-first category
iterator. Added a breadth-first iterator for level-order traversal. Existing
catalog operations use DFS, preserving category order, display indentation,
product counts, and imported SKU order.

## Files changed

* `patterns/behavioral/iterator/src/CornerStore/Catalog/BreadthFirstCategoryIterator.cs`
* `patterns/behavioral/iterator/src/CornerStore/Catalog/Catalog.cs`
* `patterns/behavioral/iterator/src/CornerStore/Catalog/DepthFirstCategoryIterator.cs`
* `patterns/behavioral/iterator/src/CornerStore/Catalog/ICategoryIterator.cs`
* `patterns/behavioral/iterator/src/CornerStore/CornerStore.csproj`
* `patterns/behavioral/iterator/tests/CornerStore.Tests/IteratorTests.cs`
* `patterns/behavioral/iterator/README.md`
* `patterns/behavioral/iterator/DIFF.md`
* `docs/pattern-map.md`
* The copy generator also registered the Iterator projects in
  `GangOfFour.slnx`.

## Validation

* Unchanged copy commit `102e099`: 41 tests passed.
* Final Release suite: 44 tests passed, including the golden master.
* Added tests verify DFS pre-order, BFS level order, sibling ordering, and
  traversal depth.
* `DIFF.md` regenerated twice with matching SHA-256
  `62E33341DE1EDC76F85D38DA8723DA2AA2C8D1F21DF986A978ED3BC423161B09`.
* README file list matches the six source/test entries in the generated diff.
* Root README link already existed.

## Commits and tag

* Copy: `102e099`.
* Refactor: pending.
* Tag: `iterator-v1` pending.
