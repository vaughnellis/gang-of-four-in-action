<!-- markdownlint-disable-file -->
# Planning Log: chain-of-responsibility

Plan: `.copilot-tracking/plans/2026-10-15/chain-of-responsibility-plan.instructions.md`
Artifact completion: 100%

## Decisions

* Handler order is fixed: stock, address, fraud, then credit.
* Each handler short-circuits by returning the first error; no later handler runs after failure.
* Stock validation retains grouped SKU quantities and ordinal SKU ordering.
* Exercise FraudScorer deterministically by setting a threshold that guarantees pass/fail in isolated tests and restoring Config after each test.
* Do not modify the frozen diff tool; use the established local shim.