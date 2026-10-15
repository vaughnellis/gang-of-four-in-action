---
description: "Implementation plan for the baseline task: CornerStore app with 23 planted smells, golden master, and copy/diff tools"
applyTo: ".copilot-tracking/changes/2026-10-15/baseline-changes.md"
---
<!-- markdownlint-disable-file -->

# Plan: baseline

Task slug: `baseline`
Date: 2026-10-15
Difficulty: Challenging (about 55 source files, 23 planted smells that are copied 23 times, ambiguity in two tooling areas that probes resolved)
Execution model: direct implementation in the agent context, one phase at a time, stop after every phase for human review and commit. No subagents for code; a subagent runs commands when terminal output is unreliable.

Related files:

* Research: `.copilot-tracking/research/2026-10-15/baseline-research.md`
* Details: `.copilot-tracking/details/2026-10-15/baseline-details.md`
* Planning log: `.copilot-tracking/plans/logs/2026-10-15/baseline-log.md`
* Critique: `.copilot-tracking/plans/critiques/2026-10-15/baseline-critique.md`

## User Requests

| ID | Request | Source |
| --- | --- | --- |
| UR-1 | Take the task through planning, implementation and review | Chat request |
| UR-2 | Treat project brief Decisions D-01 to D-12 as settled, no re-research | Chat request |
| UR-3 | Treat the foundations log decisions as settled: xUnit v3, LF endings, plain-H1 markdown. The Verify part is superseded by UR-11 | Chat request |
| UR-4 | Respect the project brief Global boundaries and this brief's Boundaries | Chat request |
| UR-5 | Use P01 to P08 as the starting point for the plan | Chat request |
| UR-6 | Stop after planning so the plan and critique can be read before any code is written | Chat request |
| UR-7 | Stop after every implementation phase for review and commit, not only at the brief's Stop points | Chat request |
| UR-8 | At each phase stop, list every SMELL marker planted in that phase with its file path, and confirm the pattern is not already applied at that site | Chat request |
| UR-9 | FR-001 to FR-018 and AC-001 to AC-008 from `prompts/02-baseline.md` | Task brief |
| UR-10 | After review, stop so the human can commit, push and create the `baseline-v1` tag | Task brief |
| UR-11 | Round 2 decisions: drop Verify (D-08, FR-013 and FR-014 now say plain xUnit v3 string comparison with `scenarios/expected/baseline.txt`, `baseline.received.txt` on mismatch, `GOLDEN_MASTER_APPROVE=1` to approve); remove Verify from `Directory.Packages.props` and declare no fee status; 11 phases with a stop after each; cross-edition scripts tested under `pwsh` and `powershell.exe`; scenario grows per phase and only the final scenario phase approves `baseline.txt`; size budget is a ceiling, not a target; `.gitignore` negation for `plans/logs/` plus `*.received.txt`; `global.json` runner setting in P01. Update plan, details and log, re-run the critique, and proceed to P01 only if it passes | Chat request, 2026-10-15 |

## Objectives

* Build CornerStore, a small but realistic console store whose shape is deliberately wrong and whose behaviour is exactly right.
* Plant all 23 smells at the sites the pattern map describes, one `// SMELL Sxx:` marker per site, with no pattern already applied at any seam.
* Make the whole trading day deterministic and approve one transcript that every future pattern solution must reproduce byte for byte.
* Give later tasks working copy and diff tooling and a CI check that keeps every `DIFF.md` fresh.

## Context summary

Instructions and rules applied:

* `.github/copilot-instructions.md`: the mess is intentional; one pattern, one seam; names never change between copies; determinism rules; transcript records business outcomes only; hands off `prompts/` and `.github/instructions/`; no push or tag; no em dashes in prose.
* `.github/instructions/baseline.instructions.md`: plant smells only as the pattern map describes, no pattern at a smell site, bugs get the smallest fix.
* `.github/instructions/patterns.instructions.md`: not edited. `patterns/` stays empty except for the temporary AC-007 scratch folder.
* hve-core `markdown.instructions.md` and `writing-style.instructions.md` for `docs/smells-catalog.md`: plain H1 and no frontmatter per the repo rule (foundations DL-7), asterisk lists, no em dashes.
* hve-core `powershell.instructions.md` for the two scripts, and `csharp.instructions.md` and `csharp-tests.instructions.md` for C# (applied as far as they do not clash with the repo rules; see the log).
* Project brief sections 3, 5, 6, 7, 8 and 10.

