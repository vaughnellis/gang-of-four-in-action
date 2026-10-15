# Task: flyweight

**Task slug:** `flyweight`
**Shared context:** `prompts/00-project-brief.md` (read it first, especially sections 6 to 10)
**Pattern:** Flyweight (Structural)
**Smell:** S11
**Folder:** `patterns/structural/flyweight/`
**Depends on:** `baseline-v1` tag exists.

## Goal

Create the Flyweight solution: a copy of the baseline in which smell S11 is refactored using Flyweight, and nothing else changes.

## The smell

Bulk import creates 100,000 listings, each holding its own copy of brand, manufacturer and specification text.

## The refactor

Move the repeated intrinsic fields into immutable `ProductSpec` flyweights. Use a `ProductSpecFactory` to share each distinct specification across listings, while keeping SKU and price on each listing. Add a BenchmarkDotNet memory benchmark comparing the original repeated-string import with the shared-flyweight import.

## Requirements

* **FR-001** Run `tools/new-pattern.ps1 -Category structural -Name flyweight`. Confirm the copy builds and the golden master passes before changing anything.
* **FR-002** Refactor smell S11 using shared `ProductSpec` instances created by a factory.
* **FR-003** Remove the `SMELL S11` markers. Leave every other `SMELL` marker in place.
* **FR-004** Add `FlyweightTests.cs` proving repeated listings share the same immutable intrinsic specification, different specifications remain distinct, and per-listing SKU and price remain independent.
* **FR-005** Fill `README.md` using the template in section 8 of the project brief, including a Mermaid class diagram of the refactored seam and a **Files changed** list.
* **FR-006** Run `tools/diff-baseline.ps1 -Category structural -Name flyweight` and commit the resulting `DIFF.md`.
* **FR-007** Set Flyweight's status to Done in `docs/pattern-map.md` and link its folder from the root README pattern index. Check whether the link already exists before editing the index.
* **FR-008** Add a BenchmarkDotNet `MemoryDiagnoser` comparison between the repeated-string baseline import and the Flyweight import. Document how to run it and report the measured allocations with the machine-dependent caveat.

## Acceptance criteria

* **AC-001** The pattern's definition of done (project brief section 9) is met.
* **AC-002** `GoldenMasterTests.cs` is unchanged from the baseline and passes.
* **AC-003** `DIFF.md` shows changes only in files listed under **Files changed** in the README.
* **AC-004** Project and namespace names are unchanged.
* **AC-005** The README states at least one situation where Flyweight is the wrong choice.
* **AC-006** The benchmark builds and reports allocated memory for both implementations.

## Boundaries

* Do not touch `baseline/`, `scenarios/` or any other pattern folder.
* Do not fix other smells, even nearby ones.
* Do not rename projects or namespaces.
* Do not push.

## Suggested phases

1. **P01** Copy and verify the baseline.
2. **P02** Refactor S11, add focused Flyweight tests, and add the memory benchmark.
3. **P03** Complete README, generated diff, map, review, local commits, tag and run-all progress.
