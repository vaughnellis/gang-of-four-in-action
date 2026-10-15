---
description: "Implementation plan for the Strategy pilot in the automatic run-all workflow"
applyTo: ".copilot-tracking/changes/2026-10-15/strategy-changes.md"
---
<!-- markdownlint-disable-file -->

# Plan: strategy

Task slug: `strategy`
Date: 2026-10-15
Artifact completion: 100%
Execution model: direct implementation; no subagents

## User requests

* Continue the run-all workflow after Facade, starting with Strategy.
* Continue through scope and stop only at run-all blocking checkpoints.
* Keep workflow state in `.copilot-tracking/` files.

## Context and constraints

* `prompts/00-project-brief.md` maps S21 to Strategy and defines behavior, copy, and naming constraints.
* `prompts/04-pilot-strategy.md` requires one strategy per shipping method, representative AU/NZ behavior, extensibility tests, README, diff, map/index, and pilot recommendation.
* `prompts/05-run-all.md` requires the two-commit pattern workflow and permits local commits/tags but no push.
* Facade is reviewed, committed, tagged, and recorded Done. Its reusable template notes were applied before Strategy to satisfy this brief's dependency; the run-all order deviation is recorded in the planning log and will be reconciled after Strategy review.
* Keep S01 and S09 plus every non-S21 smell intact. Do not change baseline or scenario files.

## Execution checklist

* [x] P01: Generate the unchanged Strategy copy, run its tests, and commit it as `da5088a`.
* [x] P02: Replace the shipping-cost switch with pluggable `IShippingCostStrategy` implementations and add the required tests.
* [x] P03: Complete README, diff, map/index verification, pilot recommendation, full validation, review, refactor commit, progress update, and local tag.

## Validation and success criteria

* Unchanged copy passes all baseline-copy tests.
* Release build succeeds with warnings as errors and all Strategy tests pass.
* Golden master remains byte-identical and passes.
* `DIFF.md` matches the exact source/test paths in README Files changed.
* Only S21 is removed; strategy selection remains extensible to a test-only implementation.
* Review is Conformant or Conformant with justified divergence before continuing.