Environment facts (research E-12, E-13, updated for round 2): Windows PowerShell 5.1 is installed. The user says PowerShell 7 is now installed, but `pwsh` was not found on PATH, in `Program Files`, in `WindowsApps` or in the machine and user PATH when checked (critique C-14), so P11 re-checks before any script work. Git is on `main` at `9169740` with no tags. The working tree has only untracked `.copilot-tracking` files.

## Decisions made in this plan

| ID | Decision | Reason |
| --- | --- | --- |
| PD-1 | Re-sequence the brief's P01 to P08 into 11 dependency-ordered phases (mapping below). Content and smell assignments stay the same, except S05 moves to P01 and the S23 tax and weight sites land in P04 | Checkout needs pricing, payments, inventory and notifications. The cart widgets need pricing, promos and shipping. Every phase must build green and be committable. See critique C-3 |
| PD-2 | `global.json` gains `"test": { "runner": "Microsoft.Testing.Platform" }` in P01. Confirmed by the user | Research E-6. SDK 10 rejects VSTest for xUnit v3. Required for `dotnet test` in CI |
| PD-3 | Verify is dropped. `Directory.Packages.props` loses the `Verify.XunitV3` entry. No `Verify_*` fee property is declared anywhere. The test project references `xunit.v3`, `xunit.runner.visualstudio` and `Microsoft.NET.Test.Sdk` only | User decision C-1, D-08 as updated. Research E-1 to E-5 no longer apply. `Microsoft.Extensions.TimeProvider.Testing` stays pinned (a foundations FR-003 entry) but is referenced only when a test needs it |
| PD-4 | The golden master is a plain xUnit v3 string comparison against `scenarios/expected/baseline.txt` (P10). On mismatch the test writes the actual text to `scenarios/expected/baseline.received.txt` and fails with a short summary of the first differing line. With `GOLDEN_MASTER_APPROVE=1` it writes `baseline.txt` and does not compare. `.gitignore` carries `*.received.txt` | User decision C-2, D-08, FR-013, FR-014. Briefs and instructions already agree, so no human wording fix is pending |
| PD-5 | All C# lives in the single namespace `CornerStore`, organised in folders per feature area. Test namespace is `CornerStore.Tests` | Brief D-05 names one namespace per project. Keeps pattern diffs free of `using` noise |
| PD-6 | Source files in `baseline/src` are ASCII only; money is `decimal`; all text is formatted with the invariant culture; the transcript writes `\n` explicitly | Keeps transcripts and `DIFF.md` identical on Windows and Linux and avoids PowerShell 5.1 code page problems |
| PD-7 | The test project disables test parallelisation with an assembly attribute | S05 `Config` and `Log` are static mutable state shared by every run. Pattern copies add test classes later, so the switch must exist from the start |
| PD-8 | `scenarios/trading-day.json` grows phase by phase. P10 completes the coverage required by FR-012, adds `GoldenMasterTests` and is the only phase that approves `baseline.txt` | Every phase stop can show a transcript excerpt of its own smells. Earlier phases never write `scenarios/expected/` |
| PD-9 | Baseline tests use only `ScenarioRunner.Run`. P01 adds a smoke test with an inline scenario; P10 adds the golden master and a determinism test | Brief section 7. Pattern refactors rename internals, so internal-facing tests would break in every copy |
| PD-10 | An interface in the baseline needs a written reason in the changes log. The only planned one is `IPaymentProcessor`, the product type that S03 Factory Method will later create | No pattern may be applied at a seam, but a shared product type is not a creator |
| PD-11 | `yield`, C# `event`, `IObservable<T>`, `IEnumerable<T>` implementations and `Lazy<T>` do not appear at the S16, S19 and S05 seams | These are the modern forms those patterns teach |
| PD-12 | Both scripts are written to run on Windows PowerShell 5.1 and PowerShell 7 (no ternary, `??`, `&&` or `-Parallel`). In P11 they are tested under both `pwsh` and `powershell.exe`, and CI runs them with `pwsh` | User decision C-4. FR-016 asks for PowerShell 7. If `pwsh` is still not found at P11, P11 stops and asks (critique C-14) |
| PD-13 | Size budget for AC-008, a ceiling and not a target: at most 60 source files and about 2,800 lines in `baseline/src`; at most 9 files in any feature folder; any method over 40 lines needs a smell reason. Smaller is better | Keeps the baseline skimmable in one sitting. Expected actual size is well under the ceiling |
| PD-14 | P01-S1 edits `.gitignore`: add `!.copilot-tracking/plans/logs/` so planning logs are committed, and replace the Verify-specific `*.received.*` and `*.received/` entries with `*.received.txt` | The `[Ll]ogs/` rule ignores the logs today (log DL-13, critique C-13). Verify is gone. User decision C-13 |
| PD-15 | `.gitattributes` keeps its `*.received.* -diff` line. It is harmless and an edit buys nothing | Smallest change. Mentioned so the Verify comment there is not mistaken for a leftover dependency |

