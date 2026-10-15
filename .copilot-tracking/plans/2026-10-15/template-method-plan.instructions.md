---
description: "Implementation plan for the Template Method task in the automatic run-all workflow"
applyTo: ".copilot-tracking/changes/2026-10-15/template-method-changes.md"
---
<!-- markdownlint-disable-file -->

# Plan: template-method

Task slug: `template-method`
Date: 2026-10-15
Artifact completion: 100%
Execution model: direct implementation; no subagents

## User requests

* Continue run-all after Builder in Batch B with Template Method.
* Refactor only S22, preserving S23.
* Continue through run-all scope until a blocking checkpoint.
* Keep workflow state in `.copilot-tracking/` files.

## Context and constraints

* `prompts/00-project-brief.md` maps S22 to duplicated report export workflow.
* `prompts/patterns/behavioral-template-method.md` is the generated task brief.
* `prompts/05-run-all.md` allows local commits/tags but forbids pushes.
* Preserve exact output, status filtering, and both S23 line-type switches.

## Execution checklist

* [x] P01: Generate unchanged copy, run 41 tests, and commit it as `c812e07`.
* [x] P02: Add the template algorithm and tests; preserve S23 hooks.
* [ ] P03: Complete README/diff/map, review, refactor commit, progress, and tag.

## Validation and success criteria

* Unchanged copy passes before edits.
* Full Release build succeeds with warnings as errors; all tests and golden master pass.
* S22 is absent and both S23 markers remain.
* `DIFF.md` matches the exact README source/test paths.
* Review is Conformant or Conformant with justified divergence before continuing.