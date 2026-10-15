<!-- markdownlint-disable-file -->
# Changes: baseline

Plan: `.copilot-tracking/plans/2026-10-15/baseline-plan.instructions.md`
Details: `.copilot-tracking/details/2026-10-15/baseline-details.md`
Planning log: `.copilot-tracking/plans/logs/2026-10-15/baseline-log.md`
Date started: 2026-10-15

## Summary

P11 of 11 complete: the baseline, 75-step scenario, approved golden master, smells catalog and copy/diff tooling are implemented. Review is conformant with justified divergence; commit `79da1bac67bc277d66de86bd7610ba9a84c6ce16` is tagged `baseline-v1`.

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

## Phase P04: Regions, pricing, promos, shipping cost, tax and weight

Added

* `baseline/src/CornerStore/Pricing/TaxCalculator.cs` (`RateFor` by region, S01; `LineTax` by line type, S23)
* `baseline/src/CornerStore/Pricing/ShippingCalculator.cs` (`RegionSurcharge`, S01)
* `baseline/src/CornerStore/Pricing/MoneyFormatter.cs` (`Format`, S01)
* `baseline/src/CornerStore/Pricing/WeightCalculator.cs` (`WeightOf`, S23)
* `baseline/src/CornerStore/Pricing/Quote.cs` (subtotal, discount, add-ons, shipping, tax, total, weight)
* `baseline/src/CornerStore/Pricing/PricingService.cs` (`BuildQuote`, `ShippingCost` switch S21, `AddOnCost` if-chain S09)
* `baseline/src/CornerStore/Pricing/PromoEngine.cs` (promo table, `Evaluate` parsing rule text by hand, S15)
* `baseline/tests/CornerStore.Tests/PricingScenarioTests.cs` (16 test cases through `ScenarioRunner.Run`)

Modified

* `baseline/src/CornerStore/Cart/Cart.cs`: public `GiftWrap` and `Insurance` flags (S09)
* `baseline/src/CornerStore/StoreConsole.cs`: ops `cart.addon`, `cart.promo`, `cart.shipping`, `quote`; cart and quote output now go through `MoneyFormatter` with the customer's region; the S18 copy-out, copy-back and `Remember` code carries the two add-on flags (new `savedGiftWrap` and `savedInsurance` dictionaries)
* `baseline/tests/CornerStore.Tests/CartScenarioTests.cs`: expectations now show `A$` because Ava is an AU customer
* `scenarios/trading-day.json`: 17 steps added (steps 28 to 44): promos (unknown, not applicable, applied), shipping (express, unknown, standard, pickup), add-ons (gift wrap, insurance, unknown), five quotes including an empty cart

Removed

* None

Deviations and decisions (also DL-19)

* Order of operations in `BuildQuote`: subtotal, promo discount, add-ons (2 percent insurance on the discounted subtotal, 4.50 gift wrap), shipping (method cost plus regional surcharge), tax on discounted line amounts per line type, total. The discount is spread over lines in proportion to line amount, and the last line takes the remainder.
* Rates: AU tax 10 percent, NZ 15 percent; NZ surcharge 3.00; standard 5.00 + 2.00 per kg, express 12.00 + 4.00 per kg, pickup 0. Gift cards are untaxed and weightless, digital items taxed and weightless, physical items taxed and weigh `WeightKg`.
* Promos: `SHOES10` is `SPEND>100 AND CATEGORY=Shoes` for 10 percent; `BIGSPEND` is `SPEND>250` for 20 off; `NZFRIEND` is `REGION=NZ AND QTY>2` for 5 percent. `CATEGORY` matches the product's own category, `QTY` is the total quantity over all lines. A code is stored on the cart only if its rule holds when `cart.promo` runs; `quote` re-evaluates and prints `promo X does not apply` if the cart changed since.
* S09 has a third site, `StoreConsole.CartAddOn`, with its own marker (DL-19). S18 gets no new marker: the existing sites were extended.
* Undo restores lines, promo, shipping and both add-on flags to their value from just before the last add or remove. `cart.promo`, `cart.shipping` and `cart.addon` are not undo points.
* `Catalog.FormatPrice` stays for regionless catalog output; `MoneyFormatter` is used wherever a customer is known.
* Independent recomputation (PA-3, early): quote totals for cu-1 (300.33) and cu-2 (136.17 standard, 129.19 pickup) were worked out by hand, line by line, before comparing with the transcript. Both match.

