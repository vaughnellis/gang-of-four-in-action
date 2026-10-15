---
description: "Implementation plan for the Adapter task in the automatic run-all workflow"
applyTo: ".copilot-tracking/changes/2026-10-15/adapter-changes.md"
---
<!-- markdownlint-disable-file -->

# Plan: adapter

Task slug: `adapter`
Date: 2026-10-15
Artifact completion: 100%
Execution model: direct implementation; no subagents

## User requests

* Continue run-all in order after Factory Method.
* Refactor only S06 with Adapter and complete its review, commits, and tag.
* Continue the run-all scope until a brief blocking checkpoint.
* Keep workflow state in `.copilot-tracking/` files.

## Context and constraints

* `prompts/00-project-brief.md` maps S06 to the direct courier SDK call.
* `prompts/patterns/structural-adapter.md` is the generated task brief.
* `prompts/05-run-all.md` permits local commits and tags, but forbids pushes.
* Keep the ShipFast transcript, S03/S10 behavior, all other smells, project names, and namespaces unchanged.

## Execution checklist

* [x] P01: Generate unchanged copy, run 41 tests, and commit it as `e3af51e`.
* [x] P02: Add the provider interface/adapter and focused tests.
* [x] P03: Complete README/diff/map, review, refactor commit, progress, and local tag.

## Validation and success criteria

* Unchanged copy passes before edits.
* Full Release build succeeds with warnings as errors; all tests and golden master pass.
* Only S06 is removed; `DIFF.md` matches the README source/test list.
* Special characters and amount conversion are covered by tests.
* Review is Conformant or Conformant with justified divergence before continuing.