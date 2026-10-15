---
description: "Implementation plan for Memento in the automatic run-all workflow"
applyTo: ".copilot-tracking/changes/2026-10-16/memento-changes.md"
---
<!-- markdownlint-disable-file -->

# Plan: memento

Task slug: `memento`
Date: 2026-10-16
Execution model: direct implementation

## User requests

* Resume the run-all queue in automatic mode.
* Complete Memento and continue through the remaining patterns unless a brief checkpoint blocks progress.

## Context and constraints

* The project brief maps S18 to encapsulated cart-state snapshots managed by a caretaker.
* The run-all brief requires separate copy and refactor commits, a local tag and progress updates. Do not push.
* The copied solution passed 41 tests; copy commit is `b454ddc`.
* Preserve one-level undo, the transcript and every non-S18 smell.
* The root README already links Memento; update only its pattern-map row.

## Execution checklist

* [x] Generate and verify the unchanged copy, then commit it as `b454ddc`.
* [x] Capture and restore cart state through `CartMemento`; add focused snapshot tests.
* [x] Complete README and generated diff, update the map, validate, review, commit, tag and record progress.

## Validation and success criteria

* Release tests and the golden master pass.
* Release build succeeds with warnings as errors.
* Only S18 markers are removed; other markers remain.
* README Files changed paths match the generated `DIFF.md`.
* Review outcome is Conformant or Conformant with justified divergence before continuing.
