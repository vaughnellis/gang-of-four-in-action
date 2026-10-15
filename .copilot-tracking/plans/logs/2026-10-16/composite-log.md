<!-- markdownlint-disable-file -->
# Planning Log: composite

## Decisions

* `ICatalogItem` exposes pricing, children, and recursive cart expansion so
  callers do not branch on Product versus Bundle.
* Nested discount allocation preserves each inner bundle's distribution while
  ensuring all resulting cart lines sum to the outer bundle price.
* Direct or indirect component cycles are rejected.
* The copied S08 test that expected nesting to fail is replaced; no baseline
  test or golden-master file is changed.
