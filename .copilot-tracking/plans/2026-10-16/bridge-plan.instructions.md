---
description: "Implementation plan for Bridge in the automatic run-all workflow"
applyTo: ".copilot-tracking/changes/2026-10-16/bridge-changes.md"
---
<!-- markdownlint-disable-file -->

# Plan: bridge

Task slug: `bridge`
Date: 2026-10-16
Execution model: direct implementation

## User requests

* Resume the run-all queue in automatic mode.
* Complete Bridge and continue through the remaining patterns unless a brief checkpoint blocks progress.

## Context and constraints

* The project brief maps S07 to independent message and channel dimensions.
* The run-all brief requires separate copy and refactor commits, a local tag and progress updates. Do not push.
* The copied solution passed 41 tests; copy commit is `6f8c853`.
* Preserve the transcript, leave S02/S19/S20 and all other smells untouched, and verify `GoldenMasterTests.cs` remains identical.
* The root README already links Bridge; update only the Bridge row in `docs/pattern-map.md`.

## Execution checklist

* [x] Generate and verify the unchanged copy, then commit it as `6f8c853`.
* [x] Separate message abstractions from channel implementations and add focused composition tests.
* [x] Complete README and generated diff, update the map, validate, review, commit, tag and record progress.

## Validation and success criteria

* Release tests and the golden master pass.
* Release build succeeds with warnings as errors.
* Only S07 markers are removed; other markers remain.
* README Files changed paths match the generated `DIFF.md`.
* Review outcome is Conformant or Conformant with justified divergence before continuing.
