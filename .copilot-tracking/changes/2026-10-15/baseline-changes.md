<!-- markdownlint-disable-file -->
# Changes: baseline

Plan: `.copilot-tracking/plans/2026-10-15/baseline-plan.instructions.md`
Details: `.copilot-tracking/details/2026-10-15/baseline-details.md`
Planning log: `.copilot-tracking/plans/logs/2026-10-15/baseline-log.md`
Date started: 2026-10-15

## Summary

P02 of 11 complete (P01 committed by the human): catalog, variants, bundles, bulk import, category walks (S04, S08, S11, S16) and the first scenario steps. Nothing pushed or tagged by the agent.

## Phase P01: App shell and determinism

Added

* `baseline/CornerStore.Baseline.slnx`
* `baseline/src/CornerStore/CornerStore.csproj` (Exe, `net10.0`, no packages)
* `baseline/src/CornerStore/Program.cs` (parses at most one path argument, defaults to `scenarios/trading-day.json` found by walking up to `GangOfFour.slnx`, calls `ScenarioRunner.Run`)
* `baseline/src/CornerStore/ScenarioRunner.cs` (the one public type; loads the scenario, resets and sets `Config` and `Log`, writes the header, dispatches steps)
* `baseline/src/CornerStore/Scenario.cs` (JSON loader: name, startTime, seed, optional config block, steps)
* `baseline/src/CornerStore/ScenarioClock.cs` (`TimeProvider` fixed at `startTime`, UTC, `Advance`)
* `baseline/src/CornerStore/IdSequence.cs` (`cu-1`, `o-1001`, `SF-0001`; instance counters; not used until P03)
* `baseline/src/CornerStore/StoreConsole.cs` (step dispatcher with ops `advance` and `config.set`; unknown ops print `unknown op` and continue)
* `baseline/src/CornerStore/Config.cs`, `Log.cs` (S05)
* `baseline/tests/CornerStore.Tests/CornerStore.Tests.csproj` (xUnit v3 Exe; `xunit.v3`, `xunit.runner.visualstudio`, `Microsoft.NET.Test.Sdk`)
* `baseline/tests/CornerStore.Tests/AssemblyAttributes.cs` (test parallelisation off, PD-7)
* `baseline/tests/CornerStore.Tests/ScenarioRunnerTests.cs` (3 tests: header and step blocks, determinism, unknown setting continues)

Modified

* `global.json`: added `"test": { "runner": "Microsoft.Testing.Platform" }` (PD-2). Edits a foundations file; no boundary forbids it
* `Directory.Packages.props`: removed `Verify.XunitV3` (PD-3)
* `.gitignore`: replaced the Verify block with `*.received.txt`; appended `!.copilot-tracking/plans/logs/` so planning logs are committed (PD-14). Both rules checked with `git check-ignore`
* `GangOfFour.slnx`: both baseline projects added under the `baseline` folder

Removed

* None

Deviations and decisions

* `xunit.v3` 4.0.2 needs `<Using Include="Xunit" />` in the test project (research E-11 said implicit usings were enough; it was wrong for this package set).
* `CollectionBehavior(DisableTestParallelization = true)` is an error in xunit.v3 4.0.2. The replacement is `[assembly: Parallelization(Mode = ParallelMode.None)]` (namespaces `Xunit.Sdk` and `Xunit.v3`).
* `Program.Main` also guards the argument count and a missing file (exit codes 2 and 1). This is argument handling at the boundary, not scenario logic.
* S05 markers sit in four files: `Config.cs`, `Log.cs`, `ScenarioRunner.cs` and `StoreConsole.cs`, because the smell is "set from anywhere".
* All types except `ScenarioRunner` are `internal`, so the single public entry point (FR-002) is the only public surface.
* hve-core C# instruction conflicts, repo rules win: no XML docs on members; `LangVersion` stays `latest` from foundations; behaviour-describing test names instead of `GivenContext_WhenAction_ExpectedResult`; no NSubstitute (baseline tests use only `ScenarioRunner.Run`).
* `baseline/.gitkeep` is left in place (foundations file, harmless).
* The pattern-leak scan matched `StringBuilder` in `StoreConsole.cs`: a BCL type, not the Builder pattern.
* `Microsoft.Extensions.TimeProvider.Testing` stays pinned in `Directory.Packages.props` but is not referenced yet.

Validation (CI flags, Release)

* `dotnet restore GangOfFour.slnx`: exit 0
* `dotnet build GangOfFour.slnx --no-restore -c Release`: 0 warnings, 0 errors; also clean with `--no-incremental`
* `dotnet test GangOfFour.slnx --no-build -c Release`: 3 passed, 0 failed
* `dotnet test baseline/CornerStore.Baseline.slnx -c Release`: 3 passed
* AC-006 search in `baseline/src`: 0 hits
* Running the app twice on a temp scenario: identical output
* `baseline/src`: 8 files, 280 lines (ceiling 60 files and 2,800 lines)
* Files: LF, no BOM, ASCII only, no em dashes

## Phase P02: Catalog

Added

