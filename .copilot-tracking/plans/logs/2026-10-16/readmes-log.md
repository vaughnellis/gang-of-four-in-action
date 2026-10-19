<!-- markdownlint-disable-file -->
# Readmes planning and validation log

## Decisions

* Used the Facade README as the section and tone exemplar.
* Continued beyond its review checkpoint after the user explicitly requested that the task be completed autonomously.
* Worked on each pattern category independently while directly handling the other README surfaces and generator template.
* The Mermaid validator required sign-in and the preview request timed out. Use the syntax references to hand-check Mermaid blocks and record the limitation.

## Validation iterations

* Facade build and tests passed; its scenario produced 417 transcript lines.
* The baseline build and tests passed; its scenario produced 417 transcript lines.
* Capstone build and tests passed.
* The first generator-check command had a PowerShell interpolation parse error before execution. Corrected the check and verified both PowerShell hosts in a temporary worktree.
* A source comparison found two inaccurate code excerpts. Corrected the Bridge excerpt and removed the incomplete Template Method excerpt; all 74 fenced C# excerpts then matched normalized source.
* The external Mermaid validator required sign-in and the preview timed out. Manually reviewed the 43 diagram blocks in the 23 pattern READMEs.

## Final validation

* All 23 pattern README heading sequences and required blog placeholders passed.
* All 368 relative links and anchors across 30 README files resolved.
* Pattern, baseline, capstone, Facade, creational, documented quick-start, and scratch generator commands passed.
* Umbrella Release build passed with 0 warnings and 0 errors; all 1,205 tests passed.
* `git diff --check` passed; changed paths were limited to README files, `tools/new-pattern.ps1`, and `.copilot-tracking/` artifacts.
* No code, test, scenario, `DIFF.md`, prompt, or protected instruction files were modified. No commit, push, or tag was made.
