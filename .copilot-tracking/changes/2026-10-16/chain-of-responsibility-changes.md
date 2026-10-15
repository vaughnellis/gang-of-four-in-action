<!-- markdownlint-disable-file -->
# Changes: chain-of-responsibility

## Summary

Completed the S13 refactor in the Chain of Responsibility copy. Validation now
runs through linked stock, address, fraud, and credit handlers in the original
order and preserves the existing first-failure messages and stock aggregation.

The unchanged-copy commit is `b74c7b0`.

## Changed files

* `patterns/behavioral/chain-of-responsibility/src/CornerStore/Checkout/OrderValidator.cs`
* `patterns/behavioral/chain-of-responsibility/src/CornerStore/Checkout/OrderValidationContext.cs`
* `patterns/behavioral/chain-of-responsibility/src/CornerStore/Checkout/OrderValidationHandler.cs`
* `patterns/behavioral/chain-of-responsibility/src/CornerStore/Checkout/StockValidationHandler.cs`
* `patterns/behavioral/chain-of-responsibility/src/CornerStore/Checkout/AddressValidationHandler.cs`
* `patterns/behavioral/chain-of-responsibility/src/CornerStore/Checkout/FraudValidationHandler.cs`
* `patterns/behavioral/chain-of-responsibility/src/CornerStore/Checkout/CreditLimitValidationHandler.cs`
* `patterns/behavioral/chain-of-responsibility/src/CornerStore/CornerStore.csproj`
* `patterns/behavioral/chain-of-responsibility/tests/CornerStore.Tests/ChainOfResponsibilityTests.cs`
* `patterns/behavioral/chain-of-responsibility/README.md`
* `patterns/behavioral/chain-of-responsibility/DIFF.md`
* `docs/pattern-map.md`

`CornerStore.csproj` adds `InternalsVisibleTo` for `CornerStore.Tests`. The
focused tests construct the internal `OrderValidator` directly to exercise
each validation failure and verify ordering without changing the app's public
API. This project metadata is needed for those tests and has no runtime effect.

## Validation

* Chain solution tests: 45 passed, 0 failed.
* Release build with warnings as errors: 0 warnings, 0 errors.
* Golden-master transcript: included in the Chain solution test run.
* Root README already links to Chain of Responsibility.

## Status

Review outcome is Conformant. The local refactor commit and tag are recorded in
`.copilot-tracking/run-all-progress.md`.
