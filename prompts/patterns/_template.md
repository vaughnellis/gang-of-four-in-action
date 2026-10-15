# Task: {{pattern-folder}}

<!--
How to use this template:
1. Copy to prompts/patterns/{{category}}-{{pattern-folder}}.md
2. Replace every {{placeholder}} using the pattern map in 00-project-brief.md (section 6)
3. Delete this comment
-->

**Task slug:** `{{pattern-folder}}`
**Shared context:** `prompts/00-project-brief.md` (read it first, especially sections 6 to 10)
**Pattern:** {{Pattern Name}} ({{Category}})
**Smell:** {{Sxx}}
**Folder:** `patterns/{{category}}/{{pattern-folder}}/`
**Depends on:** `baseline-v1` tag exists.

## Goal

Create the {{Pattern Name}} solution: a copy of the baseline in which smell {{Sxx}} is refactored using {{Pattern Name}}, and nothing else changes.

## The smell

{{Baseline smell, copied from the pattern map.}}

## The refactor

{{Refactor target, copied from the pattern map, plus any specifics: names of new types, which callers change.}}

## Requirements

- **FR-001** Run `tools/new-pattern.ps1 -Category {{category}} -Name {{pattern-folder}}`. Confirm the copy builds and the golden master passes before changing anything.
- **FR-002** Refactor smell {{Sxx}} using {{Pattern Name}} as described above.
- **FR-003** Remove the `SMELL {{Sxx}}` marker(s). Leave every other `SMELL` marker in place.
- **FR-004** Add `{{PatternName}}Tests.cs` covering the pattern itself: {{2 to 4 behaviours the tests must prove}}. Use test visibility for internal participants when needed; do not widen the production API only for tests.
- **FR-005** Fill `README.md` using the template in section 8 of the project brief, including a Mermaid class diagram of the refactored seam and a **Files changed** list.
- **FR-006** Run `tools/diff-baseline.ps1 -Category {{category}} -Name {{pattern-folder}}` and commit the resulting `DIFF.md`. If the generator fails, record the error and any non-invasive workaround in the change record; do not edit baseline tooling from a pattern task.
- **FR-007** Set this pattern's status to Done in `docs/pattern-map.md` and link its folder from the root `README.md` pattern index. Check whether the link already exists before editing the index.
{{- FR-008 Pattern-specific extra, if any. For example, a benchmark (Flyweight) or undo tests (Command).}}

## Acceptance criteria

- **AC-001** The pattern's definition of done (project brief section 9) is met.
- **AC-002** `GoldenMasterTests.cs` is unchanged from the baseline and passes.
- **AC-003** `DIFF.md` shows changes only in the files listed under **Files changed** in the README.
- **AC-004** Project and namespace names are unchanged.
- **AC-005** The README states at least one situation where {{Pattern Name}} is the wrong choice.

## Boundaries

- Do not touch `baseline/`, `scenarios/`, `prompts/` or any other pattern folder.
- Do not fix other smells, even nearby ones. They belong to other patterns.
- Do not rename projects or namespaces.
- Do not push or tag.

## Suggested phases

1. **P01** Copy the baseline (FR-001). **Stop** so the human can commit the unchanged copy.
2. **P02** Refactor and tests (FR-002 to FR-004).
3. **P03** README, `DIFF.md`, pattern map and index (FR-005 to FR-007).

## Stop point

After review, stop so the human can commit the refactor and create the `{{pattern-folder}}-v1` tag.
