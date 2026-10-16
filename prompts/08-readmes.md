# Task: readmes

**Task slug:** `readmes`
**Shared context:** `prompts/00-project-brief.md` (read it first)
**Depends on:** `capstone` committed, and CI green on `main`.

## Goal

Make every README in the repository good enough to learn from on its own. A visitor who lands on any folder from GitHub search, a blog post or a link should understand what they are looking at, why it matters, and where to go next, without reading anything else first.

READMEs are reference material for someone reading the code. They explain; they do not sell. The blog will tell the story; the READMEs are where readers check the details.

## The README set

| File | Role |
| --- | --- |
| `README.md` (root) | Landing page: what the repo is, the learning path, how to run it, the full pattern index |
| `baseline/README.md` | Meet CornerStore: what the app does, how it runs, and its 23 smells |
| `patterns/creational/README.md`, `patterns/structural/README.md`, `patterns/behavioral/README.md` | Category pages: what the category is about, and its patterns with one-line intents |
| `patterns/<category>/<pattern>/README.md` (23) | The pattern, taught through CornerStore |
| `capstone/README.md` | All 23 together (already written; bring into line with this standard) |
| `docs/*.md` | Unchanged, except links |

## Requirements

### Pattern READMEs (all 23)

- **FR-001** Every pattern README uses these sections, in this order:
    1. **At a glance.** A short table: intent in one sentence, category, smell fixed (ID and name), GoF chapter reference, files changed (from `DIFF.md`), and links to the previous and next pattern in book order.
    2. **The problem.** What goes wrong in the baseline, in two or three sentences, then a short excerpt from `baseline/` with a link to the file.
    3. **The pattern.** The idea in plain words. Then a participants table mapping each role in the pattern to the CornerStore type that plays it.
    4. **The refactor.** What was introduced and what moved. Before and after excerpts, and a Mermaid class diagram.
    5. **How it runs.** One concrete path through the code, for example what happens when an order is placed. Add a Mermaid sequence diagram where it helps.
    6. **Run it.** The exact commands to build and test this solution and to run the scenario, verified by running them.
    7. **Trade-offs.** What the pattern costs, and when not to use it.
    8. **Related patterns.** Links to the relevant sections of `docs/comparisons.md` and to related pattern folders.
    9. **Modern .NET notes.** Where the platform or language already provides the pattern.
    10. **Further reading.** The GoF chapter, and a blog post line in exactly this form: `Blog post: coming soon <!-- blog-link -->`, so a later task can swap in the real URL.
- **FR-002** Keep every accurate fact and excerpt from the existing README. Rewrite for clarity and structure, not from scratch.
- **FR-003** Book order for previous and next links: Abstract Factory, Builder, Factory Method, Prototype, Singleton, Adapter, Bridge, Composite, Decorator, Facade, Flyweight, Proxy, Chain of Responsibility, Command, Interpreter, Iterator, Mediator, Memento, Observer, State, Strategy, Template Method, Visitor.

### Other READMEs

- **FR-004** `baseline/README.md`: what CornerStore is, what the trading day does, how to run it, why the code is messy on purpose, and a table of all 23 smells linking to `docs/smells-catalog.md` and to the pattern folder that fixes each one.
- **FR-005** Category READMEs: two or three sentences on what the category is about, then a table of its patterns (book order) with a one-line intent, the smell fixed and a link.
- **FR-006** `capstone/README.md`: keep its content; adjust headings, links and the further-reading line so it matches the rest of the set.
- **FR-007** Root `README.md`:
    - What the repo is, and why: a homage to the Gang of Four.
    - **Start here:** a suggested learning path: baseline, then the categories in book order, then comparisons, then the capstone.
    - **Quick start:** clone, build and test, with commands verified by running them.
    - **Pattern index:** all 23, grouped by category in book order, each with a one-line intent and a link.
    - **How this was built:** the repo was planned, implemented and reviewed with Microsoft HVE Core's RPI workflow. Link to `prompts/README.md` and `.copilot-tracking/`, with counts taken from `.copilot-tracking/run-all-progress.md` and `.copilot-tracking/capstone-progress.md`. Facts only.
    - Remove any note that describes the repo as incomplete.
- **FR-008** Update `tools/new-pattern.ps1` so the README stub it creates uses the FR-001 section headings.

## Acceptance criteria

- **AC-001** All 23 pattern READMEs have the FR-001 sections, in order.
- **AC-002** Every code excerpt matches the current file it links to, and every relative link resolves.
- **AC-003** Every command in a Run it or Quick start section was run successfully.
- **AC-004** Every Mermaid diagram renders on GitHub. If a validator is unavailable, check each diagram's syntax by hand and record that in the change record.
- **AC-005** No changes to any file under `src/` or `tests/`, to any `DIFF.md`, to `scenarios/` or to `prompts/`. Under `baseline/`, only `baseline/README.md` may be created or changed.
- **AC-006** `dotnet build GangOfFour.slnx` and `dotnet test GangOfFour.slnx` pass, using the commands in `ci.yml`, and `tools/new-pattern.ps1` still passes a scratch-pattern check under `pwsh` and `powershell.exe` (scratch removed afterwards).
- **AC-007** Plain, direct prose: short sentences, no em dashes, no marketing language. No text or diagrams reproduced from the GoF book (D-12).

## Boundaries

- Do not commit, push or tag.
- Do not change `.github/copilot-instructions.md` or `.github/instructions/`.

## Suggested phases

1. **P01** Rewrite `patterns/structural/facade/README.md` as the exemplar for FR-001. **Stop** so the human can review tone and structure before the other 22 are written.
2. **P02** Creational patterns (5).
3. **P03** Structural patterns (the other 6).
4. **P04** Behavioral patterns (11).
5. **P05** Baseline, category and capstone READMEs (FR-004 to FR-006), and the `new-pattern.ps1` stub (FR-008).
6. **P06** Root README (FR-007).
7. **P07** Verification: excerpts, links, commands, diagrams, build and test.

## Checkpoints

- After P01, for the exemplar review.
- When blocked, or when context runs low (stop with a one-line resume note).
- After review, so the human can commit and push.
