# Task: comparisons

**Task slug:** `comparisons`
**Shared context:** `prompts/00-project-brief.md` (read it first)
**Depends on:** all 23 pattern tasks Done, and `GangOfFour.slnx` building in CI.

## Goal

Write `docs/comparisons.md`: a guide to the pattern pairs readers most often confuse, explained with real code from this repository rather than abstract examples. Each comparison should leave a reader able to say which pattern they need and why.

## Requirements

- **FR-001** Cover these four pairs, which are already stubbed as headings:
    - Strategy vs State
    - Facade vs Mediator
    - Decorator vs Proxy
    - Command vs Memento (for undo)
- **FR-002** Add these four pairs, in this order after the first four:
    - Factory Method vs Abstract Factory
    - Adapter vs Bridge
    - Composite vs Decorator
    - Template Method vs Strategy
- **FR-003** Each comparison has the same shape:
    1. **The one-line difference.** A single sentence a reader can remember.
    2. **Side-by-side table.** Rows: intent, what varies, who chooses the variation and when, how the participants relate, and the CornerStore smell each one fixed.
    3. **In CornerStore.** A short excerpt (at most about 15 lines) from each pattern's solution, with a relative link to the file it comes from.
    4. **How to tell which you need.** Two or three plain questions to ask about your own code.
    5. **Common mistake.** One sentence on the confusion people fall into.
- **FR-004** Open the document with a short introduction (three sentences at most) and a linked list of the eight comparisons.
- **FR-005** In each compared pattern's `README.md`, add a link to its comparison section under "Related patterns and common confusions". Change nothing else in those READMEs.
- **FR-006** Link `docs/comparisons.md` from the root `README.md`.

## Acceptance criteria

- **AC-001** All eight comparisons are present and each follows the FR-003 shape.
- **AC-002** Every code excerpt matches the current file it links to, and every link resolves.
- **AC-003** No text or diagrams are reproduced from the GoF book (D-12). Intents are paraphrased.
- **AC-004** Prose is plain and direct, with no em dashes, matching the writing conventions in `.github/copilot-instructions.md`.
- **AC-005** Only `docs/comparisons.md`, the root `README.md` and the compared patterns' `README.md` files change. No code, test or `DIFF.md` changes.
- **AC-006** `dotnet build GangOfFour.slnx` and `dotnet test GangOfFour.slnx` still pass.

## Boundaries

- Do not change any file under `src/` or `tests/` in any folder.
- Do not regenerate or edit any `DIFF.md`.
- Do not modify anything under `prompts/` or `.github/`.
- Do not commit, push or tag. Stop so the human can commit.

## Suggested phases

1. **P01** Gather the excerpts and facts for all eight pairs from the pattern folders and their reviews.
2. **P02** Write the four stubbed comparisons (FR-001, FR-003, FR-004).
3. **P03** Write the four added comparisons (FR-002).
4. **P04** README links (FR-005, FR-006), link and excerpt checks, build and test.

## Stop point

After review, stop so the human can commit and push.