Validation (CI flags, Release)

* `dotnet restore GangOfFour.slnx`: up to date
* `dotnet build GangOfFour.slnx --no-restore -c Release -warnaserror`: 0 warnings, 0 errors
* `dotnet test GangOfFour.slnx --no-build -c Release`: 33 passed, 0 failed
* Running the app on `scenarios/trading-day.json` twice: identical output
* AC-006 search in `baseline/src`: 0 hits
* Pattern-leak scan: `StringBuilder` in `StoreConsole.cs` (BCL type) and the word "state" in the S18 marker comment; no pattern applied
* `baseline/src`: 28 files, 1,483 lines; `Pricing/` 7 files, `Cart/` 4, `Catalog/` 9 (ceilings 60 files, 2,800 lines, 9 per folder)
* Files: LF, no BOM, ASCII only

SMELL markers planted in P04

| ID | File and line | Member | Shape |
| --- | --- | --- | --- |
| S01 | `baseline/src/CornerStore/Pricing/TaxCalculator.cs` line 7 | `RateFor` | `if (region == "AU") ... else if (region == "NZ")` for the tax rate |
| S01 | `baseline/src/CornerStore/Pricing/ShippingCalculator.cs` line 7 | `RegionSurcharge` | Separate region branch for the surcharge |
| S01 | `baseline/src/CornerStore/Pricing/MoneyFormatter.cs` line 11 | `Format` | Separate region branch for the currency symbol |
| S09 | `baseline/src/CornerStore/Cart/Cart.cs` line 14 | `Cart` | `bool GiftWrap` and `bool Insurance` public flags |
| S09 | `baseline/src/CornerStore/Pricing/PricingService.cs` line 76 | `AddOnCost` | `if (cart.GiftWrap)` and `if (cart.Insurance)` if-chain |
| S09 | `baseline/src/CornerStore/StoreConsole.cs` line 351 | `CartAddOn` | Console names each flag to set it |
| S15 | `baseline/src/CornerStore/Pricing/PromoEngine.cs` line 32 | `Evaluate` | Rule text split on `AND`, each term parsed with `IndexOf` and `Substring` |
| S21 | `baseline/src/CornerStore/Pricing/PricingService.cs` line 60 | `ShippingCost` | `switch` on the method string, no region and no cart |
| S23 | `baseline/src/CornerStore/Pricing/TaxCalculator.cs` line 22 | `LineTax` | `switch (product)` over physical, digital and gift card |
| S23 | `baseline/src/CornerStore/Pricing/WeightCalculator.cs` line 7 | `WeightOf` | `switch (product)` over the same three types |

Existing S18 sites extended (markers unchanged): `StoreConsole.cs` lines 242 (`CartAdd`), 271 (`CartRemove`), 304 (`CartUndo`), 451 (`CopyLines`). The caller now also copies and restores the two add-on flags.

## Phase P05: Cart widgets

Added

* `baseline/src/CornerStore/Widgets/ItemListWidget.cs`, `PromoBoxWidget.cs`, `ShippingSelectorWidget.cs`, `TotalsPanelWidget.cs`
* Four widgets are connected directly to one another per customer. Each change entry point refreshes its own view and calls its peers' `Refresh` methods in a fixed order. No mediator, coordinator or events were added.
* `CartScenarioTests.CartAdd_RefreshesWidgetsInTheirDirectCallOrder` verifies item list, promo, shipping and totals output order.

Modified

* `baseline/src/CornerStore/StoreConsole.cs`: register and connect the widgets; route cart add/remove/show, promo, shipping, add-on and quote operations through the relevant widget
* `scenarios/trading-day.json`: append a cart add and undo pair (steps 45 and 46), exercising refreshes while returning cu-1 to its prior cart state
* `.copilot-tracking/plans/2026-10-15/baseline-plan.instructions.md`: mark P05 complete

Removed

* None

Deviations and decisions

