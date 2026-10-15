<!-- markdownlint-disable-file -->
# Details: baseline

Plan: `.copilot-tracking/plans/2026-10-15/baseline-plan.instructions.md`
Research: `.copilot-tracking/research/2026-10-15/baseline-research.md`
Planning log: `.copilot-tracking/plans/logs/2026-10-15/baseline-log.md`

This file holds the design the plan refers to: conventions, file layout, the smell site table, the scenario and transcript format, the coverage matrix, per-phase notes, the tool designs and the verification commands.

## 1. Conventions that apply to every phase

Code

* One public type per file, file-scoped namespace `CornerStore`, nullable respected (no `!`), no `#pragma warning disable`.
* ASCII only in `baseline/src`. Money is `decimal`, rounded with `Math.Round(x, 2, MidpointRounding.AwayFromZero)` where a result is stored or printed.
* All formatting uses `CultureInfo.InvariantCulture`. The transcript writes `\n`, never `Environment.NewLine`.
* No `DateTime.Now`, `DateTime.UtcNow`, `Guid.NewGuid()`, unseeded `Random`, `Thread.Sleep`, `Task.Delay` or `string.GetHashCode()` in `baseline/src`.
* Time comes only from the `TimeProvider` the runner creates from the scenario's `startTime` (`ScenarioClock`, a small `TimeProvider` subclass with an `Advance` method).
* IDs come from `IdSequence`: customers `cu-1`, `cu-2`, orders `o-1001`, `o-1002`, shipments `SF-0001`. Counters live on the instance the runner creates, not in statics.
* `Random` is always created as `new Random(Config.Seed + customerNumber)` at the point of use and its first value is taken. No shared `Random`, so evaluation order cannot change a result.
* Enumerations that reach the transcript come from `List<T>` or from sorted data. No `HashSet<T>` or `Dictionary<K,V>` enumeration order in output.
* The new-everywhere style is intended: the `StoreConsole` constructor builds its services with `new`.
* No pattern at a seam (see PD-10, PD-11). The only baseline interface is `IPaymentProcessor`, and the reason is recorded in the changes log.
* Size budget (PD-13) is a ceiling, not a target: at most 60 files and about 2,800 lines in `baseline/src`, at most 9 files per feature folder. Smaller is better.

Transcript

* First line: `CornerStore trading day: <name> (start <startTime as ISO 8601 UTC>)`.
* Each step starts with `[NNN] <op> key=value ...`, built from the step's properties in file order.
* `Log.Info(text)` writes the text indented by two spaces. Totals use two decimals. Region currency formatting comes from `MoneyFormatter` (S01).
* Business outcomes only: actions, state changes, totals, messages sent, validation failures. No timings, memory, call counts or hash codes. The simulated inventory cost is held in a field and never printed.

Phase stop report (written at every stop)

* Phase and files added or changed (one line each).
* Smell marker table: ID, file path with line, member, shape.
* Pattern-leak confirmation per smell, with the scan output summary (PA-2).
* Build and test result with the CI flags; warning count.
* Transcript excerpt for the new steps.
* Deviations from the plan, if any.
* Suggested commit message in hve-core format.

## 2. Source layout

Files marked with a phase are created in that phase.

```text
baseline/
  CornerStore.Baseline.slnx                              P01
  src/CornerStore/
    CornerStore.csproj                                   P01
    Program.cs  ScenarioRunner.cs  Scenario.cs           P01
    ScenarioClock.cs  IdSequence.cs  StoreConsole.cs     P01 (StoreConsole grows each phase)
    Config.cs  Log.cs                                    P01 (S05)
    Catalog/   Product.cs PhysicalProduct.cs DigitalProduct.cs GiftCardProduct.cs
               Category.cs Bundle.cs Catalog.cs Listing.cs ListingImporter.cs     P02
    Cart/      Customer.cs Cart.cs CartLine.cs CartService.cs                       P03
    Pricing/   TaxCalculator.cs ShippingCalculator.cs MoneyFormatter.cs
               WeightCalculator.cs PricingService.cs Quote.cs PromoEngine.cs       P04
    Widgets/   ItemListWidget.cs PromoBoxWidget.cs ShippingSelectorWidget.cs
               TotalsPanelWidget.cs                                                P05
    Notifications/ EmailSender.cs SmsSender.cs OrderConfirmedEmail.cs
               OrderConfirmedSms.cs OrderShippedEmail.cs OrderShippedSms.cs        P06
    Inventory/ InventoryService.cs                                                 P06
    Orders/    Order.cs Invoice.cs OrderService.cs LoyaltyService.cs
               AnalyticsService.cs BackOffice.cs                                   P07
    Checkout/  IPaymentProcessor.cs CardProcessor.cs PayPalProcessor.cs
               AfterpayProcessor.cs ShipFastClient.cs FraudScorer.cs
               OrderValidator.cs                                                   P08
    Reporting/ ReportService.cs CsvReportExporter.cs JsonReportExporter.cs         P09
  tests/CornerStore.Tests/
    CornerStore.Tests.csproj  AssemblyAttributes.cs  ScenarioRunnerTests.cs        P01
    RepositoryRoot.cs  GoldenMasterTests.cs  DeterminismTests.cs                    P10
scenarios/trading-day.json                                                         P02, grows, complete in P10
scenarios/expected/baseline.txt                                                    P10 (approved once, GOLDEN_MASTER_APPROVE=1)
scenarios/expected/baseline.received.txt                                           written on mismatch only, git-ignored
docs/smells-catalog.md                                                             P11
tools/new-pattern.ps1  tools/diff-baseline.ps1                                     P11
```