Proposed additions beyond the brief:

| ID | Addition | Default |
| --- | --- | --- |
| PA-1 | Small stop report at every phase (smell marker table, pattern-leak scan, build and test evidence, transcript excerpt, suggested commit message) | Include. Required by UR-8 |
| PA-2 | A pattern-leak scan command run at every stop: searches `baseline/src` for `interface `, `Factory`, `Builder`, `Strategy`, `Observer`, `Command`, `Visitor`, `Mediator`, `Memento`, `State`, `Proxy`, `Decorator`, `Adapter`, `Bridge`, `Composite`, `Flyweight`, `Iterator`, `Interpreter`, `Template`, `Prototype`, `Singleton`, `Lazy<`, `yield`, ` event ` | Include |
| PA-3 | Independent recomputation of several order totals (tax, shipping, discount) in the review log, so the approved transcript is checked for correctness, not just approved | Include |

## Phase map to the brief

| Phase | Brief phase | Content | Smells planted |
| --- | --- | --- | --- |
| P01 | P01 | Shell, entry point, determinism, transcript, `Config` and `Log`, test project | S05 |
| P02 | P02 | Catalog, variants, bundles, bulk import, category walks | S04, S08, S11, S16 |
| P03 | P03 (part) | Customers, cart, cart undo | S18 |
| P04 | P04 (part) | Regions, pricing, promo rules, shipping cost, tax and weight by line type | S01, S09, S15, S21, S23 (tax and weight sites) |
| P05 | P03 (part) | Cart widgets | S17 |
| P06 | P05 (part) | Notifications, inventory | S07, S12 (inventory sites) |
| P07 | P05 (part) | Orders, order lifecycle, back office | S02, S14, S19, S20, S12 (back office sites) |
| P08 | P03 and P04 (part) | Checkout: validation, payments, courier SDK, orchestration | S03, S06, S10, S13 |
| P09 | P06 | Reporting and exporters | S22, S23 (export site) |
| P10 | P07 | Scenario completion, golden master, approved transcript | none |
| P11 | P08 | Smells catalog, tools, CI diff check, AC-007 tool check | none |

Stops: after every phase (UR-7). The brief's own Stop points are P01 and the end of review.

## Implementation checklist

Every phase ends with the same closing step, written once here: run restore, build (Release, zero warnings) and test with the CI flags; run the pattern-leak scan (PA-2); list every SMELL marker planted in the phase as a table of ID, file path, member and shape; state for each that its pattern is not applied; show a transcript excerpt; append the phase to the changes log; propose a commit message; STOP.

### Phase P01: App shell and determinism (FR-001 to FR-004, S05) <!-- parallelizable: false -->

