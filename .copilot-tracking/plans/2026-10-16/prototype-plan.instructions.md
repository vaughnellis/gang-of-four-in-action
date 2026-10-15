<!-- markdownlint-disable-file -->
# Prototype implementation plan

## User request

Resume the automatic run-all queue at Prototype, complete the S04 refactor, review it, commit the unchanged copy and refactor separately, tag it, and record progress.

## Context

* Shared project constraints: `prompts/00-project-brief.md`, especially sections 6 to 10.
* Run-all permissions and queue: `prompts/05-run-all.md`.
* Pattern-specific rules: `.github/instructions/patterns.instructions.md`.
* C# implementation rules: `csharp.instructions.md`.
* C# test rules: `csharp-tests.instructions.md`.
* Prototype's smell is confined to `Catalog.CreateVariant`, which copies physical-product fields into a new product.

## Approach

1. [x] Create `prompts/patterns/creational-prototype.md` from the pattern task template and run the prescribed copy generator.
2. [x] Build and test the untouched copy, then commit it separately.
3. [x] Move the field-copy behavior behind a physical-product `Clone` operation. Preserve `CreateVariant` validation and transcript behavior.
4. [x] Add focused tests proving clone identity, unchanged field preservation, and variant overrides. Use test assembly visibility rather than broadening production access.
5. [x] Complete README and generated diff. Verify the README file list exactly covers the source and test changes, S04 alone is removed, and the golden master is unchanged.
6. [x] Review, mark the map row Done, commit the refactor, tag `prototype-v1`, and update run-all progress in a separate tracking commit.

## Success criteria

* Untouched copy passes its Release tests and golden master before implementation.
* Refactor passes the entire pattern Release test suite and warnings-as-errors build.
* The shared transcript and all non-S04 smell markers are unchanged.
* README, DIFF, map, review, commits, local tag and progress record meet the run-all workflow.

## Dependencies

The unchanged-copy verification and commit precede the refactor.
