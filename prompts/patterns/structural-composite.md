# Task: composite

**Task slug:** `composite`
**Shared context:** `prompts/00-project-brief.md` (read it first, especially sections 6 to 10)
**Pattern:** Composite (Structural)
**Smell:** S08
**Folder:** `patterns/structural/composite/`
**Depends on:** `baseline-v1` tag exists.

## Goal

Create the Composite solution: a copy of the baseline in which smell S08 is
refactored using Composite, and nothing else changes.

## The smell

Bundles use separate pricing and cart-expansion paths selected by type checks,
and they cannot contain other bundles.

## The refactor

Introduce `ICatalogItem`, implemented by `Product` and `Bundle`, so catalogs
and carts use one component abstraction. A bundle contains catalog items,
calculates its price from its children, and recursively expands nested bundles
into product cart lines. Preserve one-level bundle prices and allocations,
support nested bundles, and reject self-reference or indirect cycles.

## Requirements

- **FR-001** Run `tools/new-pattern.ps1 -Category structural -Name composite`.
  Confirm the copy builds and the golden master passes before changing anything.
- **FR-002** Refactor S08 around `ICatalogItem` and nested `Bundle` components.
- **FR-003** Remove only the `SMELL S08` markers. Leave all other smell markers
  and their code in place.
- **FR-004** Update the copied test that expected nested bundles to be rejected.
  Add `CompositeTests.cs` proving nested pricing and line expansion, cycle
  rejection, and unchanged simple-product and one-level bundle outcomes.
- **FR-005** Fill `README.md` using the template in section 8 of the project
  brief, including a Mermaid class diagram and a **Files changed** list.
- **FR-006** Run `tools/diff-baseline.ps1 -Category structural -Name composite`
  and commit the resulting `DIFF.md`.
- **FR-007** Set Composite's status to Done in `docs/pattern-map.md` and verify
  its root README pattern index link.

## Acceptance criteria

- **AC-001** The definition of done (project brief section 9) is met.
- **AC-002** `GoldenMasterTests.cs` is unchanged from the baseline and passes.
- **AC-003** `DIFF.md` shows changes only in files listed in the README.
- **AC-004** Project and namespace names are unchanged.
- **AC-005** The README distinguishes Composite from Decorator and explains
  when a flat collection is simpler.

## Boundaries

- Do not touch `baseline/`, `scenarios/`, or any other pattern folder.
- Do not fix other smells, especially S04 and S16.
- Do not rename projects or namespaces.
- Do not push.

## Suggested phases

1. **P01** Copy and verify the baseline.
2. **P02** Refactor Composite, update the nested-bundle test, and add tests.
3. **P03** Complete README, `DIFF.md`, map, review, local commits, and tag
   under `prompts/05-run-all.md`.
