# CornerStore baseline

CornerStore is a fictional online store that runs as an in-memory console application. The baseline is deliberately shaped to show 23 code smells that commonly make changes harder. Its behavior is correct, and its trading-day transcript is the reference output for every pattern solution.

## The trading day

The scripted day in [`scenarios/trading-day.json`](../scenarios/trading-day.json) registers customers, browses products, builds carts, applies promotions, checks out, ships and refunds orders, adjusts stock, and exports reports. The app writes business outcomes to a transcript. Tests compare that transcript with [`scenarios/expected/baseline.txt`](../scenarios/expected/baseline.txt).

## Run CornerStore

Run these commands from the repository root:

```powershell
dotnet build baseline/CornerStore.Baseline.slnx
dotnet test baseline/CornerStore.Baseline.slnx
dotnet run --project baseline/src/CornerStore/CornerStore.csproj -- scenarios/trading-day.json
```

The solution targets .NET 10. The SDK version is pinned in [`global.json`](../global.json).

## Why the code is messy on purpose

Each smell is a learning seam. A pattern solution starts as a copy of this baseline and refactors exactly one seam. The remaining code, including its other smell markers, stays unchanged so you can compare the before and after shape without confusing the result with unrelated cleanup. See the [pattern map](../docs/pattern-map.md) for the intended refactor and the [smells catalog](../docs/smells-catalog.md) for the baseline locations and scenario coverage.

## The 23 smells

| Smell | Name | Baseline location | Fixed by |
| --- | --- | --- | --- |
| [S01](../docs/smells-catalog.md) | Regional rules are scattered | `src/CornerStore/Pricing/TaxCalculator.cs`, `ShippingCalculator.cs`, `MoneyFormatter.cs` | [Abstract Factory](../patterns/creational/abstract-factory/README.md) |
| [S02](../docs/smells-catalog.md) | Orders and invoices use positional constructors | `src/CornerStore/Orders/Order.cs`, `Invoice.cs`, `OrderService.cs` | [Builder](../patterns/creational/builder/README.md) |
| [S03](../docs/smells-catalog.md) | Checkout constructs payment processors in a switch | `src/CornerStore/StoreConsole.cs`, `Checkout/IPaymentProcessor.cs` | [Factory Method](../patterns/creational/factory-method/README.md) |
| [S04](../docs/smells-catalog.md) | Product variants copy fields by hand | `src/CornerStore/Catalog/Catalog.cs` | [Prototype](../patterns/creational/prototype/README.md) |
| [S05](../docs/smells-catalog.md) | Configuration and logging are mutable static state | `src/CornerStore/Config.cs`, `Log.cs`, `ScenarioRunner.cs`, `StoreConsole.cs` | [Singleton](../patterns/creational/singleton/README.md) |
| [S06](../docs/smells-catalog.md) | Checkout depends directly on courier XML | `src/CornerStore/StoreConsole.cs`, `Checkout/ShipFastClient.cs` | [Adapter](../patterns/structural/adapter/README.md) |
| [S07](../docs/smells-catalog.md) | Message type and delivery channel create duplicate classes | `src/CornerStore/Notifications/OrderConfirmedEmail.cs`, `OrderConfirmedSms.cs`, `OrderShippedEmail.cs`, `OrderShippedSms.cs` | [Bridge](../patterns/structural/bridge/README.md) |
| [S08](../docs/smells-catalog.md) | Bundles follow a separate catalog and pricing path | `src/CornerStore/Catalog/Catalog.cs`, `Cart/CartService.cs` | [Composite](../patterns/structural/composite/README.md) |
| [S09](../docs/smells-catalog.md) | Add-ons are flags and price-calculation branches | `src/CornerStore/Cart/Cart.cs`, `Pricing/PricingService.cs`, `StoreConsole.cs` | [Decorator](../patterns/structural/decorator/README.md) |
| [S10](../docs/smells-catalog.md) | The console coordinates checkout subsystems | `src/CornerStore/StoreConsole.cs` | [Facade](../patterns/structural/facade/README.md) |
| [S11](../docs/smells-catalog.md) | Imported listings duplicate product specifications | `src/CornerStore/Catalog/Listing.cs`, `ListingImporter.cs` | [Flyweight](../patterns/structural/flyweight/README.md) |
| [S12](../docs/smells-catalog.md) | Inventory reads are uncached and access checks are scattered | `src/CornerStore/Inventory/InventoryService.cs`, `Orders/BackOffice.cs` | [Proxy](../patterns/structural/proxy/README.md) |
| [S13](../docs/smells-catalog.md) | Checkout validation is one nested method | `src/CornerStore/Checkout/OrderValidator.cs` | [Chain of Responsibility](../patterns/behavioral/chain-of-responsibility/README.md) |
| [S14](../docs/smells-catalog.md) | Back-office actions run inline without reusable action objects | `src/CornerStore/Orders/BackOffice.cs` | [Command](../patterns/behavioral/command/README.md) |
| [S15](../docs/smells-catalog.md) | Promotion rules are parsed with string operations | `src/CornerStore/Pricing/PromoEngine.cs` | [Interpreter](../patterns/behavioral/interpreter/README.md) |
| [S16](../docs/smells-catalog.md) | Catalog operations repeat recursive tree walks | `src/CornerStore/Catalog/Catalog.cs` | [Iterator](../patterns/behavioral/iterator/README.md) |
| [S17](../docs/smells-catalog.md) | Cart widgets update each other directly | `src/CornerStore/Widgets/ItemListWidget.cs`, `PromoBoxWidget.cs`, `ShippingSelectorWidget.cs`, `TotalsPanelWidget.cs` | [Mediator](../patterns/behavioral/mediator/README.md) |
| [S18](../docs/smells-catalog.md) | Cart internals are public for undo snapshots | `src/CornerStore/Cart/Cart.cs`, `StoreConsole.cs` | [Memento](../patterns/behavioral/memento/README.md) |
| [S19](../docs/smells-catalog.md) | Order transitions call every side-effect service | `src/CornerStore/Orders/OrderService.cs` | [Observer](../patterns/behavioral/observer/README.md) |
| [S20](../docs/smells-catalog.md) | Order transitions branch on status strings | `src/CornerStore/Orders/Order.cs`, `OrderService.cs` | [State](../patterns/behavioral/state/README.md) |
| [S21](../docs/smells-catalog.md) | Shipping cost is selected with a switch | `src/CornerStore/Pricing/PricingService.cs` | [Strategy](../patterns/behavioral/strategy/README.md) |
| [S22](../docs/smells-catalog.md) | CSV and JSON exporters duplicate their workflow | `src/CornerStore/Reporting/CsvReportExporter.cs`, `JsonReportExporter.cs` | [Template Method](../patterns/behavioral/template-method/README.md) |
| [S23](../docs/smells-catalog.md) | Tax, weight and reporting switch on line-item types | `src/CornerStore/Pricing/TaxCalculator.cs`, `WeightCalculator.cs`, `Reporting/CsvReportExporter.cs`, `JsonReportExporter.cs` | [Visitor](../patterns/behavioral/visitor/README.md) |

For the complete explanations and the trading-day steps that exercise each smell, see the [smells catalog](../docs/smells-catalog.md).
