# Task: foundations

**Task slug:** `foundations`
**Shared context:** `prompts/00-project-brief.md` (read it first)
**Depends on:** nothing. This is the first task.

## Goal

Create the empty repository skeleton that every later task builds on: build settings, umbrella solution, folder layout, docs stubs and CI. No application code.

## Requirements

- **FR-001** Create the root files: `README.md`, `LICENSE` (MIT), `.gitignore` (standard .NET), `.editorconfig`, `global.json` pinning a .NET 10 SDK.
- **FR-002** Create `Directory.Build.props` applying D-07 to every project: nullable enabled, warnings as errors, implicit usings enabled, `LangVersion` latest.
- **FR-003** Create `Directory.Packages.props` with Central Package Management enabled and versions for xUnit, the xUnit runner, `Microsoft.NET.Test.Sdk`, Verify for xUnit and `Microsoft.Extensions.TimeProvider.Testing`.
- **FR-004** Create `GangOfFour.slnx` at the root with solution folders `baseline`, `patterns/creational`, `patterns/structural`, `patterns/behavioral` and `capstone`, all empty for now.
- **FR-005** Create the folder layout from section 4 of the project brief, with a `.gitkeep` where a folder would otherwise be empty: `docs/`, `baseline/`, `patterns/creational/`, `patterns/structural/`, `patterns/behavioral/`, `capstone/`, `scenarios/expected/`, `tools/`.
- **FR-006** Create stub docs: `docs/smells-catalog.md`, `docs/pattern-map.md` (the pattern map tables from the project brief, plus a Status column set to Not started) and `docs/comparisons.md` (headings only: Strategy vs State, Facade vs Mediator, Decorator vs Proxy, Command vs Memento for undo).
- **FR-007** Create `.github/workflows/ci.yml`: on push and pull request, set up .NET from `global.json`, restore, build and test `GangOfFour.slnx`. It must pass while the solution has no projects, and later fail on any build or test failure.
- **FR-008** Write the root `README.md`: what the repo is (homage to the Gang of Four), how it is organised (baseline, patterns by category, capstone), how to run a solution, a pattern index table linking to each future folder, and a short "Built with HVE Core" section pointing to `prompts/README.md` and `.copilot-tracking/`.

## Acceptance criteria

- **AC-001** `dotnet build GangOfFour.slnx` succeeds locally.
- **AC-002** The CI workflow is valid YAML and its steps match FR-007.
- **AC-003** Every folder in section 4 of the project brief exists.
- **AC-004** `docs/pattern-map.md` lists all 23 patterns with the correct smell IDs and folder names.
- **AC-005** No application or test projects exist yet.

## Boundaries

- Do not create any C# projects. That is the baseline task.
- Do not modify anything under `prompts/`.
- Do not push or create tags.

## Suggested phases

1. **P01** Root files and build settings (FR-001 to FR-004).
2. **P02** Folder layout and docs stubs (FR-005, FR-006).
3. **P03** CI workflow and root README (FR-007, FR-008).

## Stop point

After review, stop and hand back so the human can commit and push, and confirm CI runs green on GitHub.
