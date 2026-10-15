<!-- markdownlint-disable-file -->
# Details: decorator

Plan: `.copilot-tracking/plans/2026-10-15/decorator-plan.instructions.md`
Artifact completion: 100%

## P01: Copy

* Generate the structural Decorator copy and verify the unchanged solution.
* Commit the unchanged copy.

## P02: Refactor and test

* Add `IAddOnPriceComponent`, a base component, and gift-wrap/insurance decorators.
* Compose both decorators using cart flags; pass discounted subtotal explicitly to insurance.
* Add tests for zero, individual, combined, and rounded discounted pricing.

## P03: Document and review

* Complete README and diff, verify index, mark S09 Done, and record review.
* Run full Release validation, commit refactor, update progress, and tag locally.