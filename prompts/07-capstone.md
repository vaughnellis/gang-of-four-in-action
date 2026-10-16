# Task: capstone

**Task slug:** `capstone`
**Shared context:** `prompts/00-project-brief.md` (read it first, especially sections 5 to 7)
**Depends on:** `comparisons` committed, and CI green on `main`.

## Goal

Build `capstone/`: one copy of the baseline with all 23 patterns applied together, so readers can see the patterns working side by side in one codebase. The transcript must stay identical to the baseline's, and every `SMELL` marker must be gone by the end.

Each pattern folder shows one pattern on its own. The capstone shows how they fit together, including where two patterns meet in the same code.

## Requirements

### Setup

- **FR-001** Copy `baseline/src` and `baseline/tests` into `capstone/src` and `capstone/tests` unchanged. Create `capstone/Capstone.slnx` referencing both projects. Add both to `GangOfFour.slnx` in their own `capstone` solution folder. Confirm the build passes and the golden master passes, then commit the unchanged copy.
- **FR-002** Project and namespace names stay `CornerStore` and `CornerStore.Tests` (D-05).

### Integration

- **FR-003** Apply the 23 patterns in the order below, **one commit per pattern**. Use each pattern folder's solution as the reference, but integrate rather than paste: the code must compile and read as one design.
    1. Cross-cutting: Singleton (S05). This comes first because it touches the most files.
    2. Catalog: Prototype (S04), Composite (S08), Iterator (S16), Flyweight (S11)
    3. Regions and pricing: Abstract Factory (S01), Strategy (S21), Decorator (S09), Interpreter (S15)
    4. Payments and shipping: Factory Method (S03), Adapter (S06)
    5. Checkout: Builder (S02), Chain of Responsibility (S13), Facade (S10)
    6. Cart: Memento (S18), Mediator (S17)
    7. Order lifecycle and notifications: State (S20), Observer (S19), Bridge (S07)
    8. Back office: Proxy (S12), Command (S14)
    9. Reporting: Template Method (S22), Visitor (S23)
- **FR-004** After every pattern: build in Release with zero warnings, run all tests, and confirm the golden master passes. Remove only that pattern's `SMELL` markers. Commit with a message such as `capstone: apply Strategy (S21)`.
- **FR-005** Where two patterns change the same code (for example Facade calling the Chain of Responsibility validators, Observer subscribers firing on State transitions, or Template Method and Visitor in reporting), integrate them so both patterns stay recognisable. Record each meeting point and how it was resolved in the change record.
- **FR-006** Port each pattern's own tests from its folder into `capstone/tests`, adapted to the combined code. `GoldenMasterTests.cs` stays unchanged.

### Write-up

- **FR-007** Write `capstone/README.md`:
    - What the capstone is and how it differs from the pattern folders.
    - A table of all 23 patterns: smell, where the pattern lives in the capstone (file paths), and a link to its own pattern folder.
    - **Where patterns meet:** one short paragraph per meeting point from FR-005.
    - A Mermaid diagram of the main flow from checkout to order lifecycle, showing which pattern handles each step.
    - **Reading the history:** the 24 commits (copy plus 23 patterns) as a step-by-step tour, with a one-line note on each.
- **FR-008** Link the capstone from the root `README.md`.

## Acceptance criteria

- **AC-001** A search for `SMELL S` under `capstone/src` finds nothing.
- **AC-002** The golden master passes and `GoldenMasterTests.cs` is unchanged from the baseline.
- **AC-003** `dotnet build GangOfFour.slnx` in Release has zero warnings, and `dotnet test GangOfFour.slnx` passes, using the commands in `ci.yml`.
- **AC-004** There are 24 capstone commits, in the FR-003 order, each building and passing on its own.
- **AC-005** Every pattern named in the README table can be found at the paths given.
- **AC-006** Prose is plain and direct, with no em dashes. No text or diagrams reproduced from the GoF book (D-12).

## Boundaries

- Do not change `baseline/`, `scenarios/`, `prompts/`, any pattern folder, `.github/copilot-instructions.md` or `.github/instructions/`.
- Allowed outside `capstone/`: the capstone entries in `GangOfFour.slnx`, the capstone link in the root `README.md`, and files under `.copilot-tracking/`.
- If integrating a pattern would change the transcript, stop and report it rather than editing the approved transcript.

## Permissions for this task

- Make local commits as described in FR-001 and FR-004.
- No tags, no pushing.

## Progress file

Maintain `.copilot-tracking/capstone-progress.md`: one row per step (copy plus 23 patterns) with status, commit hash and test count. If you start in a fresh chat, read it first and resume from the first step that is not Done.

## Checkpoints (stop and wait for the human)

- **When blocked:** a build or test still failing after two fix attempts, or a pattern that cannot be integrated without changing the transcript.
- **When context runs low:** update the progress file, then stop with a one-line resume note.
- **At the end:** after the review, so the human can push and check CI.