* [x] P01-S1 Edit `global.json` (PD-2), `Directory.Packages.props` (remove `Verify.XunitV3`, PD-3) and `.gitignore` (PD-14). `Directory.Build.props` is not edited
* [x] P01-S1a Read hve-core `csharp.instructions.md` and `csharp-tests.instructions.md`; record conflicts with repo rules in the changes log (DL-11)
* [x] P01-S2 Create `baseline/src/CornerStore/CornerStore.csproj` (Exe, `net10.0`, no packages)
* [x] P01-S3 Create `Program.cs`, `ScenarioRunner.cs`, `Scenario.cs`, `ScenarioClock.cs`, `IdSequence.cs` and a `StoreConsole.cs` dispatcher with the first ops (`advance`, `config.set`)
* [x] P01-S4 Create `Config.cs` and `Log.cs` as static classes with public mutable fields (S05); `ScenarioRunner.Run` resets and sets them on every call
* [x] P01-S5 Create `baseline/tests/CornerStore.Tests` (xUnit v3 Exe), `AssemblyAttributes.cs` (PD-7) and `ScenarioRunnerTests.cs` (smoke and determinism on an inline scenario)
* [x] P01-S6 Create `baseline/CornerStore.Baseline.slnx`; add both projects to `GangOfFour.slnx` under `baseline`
* [x] P01-S7 Closing step. STOP (the brief's first Stop point)

### Phase P02: Catalog (S04, S08, S11, S16) <!-- parallelizable: false -->

* [x] P02-S1 Product types: `Product`, `PhysicalProduct`, `DigitalProduct`, `GiftCardProduct`, `Category`, `Bundle`
* [x] P02-S2 `Catalog` with lookup, nested categories, `CreateVariant` (S04, field-by-field copy), `PriceOf` and bundle rules with the nesting refusal (S08), and three copy-pasted recursive walks (S16)
* [x] P02-S3 `Listing` and `ListingImporter` generating 100,000 listings with per-listing string copies (S11); summary count only in the transcript
* [x] P02-S4 Ops `browse`, `variant`, `bundle.create`, `bundle.add`, `import`; first steps in `scenarios/trading-day.json`
* [x] P02-S5 Closing step. STOP

### Phase P03: Customers, cart and undo (S18) <!-- parallelizable: false -->

* [x] P03-S1 `Customer`, `Cart`, `CartLine`, `CartService` (add, remove, bundle expansion into lines)
* [x] P03-S2 Undo by public cart internals copied by the caller (S18)
* [x] P03-S3 Ops `register`, `cart.add`, `cart.remove`, `cart.undo`, `cart.show`; extend the scenario
* [x] P03-S4 Closing step. STOP

### Phase P04: Regions, pricing, promos, shipping cost, tax and weight (S01, S09, S15, S21, S23) <!-- parallelizable: false -->

* [ ] P04-S1 `TaxCalculator`, `ShippingCalculator` and `MoneyFormatter` with scattered `region == "AU"` branches (S01); line-type tax switch (S23)
* [ ] P04-S2 `WeightCalculator` with the line-type switch (S23)
* [ ] P04-S3 `PricingService` and `Quote`: add-on flags and if-chain (S09), shipping cost `switch` on method (S21)
* [ ] P04-S4 `PromoEngine` parsing rules with `Split` and `IndexOf` (S15)
* [ ] P04-S5 Ops `cart.addon`, `cart.promo`, `cart.shipping`, `quote`; extend the scenario
* [ ] P04-S6 Closing step. STOP

### Phase P05: Cart widgets (S17) <!-- parallelizable: false -->

* [ ] P05-S1 `ItemListWidget`, `PromoBoxWidget`, `ShippingSelectorWidget`, `TotalsPanelWidget`, referencing and updating each other directly (S17)
* [ ] P05-S2 Route the cart ops from P03 and P04 through the widgets; extend the scenario
* [ ] P05-S3 Closing step. STOP

### Phase P06: Notifications and inventory (S07, S12) <!-- parallelizable: false -->

* [ ] P06-S1 `EmailSender`, `SmsSender` fakes and the four message classes (S07)
* [ ] P06-S2 `InventoryService` with a simulated, uncached lookup cost and an unguarded stock mutator (S12)
* [ ] P06-S3 Closing step. STOP

### Phase P07: Orders, lifecycle and back office (S02, S12, S14, S19, S20) <!-- parallelizable: false -->

* [ ] P07-S1 `Order` and `Invoice` with 11-parameter constructors (S02)
* [ ] P07-S2 `OrderService`: string `Status`, if/else transitions (S20), direct calls to email, stock, loyalty and analytics after each change (S19); `LoyaltyService`, `AnalyticsService`
* [ ] P07-S3 `BackOffice`: inline stock, refund, cancel and lookup actions (S14) with caller-side role checks (S12)
* [ ] P07-S4 Ops `order.ship`, `order.deliver`, `admin.stock.adjust`, `admin.stock.lookup`, `admin.refund`, `admin.cancel`
* [ ] P07-S5 Closing step. STOP

### Phase P08: Checkout (S03, S06, S10, S13) <!-- parallelizable: false -->

* [ ] P08-S1 `IPaymentProcessor`, `CardProcessor`, `PayPalProcessor`, `AfterpayProcessor` fakes
* [ ] P08-S2 `ShipFastClient` fake with XML strings and prices in cents
* [ ] P08-S3 `FraudScorer` and `OrderValidator` with one long nested-if method (S13)
* [ ] P08-S4 `StoreConsole.Checkout`: fixed-order orchestration (S10) with a payment `switch` (S03) and a direct courier call (S06)
* [ ] P08-S5 Op `checkout`; extend the scenario
* [ ] P08-S6 Closing step. STOP

### Phase P09: Reporting (S22, S23 export site) <!-- parallelizable: false -->

* [ ] P09-S1 `ReportService`, `CsvReportExporter`, `JsonReportExporter` with duplicated skeletons (S22) and a line-type switch for export rows (S23)
* [ ] P09-S2 Report totals by line item type; ops `report.csv`, `report.json`
* [ ] P09-S3 Closing step. STOP

### Phase P10: Scenario, golden master, approved transcript (FR-012 to FR-014) <!-- parallelizable: false -->

* [ ] P10-S1 Complete `scenarios/trading-day.json` against the FR-012 coverage matrix in the details file
* [ ] P10-S2 `RepositoryRoot.cs`, `GoldenMasterTests.cs` (plain string comparison, received file on mismatch, `GOLDEN_MASTER_APPROVE=1` approval, first-differing-line summary), `DeterminismTests.cs`
* [ ] P10-S3 Run once without approval (expect failure: no approved file), review the received transcript by hand, recompute sample totals independently (PA-3), then run once with `GOLDEN_MASTER_APPROVE=1` to write `scenarios/expected/baseline.txt`, delete any received file, and run again without the variable to confirm a pass
* [ ] P10-S4 Run twice and confirm byte-identical output (AC-002); confirm `baseline.txt` has LF endings and no BOM; mutate one transcript character temporarily to prove the mismatch path writes `baseline.received.txt` and reports the right line, then revert
* [ ] P10-S5 Closing step. The stop report tells the human to leave `GOLDEN_MASTER_APPROVE` unset in pattern tasks (critique C-15). STOP

### Phase P11: Catalog doc, tools, CI (FR-015 to FR-018, AC-007) <!-- parallelizable: false -->

* [ ] P11-S1 Fill `docs/smells-catalog.md`: 23 entries with ID, pattern, file paths, why it hurts, scenario steps
* [ ] P11-S1a Re-check `pwsh --version` (critique C-14). If it is missing, STOP and ask. Read hve-core `powershell.instructions.md`
* [ ] P11-S2 `tools/new-pattern.ps1` and `tools/diff-baseline.ps1` (PD-12)
* [ ] P11-S3 Extend `.github/workflows/ci.yml`: regenerate every `DIFF.md` with `pwsh` and fail on any difference
* [ ] P11-S4 AC-007 tool check with `-Category structural -Name scratch-check`, run once under `pwsh` and once under `powershell.exe` (delete the scratch folder and revert the umbrella entries between runs); the folder must build and pass the golden master and `diff-baseline.ps1` must report no differences; finally delete the scratch folder and revert the umbrella entries
* [ ] P11-S5 Update the README Status section; closing step. STOP

### Review (RPI Phase 4) <!-- parallelizable: false -->

* [ ] RV-S1 Check FR-001 to FR-018 and AC-001 to AC-008 with evidence; run AC-003 and AC-006 searches; independently recompute totals; write the review log; STOP so the human can commit, push and create `baseline-v1`

## Dependencies

* .NET SDK 10.0.302 (installed), packages pinned in `Directory.Packages.props`.
* Windows PowerShell 5.1 locally and PowerShell 7 (`pwsh`) for P11; `pwsh` on the CI runner.
* Skills: none loaded. The `run-tests` skill description (`dotnet test` modes) matches the MTP finding in research E-6.

## Success criteria

* AC-001 to AC-008 met with recorded evidence.
* Every phase stop delivers the smell marker table and the pattern-leak confirmation.
* No edits to `prompts/`, `.github/copilot-instructions.md`, `.github/instructions/` or anything under `patterns/` outside the scratch check.
* Nothing committed, pushed or tagged by the agent.
