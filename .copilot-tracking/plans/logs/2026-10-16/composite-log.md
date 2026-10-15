<!-- markdownlint-disable-file -->
# Planning Log: composite

## Decisions

* `ICatalogItem` exposes pricing, children, and cart expansion so callers do
  not branch on Product versus Bundle.
* Nested bundles remain rejected because the approved transcript records that
  domain rule; changing it would violate the behavior-preservation decision.
* The Composite contract is still used for product and one-level bundle
  pricing and cart behavior.
* The copied S08 test continues to assert that nesting fails; no baseline test
  or golden-master file is changed.
