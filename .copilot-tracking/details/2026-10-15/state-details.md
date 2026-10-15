<!-- markdownlint-disable-file -->
# Details: state

Plan: `.copilot-tracking/plans/2026-10-15/state-plan.instructions.md`
Artifact completion: 100%

## P01: Copy

* Generate and verify the unchanged behavioral State copy.
* Commit the unchanged copy before implementation.

## P02: Refactor and test

* Add an abstract state contract and concrete classes for each order lifecycle state.
* Make `OrderService` delegate transitions to the current state and retain shared status-change side effects.
* Expose `Order.Status` as the current state's existing display name.
* Test the valid lifecycle, invalid transition stability, and stock restoration.

## P03: Document and review

* Complete README and generated diff, verify index, mark S20 Done, and record review.
* Run full Release validation, commit refactor, update progress, and tag locally.