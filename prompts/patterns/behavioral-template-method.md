# Task: template-method

**Task slug:** `template-method`
**Shared context:** `prompts/00-project-brief.md` (read it first, especially sections 6 to 10)
**Pattern:** Template Method (Behavioral)
**Smell:** S22
**Folder:** `patterns/behavioral/template-method/`
**Depends on:** `baseline-v1` tag exists; pattern template is locked.

## Goal

Create the Template Method solution: a baseline copy in which smell S22 is refactored using Template Method, and nothing else changes.

## The smell

CSV and JSON exporters duplicate the same load, status-filter, aggregate, and output workflow.

## The refactor

Introduce an abstract `ReportExporter` with a fixed export algorithm for loading/filtering orders, aggregating line values, and delegating format-specific row accumulation and document writing to subclasses. Keep CSV and JSON output byte-for-byte identical.

## Requirements

- **FR-001** Run `tools/new-pattern.ps1 -Category behavioral -Name template-method`. Confirm the copy builds and the golden master passes before changing anything.
- **FR-002** Move the shared export skeleton into `ReportExporter`; leave format-specific behavior in CSV and JSON subclasses.
- **FR-003** Remove the `SMELL S22` marker(s). Leave every other `SMELL` marker in place, especially S23 switches in both exporter subclasses.
- **FR-004** Add `TemplateMethodTests.cs` proving the fixed algorithm filters requested statuses, both formats preserve their structures, and each subclass supplies its own line-item accumulation and output format.
- **FR-005** Fill `README.md` using the template in section 8 of the project brief, including a Mermaid class diagram and a **Files changed** list.
- **FR-006** Run `tools/diff-baseline.ps1 -Category behavioral -Name template-method` and commit the resulting `DIFF.md`.
- **FR-007** Set Template Method's status to Done in `docs/pattern-map.md` and verify its root README pattern index link.

## Acceptance criteria

- **AC-001** The definition of done (project brief section 9) is met.
- **AC-002** `GoldenMasterTests.cs` is unchanged from the baseline and passes.
- **AC-003** `DIFF.md` shows changes only in the files listed under **Files changed** in the README.
- **AC-004** Project and namespace names are unchanged.
- **AC-005** The README distinguishes Template Method from Strategy and identifies when inheritance is not worthwhile.

## Boundaries

- Do not touch `baseline/`, `scenarios/` or any other pattern folder.
- Do not fix other smells, especially S23.
- Do not rename projects or namespaces.
- Do not push.

## Suggested phases

1. **P01** Copy and verify the baseline.
2. **P02** Refactor Template Method and add tests.
3. **P03** Complete README, `DIFF.md`, map, review, local commits, and tag under `prompts/05-run-all.md`.