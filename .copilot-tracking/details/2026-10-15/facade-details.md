<!-- markdownlint-disable-file -->
# Details: facade

Plan: `.copilot-tracking/plans/2026-10-15/facade-plan.instructions.md`
Artifact completion: 100%

## P01: Copy

* Run `tools/new-pattern.ps1 -Category structural -Name facade`.
* Run the copied solution tests before editing: 41 passed.
* Commit the unchanged copy and umbrella registration as `ca016f9`.

## P02: Refactor and test

* Add `CheckoutFacade.PlaceOrder` with the existing quote, validation, payment, order, stock, shipment, and payment-completion sequence.
* Move the existing S03 payment switch and S06 courier XML helper without changing their behavior or markers.
* Make `StoreConsole.Checkout` find the cart and delegate once to the facade.
* Add `CheckoutFacadeTests` for success side effects, validation rejection, and console wiring.
* Validate the checkout slice and then the direct facade tests.

## P03: Document, diff, and review

* Complete the pattern README, including Facade versus Mediator, an appropriate-use warning, a Mermaid diagram, and the exact four source/test paths in Files changed.
* Verify the root README link and mark S10 Done in `docs/pattern-map.md`.
* Generate `DIFF.md`; the checked-in generator fails on scalar `Count` and indexing in both PowerShell editions. Use a process-local script-text shim; do not modify the frozen generator.
* Run the full Release solution tests, inspect diagnostics and markers, write the change and review records, commit the refactor, update run-all progress, and tag `facade-v1` locally.