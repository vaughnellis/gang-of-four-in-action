<!-- markdownlint-disable-file -->
# Critique: builder

Plan: `.copilot-tracking/plans/2026-10-15/builder-plan.instructions.md`
Artifact completion: 100%

## Assessment

Both aggregates have many constructor values, but optional fields should remain optional and default to null. Builders should make required values explicit without changing the resulting object data.

## Risks and mitigations

* Validate all required values in `Build()` and use clear exception messages.
* Keep optional note, promo, tracking, cancellation fields null unless explicitly supplied.
* Keep order line copying and invoice totals unchanged.
* Avoid changing S20 state transitions or unrelated constructors.

## Outcome

The plan is sufficient. Use fluent methods that map directly to domain properties and build the existing immutable/initialized objects.