# Task: memento

**Task slug:** `memento`
**Shared context:** `prompts/00-project-brief.md` (read it first, especially sections 6 to 10)
**Pattern:** Memento (Behavioral)
**Smell:** S18
**Folder:** `patterns/behavioral/memento/`
**Depends on:** `baseline-v1` tag exists.

## Goal

Create the Memento solution: a copy of the baseline in which smell S18 is refactored using Memento, and nothing else changes.

## The smell

Cart internals are exposed so `StoreConsole` can copy and restore each state field for one-level undo.

## The refactor

Have `Cart` capture and restore an opaque `CartMemento`. Let `StoreConsole` act as caretaker by retaining the last successful pre-change memento per customer. Preserve the existing one-level undo behavior.

## Requirements

- **FR-001** Run `tools/new-pattern.ps1 -Category behavioral -Name memento`. Confirm the copy builds and the golden master passes before changing anything.
- **FR-002** Replace caller-managed copies with `CartMemento` capture and restore methods.
- **FR-003** Remove the `SMELL S18` markers. Leave every other `SMELL` marker in place.
- **FR-004** Add `MementoTests.cs` proving mementos restore cart values and do not share mutable line state.
- **FR-005** Fill `README.md` using the template in section 8 of the project brief, including a Mermaid class diagram and a **Files changed** list.
- **FR-006** Run `tools/diff-baseline.ps1 -Category behavioral -Name memento` and commit the resulting `DIFF.md`.
- **FR-007** Set Memento's status to Done in `docs/pattern-map.md` and verify its root README pattern index link.

## Acceptance criteria

- **AC-001** The definition of done in project brief section 9 is met.
- **AC-002** `GoldenMasterTests.cs` is unchanged from the baseline and passes.
- **AC-003** `DIFF.md` shows changes only in the files listed under **Files changed** in the README.
- **AC-004** Project and namespace names are unchanged.
- **AC-005** The README identifies when Memento is unnecessary and distinguishes it from Command.

## Boundaries

- Do not touch `baseline/`, `scenarios/` or any other pattern folder.
- Do not fix other smells, even nearby ones.
- Do not rename projects or namespaces.
- Do not push.

## Suggested phases

1. **P01** Copy and verify the baseline.
2. **P02** Refactor S18 and add tests.
3. **P03** Complete README, `DIFF.md`, map, review, local commits, and tag under `prompts/05-run-all.md`.
