---
description: "Implementation plan for the Facade pilot in the automatic run-all workflow"
applyTo: ".copilot-tracking/changes/2026-10-15/facade-changes.md"
---
<!-- markdownlint-disable-file -->

# Plan: facade

Task slug: `facade`
Date: 2026-10-15
Artifact completion: 100%
Execution model: direct implementation; no subagents

## User requests

* Continue run-all from the progress file at the Facade pilot.
* Treat the human approval of `baseline-v1` as clearing the baseline checkpoint.
* Continue through run-all scope, stopping only at its blocking checkpoints.
* Keep all workflow state in `.copilot-tracking/` files.

## Context and constraints

* `prompts/00-project-brief.md`, sections 6 to 10, defines the S10 seam and acceptance criteria.
* `prompts/03-pilot-facade.md` requires an unchanged-copy commit, facade tests, documentation, `DIFF.md`, map status, and pilot notes.
* `prompts/05-run-all.md` replaces individual stop points and permits local commits and pattern tags during this run. Pushing remains forbidden.
* `.github/copilot-instructions.md` and `.github/instructions/patterns.instructions.md` require one smell per copy, unchanged names, and no unrelated refactors.
* Keep S02, S03, S06, S13, and all non-S10 behavior unchanged. Baseline and scenario files remain frozen.

## Execution checklist

* [x] P01: Generate the copy, run all 41 baseline-copy tests, and commit unchanged copy `ca016f9`.
* [x] P02: Move checkout sequencing to `CheckoutFacade`, preserve adjacent smells, and add direct facade tests.
* [x] P03: Complete README, source/test diff, pattern map, pilot notes, validation, review, refactor commit, progress update, and tag.

## Validation and success criteria

* The copy passes the baseline tests before edits.
* The final pattern solution builds in Release with warnings as errors and all tests pass.
* `GoldenMasterTests.cs` is unchanged and the golden master passes.
* `DIFF.md` lists only the exact files declared in the pattern README.
* Only S10 is removed; project and namespace names remain unchanged.
* Review is Conformant or Conformant with justified divergence before proceeding.