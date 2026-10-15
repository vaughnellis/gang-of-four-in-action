---
description: "Implementation plan for the Builder task in the automatic run-all workflow"
applyTo: ".copilot-tracking/changes/2026-10-15/builder-changes.md"
---
<!-- markdownlint-disable-file -->

# Plan: builder

Task slug: `builder`
Date: 2026-10-15
Artifact completion: 100%
Execution model: direct implementation; no subagents

## User requests

* Continue run-all after State with Builder in Batch B.
* Refactor only S02 and complete its review, commits, and tag.
* Continue until the run-all scope is complete or a blocking checkpoint occurs.
* Keep workflow state in `.copilot-tracking/` files.

## Context and constraints

* `prompts/00-project-brief.md` maps S02 to positional Order and Invoice construction.
* `prompts/patterns/creational-builder.md` is the generated task brief.
* `prompts/05-run-all.md` authorizes local commits and tags, not pushes.
* Preserve object values, optional null defaults, order IDs, invoice output, and all other smells.

## Execution checklist

* [x] P01: Generate unchanged copy, run 41 tests, and commit it as `2fcb8e5`.
* [x] P02: Add fluent builders and validation tests.
* [x] P03: Complete README/diff/map, review, refactor commit, progress, and local tag.

## Validation and success criteria

* Unchanged copy passes before edits.
* Full Release build succeeds with warnings as errors; all tests and golden master pass.
* Only S02 is removed; diff summary matches README source/test paths.
* Required values are validated and optional values preserve current defaults.
* Review is Conformant or Conformant with justified divergence before continuing.