Counts: about 55 files in `baseline/src`.

## 3. Smell site table

Each site gets its own marker: `// SMELL Sxx: <short description>`. A smell with several sites has a marker at every site. "Phase" is where the site is planted.

| ID | Phase | Target pattern | Site (file and member) | Shape in the baseline | Exercised by |
| --- | --- | --- | --- | --- | --- |
| S01 | P04 | Abstract Factory | `Pricing/TaxCalculator.cs` `RateFor`; `Pricing/ShippingCalculator.cs` `RegionSurcharge`; `Pricing/MoneyFormatter.cs` `Format` | Three independent `if (region == "AU") ... else if (region == "NZ")` blocks, each taking the region as a string, so nothing keeps AU tax and NZ shipping from mixing | `quote`, `checkout` for AU and NZ customers |
| S02 | P07 | Builder | `Orders/Order.cs` and `Orders/Invoice.cs` constructors; `Orders/OrderService.cs` `Create` and `CreateInvoice` call sites | 11-parameter constructors with `null` for optional parts (promo code, gift message, note, tracking) | `checkout` |
| S03 | P08 | Factory Method | `StoreConsole.cs` `ChargePayment` | `switch (paymentType)` with `new CardProcessor()`, `new PayPalProcessor()`, `new AfterpayProcessor()` inside the checkout flow | `checkout` with card, paypal, afterpay |
| S04 | P02 | Prototype | `Catalog/Catalog.cs` `CreateVariant` | A variant is built by copying about ten fields of the parent `PhysicalProduct` by hand | `variant` |
| S05 | P01 | Singleton | `Config.cs`; `Log.cs` | Static classes with public mutable fields, set by the runner and by `StoreConsole` (`config.set`) | every step |
| S06 | P08 | Adapter | `StoreConsole.cs` `BookShipment`; `Checkout/ShipFastClient.cs` | Checkout builds an XML string, passes prices in cents and calls differently named SDK methods directly | `checkout` |
| S07 | P06 | Bridge | `Notifications/OrderConfirmedEmail.cs`, `OrderConfirmedSms.cs`, `OrderShippedEmail.cs`, `OrderShippedSms.cs` | One class per message type and channel, each with its own text and sender call | `checkout`, `order.ship` for email and sms customers |
| S08 | P02, P03 | Composite | `Catalog/Catalog.cs` `PriceOf` and `AddToBundle`; `Cart/CartService.cs` `Add` | `is Bundle` checks give bundles a separate pricing path and expansion into lines; a bundle cannot contain a bundle | `bundle.create`, `bundle.add` (nested attempt), `cart.add` of a bundle |
| S09 | P04 | Decorator | `Cart/Cart.cs` add-on flags; `Pricing/PricingService.cs` `AddOnCost` | `bool GiftWrap`, `bool Insurance` and an if-chain in the price calculation | `cart.addon` |
| S10 | P08 | Facade | `StoreConsole.cs` `Checkout` | The console flow calls validation, pricing, payment, stock reservation, courier booking, order creation and notifications itself, in a fixed order | `checkout` |
| S11 | P02 | Flyweight | `Catalog/Listing.cs`; `Catalog/ListingImporter.cs` | 100,000 listings, each with its own copy of brand, manufacturer and spec text | `import` |
| S12 | P06, P07 | Proxy | `Inventory/InventoryService.cs` `GetStock` and `AdjustStock`; `Orders/BackOffice.cs` role checks | Slow lookups repeated with no caching; the stock mutator has no protection and each caller checks the role inline | `admin.stock.lookup`, `admin.stock.adjust` (allowed and denied), `checkout` |
| S13 | P08 | Chain of Responsibility | `Checkout/OrderValidator.cs` `Validate` | One method with nested ifs for stock, address, fraud score and credit limit | `checkout` with each rule failing |
| S14 | P07 | Command | `Orders/BackOffice.cs` | Stock adjust, refund, cancel and lookup run inline with no undo, audit trail or queue | `admin.*` |
| S15 | P04 | Interpreter | `Pricing/PromoEngine.cs` `Evaluate` | Rules such as `SPEND>100 AND CATEGORY=Shoes` parsed with `Split` and `IndexOf` | `cart.promo` with three rules |
| S16 | P02 | Iterator | `Catalog/Catalog.cs` `PrintTree`, `CountProducts`, `CollectSkus` | Three copies of the same manual recursion over the category tree | `browse` |
| S17 | P05 | Mediator | `Widgets/ItemListWidget.cs`, `PromoBoxWidget.cs`, `ShippingSelectorWidget.cs`, `TotalsPanelWidget.cs` | Widgets hold references to each other and call each other's refresh methods | `cart.add`, `cart.remove`, `cart.promo`, `cart.shipping` |
| S18 | P03 | Memento | `Cart/Cart.cs` public `Saved*` fields; `Cart/CartService.cs` snapshot and `Undo` | Undo works because cart internals are public and the service copies them | `cart.undo` |
| S19 | P07 | Observer | `Orders/OrderService.cs` blocks after each status change | After a status change the service directly calls the message classes, `InventoryService`, `LoyaltyService` and `AnalyticsService` | `checkout`, `order.ship`, `admin.cancel`, `admin.refund` |
| S20 | P07 | State | `Orders/OrderService.cs` `Pay`, `Ship`, `Deliver`, `Cancel`, `Refund`; `Orders/Order.cs` `Status` | `Status` is a string checked with if/else at the top of each transition | `order.*`, `admin.*` including illegal attempts |
| S21 | P04 | Strategy | `Pricing/PricingService.cs` `ShippingCost` | `switch` on the shipping method string inside the pricing code | `cart.shipping` with standard, express, pickup |
| S22 | P09 | Template Method | `Reporting/CsvReportExporter.cs`; `Reporting/JsonReportExporter.cs` | The load, filter, aggregate and write skeleton is duplicated in both exporters | `report.csv`, `report.json` |
| S23 | P04, P09 | Visitor | `Pricing/TaxCalculator.cs` `LineTax`; `Pricing/WeightCalculator.cs` `WeightOf`; row rendering in both exporters | `switch (product)` with `PhysicalProduct`, `DigitalProduct` and `GiftCardProduct` cases in tax, weight and export code | `quote`, `report.*` with every line type |

