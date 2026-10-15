---
description: "Implementation plan for Visitor in the automatic run-all workflow"
applyTo: ".copilot-tracking/changes/2026-10-16/visitor-changes.md"
---
<!-- markdownlint-disable-file -->

# Plan: visitor

Task slug: `visitor`
Date: 2026-10-16
Execution model: direct implementation

## User requests

* Resume the run-all queue in automatic mode.
* Complete Visitor and continue with the remaining patterns unless a brief checkpoint blocks progress.

## Context and constraints

* The project brief maps S23 to Visitor dispatch for product-specific tax, weight and report behavior.
* The run-all brief requires separate copy and refactor commits, a local tag and progress updates. Do not push.
* The unchanged copy passed 41 Release tests; copy commit is `d71dc3d`.
* Preserve the golden-master transcript and every non-S23 smell, including S22 in both report exporters.
* The root README already links Visitor; update only its pattern-map row.

## Execution checklist

* [x] Generate and verify the unchanged copy, then commit it as `d71dc3d`.
* [x] Replace S23 type switches with visitor dispatch and add focused tests.
* [x] Complete README and generated diff, update the map, validate, review, commit, tag and record progress.

## Validation and success criteria

* Release tests and golden master pass.
* Release build succeeds with warnings as errors.
* Only S23 markers are removed; other markers remain.
* README Files changed paths match the generated `DIFF.md`.
* Review outcome is Conformant or Conformant with justified divergence before continuing.
