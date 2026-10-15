---
description: "Implementation plan for Singleton in the automatic run-all workflow"
applyTo: ".copilot-tracking/changes/2026-10-16/singleton-changes.md"
---
<!-- markdownlint-disable-file -->

# Plan: singleton

Task slug: `singleton`
Date: 2026-10-16
Execution model: direct implementation

## User requests

* Resume the run-all queue in automatic mode.
* Complete Singleton and continue with the remaining patterns unless a brief checkpoint blocks progress.

## Context and constraints

* The project brief maps S05 to replacing global static `Config` and `Log` state with Singleton.
* The run-all brief requires separate copy and refactor commits, a local tag and progress updates. Do not push.
* The unchanged copy passed 41 Release tests; copy commit is `496cfa0`.
* Preserve per-scenario configuration reset and log output; preserve every non-S05 smell.
* The repository has no dependency injection package. Keep the implementation dependency-free and document registering the same instances in a DI container.
* The root README already links Singleton; update only its pattern-map row.

## Execution checklist

* [x] Generate and verify the unchanged copy, then commit it as `496cfa0`.
* [x] Replace static global state with thread-safe Lazy singletons and add focused tests.
* [x] Complete README and generated diff, update the map, validate, review, commit, tag and record progress.

## Validation and success criteria

* Release tests and golden master pass.
* Release build succeeds with warnings as errors.
* Only S05 markers are removed; other markers remain.
* README Files changed paths match the generated `DIFF.md`.
* Review outcome is Conformant or Conformant with justified divergence before continuing.
