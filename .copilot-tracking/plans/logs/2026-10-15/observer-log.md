<!-- markdownlint-disable-file -->
# Planning Log: observer

Plan: `.copilot-tracking/plans/2026-10-15/observer-plan.instructions.md`
Artifact completion: 100%

## Decisions

* Use an ordered observer list so current transcript ordering remains deterministic.
* Publish the mutated order after successful transitions only; invalid transitions return before notification.
* Compose built-in observers at the `StoreConsole` boundary, keeping `OrderService` focused on order lifecycle and notification dispatch.
* Do not alter the baseline diff tool. Use the pilot-documented process-local workaround and clean generated whitespace if needed.