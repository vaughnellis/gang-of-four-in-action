# Task: visitor

**Task slug:** `visitor`
**Shared context:** `prompts/00-project-brief.md` (read it first, especially sections 6 to 10)
**Pattern:** Visitor (Behavioral)
**Smell:** S23
**Folder:** `patterns/behavioral/visitor/`
**Depends on:** `baseline-v1` tag exists.

## Goal

Create the Visitor solution: a copy of the baseline in which smell S23 is refactored using Visitor, and nothing else changes.

## The smell

Physical, digital and gift card line items are handled with type switches in tax, weight and export code.

## The refactor

Add `ILineItemVisitor` with one visit method for each concrete product type, and add dispatch through `Accept` on products. Use dedicated tax, weight and report-total visitors. Update tax and weight calculations plus both exporters to use the visitors. Preserve tax rounding, report output and the existing S22 export workflow.

## Requirements

- **FR-001** Run `tools/new-pattern.ps1 -Category behavioral -Name visitor`. Confirm the copy builds and the golden master passes before changing anything.
- **FR-002** Replace S23 type switches with `ILineItemVisitor`, product `Accept` dispatch and tax, weight and report-total visitors.
- **FR-003** Remove the `SMELL S23` markers. Leave every other `SMELL` marker in place.
- **FR-004** Add `VisitorTests.cs` proving tax dispatch by product type, quantity-based weight dispatch, and report totals by product type.
- **FR-005** Fill `README.md` using the template in section 8 of the project brief, including a Mermaid class diagram and a **Files changed** list.
- **FR-006** Run `tools/diff-baseline.ps1 -Category behavioral -Name visitor` and commit the resulting `DIFF.md`.
- **FR-007** Set Visitor's status to Done in `docs/pattern-map.md` and verify its root README pattern index link.

## Acceptance criteria

- **AC-001** The definition of done in project brief section 9 is met.
- **AC-002** `GoldenMasterTests.cs` is unchanged from the baseline and passes.
- **AC-003** `DIFF.md` shows changes only in the files listed under **Files changed** in the README.
- **AC-004** Project and namespace names are unchanged.
- **AC-005** The README identifies when Visitor is unnecessary and distinguishes it from Strategy.

## Boundaries

- Do not touch `baseline/`, `scenarios/` or any other pattern folder.
- Do not fix other smells, including S22 in the report exporters.
- Do not rename projects or namespaces.
- Do not push.

## Suggested phases

1. **P01** Copy and verify the baseline.
2. **P02** Refactor S23 and add visitor tests.
3. **P03** Complete README, generated diff, map, review, local commits, tag and progress.
