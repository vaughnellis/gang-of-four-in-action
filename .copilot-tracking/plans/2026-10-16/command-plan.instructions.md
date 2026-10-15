---
description: "Implementation plan for the Command task in the automatic run-all workflow"
applyTo: ".copilot-tracking/changes/2026-10-16/command-changes.md"
---
<!-- markdownlint-disable-file -->

# Plan: command

Task slug: `command`
Date: 2026-10-16
Execution model: direct implementation

## User requests

* Continue run-all with Command, then continue through the remaining queue.
* Complete the unchanged-copy and refactor commits, review, tag, and progress row.
* Preserve all behavior outside the S14 command seam.

## Context and constraints

* `prompts/00-project-brief.md` maps S14 to inline back-office actions.
* `prompts/patterns/behavioral-command.md` defines the detailed task.
* Keep staff checks and the duplicate inventory lookup unchanged as S12.
* Keep command audit data out of the golden-master transcript.
* Undo stock adjustments; explicitly reject unsupported undo for read-only or
  externally observable actions.

## Execution checklist

* [x] P01: Generate and verify the unchanged copy, then commit it.
* [x] P02: Add command objects, invoker, audit log, and focused tests.
* [x] P03: Complete README, diff, map, review, refactor commit, progress, and tag.

## Validation and success criteria

* Unchanged-copy tests and golden master pass before edits.
* Release build succeeds with warnings as errors; focused and golden-master
  tests pass after edits.
* Only S14 is removed. S12 code and markers remain unchanged.
* README file list matches `DIFF.md`, and the root README link exists.
* Review is Conformant or Conformant with justified divergence.
