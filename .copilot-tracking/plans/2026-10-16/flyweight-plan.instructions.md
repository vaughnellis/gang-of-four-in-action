<!-- markdownlint-disable-file -->
# Flyweight implementation plan

## User request

Resume the automatic run-all queue at Flyweight, complete the S11 refactor and required BenchmarkDotNet memory comparison, review it, commit the unchanged copy and refactor separately, tag it, and record progress.

## Context

* Shared project constraints: `prompts/00-project-brief.md`, especially sections 6 to 10.
* Run-all permissions and queue: `prompts/05-run-all.md`.
* Pattern-specific rules: `.github/instructions/patterns.instructions.md`.
* C# implementation rules: `csharp.instructions.md`.
* C# test rules: `csharp-tests.instructions.md`.
* S11 is limited to `Listing` and `ListingImporter`; imported SKU and price remain per-listing, while brand, manufacturer and spec text repeat in a 5 x 4 x 3 cycle.

## Approach

1. [x] Create `prompts/patterns/structural-flyweight.md` and run the prescribed copy generator.
2. [x] Build and test the untouched copy, including its golden master, then commit it separately.
3. [x] Move repeated metadata into an immutable `ProductSpec`; let a factory reuse flyweights by their intrinsic brand/material/origin key. Preserve per-listing SKU and price and all transcript behavior.
4. [x] Add tests for shared reference identity, distinct specification keys, and independent extrinsic listing data. Expose internals only to the test assembly.
5. [x] Add a separate BenchmarkDotNet project with a MemoryDiagnoser comparison against an implementation of the original repeated-string import. Keep the benchmark isolated from the production project.
6. [x] Complete README and generated diff. Verify the README file list, S11-only marker removal, unchanged golden master and benchmark build/run.
7. [x] Review, update the map, commit and tag `flyweight-v1`, and commit progress/tracking separately.

## Success criteria

* Untouched copy passes its Release tests and golden master before implementation.
* Refactored pattern tests and benchmark build pass. Release builds have zero warnings.
* The final benchmark reports allocated bytes for both implementations over 100,000 listings.
* The shared transcript and all non-S11 smell markers are unchanged.
* README, DIFF, map, review, commits, local tag and progress record meet the run-all workflow.

## Dependencies

The unchanged-copy verification and commit precede implementation. The BenchmarkDotNet package reference is isolated to the new benchmark project.
