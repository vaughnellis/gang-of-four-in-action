# Task: abstract-factory

**Task slug:** `abstract-factory`
**Shared context:** `prompts/00-project-brief.md` (read it first, especially sections 6 to 10)
**Pattern:** Abstract Factory (Creational)
**Smell:** S01
**Folder:** `patterns/creational/abstract-factory/`
**Depends on:** `baseline-v1` tag exists.

## Goal

Create the Abstract Factory solution: a copy of the baseline in which smell S01 is refactored using Abstract Factory, and nothing else changes.

## The smell

`if (region == "AU")` branches are scattered across tax, shipping and currency code. Nothing stops AU tax from being combined with NZ shipping.

## The refactor

Introduce `IRegionFactory` with AU and NZ factories that create matched tax, shipping and currency products. Centralize region selection and retain the existing fallback for unsupported regions.

## Requirements

- **FR-001** Run `tools/new-pattern.ps1 -Category creational -Name abstract-factory`. Confirm the copy builds and the golden master passes before changing anything.
- **FR-002** Route regional tax, shipping surcharge and currency formatting through the selected region factory and its product family.
- **FR-003** Remove the `SMELL S01` markers. Leave every other `SMELL` marker in place.
- **FR-004** Add `AbstractFactoryTests.cs` proving AU and NZ factories return matching regional rules and that the fallback behavior is preserved.
- **FR-005** Fill `README.md` using the template in section 8 of the project brief, including a Mermaid class diagram and a **Files changed** list.
- **FR-006** Run `tools/diff-baseline.ps1 -Category creational -Name abstract-factory` and commit the resulting `DIFF.md`.
- **FR-007** Set Abstract Factory's status to Done in `docs/pattern-map.md` and verify its root README pattern index link.

## Acceptance criteria

- **AC-001** The definition of done in project brief section 9 is met.
- **AC-002** `GoldenMasterTests.cs` is unchanged from the baseline and passes.
- **AC-003** `DIFF.md` shows changes only in the files listed under **Files changed** in the README.
- **AC-004** Project and namespace names are unchanged.
- **AC-005** The README identifies a situation where Abstract Factory is the wrong choice and distinguishes it from Factory Method.

## Boundaries

- Do not touch `baseline/`, `scenarios/` or any other pattern folder.
- Do not fix other smells, even nearby ones.
- Do not rename projects or namespaces.
- Do not push.

## Suggested phases

1. **P01** Copy and verify the baseline.
2. **P02** Refactor S01 and add tests.
3. **P03** Complete README, `DIFF.md`, map, review, local commits, and tag under `prompts/05-run-all.md`.