Seam clarifications (kept so that no smell silently turns into another):

* S10, S06 and S03 share `StoreConsole`, each in its own private method (`Checkout`, `BookShipment`, `ChargePayment`). `Checkout` only sequences calls.
* S08 is confined to the catalog and the cart expansion. Everything downstream (tax, weight, export, orders) sees only `Product` subclasses, so S23 switches never contain a bundle case.
* S01 (region) and S21 (method) both affect shipping, in separate methods: `ShippingCalculator.RegionSurcharge(region)` and `PricingService.ShippingCost(method, weightKg)`.
* S01 (region tax rate) and S23 (tax by line type) are separate methods: `RateFor(region)` and `LineTax(product, amount, rate)`.
* S12 is a catalog of two kinds of site: the uncached, unguarded `InventoryService` (P06) and the inline role checks in `BackOffice` (P07).
* The role check exists in the baseline so that the unauthorised attempt in the scenario is refused and the transcript says so. The smell is where the check lives, not that it is missing.

## 4. Scenario and ops

Shape of `scenarios/trading-day.json`:

```json
{
  "name": "trading-day",
  "startTime": "2026-03-02T09:00:00Z",
  "seed": 20260302,
  "config": { "fraudThreshold": 80, "lowStockLevel": 3 },
  "steps": [
    { "op": "register", "name": "Ava", "region": "AU", "channel": "email", "creditLimit": 800, "role": "customer" },
    { "op": "cart.add", "customer": "cu-1", "sku": "SHOE-RUN-42", "qty": 2 }
  ]
}
```

