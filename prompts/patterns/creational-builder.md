# Task: builder

**Task slug:** `builder`
**Shared context:** `prompts/00-project-brief.md` (read it first, especially sections 6 to 10)
**Pattern:** Builder (Creational)
**Smell:** S02
**Folder:** `patterns/creational/builder/`
**Depends on:** `baseline-v1` tag exists; pattern template is locked.

## Goal

Create the Builder solution: a baseline copy in which smell S02 is refactored using Builder, and nothing else changes.

## The smell

`Order` and `Invoice` are assembled through eleven positional constructor arguments, including nulls for optional fields.

## The refactor

Introduce fluent `OrderBuilder` and `InvoiceBuilder` classes. Each builder names its inputs, supplies appropriate defaults for optional values, and validates required values in `Build()`.

## Requirements

- **FR-001** Run `tools/new-pattern.ps1 -Category creational -Name builder`. Confirm the copy builds and the golden master passes before changing anything.
- **FR-002** Replace the positional order and invoice assembly calls with fluent builders that validate required inputs.
- **FR-003** Remove the `SMELL S02` marker(s). Leave every other `SMELL` marker in place.
- **FR-004** Add `BuilderTests.cs` proving valid order/invoice construction, clear failure for missing required values, and defaults for optional fields.
- **FR-005** Fill `README.md` using the template in section 8 of the project brief, including a Mermaid class diagram and a **Files changed** list.
- **FR-006** Run `tools/diff-baseline.ps1 -Category creational -Name builder` and commit the resulting `DIFF.md`.
- **FR-007** Set Builder's status to Done in `docs/pattern-map.md` and verify its root README pattern index link.

## Acceptance criteria

- **AC-001** The definition of done (project brief section 9) is met.
- **AC-002** `GoldenMasterTests.cs` is unchanged from the baseline and passes.
- **AC-003** `DIFF.md` shows changes only in the files listed under **Files changed** in the README.
- **AC-004** Project and namespace names are unchanged.
- **AC-005** The README distinguishes Builder from Factory Method and identifies when Builder is unnecessary.

## Boundaries

- Do not touch `baseline/`, `scenarios/` or any other pattern folder.
- Do not fix other smells, even nearby ones.
- Do not rename projects or namespaces.
- Do not push.

## Suggested phases

1. **P01** Copy and verify the baseline.
2. **P02** Refactor Builder and add tests.
3. **P03** Complete README, `DIFF.md`, map, review, local commits, and tag under `prompts/05-run-all.md`.