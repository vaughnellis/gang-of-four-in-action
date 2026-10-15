<!-- markdownlint-disable-file -->
# Details: template-method

Plan: `.copilot-tracking/plans/2026-10-15/template-method-plan.instructions.md`
Artifact completion: 100%

## P01: Copy

* Generate and validate the unchanged behavioral copy.
* Commit the unchanged copy before edits.

## P02: Refactor and test

* Add `ReportExporter` with the shared filter, traversal, and aggregate workflow.
* Add a totals carrier and abstract hooks for per-line accumulation and document output.
* Make CSV and JSON exporters inherit the base and preserve their S23 switches.
* Test filtering and format-specific behavior; preserve golden output.

## P03: Document and review

* Complete README/diff/map, verify index, and record review.
* Run full Release validation, commit refactor, update progress, and tag locally.