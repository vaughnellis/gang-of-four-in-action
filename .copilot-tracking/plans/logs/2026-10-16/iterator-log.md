<!-- markdownlint-disable-file -->
# Planning Log: iterator

## Decisions

* Use an explicit `ICategoryIterator` over `IEnumerator<Category>` with depth
  available for existing browse indentation.
* Implement DFS with a stack and BFS with a queue, preserving child order.
* Keep Catalog clients on DFS to preserve the established output and SKU order.
* Use the existing `InternalsVisibleTo` project convention for focused tests.
