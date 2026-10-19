<!-- markdownlint-disable-file -->
# Readmes implementation plan

## User requests

* Complete `prompts/08-readmes.md` using `prompts/00-project-brief.md` as shared context.
* Treat the project brief's Decisions as settled and respect all repository boundaries.
* Work in automatic mode and complete the task.
* The initial instruction required stopping after the Facade exemplar; the user then explicitly directed autonomous continuation to complete the task.

## Objectives

* Bring all README files named in the task brief into its specified structure and content standard.
* Update the new-pattern README scaffold to use the pattern README headings.
* Verify all links, code excerpts, documented commands, diagrams, and repository build/tests.
* Do not modify `src/`, `tests/`, `DIFF.md`, `scenarios/`, `prompts/`, or protected instruction files.
* Do not commit, push, or tag.

## Context

* Shared project decisions: `prompts/00-project-brief.md`.
* Task requirements and checkpoints: `prompts/08-readmes.md`.
* README writing conventions: the Markdown and writing-style instructions.
* Baseline and pattern change boundaries: `.github/instructions/baseline.instructions.md` and `.github/instructions/patterns.instructions.md`.
* Facade exemplar: `patterns/structural/facade/README.md`.

## Implementation phases

* [x] P01: complete the Facade exemplar and validate its commands, excerpts, links, and diagrams.
* [x] P02-P04: rewrite the creational, remaining structural, and behavioral pattern READMEs.
* [x] P05: create the baseline and category READMEs, align the capstone README, and update the generator scaffold.
* [x] P06: replace the root README with the landing page, learning path, quick start, pattern index, and provenance facts.
* [x] P07: validate scope, headings, links, excerpts, commands, diagrams, build, tests, and generator compatibility.

## Success criteria

* Every pattern README has the ten task-brief sections in order.
* Root, baseline, category and capstone READMEs meet their brief requirements.
* The generator emits the same ten section headings and passes scratch checks in both supported PowerShell hosts.
* All runnable documentation commands succeed; all relative links resolve.
* The umbrella Release build and tests pass.
* Only task-authorized files change.

## Dependencies

* None. This documentation task uses existing repository tooling and no additional packages.
