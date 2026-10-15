# Task: strategy (pilot 2 of 2)

**Task slug:** `strategy`
**Shared context:** `prompts/00-project-brief.md` (read it first, especially sections 6 to 10)
**Pattern:** Strategy (Behavioral)
**Smell:** S21
**Folder:** `patterns/behavioral/strategy/`
**Depends on:** `facade` reviewed and committed, with its pilot edits applied to `prompts/`.

## Goal

Create the second pattern solution, this time a behavioral one, and confirm the workflow holds after the Facade pilot's edits. When this task passes review, `prompts/patterns/_template.md` is considered locked and used for the remaining 21 patterns.

## The smell

The pricing code calculates shipping cost with a `switch` on the shipping method (standard, express, click and collect). Adding a method means editing the pricing code, and the rules for each method cannot be tested on their own.

## The refactor

Introduce `IShippingCostStrategy` with one implementation per shipping method. Checkout chooses the strategy from the selected method and the pricing code calls it. The pricing code no longer knows the individual rules.

## Requirements

- **FR-001** Run `tools/new-pattern.ps1 -Category behavioral -Name strategy`. Confirm the copy builds and the golden master passes before changing anything.
- **FR-002** Introduce `IShippingCostStrategy` and one strategy per shipping method, and route the pricing code through it.
- **FR-003** Remove the `SMELL S21` marker(s). Leave every other `SMELL` marker in place, including S01 (region branches) and S09 (add-ons), which sit close by.
- **FR-004** Add `StrategyTests.cs` proving that:
    - each strategy returns the same cost as the baseline for representative AU and NZ orders;
    - checkout selects the correct strategy for each shipping method;
    - a new strategy defined only in the test project can be used without changing the pricing code.
- **FR-005** Fill `README.md` using the template in section 8 of the project brief, including a Mermaid class diagram and a **Files changed** list.
- **FR-006** Run `tools/diff-baseline.ps1 -Category behavioral -Name strategy` and commit the resulting `DIFF.md`.
- **FR-007** Set Strategy's status to Done in `docs/pattern-map.md` and link its folder from the root `README.md` pattern index.
- **FR-008** In the change record, add a **Pilot notes** section as in the Facade pilot, focused on whether the template is ready to lock. Do not edit `prompts/`.

## Acceptance criteria

- **AC-001** The definition of done (project brief section 9) is met.
- **AC-002** `GoldenMasterTests.cs` is unchanged from the baseline and passes.
- **AC-003** `DIFF.md` shows changes only in the files listed under **Files changed** in the README.
- **AC-004** Project and namespace names are unchanged.
- **AC-005** The README explains how Strategy differs from State, and states at least one situation where Strategy is overkill (for example, two branches that never change).
- **AC-006** The change record contains Pilot notes with a clear recommendation: lock the template as is, or the specific edits needed first.

## Boundaries

- Do not touch `baseline/`, `scenarios/`, `prompts/` or any other pattern folder.
- Do not fix other smells, even nearby ones.
- Do not rename projects or namespaces.
- Do not push or tag.

## Suggested phases

1. **P01** Copy the baseline (FR-001). **Stop** so the human can commit the unchanged copy.
2. **P02** Strategy refactor and tests (FR-002 to FR-004).
3. **P03** README, `DIFF.md`, pattern map, index and pilot notes (FR-005 to FR-008).

## Stop point

After review, stop so the human can commit the refactor, lock the template, and create the `strategy-v1` tag.