* S18 remains caller-managed in `StoreConsole`: the existing saved-line, promo, shipping and add-on copies are still captured before add/remove and restored by undo. Widget refresh output runs before the established operation-result lines.
* `CartShow` now renders through `ItemListWidget`; the pricing result remains in `TotalsPanelWidget.ShowQuote` with the same business values and formatting.
* Transcript review corrected `TotalsPanelWidget.Refresh` to show the full quote total rather than subtotal; it shows zero for an empty cart.

Validation (Release)

* `dotnet restore GangOfFour.slnx`: succeeded
* `dotnet build GangOfFour.slnx --no-restore -c Release -warnaserror`: succeeded, 0 warnings and 0 errors
* `dotnet test GangOfFour.slnx --no-build -c Release`: 34 passed, 0 failed
* `fc.exe /b` on two runs of `scenarios/trading-day.json`: no differences
* `git diff --check`: succeeded; editor diagnostics: no errors in changed C# files
* Pattern-leak scan: `StringBuilder` in `StoreConsole.cs` (BCL type) and `state` in the S18 marker comment; no pattern is applied
* `baseline/src`, excluding `bin` and `obj`: 32 C# files, 1,405 lines

SMELL markers planted in P05

| ID | File and line | Member | Shape |
| --- | --- | --- | --- |
| S17 | `baseline/src/CornerStore/Widgets/ItemListWidget.cs` line 48 | `RefreshAfterChange` | Item list directly refreshes the promo, shipping and totals widgets |
| S17 | `baseline/src/CornerStore/Widgets/PromoBoxWidget.cs` line 38 | `RefreshAfterChange` | Promo widget directly refreshes the item list, shipping and totals widgets |
| S17 | `baseline/src/CornerStore/Widgets/ShippingSelectorWidget.cs` line 33 | `RefreshAfterChange` | Shipping widget directly refreshes the item list, promo and totals widgets |
| S17 | `baseline/src/CornerStore/Widgets/TotalsPanelWidget.cs` line 25 | `RefreshAfterChange` | Totals widget directly refreshes the item list, promo and shipping widgets |

Every S17 marker is an intentional direct reference and refresh call; no mediator or event system is present. Existing S18 markers and copy/restore behavior are unchanged.

## Phase P06: Notifications and inventory

Added

* `baseline/src/CornerStore/Notifications/EmailSender.cs` and `SmsSender.cs`, in-process transcript fakes
* `baseline/src/CornerStore/Notifications/OrderConfirmedEmail.cs`, `OrderConfirmedSms.cs`, `OrderShippedEmail.cs` and `OrderShippedSms.cs` (S07)
* `baseline/src/CornerStore/Inventory/InventoryService.cs`: deterministic stock values, uncached lookups with a private simulated cost, and an unguarded stock adjustment method (S12)

Modified

* `.copilot-tracking/plans/2026-10-15/baseline-plan.instructions.md`: mark P06 complete

Removed

* None

Validation

* `dotnet test baseline/CornerStore.Baseline.slnx -c Release`: 34 passed, 0 failed; P06 source compiles
* Notification and stock behavior will be exercised by P07/P08 scenario steps

SMELL markers planted in P06

| ID | File and line | Member | Shape |
| --- | --- | --- | --- |
| S07 | `baseline/src/CornerStore/Notifications/OrderConfirmedEmail.cs` line 5 | `Send` | Confirmation text and email sender call in a message/channel-specific class |
| S07 | `baseline/src/CornerStore/Notifications/OrderConfirmedSms.cs` line 5 | `Send` | Confirmation text and SMS sender call in a message/channel-specific class |
| S07 | `baseline/src/CornerStore/Notifications/OrderShippedEmail.cs` line 5 | `Send` | Shipping text and email sender call in a message/channel-specific class |
| S07 | `baseline/src/CornerStore/Notifications/OrderShippedSms.cs` line 5 | `Send` | Shipping text and SMS sender call in a message/channel-specific class |
| S12 | `baseline/src/CornerStore/Inventory/InventoryService.cs` line 24 | `GetStock` | Every lookup increments a private cost and reads stock without caching |
| S12 | `baseline/src/CornerStore/Inventory/InventoryService.cs` line 31 | `AdjustStock` | Stock can be changed without a role check |