Catalog data is built in code (in-memory, D-03); the scenario holds actions only. Op vocabulary by phase:

| Phase | Ops |
| --- | --- |
| P01 | `advance`, `config.set` (parsed and dispatched through `StoreConsole`; unknown ops print `unknown op` and continue) |
| P02 | `browse`, `variant`, `bundle.create`, `bundle.add`, `import` |
| P03 | `register`, `cart.add`, `cart.remove`, `cart.undo`, `cart.show` |
| P04 | `cart.addon`, `cart.promo`, `cart.shipping`, `quote` |
| P05 | no new ops; the cart ops go through the widgets |
| P06 | none (services only; exercised from P07 and P08) |
| P07 | `order.ship`, `order.deliver`, `admin.stock.adjust`, `admin.stock.lookup`, `admin.refund`, `admin.cancel` |
| P08 | `checkout` |
| P09 | `report.csv`, `report.json` |

Promo rules (the promo table lives in `PromoEngine`): `SHOES10` is `SPEND>100 AND CATEGORY=Shoes` for 10 percent off; `BIGSPEND` is `SPEND>250` for 20 off; `NZFRIEND` is `REGION=NZ AND QTY>2` for 5 percent off. Grammar is AND-only with `SPEND>n`, `QTY>n`, `CATEGORY=x`, `REGION=x`.

Shipping methods: `standard`, `express`, `pickup`. Add-ons: gift wrap and insurance. Payment types: `card`, `paypal`, `afterpay`. Order states: Pending, Paid, Shipped, Delivered, Cancelled, Refunded.

Legal transitions: Pending to Paid or Cancelled; Paid to Shipped, Cancelled or Refunded; Shipped to Delivered or Refunded; Delivered to Refunded. Everything else is refused with a transcript line and no state change.

## 5. FR-012 coverage matrix

Each row must be exercised by at least one scenario step. P10 checks the whole matrix; earlier phases tick their own rows.

| Requirement | Exercised by | Phase |
| --- | --- | --- |
| Both regions, AU and NZ | `register` AU and NZ customers; `quote` and `checkout` for each | P03, P04, P08 |
| Three payment types | `checkout` card, paypal, afterpay | P08 |
| Every shipping method | `cart.shipping` standard, express, pickup | P04 |
| Every add-on | `cart.addon` gift wrap and insurance | P04 |
| Nested category walk | `browse` from the root | P02 |
| A bundle and a nested bundle attempt | `bundle.create`, `bundle.add` of a bundle | P02 |
| Variant creation | `variant` | P02 |
| At least two promo rules | `cart.promo` SHOES10, BIGSPEND, NZFRIEND (plus one that does not apply) | P04 |
| A cart undo | `cart.undo` after an add and after a remove | P03 |
| Widget updates | cart ops through the widgets, with visible refresh lines | P05 |
| Each validation rule failing once | `checkout` with out-of-stock, missing address, fraud score above threshold, total above credit limit | P08 |
| Every back-office action with one unauthorised attempt | `admin.stock.adjust` (staff allowed, customer denied), `admin.stock.lookup`, `admin.refund`, `admin.cancel` | P07 |
| A cancel and a refund | `admin.cancel`, `admin.refund` | P07 |
| An illegal order transition | `order.ship` on a cancelled order; `order.deliver` on a paid order | P07 |
| Both report exports | `report.csv`, `report.json` | P09 |
| Every line item type | physical, digital and gift card lines in orders and reports | P04, P09 |
| Bulk import summary | `import` | P02 |
| Email and sms customers | `checkout` and `order.ship` for both channels | P06, P08 |

## 6. Per-phase notes

P01

