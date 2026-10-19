<!-- markdownlint-disable-file -->
# Readmes implementation details

## Context

Plan: `.copilot-tracking/plans/2026-10-16/readmes-plan.instructions.md`
Task brief: `prompts/08-readmes.md`
Shared context: `prompts/00-project-brief.md`

## Execution

* P01: rewrote the Facade README as the example, with linked excerpts, participant table, class and sequence diagrams, trade-offs, .NET notes, comparison links, and verified run commands.
* P02-P04: rewrote the three pattern categories' 23 READMEs, preserving existing accurate content and limiting edits to those files.
* P05: added the baseline and three category READMEs, aligned capstone headings and links, and updated `New-PatternReadme` with the required ordered section skeleton.
* P06: replaced the root page with its learning path, verified quick start, all 23 pattern intents, and progress facts from the two specified tracking files.
* P07: checked heading order, placeholders, links and anchors, source excerpts, documented commands, change scope, Mermaid syntax, umbrella build and tests.

## Acceptance checks

* Confirm no changes in protected code, test, diff, scenario, prompt, or instruction paths.
* Confirm every generated pattern README has the ten required headings, exactly and in order.
* Resolve every relative Markdown link and confirm excerpts against the linked source.
* Verify the README commands in their documented working directory.
* Run `dotnet restore GangOfFour.slnx`, `dotnet build GangOfFour.slnx --no-restore -c Release`, and `dotnet test GangOfFour.slnx --no-build -c Release`.
* Run `tools/new-pattern.ps1` with a scratch name under both `pwsh` and `powershell.exe`, and remove the scratch workspace.
