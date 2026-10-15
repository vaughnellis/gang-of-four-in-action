<!-- markdownlint-disable-file -->
# Critique: decorator

Plan: `.copilot-tracking/plans/2026-10-15/decorator-plan.instructions.md`
Artifact completion: 100%

## Assessment

The key behavior is not merely addition: insurance is rounded from the post-discount subtotal, while gift wrap is a separate fixed charge. Keep the insurance basis explicit in the decorator so wrapping order does not change pricing.

## Risks and mitigations

* Compose wrappers for both enabled and disabled flags so pricing contains no add-on cost if-chain.
* Preserve the existing two-decimal midpoint rounding and apply insurance only to the discounted subtotal.
* Leave cart flag storage and unrelated S18/S21 code untouched.
* Confirm the root link before editing and use the documented diff workaround.

## Outcome

The plan is sufficient. A small `IAddOnPriceComponent` with explicit wrappers is adequate for this teaching example.