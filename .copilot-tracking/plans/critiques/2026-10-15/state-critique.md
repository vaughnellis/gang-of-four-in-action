<!-- markdownlint-disable-file -->
# Critique: state

Plan: `.copilot-tracking/plans/2026-10-15/state-plan.instructions.md`
Artifact completion: 100%

## Assessment

Status is consumed by reports and notification logic, so retain its string projection while letting an internal state object own the transition rules. This isolates S20 without expanding into S19 side effects.

## Risks and mitigations

* Preserve the exact error text for invalid transitions.
* Keep the order of state assignment, tracking-number assignment, stock restoration, and status-change side effects.
* Do not move notification or inventory callbacks into state classes; they remain the S19 seam.
* Keep the eleven-argument `Order` constructor smell S02 intact except for the status parameter's representation if required.

## Outcome

The plan is sufficient. Concrete states can call narrow `OrderService` transition helpers for shared side effects.