<!-- markdownlint-disable-file -->
# Details: adapter

Plan: `.copilot-tracking/plans/2026-10-15/adapter-plan.instructions.md`
Artifact completion: 100%

## P01: Copy

* Generate the structural Adapter copy and verify it before edits.
* Commit the unchanged copy.

## P02: Refactor and test

* Add `IShippingProvider` and `ShipFastAdapter`.
* Move XML creation, cents conversion, client invocation, and response parsing into the adapter.
* Route the console shipment operation through the interface.
* Test request compatibility, XML escaping, and tracking extraction.

## P03: Document and review

* Complete README and generated diff, verify root link, mark S06 Done, and record review.
* Run full Release validation, commit refactor, update progress, and create a local tag.