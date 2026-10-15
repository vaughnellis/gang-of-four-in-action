---
description: "Implementation plan for Abstract Factory in the automatic run-all workflow"
applyTo: ".copilot-tracking/changes/2026-10-16/abstract-factory-changes.md"
---
<!-- markdownlint-disable-file -->

# Plan: abstract-factory

Task slug: `abstract-factory`
Date: 2026-10-16
Execution model: direct implementation

## User requests

* Resume run-all in automatic mode from its progress file.
* Complete Abstract Factory in queue order and continue through remaining patterns unless a brief checkpoint blocks progress.

## Context and constraints

* `prompts/00-project-brief.md` maps S01 to an `IRegionFactory` producing matched AU/NZ tax, shipping and currency products.
* `prompts/05-run-all.md` requires an unchanged-copy commit, a refactor commit, a local tag, and a progress update. No push.
* The copied suite passed 41 tests before edits; copy commit is `ff3f207`.
* Preserve the unknown-region fallback, golden-master transcript and all non-S01 smells, including S23 in `TaxCalculator`.
* The root README already links Abstract Factory. Update only its pattern-map status row.

## Execution checklist

* [x] Generate and verify the unchanged copy, then commit it as `ff3f207`.
* [x] Centralize region selection in family factories, route tax, surcharge and currency formatting through them, and add focused tests.
* [x] Complete README and generated diff, update the map, validate the full solution, review, commit, tag and record progress.

## Validation and success criteria

* Release suite passes with golden master unchanged.
* Release build succeeds with warnings as errors.
* Only S01 markers are removed; all other markers remain.
* `DIFF.md` paths match the README Files changed list.
* Review outcome is Conformant or Conformant with justified divergence before proceeding.
