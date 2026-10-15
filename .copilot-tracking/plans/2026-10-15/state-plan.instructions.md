---
description: "Implementation plan for the State task in the automatic run-all workflow"
applyTo: ".copilot-tracking/changes/2026-10-15/state-changes.md"
---
<!-- markdownlint-disable-file -->

# Plan: state

Task slug: `state`
Date: 2026-10-15
Artifact completion: 100%
Execution model: direct implementation; no subagents

## User requests

* Continue run-all in order after Batch A, starting with State.
* Refactor only S20 and complete its review, commits, and tag.
* Continue through remaining scope, stopping only at blocking checkpoints.
* Keep workflow state in `.copilot-tracking/` files.

## Context and constraints

* `prompts/00-project-brief.md` maps S20 to string-based lifecycle transitions.
* `prompts/patterns/behavioral-state.md` is the generated task brief.
* `prompts/05-run-all.md` permits local commits/tags but forbids pushing.
* Preserve all transition results, status output, side-effect order, and other smells, especially S19 observer-like calls in the baseline copy.

## Execution checklist

* [x] P01: Generate unchanged copy, run 41 tests, and commit it as `419ff5d`.
* [x] P02: Move transition decisions to state classes and add focused tests.
* [ ] P03: Complete README/diff/map, review, refactor commit, progress, and local tag.

## Validation and success criteria

* Unchanged copy passes before edits.
* Full Release build succeeds with warnings as errors; all tests and golden master pass.
* Only S20 is removed; `Order.Status` remains available to reporting and notifications.
* Invalid transitions preserve status and return the same error; valid transitions preserve stock and side effects.
* Review is Conformant or Conformant with justified divergence before continuing.