---
description: "Implementation plan for the Chain of Responsibility task in the automatic run-all workflow"
applyTo: ".copilot-tracking/changes/2026-10-16/chain-of-responsibility-changes.md"
---
<!-- markdownlint-disable-file -->

# Plan: chain-of-responsibility

Task slug: `chain-of-responsibility`
Date: 2026-10-15
Artifact completion: 100%
Execution model: direct implementation; no subagents

## User requests

* Continue run-all after Template Method with Chain of Responsibility in Batch B.
* Refactor only S13 and complete its review, commits, and tag.
* Continue through scope until a blocking checkpoint.
* Keep workflow state in `.copilot-tracking/` files.

## Context and constraints

* `prompts/00-project-brief.md` maps S13 to the combined checkout validator.
* `prompts/patterns/behavioral-chain-of-responsibility.md` is the generated task brief.
* `prompts/05-run-all.md` authorizes local commits/tags but forbids pushing.
* Preserve check order, exact error strings, SKU aggregation/sorting, and all other smells.

## Execution checklist

* [x] P01: Generate unchanged copy, run tests, and commit it.
* [x] P02: Add four linked handlers and focused tests.
* [x] P03: Complete README/diff/map, review, refactor commit, progress, and local tag.

## Validation and success criteria

* Unchanged copy passes before edits.
* Full Release build succeeds with warnings as errors; all tests and golden master pass.
* Only S13 is removed; `DIFF.md` matches README source/test paths.
* Each handler short-circuits in the existing order with unchanged messages.
* Review is Conformant or Conformant with justified divergence before continuing.