## Phase P07: Orders, lifecycle and back office

Added

* `baseline/src/CornerStore/Orders/Order.cs` and `Invoice.cs`, each constructed with eleven positional parameters (S02)
* `baseline/src/CornerStore/Orders/OrderService.cs`: string status guards for pay, ship, deliver, cancel and refund (S20); direct notification, stock lookup, loyalty and analytics calls after transitions (S19)
* `baseline/src/CornerStore/Orders/LoyaltyService.cs` and `AnalyticsService.cs`
* `baseline/src/CornerStore/Orders/BackOffice.cs`: inline stock, lookup, refund and cancel actions with repeated caller-side staff checks (S12, S14)
* `baseline/tests/CornerStore.Tests/OrderScenarioTests.cs`: authorized staff stock updates and lookups; customer attempts are rejected

Modified

* `baseline/src/CornerStore/StoreConsole.cs`: order and admin operation dispatch, order/customer lookup, and service wiring
* `scenarios/trading-day.json`: add authorized and denied stock adjustment and lookup steps; the staff adjustment sets `KETTLE-1L` to zero for checkout validation in P08
* `.copilot-tracking/plans/2026-10-15/baseline-plan.instructions.md`: mark P07 complete

Removed

* None

Deviations and decisions

* The order transitions are implemented but cannot yet be reached through the scenario because checkout is P08. P08 will create orders and exercise shipping, delivery, cancellation, refund and illegal transitions.
* `BackOffice.LookupStock` performs two uncached reads of the same SKU, keeping simulated lookup cost internal and out of the transcript.

Validation

* `dotnet test baseline/CornerStore.Baseline.slnx -c Release`: 35 passed, 0 failed
* Editor diagnostics: no errors in changed C# files

SMELL markers planted in P07

| ID | File and line | Member | Shape |
| --- | --- | --- | --- |
| S02 | `baseline/src/CornerStore/Orders/OrderService.cs` line 18 | `Create` | Eleven positional order arguments, including null optional message, note, tracking and cancellation fields |
| S02 | `baseline/src/CornerStore/Orders/OrderService.cs` line 31 | `CreateInvoice` | Eleven positional invoice arguments with a null note |
| S12 | `baseline/src/CornerStore/Orders/BackOffice.cs` lines 8, 22, 36, 49 | `AdjustStock`, `LookupStock`, `Refund`, `Cancel` | Each caller checks the staff role inline |
| S14 | `baseline/src/CornerStore/Orders/BackOffice.cs` lines 7, 21, 35, 48 | Back-office methods | Actions execute inline without command objects or an audit queue |
| S19 | `baseline/src/CornerStore/Orders/OrderService.cs` line 122 | `RecordStatusChange` | Order service directly calls inventory, message, loyalty and analytics services |
| S20 | `baseline/src/CornerStore/Orders/OrderService.cs` lines 46, 59, 73, 86, 104 | `Pay`, `Ship`, `Deliver`, `Cancel`, `Refund` | Transition guards compare the `Status` string directly |

## Phase P08: Checkout

Added

* `baseline/src/CornerStore/Checkout/IPaymentProcessor.cs`, `CardProcessor.cs`, `PayPalProcessor.cs` and `AfterpayProcessor.cs`
* `baseline/src/CornerStore/Checkout/ShipFastClient.cs`, accepting XML and prices in cents
* `baseline/src/CornerStore/Checkout/FraudScorer.cs` and `OrderValidator.cs` with ordered nested validation for stock, address, fraud and credit
* `baseline/tests/CornerStore.Tests/CheckoutScenarioTests.cs`: successful card checkout and missing-address rejection without payment

Modified

* `baseline/src/CornerStore/StoreConsole.cs`: checkout operation coordinates validation, payment, order creation, stock reservation, courier booking and payment status; payment processor selection is a switch; courier calls and XML stay in the console
* `baseline/src/CornerStore/Orders/OrderService.cs`: retain the ShipFast tracking number when the paid order later transitions to Shipped
* `scenarios/trading-day.json`: exercise all validation failures, all payment types, both notification channels at checkout and shipping, cancellation, refund, delivery and illegal transitions
* `.copilot-tracking/plans/2026-10-15/baseline-plan.instructions.md`: mark P08 complete

