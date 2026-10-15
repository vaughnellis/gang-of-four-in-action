<!-- markdownlint-disable-file -->
# Critique: iterator

## Assessment

The iterators must preserve the current depth-first pre-order walk because
both the browse transcript and bulk-import SKU sequence depend on it. The
breadth-first iterator is an additional pattern capability and should have an
independent ordering test.

## Risks and mitigations

* Preserve child order by pushing DFS children in reverse order.
* Track depth in traversal state so browsing retains exact indentation.
* Leave category storage, product ordering, and other smell code unchanged.
* Add internal test visibility rather than exposing traversal types publicly.

## Outcome

Implement explicit enumerator participants and exercise both traversal
orders, then run the golden master.
