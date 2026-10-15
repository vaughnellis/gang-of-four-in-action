<!-- markdownlint-disable-file -->
# Planning Log: command

## Decisions

* Keep the existing staff authorization branches in `BackOffice`.
* Command objects preserve all four existing result strings.
* Only stock adjustment is undoable because it has a local inverse operation.
* Lookup, refund, and cancellation explicitly reject undo; audit entries remain
  in memory and never affect the business transcript.
