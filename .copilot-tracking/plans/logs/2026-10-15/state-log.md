<!-- markdownlint-disable-file -->
# Planning Log: state

Plan: `.copilot-tracking/plans/2026-10-15/state-plan.instructions.md`
Artifact completion: 100%

## Decisions

* Use state objects for transition rules and retain `Order.Status` as a string projection for existing readers.
* State classes may request status-change side effects from `OrderService`, but do not own S19 collaborators.
* Preserve exact invalid-transition error text and operation ordering.
* Use the documented process-local diff-generator workaround without editing baseline tooling.