* `ScenarioRunner.Run(string scenarioPath, TextWriter output)` loads the file with `System.Text.Json`, resets `Config` and `Log`, sets `Log.Out`, builds `ScenarioClock` and `StoreConsole`, writes the header, then calls `StoreConsole.Handle(stepNumber, JsonElement step)` for each step.
* `Program.Main(string[] args)` parses one optional path argument (default: walk up to `GangOfFour.slnx` and use `scenarios/trading-day.json`) and calls `ScenarioRunner.Run(path, Console.Out)`.
* `ScenarioRunnerTests` writes an inline scenario to a temp file and checks that two runs return identical text and that the header line is present. This keeps `dotnet test` from seeing zero tests.
* Tests project references: `xunit.v3`, `xunit.runner.visualstudio`, `Microsoft.NET.Test.Sdk` (versions from `Directory.Packages.props`). No Verify and no `Microsoft.Extensions.TimeProvider.Testing` until a test needs it (PD-3). `OutputType` is Exe for xUnit v3. The project needs `<Using Include="Xunit" />`; xunit.v3 4.0.2 does not add the global using (this corrects research E-11). The project reference path to the app is `..\..\src\CornerStore\CornerStore.csproj` so the layout works unchanged in every copy.
* Parallelisation (PD-7) in xunit.v3 4.0.2: `CollectionBehaviorAttribute.DisableTestParallelization` is an error (obsolete). Use `[assembly: Parallelization(Mode = ParallelMode.None)]` with `using Xunit.Sdk;` and `using Xunit.v3;`.
* `Directory.Packages.props` loses the `Verify.XunitV3` line in P01-S1. `Directory.Build.props` is unchanged.
* `.gitignore` after P01-S1: the Verify block (`*.received.*`, `*.received/`) becomes `*.received.txt`, and `!.copilot-tracking/plans/logs/` is appended at the end of the file (the negation must come after the `[Ll]ogs/` rule it overrides).
* The csharp-tests instruction asks for `GivenContext_WhenAction_ExpectedResult` names and NSubstitute. The repo rule is behaviour-describing names such as `PlaceOrder_WhenValidationFails_TakesNoPayment`, and baseline tests use no mocks (PD-9). Repo rules win; recorded in the changes log.

P02

* Sample data: about 12 products across a three-level category tree (Footwear > Shoes > Running, Home, Digital, Gift cards), at least one of each product type, two bundles' worth of components.
* `import` generates 100,000 listings from a deterministic formula; each listing builds `Brand`, `Manufacturer` and `SpecText` with `string.Concat` so no two listings share an instance. Only `imported 100000 listings` goes to the transcript.

P03

* `CartService` snapshots `Lines`, `PromoCode`, `GiftWrap`, `Insurance` and `ShippingMethod` into the cart's public `Saved*` fields before each change; `Undo` copies them back. One level of undo.

P04

* `PricingService.Quote(Cart, Customer)` returns a `Quote` (subtotal, discount, add-ons, shipping, tax, total). Order of operations is fixed so every later copy reproduces the same arithmetic: subtotal, promo discount, add-ons, shipping (method cost plus regional surcharge), tax on the discounted line amounts per line type, total.
* Gift cards are not taxed and weigh nothing; digital items are taxed and weigh nothing; physical items are taxed and weigh their `WeightKg`.
* AU rate 10 percent, NZ rate 15 percent. AU currency shows `A$`, NZ shows `NZ$`.

P05

* The four widgets are plain classes, each holding references to the other three. Each `Refresh` writes its lines to the transcript. The refresh order after a change is fixed in code and any mediator refactor must keep it.

P06

* `InventoryService.GetStock(sku)` adds a fixed cost to a private counter for every call and never prints it. `StoreConsole` and `BackOffice` call it repeatedly for the same sku.

P07

* `OrderService.Create` and `CreateInvoice` call the 11-parameter constructors. Each transition method has three zones in this order: guard (S20), state change, side effects (S19), so the zones stay readable and separately marked.

P08

* `OrderValidator.Validate(Customer, Cart, Quote)` returns the first failure message or null. Order of checks: stock, address, fraud, credit limit. `FraudScorer.Score(customer)` is `new Random(Config.Seed + customerNumber).Next(0, 100)`.
* `Checkout` order of operations (fixed): validate, quote, charge, create order, reserve stock, book shipment, mark paid, confirm. A pattern refactor must preserve this order.

P09

* The two exporters each load orders, filter by status, aggregate totals by line type and write rows, as separate copies of the same skeleton. CSV and JSON are written by hand with `StringBuilder`; no serializer.

P10

