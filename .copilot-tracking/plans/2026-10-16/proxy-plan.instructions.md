---
description: "Implementation plan for the Proxy task in the automatic run-all workflow"
applyTo: ".copilot-tracking/changes/2026-10-16/proxy-changes.md"
---
<!-- markdownlint-disable-file -->

# Plan: proxy

Task slug: `proxy`
Date: 2026-10-16
Execution model: direct implementation

## User requests

* Continue run-all with Proxy after Iterator.
* Complete the unchanged copy, proxy refactor, review, tag, and progress row.
* Refactor only S12 and preserve the golden-master transcript.

## Context and constraints

* `prompts/00-project-brief.md` maps S12 to caching and protection proxies
  behind `IInventoryService`.
* Inventory is shared by checkout validation, order lifecycle, and back-office
  operations; only back-office stock lookup and adjustment require staff.
* Preserve S14 action markers and all other smell code.

## Execution checklist

* [x] P01: Generate and verify the unchanged copy; commit `33063ce`.
* [x] P02: Add the inventory interface, proxies, wiring, and focused tests.
* [x] P03: Complete README, diff, map, review, refactor commit, progress, and tag.

## Validation and success criteria

* Unchanged-copy Release suite passed (41 tests).
* Final Release suite and golden master pass.
* Cache remains correct after stock reservations and adjustments.
* Staff checks are enforced at the protection proxy boundary.
* README file list matches `DIFF.md`; only S12 changes.
