# Task: mediator

**Task slug:** `mediator`
**Shared context:** `prompts/00-project-brief.md` (read it first, especially sections 6 to 10)
**Pattern:** Mediator (Behavioral)
**Smell:** S17
**Folder:** `patterns/behavioral/mediator/`
**Depends on:** `baseline-v1` tag exists.

## Goal

Create the Mediator solution: a copy of the baseline in which smell S17 is refactored using Mediator, and nothing else changes.

## The smell

Cart widgets store references to peer widgets and refresh those peers directly after a state change.

## The refactor

Introduce a `CartMediator` that coordinates widget refreshes and cart actions. Widgets communicate through it rather than holding references to one another. Preserve the existing update order and transcript.

## Requirements

- **FR-001** Run `tools/new-pattern.ps1 -Category behavioral -Name mediator`. Confirm the copy builds and the golden master passes before changing anything.
- **FR-002** Move inter-widget coordination and widget actions used by `StoreConsole` behind `CartMediator`.
- **FR-003** Remove the `SMELL S17` markers. Leave every other `SMELL` marker in place.
- **FR-004** Add `MediatorTests.cs` proving item, promo, shipping and totals changes produce the established mediator-coordinated refresh order.
- **FR-005** Fill `README.md` using the template in section 8 of the project brief, including a Mermaid class diagram and a **Files changed** list.
- **FR-006** Run `tools/diff-baseline.ps1 -Category behavioral -Name mediator` and commit the resulting `DIFF.md`.
- **FR-007** Set Mediator's status to Done in `docs/pattern-map.md` and verify its root README pattern index link.

## Acceptance criteria

- **AC-001** The definition of done in project brief section 9 is met.
- **AC-002** `GoldenMasterTests.cs` is unchanged from the baseline and passes.
- **AC-003** `DIFF.md` shows changes only in the files listed under **Files changed** in the README.
- **AC-004** Project and namespace names are unchanged.
- **AC-005** The README identifies when Mediator is unnecessary and distinguishes it from Observer.

## Boundaries

- Do not touch `baseline/`, `scenarios/` or any other pattern folder.
- Do not fix other smells, even nearby ones.
- Do not rename projects or namespaces.
- Do not push.

## Suggested phases

1. **P01** Copy and verify the baseline.
2. **P02** Refactor S17 and add tests.
3. **P03** Complete README, `DIFF.md`, map, review, local commits, and tag under `prompts/05-run-all.md`.
