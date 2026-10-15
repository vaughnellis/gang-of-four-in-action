# Task: decorator

**Task slug:** `decorator`
**Shared context:** `prompts/00-project-brief.md` (read it first, especially sections 6 to 10)
**Pattern:** Decorator (Structural)
**Smell:** S09
**Folder:** `patterns/structural/decorator/`
**Depends on:** `baseline-v1` tag exists; pattern template is locked.

## Goal

Create the Decorator solution: a baseline copy in which smell S09 is refactored using Decorator, and nothing else changes.

## The smell

`Cart` stores add-ons as boolean flags, and `PricingService` calculates gift wrap and insurance with an if-chain.

## The refactor

Introduce an add-on price component with a base component and `GiftWrap` and `Insurance` decorators. Wrap the base component according to the cart's enabled add-ons. Insurance continues to calculate from the discounted subtotal, not from the gift-wrap amount.

## Requirements

- **FR-001** Run `tools/new-pattern.ps1 -Category structural -Name decorator`. Confirm the copy builds and the golden master passes before changing anything.
- **FR-002** Move add-on price calculation into composable decorators around a base price component.
- **FR-003** Remove the `SMELL S09` marker(s). Leave every other `SMELL` marker in place.
- **FR-004** Add `DecoratorTests.cs` proving base cost, each individual decorator, the combined decorators, and insurance's discounted-subtotal basis.
- **FR-005** Fill `README.md` using the template in section 8 of the project brief, including a Mermaid class diagram and a **Files changed** list.
- **FR-006** Run `tools/diff-baseline.ps1 -Category structural -Name decorator` and commit the resulting `DIFF.md`.
- **FR-007** Set Decorator's status to Done in `docs/pattern-map.md` and verify its root README pattern index link.

## Acceptance criteria

- **AC-001** The definition of done (project brief section 9) is met.
- **AC-002** `GoldenMasterTests.cs` is unchanged from the baseline and passes.
- **AC-003** `DIFF.md` shows changes only in the files listed under **Files changed** in the README.
- **AC-004** Project and namespace names are unchanged.
- **AC-005** The README distinguishes Decorator from Adapter and identifies when Decorator is overkill.

## Boundaries

- Do not touch `baseline/`, `scenarios/` or any other pattern folder.
- Do not fix other smells, even nearby ones.
- Do not rename projects or namespaces.
- Do not push.

## Suggested phases

1. **P01** Copy and verify the baseline.
2. **P02** Refactor Decorator and add tests.
3. **P03** Complete README, `DIFF.md`, map, review, local commits, and tag under `prompts/05-run-all.md`.