Removed

* None

Deviations and decisions

* `Checkout` calculates the quote before calling `OrderValidator.Validate`, because the planned validation signature takes a `Quote`. Validation still occurs before payment, order creation, stock reservation and shipment booking.
* The post-checkout `order.ship` operation retains the tracking identifier already returned by ShipFast instead of assigning a second one.
* Final review found that separate plain and bundle lines could request more of one SKU than inventory held. `OrderValidator` now aggregates requested quantities by SKU, with a regression test proving payment is not taken.

Validation

* `dotnet build baseline/CornerStore.Baseline.slnx -c Release -warnaserror`: succeeded, 0 warnings and 0 errors
* `dotnet test baseline/CornerStore.Baseline.slnx --no-build -c Release`: 37 passed, 0 failed
* Scenario coverage assertions found out-of-stock, missing-address, fraud and credit failures; card, PayPal and Afterpay charges; email and SMS confirmation and shipping; cancellation, refund, delivery and both illegal transitions
* Editor diagnostics: no errors in changed C# files

SMELL markers planted in P08

| ID | File and line | Member | Shape |
| --- | --- | --- | --- |
| S03 | `baseline/src/CornerStore/StoreConsole.cs` line 449 | `ChargePayment` | Switch constructs a processor for each payment string |
| S06 | `baseline/src/CornerStore/StoreConsole.cs` line 475 | `BookShipment` | Console sends XML and parses the courier response directly |
| S10 | `baseline/src/CornerStore/StoreConsole.cs` line 382 | `Checkout` | Console calls each checkout subsystem in sequence |
| S13 | `baseline/src/CornerStore/Checkout/OrderValidator.cs` line 9 | `Validate` | Nested stock, address, fraud and credit checks in one method |

The baseline intentionally retains the payment product interface required for the later Factory Method pattern. No other pattern is applied at these sites.

## Phase P09: Reporting

Added

* `baseline/src/CornerStore/Reporting/ReportService.cs`, `CsvReportExporter.cs` and `JsonReportExporter.cs`
* Both exporters independently load, filter and aggregate orders, switch on physical/digital/gift-card line types, and write their format (S22, S23)
* `baseline/tests/CornerStore.Tests/ReportingScenarioTests.cs`: physical, digital and gift-card totals in both formats

Modified

* `baseline/src/CornerStore/Orders/OrderService.cs`: expose orders in stable ID order for reports
* `baseline/src/CornerStore/StoreConsole.cs`: add `report.csv` and `report.json` operations
* `scenarios/trading-day.json`: append both report operations after orders of each line-item type exist
* `.copilot-tracking/plans/2026-10-15/baseline-plan.instructions.md`: mark P09 complete

Removed

* None

Validation

* `dotnet test baseline/CornerStore.Baseline.slnx -c Release`: 38 passed, 0 failed
* Full scenario output includes Physical 543.32, Digital 7.50 and GiftCard 25.00 in both formats
* Editor diagnostics: no errors in changed C# files

SMELL markers planted in P09

| ID | File and line | Member | Shape |
| --- | --- | --- | --- |
| S22 | `baseline/src/CornerStore/Reporting/CsvReportExporter.cs` line 10 | `Export` | CSV exporter duplicates the common load, filter, aggregate and write flow |
| S22 | `baseline/src/CornerStore/Reporting/JsonReportExporter.cs` line 10 | `Export` | JSON exporter duplicates the same flow |
| S23 | `baseline/src/CornerStore/Reporting/CsvReportExporter.cs` line 20 | `Export` | CSV rendering switches on physical, digital and gift-card product types |
| S23 | `baseline/src/CornerStore/Reporting/JsonReportExporter.cs` line 20 | `Export` | JSON rendering switches on the same product types |

## Phase P10: Scenario and golden master

Added

* `baseline/tests/CornerStore.Tests/RepositoryRoot.cs`, `GoldenMasterTests.cs` and `DeterminismTests.cs`
* `scenarios/expected/baseline.txt`, approved from the reviewed 75-step trading-day transcript

