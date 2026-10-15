<!-- markdownlint-disable-file -->
# Critique: composite

## Assessment

The abstraction must be used by lookup, pricing, and cart expansion; only
changing the data model would leave the S08 type branches. The component
contract must not change the established nested-bundle behavior.

## Risks and mitigations

* Preserve current rounding and last-component remainder allocation for
  one-level bundles.
* Preserve rounding and last-component remainder allocation for current
  one-level bundles.
* Do not change physical-product variants or category recursion.
* Retain the copied test that expects bundle nesting to be rejected.

## Outcome

Proceed with `ICatalogItem` on products and bundles, shared pricing and cart
dispatch, and focused tests that prove the preserved behavior.
