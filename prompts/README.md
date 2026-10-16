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
| `05-run-all.md` | `run-all` | Orchestrates everything after foundations in one automatic run: finishes the baseline, runs both pilots, applies their notes, then generates and runs the other 21 pattern briefs. |
| `06-comparisons.md` | `comparisons` | `docs/comparisons.md`: eight commonly confused pattern pairs, explained with code from this repo. |
| `07-capstone.md` | `capstone` | All 23 patterns applied together in `capstone/`, one commit per pattern, held to the same golden master. |
| `08-readmes.md` | `readmes` | Every README rewritten to one educational standard: root, baseline, the three categories, all 23 patterns and the capstone. |
| `09-release-v1.md` | `release-v1` | Release preparation: final accuracy pass, pinned actions, Dependabot, changelog, link check. |
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

Exact file names depend on the HVE Core version, so use the paths the agent reports at the end of planning. With the version used to build this repo, a task produces:

```text
.copilot-tracking/
├── plans/<date>/<task slug>-plan.instructions.md
├── plans/logs/<date>/<task slug>-log.md
├── plans/critiques/<date>/<task slug>-critique.md
├── details/<date>/<task slug>-details.md
├── changes/<date>/<task slug>-changes.md
└── reviews/<date>/<task slug>-plan-review.md
```

**2. Implement, one phase at a time**

```text
/rpi-implement plan=<plan path reported by the agent> task=P01
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

Skip `/rpi-research` by default. The project brief settles the main decisions. Use it only if the plan critique or the agent flags a real evidence gap, for example a package or SDK behaviour that changed after the briefs were written.

## Adding a pattern brief

1. Copy `patterns/_template.md` to `patterns/<category>-<pattern>.md`, for example `patterns/behavioral-observer.md`.
2. Fill the placeholders from the pattern map in `00-project-brief.md`.
3. Run it with Option A or B, using the pattern's folder name as the task slug.
