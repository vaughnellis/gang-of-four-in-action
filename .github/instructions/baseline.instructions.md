---
applyTo: "baseline/**"
---

# Baseline instructions

Everything under `baseline/` is the deliberately messy starting point that all 23 pattern solutions copy. Its shape is wrong on purpose; its behaviour must be right.

- Do not apply any design pattern at a `SMELL` site. No new interfaces, factories, strategies, state classes or event systems there.
- Do not refactor, rename or reorganise code to make it cleaner. If something looks bad, it is probably a planted smell.
- When adding baseline code during the baseline task, plant smells only as described in the pattern map (`prompts/00-project-brief.md`, section 6), each with a `// SMELL Sxx:` marker.
- Bugs are different from smells. A real behaviour bug gets the smallest possible fix that keeps the smell intact.
- After the `baseline-v1` tag exists, change `baseline/` only in a task explicitly labelled as a baseline fix.
