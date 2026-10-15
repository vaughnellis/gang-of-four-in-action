# Task: observer

**Task slug:** `observer`
**Shared context:** `prompts/00-project-brief.md` (read it first, especially sections 6 to 10)
**Pattern:** Observer (Behavioral)
**Smell:** S19
**Folder:** `patterns/behavioral/observer/`
**Depends on:** `baseline-v1` tag exists; pattern template is locked after the pilots.

## Goal

Create the Observer solution: a copy of the baseline in which smell S19 is refactored using Observer, and nothing else changes.

## The smell

After an order status change, `OrderService` directly calls inventory, notification, loyalty, and analytics services. Adding a new reaction requires changing the order service.

## The refactor

Introduce an order status observer contract. `OrderService` publishes a status-change notification to its registered observers. Move the current inventory lookup, email/SMS notification, loyalty, and analytics reactions into observers composed by `StoreConsole`.

## Requirements

- **FR-001** Run `tools/new-pattern.ps1 -Category behavioral -Name observer`. Confirm the copy builds and the golden master passes before changing anything.
- **FR-002** Refactor S19 to publish order status changes to registered observers. Keep status transition rules in `OrderService` and preserve the order of existing observer side effects.
- **FR-003** Remove the `SMELL S19` marker(s). Leave every other `SMELL` marker in place.
- **FR-004** Add `ObserverTests.cs` proving that registered observers receive status changes, the built-in observers preserve the existing side effects, a test-only observer can be registered without changing `OrderService`, and invalid transitions do not notify observers.
- **FR-005** Fill `README.md` using the template in section 8 of the project brief, including a Mermaid class diagram and a **Files changed** list.
- **FR-006** Run `tools/diff-baseline.ps1 -Category behavioral -Name observer` and commit the resulting `DIFF.md`.
- **FR-007** Set Observer's status to Done in `docs/pattern-map.md` and verify its folder link in the root README pattern index.

## Acceptance criteria

- **AC-001** The definition of done (project brief section 9) is met.
- **AC-002** `GoldenMasterTests.cs` is unchanged from the baseline and passes.
- **AC-003** `DIFF.md` shows changes only in the files listed under **Files changed** in the README.
- **AC-004** Project and namespace names are unchanged.
- **AC-005** The README explains how Observer differs from Mediator and states a situation where Observer is not useful, such as a single fixed reaction.

## Boundaries

- Do not touch `baseline/`, `scenarios/` or any other pattern folder.
- Do not fix other smells, even nearby ones.
- Do not rename projects or namespaces.
- Do not push.

## Suggested phases

1. **P01** Copy and verify the baseline.
2. **P02** Refactor Observer and add tests.
3. **P03** Complete README, `DIFF.md`, pattern map, review, local commits, and tag under `prompts/05-run-all.md`.