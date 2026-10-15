<!-- markdownlint-disable-file -->
# Details: strategy

Plan: `.copilot-tracking/plans/2026-10-15/strategy-plan.instructions.md`
Artifact completion: 100%

## P01: Copy

* Generate `patterns/behavioral/strategy/` from the approved baseline.
* Run the copied solution tests before edits and commit the unchanged copy.

## P02: Refactor and test

* Introduce `IShippingCostStrategy` and implementations for standard, express, and pickup.
* Select strategies by the cart's shipping method through an injectable registry or resolver consumed by `PricingService`.
* Preserve `ShippingCalculator.RegionSurcharge` and unknown-method behavior.
* Add tests for method costs in AU/NZ quotes, correct method selection, and a test-only strategy.

## P03: Document and review

* Complete README with Strategy versus State, overkill warning, Mermaid diagram, and exact changed-file list.
* Verify the root README index link, mark S21 Done, generate `DIFF.md`, and record the template-lock recommendation.
* Run full Release build and tests, verify golden-master identity and markers, write changes/review artifacts, commit, update progress, and create a local tag.
* Fold Strategy's reusable pilot notes into `prompts/patterns/_template.md`; Facade notes were applied early to satisfy the Strategy brief dependency.