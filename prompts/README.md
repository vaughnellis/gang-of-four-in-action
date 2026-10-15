# prompts/

This repository is built with [Microsoft HVE Core](https://github.com/microsoft/hve-core) using its RPI workflow: Research, Plan, Implement, Review. This folder holds the briefs the RPI agent works from. Each brief is one bounded, reviewable task; together they build the whole repository.

HVE Core writes its working artifacts (plans, plan critiques, change records, reviews) to `.copilot-tracking/`. Those artifacts are committed, so anyone can trace how each part of the repo was planned, built and reviewed.

## Files

| File | Task slug | What it builds |
| --- | --- | --- |
| `00-project-brief.md` | (context only) | The full plan: decisions, structure, smells, pattern map, safety net, definition of done. Every task reads it. |
| `01-foundations.md` | `foundations` | Repo skeleton, build settings, umbrella solution, CI. |
| `02-baseline.md` | `baseline` | The messy CornerStore app, its 23 smells, the golden master and the copy/diff tools. |
| `03-pilot-facade.md` | `facade` | The first pattern solution. Proves the per-pattern workflow. |
| `04-pilot-strategy.md` | `strategy` | The second pattern solution. Locks the pattern template. |
| `patterns/_template.md` | (template) | Copy and fill to create the brief for each remaining pattern. |

Alongside these briefs, `.github/copilot-instructions.md` and `.github/instructions/` hold standing rules that Copilot applies automatically to every interaction in the repo, RPI or not. The briefs say what to build; the instructions say how to behave while building it. Keep the two consistent: if a rule changes in `00-project-brief.md`, update the instructions too.

Run them in order. Do not start a brief until the previous one has a review outcome of Conformant (or Conformant with justified divergence) and the human has committed.

## How to run a brief

Use a fresh Copilot Chat for each brief, with HVE Core installed in VS Code.

### Option A: guided, phase by phase (recommended for 01 and 02)

**1. Plan**

```text
/rpi-plan
Task: <task slug>
Shared context: #file:prompts/00-project-brief.md
This task: #file:prompts/<brief file>
Plan only the work in this task's brief. Treat the project brief's Decisions as settled and do not re-research them. Respect the Global boundaries and this brief's Boundaries. Use the brief's suggested phases as a starting point.
```

Read the plan and its critique in `.copilot-tracking/`. If the critique says Revise, ask `/rpi-plan` to address it before moving on.

**2. Implement, one phase at a time**

```text
/rpi-implement plan=.copilot-tracking/plans/<date>/<task slug>-plan.md task=P01
```

Repeat for `P02`, `P03` and so on. Check the change record after each phase.

**3. Review**

```text
/rpi-review task=<task slug>
```

Route any findings as the review recommends, then commit.

### Option B: RPI Agent (good for pattern briefs once the template is locked)

Select **RPI Agent** in the agent picker and send:

```text
Task: <task slug>
Shared context: #file:prompts/00-project-brief.md
This task: #file:prompts/<brief file>
Take this task through planning, implementation and review. Treat the project brief's Decisions as settled. Stop at each Stop point in the brief so I can commit.
```

RPI Agent starts in manual mode. Choose Full Auto only for pattern briefs after the pilots, when the workflow is proven.

## Research

Skip `/rpi-research` by default. The project brief settles the main decisions. Use it only if the plan critique or the agent flags a real evidence gap, for example how to configure Verify to write to a shared path.

## Adding a pattern brief

1. Copy `patterns/_template.md` to `patterns/<category>-<pattern>.md`, for example `patterns/behavioral-observer.md`.
2. Fill the placeholders from the pattern map in `00-project-brief.md`.
3. Run it with Option A or B, using the pattern's folder name as the task slug.
