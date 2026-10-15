<!-- markdownlint-disable-file -->
# Details: command

## P01: Copy

* Generate the Behavioral Command solution from the approved baseline.
* Run the unchanged Release tests and commit the copy before editing it.

## P02: Refactor and test

* Add an `ICommand` contract, four action commands, an invoker, and audit entries.
* Keep staff authorization in `BackOffice` and retain the duplicate stock read.
* Implement guarded stock-adjustment undo; reject undo for other commands.
* Test results, audit order, undo behavior, and unsupported undo.

## P03: Document and review

* Complete the pattern README and regenerate `DIFF.md`.
* Update the pattern map, verify the root index, run Release validation, review,
  commit, tag, and record the task in run-all progress.
