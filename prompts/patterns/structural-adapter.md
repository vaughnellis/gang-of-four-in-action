# Task: adapter

**Task slug:** `adapter`
**Shared context:** `prompts/00-project-brief.md` (read it first, especially sections 6 to 10)
**Pattern:** Adapter (Structural)
**Smell:** S06
**Folder:** `patterns/structural/adapter/`
**Depends on:** `baseline-v1` tag exists; pattern template is locked.

## Goal

Create the Adapter solution: a baseline copy in which smell S06 is refactored using Adapter, and nothing else changes.

## The smell

Checkout constructs courier XML, converts the order total to cents, calls `ShipFastClient.CreateConsignmentFromXml`, and parses an XML response directly.

## The refactor

Introduce an `IShippingProvider` application interface and a `ShipFastAdapter` that translates order data to and from the courier client's XML and cents-based API. Route checkout through the interface.

## Requirements

- **FR-001** Run `tools/new-pattern.ps1 -Category structural -Name adapter`. Confirm the copy builds and the golden master passes before changing anything.
- **FR-002** Move ShipFast protocol conversion behind `ShipFastAdapter` and make checkout use `IShippingProvider`.
- **FR-003** Remove the `SMELL S06` marker(s). Leave every other `SMELL` marker in place.
- **FR-004** Add `AdapterTests.cs` proving shipment details are translated correctly, special XML characters are handled safely, and the adapter returns the courier tracking number.
- **FR-005** Fill `README.md` using the template in section 8 of the project brief, including a Mermaid class diagram and a **Files changed** list.
- **FR-006** Run `tools/diff-baseline.ps1 -Category structural -Name adapter` and commit the resulting `DIFF.md`.
- **FR-007** Set Adapter's status to Done in `docs/pattern-map.md` and verify its root README pattern index link.

## Acceptance criteria

- **AC-001** The definition of done (project brief section 9) is met.
- **AC-002** `GoldenMasterTests.cs` is unchanged from the baseline and passes.
- **AC-003** `DIFF.md` shows changes only in the files listed under **Files changed** in the README.
- **AC-004** Project and namespace names are unchanged.
- **AC-005** The README distinguishes Adapter from Decorator and identifies when an adapter is unnecessary.

## Boundaries

- Do not touch `baseline/`, `scenarios/` or any other pattern folder.
- Do not fix other smells, even nearby ones.
- Do not rename projects or namespaces.
- Do not push.

## Suggested phases

1. **P01** Copy and verify the baseline.
2. **P02** Refactor Adapter and add tests.
3. **P03** Complete README, `DIFF.md`, map, review, local commits, and tag under `prompts/05-run-all.md`.