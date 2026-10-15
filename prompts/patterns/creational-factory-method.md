# Task: factory-method

**Task slug:** `factory-method`
**Shared context:** `prompts/00-project-brief.md` (read it first, especially sections 6 to 10)
**Pattern:** Factory Method (Creational)
**Smell:** S03
**Folder:** `patterns/creational/factory-method/`
**Depends on:** `baseline-v1` tag exists; pattern template is locked.

## Goal

Create the Factory Method solution: a copy of the baseline in which smell S03 is refactored using Factory Method, and nothing else changes.

## The smell

Checkout switches on the payment type and constructs `CardProcessor`, `PayPalProcessor`, or `AfterpayProcessor` inline. Adding a payment processor changes the checkout method.

## The refactor

Introduce an abstract payment processor creator with a factory method that creates an `IPaymentProcessor`. Add one concrete creator per payment type. Keep payment selection behavior and processor outputs unchanged.

## Requirements

- **FR-001** Run `tools/new-pattern.ps1 -Category creational -Name factory-method`. Confirm the copy builds and the golden master passes before changing anything.
- **FR-002** Move processor creation into concrete creator overrides and route checkout charging through the creators.
- **FR-003** Remove the `SMELL S03` marker(s). Leave every other `SMELL` marker in place.
- **FR-004** Add `FactoryMethodTests.cs` proving each creator charges through the correct processor and unknown payment types remain rejected.
- **FR-005** Fill `README.md` using the template in section 8 of the project brief, including a Mermaid class diagram and a **Files changed** list.
- **FR-006** Run `tools/diff-baseline.ps1 -Category creational -Name factory-method` and commit the resulting `DIFF.md`.
- **FR-007** Set Factory Method's status to Done in `docs/pattern-map.md` and verify its root README pattern index link.

## Acceptance criteria

- **AC-001** The definition of done (project brief section 9) is met.
- **AC-002** `GoldenMasterTests.cs` is unchanged from the baseline and passes.
- **AC-003** `DIFF.md` shows changes only in the files listed under **Files changed** in the README.
- **AC-004** Project and namespace names are unchanged.
- **AC-005** The README explains how Factory Method differs from Abstract Factory and identifies a situation where it is unnecessary.

## Boundaries

- Do not touch `baseline/`, `scenarios/` or any other pattern folder.
- Do not fix other smells, even nearby ones.
- Do not rename projects or namespaces.
- Do not push.

## Suggested phases

1. **P01** Copy and verify the baseline.
2. **P02** Refactor Factory Method and add tests.
3. **P03** Complete README, `DIFF.md`, map, review, local commits, and tag under `prompts/05-run-all.md`.