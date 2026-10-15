<!-- markdownlint-disable-file -->
# Interpreter implementation plan

## User request

Resume the automatic run-all task from `.copilot-tracking/run-all-progress.md`. Interpreter is the sole remaining pattern. Complete its S15 refactor, review and validate it, make the unchanged-copy and refactor commits, tag it, update the pattern map and progress record, and stop at any blocking checkpoint.

## Context

* Shared constraints and pattern map: `prompts/00-project-brief.md`, sections 6 to 10.
* Queue, commit/tag permission and checkpoints: `prompts/05-run-all.md`.
* Current status: every task through Flyweight is Done; Interpreter is Not started.
* Pattern-specific rules: `.github/instructions/patterns.instructions.md`.
* Baseline rules: `.github/instructions/baseline.instructions.md`.
* C# implementation and test guidance: `csharp.instructions.md` and `csharp-tests.instructions.md`.
* The baseline rule language has four predicate forms and conjunctions only. Preserve strict greater-than semantics, summed quantity, any-line category matching, and exact region matching.

## Approach

1. [x] Create the Interpreter pattern brief, plan and critique, then generate the unchanged copy.
2. [x] Run the copied solution's Release tests and commit the unchanged copy separately as `0cd28fb`.
3. [x] Add a small recursive-descent parser and expression tree; parse configured promo rules once and evaluate them against a context containing cart, customer and subtotal.
4. [x] Add focused tests for conjunction, each predicate, strict thresholds, quantity aggregation and invalid syntax. Grant test visibility rather than widening production types.
5. [x] Complete the README and regenerate `DIFF.md`; verify all listed files, the S15-only change, unchanged golden-master test and transcript, and all non-S15 markers.
6. [x] Run targeted tests and a Release build, review the final diff, update the map, commit the refactor as `71d1cea`, tag `interpreter-v1`, and record the finished task in progress.

## Success criteria

* The copied solution passes tests and golden master before implementation.
* The Interpreter suite passes and its Release build has no warnings or errors.
* Existing promo rule strings and all scenario output remain byte-for-byte unchanged.
* S15 is removed only from the Interpreter copy; all other markers match baseline.
* `DIFF.md` and README's Files changed list agree.
* Both pattern commits, the local tag, review and run-all progress are recorded. No push is made.

## Dependencies

The implementation follows the unchanged-copy test and commit. README and diff generation depend on the final code and test edits. Review, tag and progress completion follow successful validation.
