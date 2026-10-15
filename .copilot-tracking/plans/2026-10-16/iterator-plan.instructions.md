---
description: "Implementation plan for the Iterator task in the automatic run-all workflow"
applyTo: ".copilot-tracking/changes/2026-10-16/iterator-changes.md"
---
<!-- markdownlint-disable-file -->

# Plan: iterator

Task slug: `iterator`
Date: 2026-10-16
Execution model: direct implementation

## User requests

* Continue the ordered run-all queue after Composite.
* Complete Iterator as an unchanged copy followed by a refactor, review, tag,
  and progress update.
* Refactor only S16 and preserve the golden-master transcript.

## Context and constraints

* `prompts/00-project-brief.md` maps S16 to depth-first and breadth-first
  category iterators.
* The copied category tree is traversed pre-order in `Catalog.PrintTree`,
  `CountProducts`, and `CollectSkus`.
* Keep all other smell markers and their code unchanged.

## Execution checklist

* [x] P01: Generate and verify the unchanged copy; commit `102e099`.
* [x] P02: Implement depth-first and breadth-first iterators and focused tests.
* [x] P03: Complete README, diff, map, review, refactor commit, progress, and tag.

## Validation and success criteria

* Unchanged-copy Release suite passed (41 tests).
* Final Release build and all tests pass, including the golden master.
* Existing traversal output and collected SKU order are unchanged.
* Only S16 changes; the README file list matches `DIFF.md`.
