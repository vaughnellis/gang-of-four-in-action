<!-- markdownlint-disable-file -->
# Planning Log: builder

Plan: `.copilot-tracking/plans/2026-10-15/builder-plan.instructions.md`
Artifact completion: 100%

## Decisions

* Builder methods correspond to named Order/Invoice properties.
* Required values are nullable builder state until validated in `Build()`.
* Optional arguments preserve null defaults.
* Do not edit baseline tooling; use the diff-generator workaround already recorded in pilot changes.