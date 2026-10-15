---
description: "Implementation plan for the Composite task in the automatic run-all workflow"
applyTo: ".copilot-tracking/changes/2026-10-16/composite-changes.md"
---
<!-- markdownlint-disable-file -->

# Plan: composite

Task slug: `composite`
Date: 2026-10-16
Execution model: direct implementation

## User requests

* Continue run-all with Composite and then continue through the remaining queue.
* Complete the copy/refactor commits, review, tag, and progress row.
* Refactor only S08 and preserve the golden-master transcript.

## Context and constraints

* `prompts/00-project-brief.md` maps S08 to bundle pricing and cart type checks.
* The Composite target is the S08 description in
  `prompts/00-project-brief.md`.
* Keep S04 variant construction and S16 category recursion unchanged.
* Preserve simple-product and existing one-level bundle pricing and cart lines.
* Preserve the approved transcript by retaining the nested-bundle rejection.

## Execution checklist

* [x] P01: Generate and verify the unchanged copy, then commit it.
* [x] P02: Implement the shared component contract and cart behavior while retaining the nesting rule.
* [x] P03: Complete documentation, map, diff, review, refactor commit, progress, and tag.

## Validation and success criteria

* Unchanged-copy tests and golden master pass before edits.
* Release build succeeds with warnings as errors; all tests and golden master
  pass after edits.
* Only S08 changes; all other smell markers and code remain untouched.
* README file list matches the generated diff and root README link exists.
* Review is Conformant or Conformant with justified divergence.
