<!-- markdownlint-disable-file -->
# Planning Log: template-method

Plan: `.copilot-tracking/plans/2026-10-15/template-method-plan.instructions.md`
Artifact completion: 100%

## Decisions

* The base class owns load/filter/iteration and shared aggregation orchestration.
* Subclasses retain their own S23 switches in an accumulation hook and implement their own writer.
* Keep report totals internal to the template algorithm and preserve exact serialization.
* Use the documented diff-generator shim; do not alter frozen tooling.