<!-- markdownlint-disable-file -->
# Changes: baseline

Plan: `.copilot-tracking/plans/2026-10-15/baseline-plan.instructions.md`
Details: `.copilot-tracking/details/2026-10-15/baseline-details.md`
Planning log: `.copilot-tracking/plans/logs/2026-10-15/baseline-log.md`
Date started: 2026-10-15

## Summary

P03 of 11 complete (P01 and P02 committed by the human): customers, cart, bundle expansion and one-level undo (S18, plus a third S08 site) with the matching scenario steps. Nothing pushed or tagged by the agent.

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

## Phase P03: Customers, cart and undo

Added

* `baseline/src/CornerStore/Cart/Customer.cs` (id, number, name, region, channel, address, credit limit, role)
* `baseline/src/CornerStore/Cart/CartLine.cs` (public fields: product, quantity, unit price, `FromBundle`)
* `baseline/src/CornerStore/Cart/Cart.cs` (public fields `Lines`, `PromoCode`, `ShippingMethod`; S18)
* `baseline/src/CornerStore/Cart/CartService.cs` (`Add`, `Remove`, `Subtotal`, private bundle expansion; third S08 site)
* `baseline/tests/CornerStore.Tests/CartScenarioTests.cs` (8 tests through `ScenarioRunner.Run`)

Modified

* `baseline/src/CornerStore/StoreConsole.cs`: explicit constructor instead of a primary constructor (a field initializer cannot reference another instance field, and `CartService` needs the catalog); customer, cart and saved-copy dictionaries; ops `register`, `cart.add`, `cart.remove`, `cart.undo`, `cart.show`
* `scenarios/trading-day.json`: three registrations (Ava AU email customer, Ben NZ sms customer without an address, Sam AU email staff) and 15 cart steps (steps 10 to 27 in the transcript)

Removed

* None

Deviations and decisions

* S18 follows the P03 instruction, not the earlier details wording (DL-18): the saved copies live in `StoreConsole`, not in `Cart`; there is no snapshot or history type and no command object. `Cart` is only public fields. A change is remembered only when the service accepts it, so a rejected add or remove keeps the earlier undo state (tested).
* Bundle lines: each component becomes its own line tagged with `FromBundle`. Components take the discounted price rounded to two decimals, and the last component takes the remainder so the lines add up to `Catalog.PriceOf` of the bundle (91.76 for `BUN-RUN-KIT`: 80.96 + 10.80; 38.25 for `BUN-GIFT`: 13.05 + 25.20).
* Lines merge only when SKU and `FromBundle` both match. `cart.remove` with a bundle SKU removes that bundle's lines; with a plain SKU it removes only the plain lines.
* Customers get `Number` (registration order) for the fraud score in P08. `address` is optional; Ben has none, for the P08 validation rule.
* `Cart` has no add-on flags yet. P04 adds them with the S09 marker and must extend the copy-out, copy-back and `Remember` code in `StoreConsole` (S18 maintenance cost, deliberate).
* Cart output prints plain two-decimal prices until `MoneyFormatter` (P04). No widget coupling (S17 is P05).
* One test-expectation check by hand: 89.95 x 0.9 = 80.955, rounds away from zero to 80.96; 12.00 x 0.9 = 10.80; sum 91.76 equals the bundle price.

Validation (CI flags, Release)

* `dotnet restore GangOfFour.slnx`: up to date
* `dotnet build GangOfFour.slnx --no-restore -c Release -warnaserror`: 0 warnings, 0 errors
* `dotnet test GangOfFour.slnx --no-build -c Release`: 17 passed, 0 failed
* Running the app on `scenarios/trading-day.json` twice: identical output
* AC-006 search in `baseline/src`: 0 hits
* Pattern-leak scan: `StringBuilder` in `StoreConsole.cs` (BCL type) and the word "state" in the S18 marker comment; no pattern applied
* `baseline/src`: 21 files, 1,034 lines; `Cart/` 4 files, `Catalog/` 9 files (ceilings 60 files, 2,800 lines, 9 per folder)
* Files: LF, no BOM, ASCII only

SMELL markers planted in P03

| ID | File and line | Member | Shape |
| --- | --- | --- | --- |
| S18 | `baseline/src/CornerStore/Cart/Cart.cs` line 3 | `Cart` | Every internal is a public field |
| S18 | `baseline/src/CornerStore/StoreConsole.cs` line 225 | `CartAdd` | Caller copies lines, promo and shipping out before the change |
| S18 | `baseline/src/CornerStore/StoreConsole.cs` line 252 | `CartRemove` | Same copy-out, second copy |
| S18 | `baseline/src/CornerStore/StoreConsole.cs` line 283 | `CartUndo` | Caller writes the saved internals straight back into the cart |
| S18 | `baseline/src/CornerStore/StoreConsole.cs` line 320 | `CopyLines` | Caller must know every field of a cart line |
| S08 | `baseline/src/CornerStore/Cart/CartService.cs` line 14 | `Add` | `is Bundle` check expands a bundle into discounted lines |

## Follow-on and open items

* `pwsh` still not found on this machine (critique C-14). P11-S1a re-checks.
* After this commit the human can also commit `foundations-log.md` and `baseline-log.md`, now visible to git.
