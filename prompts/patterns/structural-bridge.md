# Task: bridge

**Task slug:** `bridge`
**Shared context:** `prompts/00-project-brief.md` (read it first, especially sections 6 to 10)
**Pattern:** Bridge (Structural)
**Smell:** S07
**Folder:** `patterns/structural/bridge/`
**Depends on:** `baseline-v1` tag exists.

## Goal

Create the Bridge solution: a copy of the baseline in which smell S07 is refactored using Bridge, and nothing else changes.

## The smell

Message wording and delivery channel are combined in four classes: confirmed email/SMS and shipped email/SMS.

## The refactor

Separate order message abstractions from `IChannel` implementations. Make confirmation and shipping messages work with either email or SMS without adding a class for each combination.

## Requirements

- **FR-001** Run `tools/new-pattern.ps1 -Category structural -Name bridge`. Confirm the copy builds and the golden master passes before changing anything.
- **FR-002** Route confirmation and shipping messages through the message abstraction and `IChannel` implementation.
- **FR-003** Remove the `SMELL S07` markers. Leave every other `SMELL` marker in place.
- **FR-004** Add `BridgeTests.cs` proving each message abstraction can use both channels and preserves its message text.
- **FR-005** Fill `README.md` using the template in section 8 of the project brief, including a Mermaid class diagram and a **Files changed** list.
- **FR-006** Run `tools/diff-baseline.ps1 -Category structural -Name bridge` and commit the resulting `DIFF.md`.
- **FR-007** Set Bridge's status to Done in `docs/pattern-map.md` and verify its root README pattern index link.

## Acceptance criteria

- **AC-001** The definition of done in project brief section 9 is met.
- **AC-002** `GoldenMasterTests.cs` is unchanged from the baseline and passes.
- **AC-003** `DIFF.md` shows changes only in the files listed under **Files changed** in the README.
- **AC-004** Project and namespace names are unchanged.
- **AC-005** The README identifies when Bridge is unnecessary and distinguishes it from Adapter.

## Boundaries

- Do not touch `baseline/`, `scenarios/` or any other pattern folder.
- Do not fix other smells, even nearby ones.
- Do not rename projects or namespaces.
- Do not push.

## Suggested phases

1. **P01** Copy and verify the baseline.
2. **P02** Refactor S07 and add tests.
3. **P03** Complete README, `DIFF.md`, map, review, local commits, and tag under `prompts/05-run-all.md`.
