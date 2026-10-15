# Task: prototype

**Task slug:** `prototype`
**Shared context:** `prompts/00-project-brief.md` (read it first, especially sections 6 to 10)
**Pattern:** Prototype (Creational)
**Smell:** S04
**Folder:** `patterns/creational/prototype/`
**Depends on:** `baseline-v1` tag exists.

## Goal

Create the Prototype solution: a copy of the baseline in which smell S04 is refactored using Prototype, and nothing else changes.

## The smell

Size and colour variants are built by copying every field of the parent product by hand.

## The refactor

Add a `Clone` operation to physical products that returns a distinct product variant, preserving the prototype's other attributes while setting the requested SKU, size and colour. Change `Catalog.CreateVariant` to use the prototype's clone operation instead of copying fields itself. Document C# records and `with` expressions as a modern alternative for copying immutable data.

## Requirements

* **FR-001** Run `tools/new-pattern.ps1 -Category creational -Name prototype`. Confirm the copy builds and the golden master passes before changing anything.
* **FR-002** Refactor smell S04 using a `Clone` operation on the physical product prototype.
* **FR-003** Remove the `SMELL S04` marker. Leave every other `SMELL` marker in place.
* **FR-004** Add `PrototypeTests.cs` proving that a clone is distinct from its parent, preserves unchanged product attributes, and contains the requested variant identity and size/colour.
* **FR-005** Fill `README.md` using the template in section 8 of the project brief, including a Mermaid class diagram of the refactored seam and a **Files changed** list.
* **FR-006** Run `tools/diff-baseline.ps1 -Category creational -Name prototype` and commit the resulting `DIFF.md`.
* **FR-007** Set Prototype's status to Done in `docs/pattern-map.md` and link its folder from the root README pattern index. Check whether the link already exists before editing the index.
* **FR-008** Explain when a clone operation is preferable to reconstructing an object, and note records with `with` expressions as a modern alternative.

## Acceptance criteria

* **AC-001** The pattern's definition of done (project brief section 9) is met.
* **AC-002** `GoldenMasterTests.cs` is unchanged from the baseline and passes.
* **AC-003** `DIFF.md` shows changes only in the files listed under **Files changed** in the README.
* **AC-004** Project and namespace names are unchanged.
* **AC-005** The README states at least one situation where Prototype is the wrong choice.

## Boundaries

* Do not touch `baseline/`, `scenarios/` or any other pattern folder.
* Do not fix other smells, even nearby ones.
* Do not rename projects or namespaces.
* Do not push.

## Suggested phases

1. **P01** Copy and verify the baseline.
2. **P02** Refactor S04 and add focused clone tests.
3. **P03** Complete the README, generated diff, map, review, local commits, tag and run-all progress.
