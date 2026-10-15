<!-- markdownlint-disable-file -->
# Details: observer

Plan: `.copilot-tracking/plans/2026-10-15/observer-plan.instructions.md`
Artifact completion: 100%

## P01: Copy

* Generate the behavioral Observer copy from the approved baseline.
* Run the unchanged solution tests and commit the copy.

## P02: Refactor and test

* Add an internal observer contract and let `OrderService` register and notify observers after valid transitions.
* Move inventory lookup, notifications, loyalty, and analytics into observers composed in `StoreConsole`.
* Preserve the exact notification order and existing transcript.
* Test registration, side effects, a test-only observer, and invalid-transition silence.

## P03: Document and review

* Complete README and generated diff, verify the root index link, mark S19 Done, and record review findings.
* Run full Release build/tests and invariant checks, commit refactor, update tracking progress, and tag locally.