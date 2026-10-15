<!-- markdownlint-disable-file -->
# Planning Log: decorator

Plan: `.copilot-tracking/plans/2026-10-15/decorator-plan.instructions.md`
Artifact completion: 100%

## Decisions

* Decorators wrap a zero-cost base add-on component and each contributes its own optional cost.
* Insurance receives the post-discount subtotal separately so gift-wrap cost is excluded from the insurance calculation.
* Cart boolean flags remain unchanged because S09's targeted seam is the pricing if-chain.
* Do not edit baseline tooling; generate the diff with the documented local shim and normalize output whitespace only.