* `GoldenMasterTests.TradingDay_MatchesApprovedTranscript`, a plain xUnit v3 test with no snapshot library:
  * `RepositoryRoot.Find()` walks up from `AppContext.BaseDirectory` to the folder that holds `GangOfFour.slnx`.
  * It runs `<root>/scenarios/trading-day.json` through `ScenarioRunner.Run` into a `StringWriter`, giving `actual`.
  * If `GOLDEN_MASTER_APPROVE` equals `1`, it writes `actual` to `<root>/scenarios/expected/baseline.txt` (UTF-8 without BOM, no newline translation) and returns without comparing.
  * Otherwise it reads `baseline.txt` and compares with `string.Equals(..., StringComparison.Ordinal)`. A missing approved file counts as a mismatch.
  * On mismatch it writes `actual` to `baseline.received.txt` in the same folder, splits both texts on `\n`, finds the first differing line number and calls `Assert.Fail` with that line number and the expected and actual line (truncated to a readable length).
  * The file contains no baseline-specific path or name: only the shared `scenarios/` locations.
* Approval procedure: run once without the variable (it fails and writes the received file), review the received transcript by hand, run once with `GOLDEN_MASTER_APPROVE=1` (writes `baseline.txt`), delete `baseline.received.txt` if present, run again without the variable and see it pass. Only P10 does this. Every pattern task runs without the variable.
* Both files are read and written with an explicit `UTF8Encoding(false)`, so there is no BOM and the bytes equal what `Run` produced. `.gitattributes` forces LF for `*.txt` on checkout.
* `DeterminismTests.TradingDay_RunTwice_IsIdentical` runs the scenario twice and compares the strings (AC-002).

## 7. Tool designs (P11)

`tools/new-pattern.ps1 -Category <creational|structural|behavioral> -Name <kebab-name>`

* Validates the category and a kebab-case name; finds the repo root by walking up to `GangOfFour.slnx`; refuses if `patterns/<category>/<name>` exists.
* Copies `baseline/src` and `baseline/tests` into the pattern folder, skipping `bin` and `obj`.
* Writes `<PascalName>.slnx` referencing `src/CornerStore/CornerStore.csproj` and `tests/CornerStore.Tests/CornerStore.Tests.csproj`.
* Adds both projects to `GangOfFour.slnx` inside the `/patterns/<category>/` folder element, keeping two-space indentation, LF endings and no BOM.
* Writes `README.md` with a plain H1 and the eight headings from brief section 8.
* Written for Windows PowerShell 5.1 and PowerShell 7 (PD-12); `Set-StrictMode -Version Latest`, `$ErrorActionPreference = 'Stop'`. Tested in P11-S4 under both `pwsh` and `powershell.exe`. File writes use `[IO.File]::WriteAllText` with an absolute path and `UTF8Encoding($false)` because 5.1 `Set-Content -Encoding UTF8` adds a BOM.

`tools/diff-baseline.ps1 -Category <category> -Name <kebab-name>`

* Copies `baseline/src`, `baseline/tests` and the pattern's `src` and `tests` into a temp directory laid out as `baseline/...` and `pattern/...` (skipping `bin` and `obj`), then runs `git diff --no-index --no-color` from that directory, so paths in the output are stable and relative.
* Writes `DIFF.md` in the pattern folder: a plain H1, a file-level summary table (status, file, added, removed) and the full diff in a fenced block. With no changes it says `No differences from the baseline.`
* UTF-8 without BOM, LF endings, no timestamps or machine paths, so Windows and Linux produce identical files.

CI addition (FR-018): after the test step, a step with `shell: pwsh` loops over `patterns/*/*/` folders that contain a `.slnx`, runs `diff-baseline.ps1` for each, then `git diff --exit-code -- patterns` and fails if any `DIFF.md` is missing or changed. With no pattern folders it passes.

## 8. Verification commands

```powershell
dotnet restore GangOfFour.slnx
dotnet build GangOfFour.slnx --no-restore -c Release
dotnet test GangOfFour.slnx --no-build -c Release
```

AC-003, markers: every ID S01 to S23 present and nothing else.

```powershell
Get-ChildItem baseline/src -Recurse -Filter *.cs | Select-String -Pattern 'SMELL S\d+' -AllMatches | ForEach-Object { $_.Matches.Value } | Sort-Object -Unique
```

AC-006, banned APIs:

```powershell
Get-ChildItem baseline/src -Recurse -Filter *.cs | Select-String -Pattern 'DateTime\.(Now|UtcNow)|Guid\.NewGuid|Thread\.Sleep|Task\.Delay|GetHashCode'
```

AC-002, determinism: `DeterminismTests` plus a manual double run with `fc.exe` on the two outputs.

Pattern-leak scan (PA-2): case-insensitive search of `baseline/src` for the identifiers listed in the plan; every hit is either explained in the stop report or removed. `StringBuilder` (BCL) matches `Builder` and is a known false positive, not the Builder pattern.
