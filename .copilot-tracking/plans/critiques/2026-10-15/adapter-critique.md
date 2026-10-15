<!-- markdownlint-disable-file -->
# Critique: adapter

Plan: `.copilot-tracking/plans/2026-10-15/adapter-plan.instructions.md`
Artifact completion: 100%

## Assessment

The console owns both domain checkout and the courier's wire format. A provider interface with one ShipFast adapter creates the intended boundary without changing the existing fake SDK.

## Risks and mitigations

* Escape recipient and address text before building XML; special-character coverage must prove the request remains parseable.
* Preserve cents rounding, tracking extraction, and exact ShipFast transcript text.
* Keep pricing, checkout orchestration, and payment construction outside this refactor.
* Verify the pre-existing root README link rather than editing it redundantly.

## Outcome

The plan is sufficient. The adapter should depend on the existing client and expose a domain-level shipment operation.