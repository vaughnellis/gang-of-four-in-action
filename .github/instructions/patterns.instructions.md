---
applyTo: "patterns/**"
---

# Pattern solution instructions

Each folder under `patterns/<category>/<pattern>/` is a copy of the baseline that fixes exactly one smell with one pattern. The folder name tells you which pattern; the pattern map in `prompts/00-project-brief.md` (section 6) tells you which smell.

- Refactor only this folder's own smell. Every other `SMELL` marker and its code stays exactly as in the baseline, even if it is next to your change.
- Remove only this pattern's `SMELL` marker(s) once the refactor is done.
- Do not edit `tests/CornerStore.Tests/GoldenMasterTests.cs`. Add pattern tests in `<PatternName>Tests.cs`.
- Do not edit any file outside this folder, except this pattern's row in `docs/pattern-map.md` and its link in the root `README.md`.
- Keep the README's **Files changed** list accurate, and regenerate `DIFF.md` with `tools/diff-baseline.ps1` after any code change.
- Name types after the pattern's participants where it reads naturally, so a reader can map the code to the pattern.
