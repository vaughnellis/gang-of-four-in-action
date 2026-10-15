# Task: singleton

**Task slug:** `singleton`
**Shared context:** `prompts/00-project-brief.md` (read it first, especially sections 6 to 10)
**Pattern:** Singleton (Creational)
**Smell:** S05
**Folder:** `patterns/creational/singleton/`
**Depends on:** `baseline-v1` tag exists.

## Goal

Create the Singleton solution: a copy of the baseline in which smell S05 is refactored using Singleton, and nothing else changes.

## The smell

Static `Config` and `Log` classes expose public mutable fields that any caller can change.

## The refactor

Replace `Config` and `Log` with sealed, process-wide instances created through thread-safe `Lazy<T>`. Replace public mutable fields with controlled instance methods and read-only properties, then update callers to use the instances. Preserve per-scenario reset behavior and transcript output. Explain how a host using dependency injection can register the same instance as a singleton without adding a DI dependency to CornerStore.

## Requirements

- **FR-001** Run `tools/new-pattern.ps1 -Category creational -Name singleton`. Confirm the copy builds and the golden master passes before changing anything.
- **FR-002** Refactor S05 using thread-safe `Lazy<T>` singleton instances for `Config` and `Log`.
- **FR-003** Remove the `SMELL S05` markers. Leave every other `SMELL` marker in place.
- **FR-004** Add `SingletonTests.cs` proving concurrent callers receive the same instances, configuration can be reset and changed through its API, and the log instance writes to its configured output.
- **FR-005** Fill `README.md` using the template in section 8 of the project brief, including a Mermaid class diagram and a **Files changed** list.
- **FR-006** Run `tools/diff-baseline.ps1 -Category creational -Name singleton` and commit the resulting `DIFF.md`.
- **FR-007** Set Singleton's status to Done in `docs/pattern-map.md` and verify its root README pattern index link.
- **FR-008** Explain the trade-offs of manual Singleton access versus registering the same instance in a dependency injection container. Do not add a DI package.

## Acceptance criteria

- **AC-001** The definition of done in project brief section 9 is met.
- **AC-002** `GoldenMasterTests.cs` is unchanged from the baseline and passes.
- **AC-003** `DIFF.md` shows changes only in the files listed under **Files changed** in the README.
- **AC-004** Project and namespace names are unchanged.
- **AC-005** The README identifies when Singleton is unnecessary and distinguishes it from dependency injection.

## Boundaries

- Do not touch `baseline/`, `scenarios/` or any other pattern folder.
- Do not fix other smells, even nearby ones.
- Do not rename projects or namespaces.
- Do not push.

## Suggested phases

1. **P01** Copy and verify the baseline.
2. **P02** Refactor S05 and add tests.
3. **P03** Complete README, generated diff, map, review, local commits, tag and progress.
