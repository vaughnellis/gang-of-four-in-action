# Task: baseline

**Task slug:** `baseline`
**Shared context:** `prompts/00-project-brief.md` (read it first, especially sections 5, 6 and 7)
**Depends on:** `foundations` reviewed and committed.

## Goal

Build CornerStore, the deliberately messy but fully working baseline app, with all 23 smells from the pattern map planted and marked, a scripted trading day, an approved golden master transcript, and the tools that copy and diff the baseline for each pattern.

This is the most important task in the repo. Every pattern solution is a copy of what this task produces, so mistakes here are copied 23 times.

## Requirements

### App shell

- **FR-001** Create `baseline/src/CornerStore` (console app) and `baseline/tests/CornerStore.Tests` (xUnit), and `baseline/CornerStore.Baseline.slnx` referencing both. Add both projects to `GangOfFour.slnx` under the `baseline` folder.
- **FR-002** Expose a single public entry point, `ScenarioRunner.Run(string scenarioPath, TextWriter output)`. `Program.Main` only parses arguments and calls it.
- **FR-003** Make the app deterministic per D-09: a `TimeProvider` set to the scenario's `startTime`, sequential order and customer IDs, `Random` seeded from the scenario, invariant culture for all formatting.
- **FR-004** The transcript records business outcomes only: actions, state changes, totals, messages sent, validation failures. It never records timings, memory use, call counts or hash codes. (This lets Proxy, Flyweight and Command add behaviour without changing the transcript.)

### Features and smells

- **FR-005** Implement every feature area in section 5 of the project brief.
- **FR-006** Plant each of the 23 smells exactly as described in the pattern map (section 6), at a realistic location, with a `// SMELL Sxx: <short description>` marker at each site.
- **FR-007** The seams listed in the pattern map must not already use their pattern. For example, there is no `IShippingCostStrategy`, no state classes for orders, no factory for payment processors.
- **FR-008** External systems are in-process fakes that write to the transcript: card, PayPal and Afterpay payments; the `ShipFastClient` courier SDK (with its own awkward API: XML strings, prices in cents); email and SMS senders.
- **FR-009** The slow `InventoryService` simulates latency by counting a cost internally, never with real sleeps. The cost is not written to the transcript.
- **FR-010** Cart "widgets" for the Mediator smell (S17) are plain classes (item list, promo box, shipping selector, totals panel) that render lines into the transcript.
- **FR-011** The bulk listing import for S11 generates 100,000 listings deterministically in memory and reports only a summary count to the transcript.

### Scenario and golden master

- **FR-012** Write `scenarios/trading-day.json` so the day exercises every smell site at least once, including: both regions (AU and NZ), all three payment types, every shipping method, every add-on, a nested category walk, a bundle and a nested bundle attempt, variant creation, at least two promo rules, a cart undo, widget updates, each validation rule failing at least once, every back-office action including one unauthorised attempt, a cancel, a refund, an illegal order transition attempt, both report exports, and every line item type.
- **FR-013** Write `GoldenMasterTests` that locates the repo root by walking up to `GangOfFour.slnx`, runs `scenarios/trading-day.json` through `ScenarioRunner.Run`, and compares the output with `scenarios/expected/baseline.txt` using Verify (D-08). The test file must contain no baseline-specific paths, so it works unchanged in every copy.
- **FR-014** Generate and approve `scenarios/expected/baseline.txt`.

### Docs and tools

- **FR-015** Fill `docs/smells-catalog.md`: one entry per smell with ID, pattern, file path(s), a two-to-three line description of why it hurts, and the scenario steps that exercise it.
- **FR-016** Write `tools/new-pattern.ps1 -Category <creational|structural|behavioral> -Name <kebab-name>` (PowerShell 7). It copies `baseline/src` and `baseline/tests` into `patterns/<category>/<name>/`, creates `<PascalName>.slnx` referencing the copied projects, adds them to `GangOfFour.slnx` under the category folder, and creates a `README.md` stub with the eight template headings from section 8 of the project brief. It refuses to overwrite an existing folder.
- **FR-017** Write `tools/diff-baseline.ps1 -Category <category> -Name <kebab-name>`. It writes `DIFF.md` in the pattern folder with a file-level summary and the full diff of `src` and `tests` against `baseline`.
- **FR-018** Extend `.github/workflows/ci.yml` to regenerate every pattern's `DIFF.md` and fail if any differs from the committed file.

## Acceptance criteria

- **AC-001** `dotnet build GangOfFour.slnx` succeeds with zero warnings, and `dotnet test` passes.
- **AC-002** Running the scenario twice produces byte-identical transcripts.
- **AC-003** A search for `SMELL S` finds markers for all 23 IDs, S01 to S23, and no others.
- **AC-004** For each smell, the location and shape match the pattern map's description, and the pattern is not already applied there.
- **AC-005** Every smell site is exercised by the scenario (each catalog entry names its scenario steps).
- **AC-006** No `DateTime.Now`, `DateTime.UtcNow`, `Guid.NewGuid()`, unseeded `Random` or `Thread.Sleep` in `baseline/src`.
- **AC-007** Tool check: running `new-pattern.ps1 -Category structural -Name scratch-check` creates a folder that builds and passes the golden master, and `diff-baseline.ps1` on it reports no differences. The scratch folder and its umbrella entries are then removed.
- **AC-008** A reader can skim `baseline/src` in one sitting. If any feature needs more than a handful of classes to show its smell, simplify it and record why in the change record.

## Boundaries

- Behaviour must be correct. The mess is in the shape of the code, never in its results.
- Do not create anything under `patterns/` except the temporary scratch check in AC-007.
- Do not modify anything under `prompts/`.
- Do not push or create the `baseline-v1` tag.

## Suggested phases

1. **P01** App shell, entry point, determinism and transcript writer (FR-001 to FR-004).
2. **P02** Catalog: S04, S08, S11, S16.
3. **P03** Cart and checkout: S02, S10, S13, S17, S18.
4. **P04** Pricing, promotions, payments, shipping and regions: S01, S03, S06, S09, S15, S21.
5. **P05** Order lifecycle, notifications, back office, configuration and logging: S05, S07, S12, S14, S19, S20.
6. **P06** Reporting: S22, S23.
7. **P07** Scenario, golden master and approved transcript (FR-012 to FR-014).
8. **P08** Smells catalog, tools and CI diff check (FR-015 to FR-018), then the AC-007 tool check.

## Stop points

- After P01, stop so the human can review the shell and commit.
- After review, stop so the human can commit, push and create the `baseline-v1` tag.
