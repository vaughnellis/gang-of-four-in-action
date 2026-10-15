<!-- markdownlint-disable-file -->
# Review: flyweight

Plan: `.copilot-tracking/plans/2026-10-16/flyweight-plan.instructions.md`
Changes: `.copilot-tracking/changes/2026-10-16/flyweight-changes.md`
Reviewer: RPI Agent (Phase 4)
Date: 2026-10-16
Outcome: Conformant with justified divergence

## Executive outcome

S11's repeated metadata is now immutable `ProductSpec` state shared through a keyed factory. SKU and price remain per listing. Tests prove shared reference identity and independent extrinsic state. The Flyweight pattern solution builds and its tests, including the golden master, pass. A BenchmarkDotNet MemoryDiagnoser run measured 42.23 MB allocated by the repeated-metadata import and 13.98 MB by the Flyweight import for 100,000 listings.

## Findings

The root `GangOfFour.slnx` build fails because project names duplicate within the existing `/patterns/behavioral/` folder. The same duplicates were present before the Flyweight copy commit. This is outside the pattern seam and does not affect the dedicated Flyweight solution, which builds with zero warnings and errors. It is recorded as a justified divergence and left for separate root-solution maintenance.

No unresolved findings remain within the Flyweight task scope.

## Request fulfillment

| Request | Result | Evidence |
| --- | --- | --- |
| Create and verify the unchanged copy | Complete | 41 tests passed; copy commit `b8459d3` |
| Replace repeated metadata with Flyweights | Complete | Immutable `ProductSpec` and keyed `ProductSpecFactory` |
| Keep SKU and price per listing | Complete | Focused test verifies distinct listing data |
| Add focused Flyweight tests | Complete | Tests prove specification reuse, exact metadata and separate extrinsic values |
| Add BenchmarkDotNet memory comparison | Complete | MemoryDiagnoser ShortRun over 100,000 listings; 42.23 MB vs 13.98 MB |
| Complete README and generated diff | Complete | Mermaid, trade-offs, benchmark instructions/results and covered source/test paths |
| Update map and verify root index | Complete | S11 row is Done; existing root README link verified |

## Validation

* Unchanged-copy suite: 41/41 passed.
* Final Flyweight suite: 43/43 passed.
* Flyweight Release solution build: 0 warnings, 0 errors.
* BenchmarkDotNet ShortRun completed both benchmark methods.
* `GoldenMasterTests.cs` SHA-256 matches baseline.
* All non-S11 marker/file counts match baseline.
* `git diff --check` passed.
* Root solution build error was reproduced after verifying its duplicate names predate this task.

## Release record

The unchanged copy is `b8459d3`; the refactor is `74b9d1d`, tagged `flyweight-v1`. Progress and tracking records are being committed separately. No push was made.
