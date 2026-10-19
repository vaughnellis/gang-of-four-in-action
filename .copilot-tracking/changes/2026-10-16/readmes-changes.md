<!-- markdownlint-disable-file -->
# Changes: readmes

Implementation date: 2026-10-16

Plan: `.copilot-tracking/plans/2026-10-16/readmes-plan.instructions.md`

## Summary

Completed the repository README refresh and pattern README generator update. The Facade exemplar checkpoint was completed first; work continued after the user's instruction to finish autonomously.

## Files changed

* Added the root landing page, baseline guide, three category guides, and all 23 pattern guides.
* Updated the capstone README.
* Updated `tools/new-pattern.ps1` to generate the required pattern README section headings.
* Added planning, execution details, validation, and change tracking artifacts under `.copilot-tracking/`.

## Validation

| Check | Result |
| --- | --- |
| README section headings and required blog placeholders | Passed for all 23 pattern pages |
| Relative links and anchors | 368 checked across 30 README files; all resolve |
| C# excerpts | All 74 fenced C# excerpts match normalized source |
| Pattern solution build, test, and scenario commands | Passed for all 23 patterns; scenarios each produced 417 transcript lines |
| Baseline build, tests, and scenario | Passed; 41 tests, scenario produced 417 transcript lines |
| Capstone build and tests | Passed; 131 tests |
| Facade build, tests, and scenario | Passed with 0 warnings; 44 tests; scenario produced 417 transcript lines |
| Creational solution commands | All five build/test command sets and scenario runs passed |
| Umbrella Release build | Passed with 0 warnings and 0 errors |
| Umbrella tests | 1,205 passed |
| Public clone and quick-start commands | Passed in a temporary clone |
| Generator scratch checks | Passed under `pwsh` and `powershell.exe`; temporary worktree and scratch patterns removed |
| Mermaid diagrams | 43 blocks across 23 pattern READMEs; manually reviewed. External validator required sign-in and preview timed out. |
| `git diff --check` | Passed |
| Change scope | Only README files, `tools/new-pattern.ps1`, and `.copilot-tracking/` artifacts changed |

## Boundaries

No source code, tests, `DIFF.md`, scenarios, prompts, or protected instruction files were changed. No commit, push, or tag was made.
