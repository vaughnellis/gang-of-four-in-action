---
description: "Implementation plan for the Observer pattern in the automatic run-all workflow"
applyTo: ".copilot-tracking/changes/2026-10-15/observer-changes.md"
---
<!-- markdownlint-disable-file -->

# Plan: observer

Task slug: `observer`
Date: 2026-10-15
Artifact completion: 100%
Execution model: direct implementation; no subagents

## User requests

* Continue run-all in its documented order after Strategy.
* Implement Observer as a full copied solution fixing only S19.
* Continue until the run-all scope is complete, stopping only at its blocking checkpoints.
* Keep workflow state in `.copilot-tracking/` files.

## Context and constraints

* `prompts/00-project-brief.md` maps S19 to Observer and defines copy, naming, determinism, and golden-master boundaries.
* `prompts/patterns/behavioral-observer.md` is the generated task brief for this task.
* `prompts/patterns/_template.md` is locked after both pilot reviews.
* `prompts/05-run-all.md` authorizes local pattern commits and tags; never push.
* `.github/copilot-instructions.md` and `.github/instructions/patterns.instructions.md` require one-smell-only changes and unchanged project/namespace names.
* Keep all non-S19 behavior, including the order of existing status-change side effects.

## Execution checklist

* [x] P01: Copy baseline, run 41 tests, and commit unchanged copy as `8470705`.
* [x] P02: Publish status changes to registered observers and add behavior tests.
* [x] P03: Complete docs/diff/map, review, commits, progress row, and local tag.

## Validation and success criteria

* Unchanged copy passes all tests before edits.
* Final Release build succeeds with warnings as errors and all tests pass.
* Golden master is unchanged and passes.
* `DIFF.md` matches README Files changed; only S19 is removed.
* Observer fan-out is extensible and invalid status transitions do not notify.
* Review is Conformant or Conformant with justified divergence before continuing.