<!-- markdownlint-disable-file -->
# Details: composite

## P01: Copy

* Generate the Structural Composite solution from the approved baseline.
* Run unchanged Release tests and commit the copy before editing it.

## P02: Refactor and test

* Add `ICatalogItem` and implement it on `Product` and `Bundle`.
* Change catalog pricing, component addition, and cart expansion to use the
  component contract instead of S08 type checks.
* Allocate nested bundle prices deterministically and guard against cycles.
* Replace the old nested-bundle rejection test and add focused scenarios.

## P03: Document and review

* Complete the pattern README and regenerate `DIFF.md`.
* Update the pattern map, verify the root index, run Release validation, review,
  commit, tag, and record the task in run-all progress.
