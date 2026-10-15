<!-- markdownlint-disable-file -->
# Critique: composite

## Assessment

The abstraction must be used by lookup, pricing, bundle composition, and cart
expansion; only changing the data model would leave the S08 type branches.

## Risks and mitigations

* Preserve current rounding and last-component remainder allocation for
  one-level bundles.
* Define nested discount allocation so the final cart lines sum to the outer
  bundle price.
* Reject direct and indirect cycles before linking components.
* Do not change physical-product variants or category recursion.
* Update only the copied test that expects bundle nesting to be rejected.

## Outcome

Proceed with `ICatalogItem` on products and bundles, recursive component
pricing and expansion, and focused scenario tests for nested bundles.
