<!-- markdownlint-disable-file -->
# Details: chain-of-responsibility

Plan: `.copilot-tracking/plans/2026-10-15/chain-of-responsibility-plan.instructions.md`
Artifact completion: 100%

## P01: Copy

* Generate and test the unchanged behavioral copy.
* Commit the copy before edits.

## P02: Refactor and test

* Add a validation context and linked handler base.
* Implement stock, address, fraud, and credit handlers in the existing order.
* Compose the chain in `OrderValidator` and delegate validation.
* Test short-circuiting, stock aggregation, and each existing rejection message.

## P03: Document and review

* Complete README/diff/map, verify root link, and record review.
* Run full Release validation, commit refactor, update progress, and tag locally.