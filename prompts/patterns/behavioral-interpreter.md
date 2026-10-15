# Task: interpreter

**Task slug:** `interpreter`
**Shared context:** `prompts/00-project-brief.md` (read it first, especially sections 6 to 10)
**Pattern:** Interpreter (Behavioral)
**Smell:** S15
**Folder:** `patterns/behavioral/interpreter/`
**Depends on:** `baseline-v1` tag exists.

## Goal

Create the Interpreter solution: a copy of the baseline in which smell S15 is refactored using Interpreter, and nothing else changes.

## The smell

Promo rules such as `SPEND>100 AND CATEGORY=Shoes` are parsed with `Split` and `IndexOf` each time a promotion is evaluated.

## The refactor

Define a small grammar for the existing rules and parse each configured rule into an expression tree once. Interpret the tree against the cart, customer and subtotal when evaluating a promo. Keep `RuleOf` and the transcript text unchanged.

The grammar must support the existing predicates `SPEND>number`, `QTY>number`, `CATEGORY=value`, and `REGION=value`, combined with `AND`. Greater-than comparisons remain strict. Category matches when any cart line has that category, quantity is summed across all cart lines, and region compares with the customer region.

## Requirements

* **FR-001** Run `tools/new-pattern.ps1 -Category behavioral -Name interpreter`. Confirm the copy builds and the golden master passes before changing anything.
* **FR-002** Refactor smell S15 with a parser and an expression tree. Parse the configured rules once rather than on every evaluation.
* **FR-003** Remove the `SMELL S15` marker. Leave every other `SMELL` marker in place.
* **FR-004** Add `InterpreterTests.cs` covering compound evaluation, strict numeric comparisons, category and region predicates, quantity aggregation, and malformed rule rejection.
* **FR-005** Fill `README.md` using the template in section 8 of the project brief, including a Mermaid class diagram of the grammar and expression participants and a **Files changed** list.
* **FR-006** Run `tools/diff-baseline.ps1 -Category behavioral -Name interpreter` and commit the resulting `DIFF.md`.
* **FR-007** Set Interpreter's status to Done in `docs/pattern-map.md`. Verify the Interpreter link in the root README pattern index exists before deciding whether to edit it.

## Acceptance criteria

* **AC-001** The pattern's definition of done in project brief section 9 is met.
* **AC-002** `GoldenMasterTests.cs` is unchanged from the baseline and passes.
* **AC-003** `DIFF.md` shows changes only in the files listed under **Files changed** in the README.
* **AC-004** Project and namespace names are unchanged.
* **AC-005** The README states at least one situation where Interpreter is the wrong choice.
* **AC-006** Promo evaluation uses the parsed expression tree and preserves all existing promo results and transcript text.

## Boundaries

* Do not touch `baseline/`, `scenarios/` or any other pattern folder.
* Do not fix other smells, even nearby ones.
* Do not rename projects or namespaces.
* Do not push.

## Suggested phases

1. **P01** Create and verify the unchanged copy.
2. **P02** Parse S15 rules into expressions, add focused tests, and preserve the golden master.
3. **P03** Complete README and generated diff, update the map, review, make the refactor commit, tag `interpreter-v1`, and record run-all progress.
