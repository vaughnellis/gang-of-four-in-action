<!-- markdownlint-disable-file -->
# Critique: observer

Plan: `.copilot-tracking/plans/2026-10-15/observer-plan.instructions.md`
Artifact completion: 100%

## Assessment

The owning behavior is `OrderService.RecordStatusChange`. Publishing the same order status to an ordered observer list lets reactions evolve without changing transition logic.

## Risks and mitigations

* Preserve the existing side-effect order because it is observable in the golden transcript.
* Notify only after a valid state transition, and keep S20 transition conditions untouched.
* Keep notifications and other reactions in separate observers rather than growing the subject.
* Avoid editing the baseline; the diff generator workaround is already documented by the pilots.

## Outcome

The plan is sufficient. Use an internal observer contract and keep concrete observer types internal.