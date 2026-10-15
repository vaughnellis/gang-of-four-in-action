<!-- markdownlint-disable-file -->
# Details: factory-method

Plan: `.copilot-tracking/plans/2026-10-15/factory-method-plan.instructions.md`
Artifact completion: 100%

## P01: Copy

* Generate the creational Factory Method copy and verify the unchanged solution.
* Commit the unchanged copy before source changes.

## P02: Refactor and test

* Add an abstract payment processor creator with an overridable factory method.
* Add Card, PayPal, and Afterpay concrete creators.
* Make the console choose a creator and invoke its charge operation; preserve unknown-type behavior.
* Add creator tests for matching processor outputs and rejection behavior.

## P03: Document and review

* Complete README and DIFF, verify the index, mark S03 Done, and record review.
* Run full Release validation and invariants, commit refactor, update progress, and tag locally.