# Task: command

**Task slug:** `command`
**Shared context:** `prompts/00-project-brief.md` (read it first, especially sections 6 to 10)
**Pattern:** Command (Behavioral)
**Smell:** S14
**Folder:** `patterns/behavioral/command/`
**Depends on:** `baseline-v1` tag exists.

## Goal

Create the Command solution: a copy of the baseline in which smell S14 is
refactored using Command, and nothing else changes.

## The smell

Back-office actions run inline with no undo, audit trail, or queue.

## The refactor

Represent stock adjustment, stock lookup, refund, and cancellation as command
objects with `Execute` and `Undo` operations, and dispatch them through a
command invoker that records an in-memory audit trail. Keep staff authorization
in `BackOffice`, outside the S14 seam. Preserve its existing duplicate stock
lookup and every S12 marker. Stock adjustment is reversible; lookup is
read-only, and refund/cancellation have side effects that cannot be undone
safely in this example. Make unsupported undo explicit rather than implying
that external effects were reversed. Audit data must not appear in the
business transcript.

## Requirements

- **FR-001** Run `tools/new-pattern.ps1 -Category behavioral -Name command`.
  Confirm the copy builds and the golden master passes before changing anything.
- **FR-002** Move the S14 actions into ordered command objects with an invoker
  and an audit log. Preserve the existing result strings and staff checks.
- **FR-003** Remove only the `SMELL S14` markers. Keep every other marker,
  including S12, and preserve the duplicate stock lookup.
- **FR-004** Add `CommandTests.cs` proving command execution, stock-adjustment
  undo, explicit rejection of unsupported undo, audit recording, and unchanged
  business results.
- **FR-005** Fill `README.md` using the template in section 8 of the project
  brief, including a Mermaid class diagram and a **Files changed** list.
- **FR-006** Run `tools/diff-baseline.ps1 -Category behavioral -Name command`
  and commit the resulting `DIFF.md`.
- **FR-007** Set Command's status to Done in `docs/pattern-map.md` and verify
  its root README pattern index link.

## Acceptance criteria

- **AC-001** The definition of done (project brief section 9) is met.
- **AC-002** `GoldenMasterTests.cs` is unchanged from the baseline and passes.
- **AC-003** `DIFF.md` shows changes only in files listed in the README.
- **AC-004** Project and namespace names are unchanged.
- **AC-005** The README distinguishes Command from Strategy and explains when
  command objects and undo history add unnecessary complexity.

## Boundaries

- Do not touch `baseline/`, `scenarios/`, or any other pattern folder.
- Do not fix other smells, especially S12.
- Do not rename projects or namespaces.
- Do not push.

## Suggested phases

1. **P01** Copy and verify the baseline.
2. **P02** Refactor S14 and add command and audit tests.
3. **P03** Complete README, `DIFF.md`, map, review, local commits, and tag
   under `prompts/05-run-all.md`.
