# Task: facade (pilot 1 of 2)

**Task slug:** `facade`
**Shared context:** `prompts/00-project-brief.md` (read it first, especially sections 6 to 10)
**Pattern:** Facade (Structural)
**Smell:** S10
**Folder:** `patterns/structural/facade/`
**Depends on:** `baseline-v1` tag exists.

## Goal

Create the first pattern solution and use it to prove the per-pattern workflow end to end: copy, refactor, test, document, diff. This is a pilot, so recording what worked and what did not matters as much as the code.

## The smell

The console flow calls inventory, pricing, payment, shipping and notifications directly, in a fixed order. Every caller has to know that order, and getting it wrong can charge a card for stock that was never reserved.

## The refactor

Introduce `CheckoutFacade` with a single `PlaceOrder(...)` method that owns the order of subsystem calls. The console flow calls only the facade. The subsystems themselves do not change.

## Requirements

- **FR-001** Run `tools/new-pattern.ps1 -Category structural -Name facade`. Confirm the copy builds and the golden master passes before changing anything.
- **FR-002** Introduce `CheckoutFacade` as described above and route the console checkout flow through it.
- **FR-003** Remove the `SMELL S10` marker(s). Leave every other `SMELL` marker in place, including S13 (validation) and S02 (order construction), which sit close by.
- **FR-004** Add `FacadeTests.cs` proving that:
    - a valid cart placed through the facade ends with the order paid, stock reserved, shipment booked and a confirmation sent;
    - when validation fails, no payment is taken and no stock is reserved;
    - the console checkout flow depends on `CheckoutFacade` and not on the individual subsystems.
- **FR-005** Fill `README.md` using the template in section 8 of the project brief, including a Mermaid class diagram showing the flow, the facade and the five subsystems, and a **Files changed** list.
- **FR-006** Run `tools/diff-baseline.ps1 -Category structural -Name facade` and commit the resulting `DIFF.md`.
- **FR-007** Set Facade's status to Done in `docs/pattern-map.md` and link its folder from the root `README.md` pattern index.
- **FR-008** In the change record, add a **Pilot notes** section: friction with the tools, gaps or ambiguities in the project brief or `prompts/patterns/_template.md`, and specific suggested edits. Do not edit those files; the human applies the edits.

## Acceptance criteria

- **AC-001** The definition of done (project brief section 9) is met.
- **AC-002** `GoldenMasterTests.cs` is unchanged from the baseline and passes.
- **AC-003** `DIFF.md` shows changes only in the files listed under **Files changed** in the README.
- **AC-004** Project and namespace names are unchanged.
- **AC-005** The README explains how Facade differs from Mediator, and states at least one situation where a facade is the wrong choice (for example, when it grows into a new god class).
- **AC-006** The change record contains Pilot notes with concrete suggested edits, or states explicitly that none were needed.

## Boundaries

- Do not touch `baseline/`, `scenarios/`, `prompts/` or any other pattern folder.
- Do not fix other smells, even nearby ones.
- Do not rename projects or namespaces.
- Do not push or tag.

## Suggested phases

1. **P01** Copy the baseline (FR-001). **Stop** so the human can commit the unchanged copy.
2. **P02** Facade refactor and tests (FR-002 to FR-004).
3. **P03** README, `DIFF.md`, pattern map, index and pilot notes (FR-005 to FR-008).

## Stop point

After review, stop so the human can commit the refactor, apply any pilot edits to `prompts/`, and create the `facade-v1` tag.