* `baseline/src/CornerStore/Catalog/Product.cs`, `PhysicalProduct.cs`, `DigitalProduct.cs`, `GiftCardProduct.cs`, `Category.cs`, `Bundle.cs`
* `baseline/src/CornerStore/Catalog/Catalog.cs` (12 seeded products in a Store > Footwear > Shoes > Kids tree plus Socks, Home, Kitchen, Digital, Gift Cards; seeded bundle `BUN-RUN-KIT`; `Lookup`, `PriceOf`, `CreateBundle`, `AddToBundle`, `CreateVariant`, three category walks)
* `baseline/src/CornerStore/Catalog/Listing.cs`, `ListingImporter.cs` (100,000 listings built from a deterministic formula, no `Random`)
* `baseline/tests/CornerStore.Tests/CatalogScenarioTests.cs` (6 tests through `ScenarioRunner.Run`: tree walk, variant, rejected digital variant, nested bundle rejection, bundle price, import summary only)
* `scenarios/trading-day.json` (config block and 9 steps; grows in later phases, not approved until P10)

Modified

* `baseline/src/CornerStore/StoreConsole.cs`: ops `browse`, `variant`, `bundle.create`, `bundle.add`, `import`; a `Text` helper; `Catalog`, `ListingImporter` and the listing list held as fields

Removed

* None

Deviations and decisions

* `Catalog.Lookup` returns `object?` (a `Product` or a `Bundle`) and `PriceOf` and `AddToBundle` use `is Bundle` checks. That untyped lookup is the S08 shape: bundle and product share no type. `Bundle` holds component SKUs, not objects.
* Bundle price is the component sum less `DiscountPercent` (10), rounded away from zero to two decimals.
* Rejections (nested bundle, digital variant, duplicate SKU, unknown SKU) return an error string and print `rejected: ...`; nothing throws. `PriceOf` throws only for an unknown SKU, which no scenario step can reach.
* S16 call sites: `PrintTree` and `CountProducts` are called by `browse`; `CollectSkus` is called by `import`. Each walk is a separate copy of the same recursion in `Catalog`, each with its own marker. A `FindCategory` dictionary keeps a fourth recursion out of the file.
* Prices print as plain two-decimal numbers until `MoneyFormatter` (S01) arrives in P04.
* The `import` count comes from the step (`count`, default 100000). Only `imported 100000 listings` reaches the transcript; the list is kept in a field so the copies stay alive, which is the Flyweight cost.
* Added `CatalogScenarioTests`, not named in the plan. They use only `ScenarioRunner.Run` (PD-9) and give the Composite, Prototype, Flyweight and Iterator refactors something to keep green before P10.
* New files written by the edit tool had CRLF; converted to LF.

Validation (CI flags, Release)

* `dotnet restore GangOfFour.slnx`: up to date
* `dotnet build GangOfFour.slnx --no-restore -c Release`: 0 warnings, 0 errors
* `dotnet test GangOfFour.slnx --no-build -c Release`: 9 passed, 0 failed
* Running the app on `scenarios/trading-day.json` twice: identical output; whole run about 0.2 s including process start
* AC-006 search in `baseline/src`: 0 hits
* Pattern-leak scan: one hit, `StringBuilder` in `StoreConsole.cs` (BCL type, not Builder)
* `baseline/src`: 17 files, 712 lines; `Catalog/` folder 9 files (ceilings 60 files, 2,800 lines, 9 per folder)
* Files: LF, no BOM, ASCII only

SMELL markers planted in P02

| ID | File and line | Member | Shape |
| --- | --- | --- | --- |
| S04 | `baseline/src/CornerStore/Catalog/Catalog.cs` line 141 | `CreateVariant` | Eleven fields assigned by hand into a new `PhysicalProduct`, most copied from the parent |
| S08 | `baseline/src/CornerStore/Catalog/Catalog.cs` line 69 | `PriceOf` | `is Bundle` check gives bundles their own pricing path |
| S08 | `baseline/src/CornerStore/Catalog/Catalog.cs` line 109 | `AddToBundle` | `is Bundle` check refuses nesting |
| S11 | `baseline/src/CornerStore/Catalog/Listing.cs` line 3 | `Listing` | Each listing owns its brand, manufacturer and spec text |
| S11 | `baseline/src/CornerStore/Catalog/ListingImporter.cs` line 20 | `Import` | `string.Concat` builds fresh copies for every listing |
| S16 | `baseline/src/CornerStore/Catalog/Catalog.cs` line 163 | `PrintTree` | Manual recursion, copy 1 |
| S16 | `baseline/src/CornerStore/Catalog/Catalog.cs` line 179 | `CountProducts` | Manual recursion, copy 2 |
| S16 | `baseline/src/CornerStore/Catalog/Catalog.cs` line 191 | `CollectSkus` | Manual recursion, copy 3 |

## Follow-on and open items

* `pwsh` still not found on this machine (critique C-14). P11-S1a re-checks.
* After this commit the human can also commit `foundations-log.md` and `baseline-log.md`, now visible to git.
