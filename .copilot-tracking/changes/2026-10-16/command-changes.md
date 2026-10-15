<!-- markdownlint-disable-file -->
# Changes: command

## Summary

Refactored S14 into four command classes dispatched by `CommandInvoker`.
Execution results are captured in an in-memory audit log. Stock adjustments
support undo; lookup, refund, and cancellation reject undo where no safe
compensation exists.

The unchanged-copy commit is `72156d3`.

## Changed files

* `patterns/behavioral/command/src/CornerStore/Orders/BackOffice.cs`
* `patterns/behavioral/command/src/CornerStore/Orders/ICommand.cs`
* `patterns/behavioral/command/src/CornerStore/Orders/CommandInvoker.cs`
* `patterns/behavioral/command/src/CornerStore/Orders/CommandAuditEntry.cs`
* `patterns/behavioral/command/src/CornerStore/Orders/AdjustStockCommand.cs`
* `patterns/behavioral/command/src/CornerStore/Orders/LookupStockCommand.cs`
* `patterns/behavioral/command/src/CornerStore/Orders/RefundCommand.cs`
* `patterns/behavioral/command/src/CornerStore/Orders/CancelCommand.cs`
* `patterns/behavioral/command/src/CornerStore/CornerStore.csproj`
* `patterns/behavioral/command/tests/CornerStore.Tests/CommandTests.cs`
* `patterns/behavioral/command/README.md`
* `patterns/behavioral/command/DIFF.md`
* `docs/pattern-map.md`

`CornerStore.csproj` adds `InternalsVisibleTo` for `CornerStore.Tests` so the
focused tests can exercise internal command and back-office types without
making them part of the application API. It has no runtime effect.

## Validation

* Unchanged copy: 41 tests passed.
* Refactored solution: 44 tests passed, 0 failed.
* Release build with warnings as errors: 0 warnings, 0 errors.
* Golden master passed; `GoldenMasterTests.cs` remains unchanged.
* Command `DIFF.md` lists only files declared in the README.
* Root README already links to Command.

## Status

Review is Conformant with justified divergence. The local refactor commit and
tag are recorded in `.copilot-tracking/run-all-progress.md`.
