# Task: run-all (automatic run)

**Task slug:** `run-all`
**Shared context:** `prompts/00-project-brief.md` (read it first)
**Depends on:** `foundations` committed; `baseline` in progress or complete.

## Purpose

Complete the rest of the repository in one automatic RPI run, with as little human supervision as possible, to see how far HVE Core gets on its own. This brief orchestrates the other briefs; it does not replace them.

## Scope, in order

1. **Finish `02-baseline.md`.** Resume from its existing plan and change record in `.copilot-tracking/`. Do not re-plan or redo completed phases.
2. **Run `03-pilot-facade.md`.**
3. **Run `04-pilot-strategy.md`.**
4. **Apply the pilot notes** from both pilots' change records to `prompts/patterns/_template.md`.
5. **Run the remaining 21 patterns in batch order.** For each one, create `prompts/patterns/<category>-<folder>.md` from the template using the pattern map (project brief section 6), then run it.
    - **Batch A:** Observer, Factory Method, Adapter, Decorator
    - **Batch B:** State, Builder, Template Method, Chain of Responsibility, Command, Composite, Iterator, Proxy
    - **Batch C:** Abstract Factory, Bridge, Mediator, Memento, Visitor, Singleton
    - **Batch D:** Prototype, Flyweight, Interpreter

Out of scope for this run: `capstone/` and `docs/comparisons.md`.

## How to run each task

- Each brief is its own RPI task identity, with its own plan, critique, change record and review under `.copilot-tracking/`, named after the brief's task slug.
- Skip research unless there is a real evidence gap.
- Move to the next task only when the review outcome is Conformant or Conformant with justified divergence. Otherwise fix the findings with implementation, at most two attempts, then stop (see Checkpoints).
- Ignore the "Stop" points inside individual briefs; this brief's Checkpoints replace them for this run.

## Permissions granted for this run only

These override the default boundaries in the project brief and the Copilot instructions, and apply only while this task is active:

- Make local git commits with conventional commit messages. For pattern tasks, keep the two-commit workflow: one commit for the unchanged copy, one for the refactor.
- Create local tags: `baseline-v1` after the baseline review passes, and `<folder>-v1` after each pattern review passes.
- Create new pattern briefs under `prompts/patterns/`, and edit `prompts/patterns/_template.md` with pilot notes.

Still forbidden during this run:

- Pushing, or creating releases.
- Editing `baseline/` after `baseline-v1`, or `scenarios/expected/baseline.txt` after it is approved.
- Editing `00-project-brief.md`, the numbered briefs, `.github/copilot-instructions.md` or `.github/instructions/`.

## Progress file

Maintain `.copilot-tracking/run-all-progress.md`: one row per task in scope with its status (Not started, In progress, Done, Blocked), review outcome, commit hashes and tag. Update it after every task.

If you start in a fresh chat, read this file first and resume from the first task that is not Done.

## Checkpoints (stop and wait for the human)

- **After `baseline-v1` is tagged.** The human reviews the baseline once before it is copied 23 times. Delete this line to run straight through.
- **When blocked:** a build or test still failing after two fix attempts, a change that would alter a settled Decision, or a missing tool (for example `pwsh`).
- **When context runs low:** update the progress file, then stop with a one-line resume note.

## Final report

When every task in scope is Done, report:

- The progress table.
- Review findings that were accepted rather than fixed.
- The pilot notes applied to the template.
- Per pattern: files changed and line counts from its `DIFF.md`.
