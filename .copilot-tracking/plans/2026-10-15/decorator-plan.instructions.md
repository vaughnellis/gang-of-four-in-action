---
description: "Implementation plan for the Decorator task in the automatic run-all workflow"
applyTo: ".copilot-tracking/changes/2026-10-15/decorator-changes.md"
---
<!-- markdownlint-disable-file -->

# Plan: decorator

Task slug: `decorator`
Date: 2026-10-15
Artifact completion: 100%
Execution model: direct implementation; no subagents

## User requests

* Continue run-all after Adapter with Decorator, the final Batch A task.
* Refactor only S09 and complete its review, commits, and tag.
* Continue remaining run-all scope until a blocking checkpoint.
* Keep workflow state in `.copilot-tracking/` files.

## Context and constraints

* `prompts/00-project-brief.md` maps S09 to boolean add-on pricing.
* `prompts/patterns/structural-decorator.md` is the generated task brief.
* `prompts/05-run-all.md` authorizes local commits and tags, not pushes.
* Preserve gift-wrap fixed cost, insurance rate and rounding, discounted-subtotal basis, and every non-S09 smell.

## Execution checklist

* [x] P01: Generate unchanged copy, run 41 tests, and commit it as `b5c83ac`.
* [x] P02: Add component/decorator types and focused pricing tests.
* [ ] P03: Complete README/diff/map, review, refactor commit, progress, and local tag.

## Validation and success criteria

* Unchanged copy passes before edits.
* Full Release build succeeds with warnings as errors; all tests and golden master pass.
* Only S09 is removed; diff summary matches README source/test paths.
* Insurance remains based on discounted subtotal when combined with gift wrap.
* Review is Conformant or Conformant with justified divergence before continuing.