<!-- markdownlint-disable-file -->
# Details: builder

Plan: `.copilot-tracking/plans/2026-10-15/builder-plan.instructions.md`
Artifact completion: 100%

## P01: Copy

* Generate the creational Builder copy and verify it before edits.
* Commit the unchanged copy.

## P02: Refactor and test

* Add fluent OrderBuilder and InvoiceBuilder APIs.
* Validate required identity/customer/line/quote/total inputs in Build.
* Preserve optional null defaults and replace only the two positional call sites.
* Test completed values, missing required values, and optional defaults.

## P03: Document and review

* Complete README/diff/map, verify root link, and record review.
* Run full Release validation, commit refactor, update progress, and tag locally.