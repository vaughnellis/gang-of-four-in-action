<!-- markdownlint-disable-file -->
# Critique: template-method

Plan: `.copilot-tracking/plans/2026-10-15/template-method-plan.instructions.md`
Artifact completion: 100%

## Assessment

The shared algorithm must stop before the type-specific aggregation switches so S23 remains a separate future seam. CSV and JSON should override the same two hooks while inheriting load/filter/iteration order.

## Risks and mitigations

* Keep output punctuation, newline placement, ordering, and invariant formatting exact.
* Preserve status filtering for `all` and named statuses.
* Keep both S23 switches in their current subclasses; do not centralize or replace them.
* Use a shared totals carrier only to communicate accumulated values to the writer.

## Outcome

The plan is sufficient. Use a small report totals object plus two abstract hooks: accumulate one line and write the document.