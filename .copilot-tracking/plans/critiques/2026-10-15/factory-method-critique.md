<!-- markdownlint-disable-file -->
# Critique: factory-method

Plan: `.copilot-tracking/plans/2026-10-15/factory-method-plan.instructions.md`
Artifact completion: 100%

## Assessment

The switch currently chooses both product and concrete type. A creator hierarchy can move product construction into overrides while leaving the payment-type choice visible in checkout.

## Risks and mitigations

* Keep the selection branch limited to choosing a creator; do not leave processor construction in the console.
* Preserve exact processor outputs and unknown payment rejection.
* Do not absorb checkout orchestration S10 or courier integration S06.
* Keep the creator API internal and test behavior through its charge operation.

## Outcome

The plan is sufficient. Use the smallest creator hierarchy that makes the construction override explicit.