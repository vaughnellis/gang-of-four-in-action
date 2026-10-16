<!-- markdownlint-disable-file -->
# Capstone Plan

## User request

Build the integrated `capstone/` solution from the baseline, apply all 23 patterns in the specified order with one local commit per step, preserve the golden-master transcript, add pattern tests and documentation, and review the finished result. Make no tags or pushes.

## Context and constraints

- Follow `prompts/00-project-brief.md` and `prompts/07-capstone.md`.
- Keep the baseline, scenarios, pattern folders, prompts, and repository instructions unchanged.
- Outside `capstone/`, change only the capstone entries in `GangOfFour.slnx`, the capstone link in the root `README.md`, and `.copilot-tracking/`.
- Preserve `CornerStore` and `CornerStore.Tests` names and the byte-identical golden master test.
- Record each meeting point between patterns in the capstone README.
- Build Release with zero warnings and run all tests after every pattern before committing that step.
- The brief requires comparisons to be committed and CI on `main` to be green before starting.

## Execution plan

1. Resume only after CI on `main` is green. Confirm the working tree and required dependency state.
2. Copy `baseline/src` and `baseline/tests` unchanged into `capstone/`, add `Capstone.slnx`, add its two projects to `GangOfFour.slnx`, verify build and golden master, then commit the copy.
3. In the exact FR-003 order, integrate one pattern at a time. Port its specific tests, keep all other pattern changes intact, confirm only that smell is removed, build Release with zero warnings, run all tests and golden master, record test count and commit hash, then commit.
4. Add `capstone/README.md` with all required pattern locations, pattern-folder links, meeting points, Mermaid flow, and the commit-history tour. Add the capstone link to the root README.
5. Run `dotnet build GangOfFour.slnx --no-restore -c Release` and `dotnet test GangOfFour.slnx --no-build -c Release`. Verify no `SMELL S` marker remains under `capstone/src`, the golden-master test is unchanged, and all 24 capstone commits are present in order.
6. Review the diff against the request and update the progress record. Stop for human review and push/CI.

## Current status

CI on `main` is green at `f641396`. The unchanged copy and 23 ordered pattern commits are complete. Final commit `81f424a` integrates Visitor with Template Method and adds the capstone documentation. The umbrella Release build completed with zero warnings, the full solution test suite passed, all 131 capstone tests passed, and the golden master remains byte-identical to baseline. No tags or pushes were made.