Modified

* `.copilot-tracking/plans/2026-10-15/baseline-plan.instructions.md`: mark P10 complete

Removed

* The generated, ignored `scenarios/expected/baseline.received.txt` after approval and mismatch-path verification

Review and independent arithmetic

* Ava: subtotal 271.66 - discount 27.17 + add-ons 9.39 + express shipping 22.00 + tax 24.45 = A$300.33
* Ben: subtotal 116.74 - discount 5.84 + standard shipping 12.20 + tax 13.07 = NZ$136.17; pickup plus insurance gives NZ$129.19
* Jo: gift card 25.00 + ebook 7.50 + shipping 8.00 + tax 1.13 = NZ$41.63
* Report totals: physical 271.66 x 2 = 543.32; digital 7.50; gift card 25.00

Validation

* `GOLDEN_MASTER_APPROVE` was unset before the initial comparison. The test failed only because the approved transcript was absent and wrote the received candidate.
* After manual review and arithmetic checks, the test approved `baseline.txt` with `GOLDEN_MASTER_APPROVE=1`; the final full Release build and test run passed, 41 tests total
* `baseline.txt`: 14,887 bytes, UTF-8 without BOM, LF only, final LF present
* After changing the first expected character, `GoldenMasterTests` reported line 1 and wrote the received output; after restoring it, the focused test passed
* The received file is ignored by `.gitignore` and was removed
* `DeterminismTests` compares two complete scenario runs

## Follow-on and open items

* PowerShell 7 was found at `C:\Users\echau\AppData\Local\Microsoft\WindowsApps\pwsh.exe` during P11-S1a and both PowerShell editions passed AC-007.
* The baseline review is recorded at `.copilot-tracking/reviews/2026-10-15/baseline-review.md`; run-all permits the local baseline commit and tag, but never a push.

## Phase P11: Catalog, tools and CI

Added

* `tools/new-pattern.ps1`: copies source and tests without `bin`/`obj`, creates the pattern solution and README stub, and updates `GangOfFour.slnx` through XML APIs
* `tools/diff-baseline.ps1`: writes a stable file summary and unified diff for source and tests, excluding build output
* 23 smell entries in `docs/smells-catalog.md`, each with locations, impact and scenario steps

Modified

* `.github/workflows/ci.yml`: regenerate each pattern `DIFF.md` with PowerShell 7 and fail if tracked pattern files differ
* `README.md`: describe the complete baseline, approved transcript and copy/diff tooling
* `.copilot-tracking/plans/2026-10-15/baseline-plan.instructions.md`: mark P11 complete

Removed

* The temporary `patterns/structural/scratch-check/` copy and its umbrella solution entries after AC-007 passed

Deviations and decisions

* The PowerShell instruction asks for `#Requires -Version 7.0`, while settled PD-12 requires both scripts to run under Windows PowerShell 5.1 and PowerShell 7. The scripts require 5.1 and use syntax supported by both editions.
* Git autocrlf warnings from temporary diff trees are avoided with per-command `core.autocrlf=false` and `core.safecrlf=false`; repository configuration is unchanged.

Validation

* `pwsh --version`: PowerShell 7.6.7 at `WindowsApps/pwsh.exe`
* Under both `pwsh` and `powershell.exe`, `new-pattern.ps1 -Category structural -Name scratch-check` created a copy that passed `dotnet test`, 40 passed including the golden master
* Under both editions, `diff-baseline.ps1` produced one `None` summary row and `No differences from the baseline.`
* A temporary added-file probe produced `Added | CornerStore/DiffProbe.txt | 1 | 0`; it was removed and the clean diff regenerated
* The CI diff loop passed locally; the smell catalog contains exactly 23 entries
* `dotnet build GangOfFour.slnx -c Release -warnaserror`: succeeded after removing scratch solution entries

SMELL markers planted in P11

* None

## Baseline review complete

P01-P11 and the final acceptance checks are complete. The review outcome is Conformant with justified divergence. Commit `79da1bac67bc277d66de86bd7610ba9a84c6ce16` is tagged `baseline-v1`; the run-all human checkpoint is now active.
