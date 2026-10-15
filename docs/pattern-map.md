# Pattern map

Each of the 23 patterns is paired with one smell in the CornerStore baseline. Fit is Natural when the domain has the smell anyway, and Extension when the baseline carries a small feature mainly to host the pattern.

## Creational

| Smell | Pattern | Folder | Baseline smell | Refactor target | Fit | Status |
| --- | --- | --- | --- | --- | --- | --- |
| S01 | Abstract Factory | `abstract-factory` | `if (region == "AU")` branches scattered across tax, shipping and currency code; nothing stops AU tax mixing with NZ shipping | `IRegionFactory` with AU and NZ factories producing matched families | Natural | Done |
| S02 | Builder | `builder` | `Order` and `Invoice` built through 11-parameter constructors with nulls for optional parts | Fluent `OrderBuilder` and `InvoiceBuilder` that validate on `Build()` | Natural | Done |
| S03 | Factory Method | `factory-method` | `switch (paymentType)` with `new CardProcessor()` and friends inside checkout | Creator classes that override `CreateProcessor()` | Natural | Done |
| S04 | Prototype | `prototype` | Size and colour variants built by copying every field of the parent product by hand | `Clone()` on products; note records and `with` as the modern form | Natural | Not started |
| S05 | Singleton | `singleton` | Static `Config` and `Log` classes with public mutable fields, set from anywhere | Thread-safe Singleton via `Lazy<T>`, then the same object as a DI singleton lifetime, with trade-offs | Natural | Not started |

## Structural

| Smell | Pattern | Folder | Baseline smell | Refactor target | Fit | Status |
| --- | --- | --- | --- | --- | --- | --- |
| S06 | Adapter | `adapter` | Checkout calls the `ShipFastClient` SDK directly: XML strings, prices in cents, mismatched method names | `IShippingProvider` with a `ShipFastAdapter` | Natural | Done |
| S07 | Bridge | `bridge` | Class explosion: `OrderConfirmedEmail`, `OrderConfirmedSms`, `OrderShippedEmail`, `OrderShippedSms` | Message abstraction bridged to an `IChannel` implementation | Natural | Done |
| S08 | Composite | `composite` | Bundles priced through a separate path with `is Bundle` checks; bundles cannot nest | `ICatalogItem` shared by `Product` and `Bundle` | Natural | Done |
| S09 | Decorator | `decorator` | Add-ons as boolean flags and an if-chain in the price calculation | `GiftWrap` and `Insurance` decorators around a base price component | Natural | Done |
| S10 | Facade | `facade` | The console flow calls inventory, pricing, payment, shipping and notifications directly in a fixed order | `CheckoutFacade.PlaceOrder()` | Natural | Done |
| S11 | Flyweight | `flyweight` | Bulk import of 100,000 listings, each holding its own copy of brand, manufacturer and spec text | Shared `ProductSpec` flyweights from a factory, with a BenchmarkDotNet memory comparison | Extension | Not started |
| S12 | Proxy | `proxy` | Slow `InventoryService` called repeatedly with no caching; admin-only stock changes unchecked | Caching proxy and protection proxy behind `IInventoryService` | Natural | Done |

## Behavioral

| Smell | Pattern | Folder | Baseline smell | Refactor target | Fit | Status |
| --- | --- | --- | --- | --- | --- | --- |
| S13 | Chain of Responsibility | `chain-of-responsibility` | One long validation method: stock, address, fraud score and credit limit in nested ifs | Ordered validation handlers | Natural | Done |
| S14 | Command | `command` | Back-office actions run inline with no undo, audit trail or queue | `ICommand` objects with `Execute` and `Undo`, plus an audit log | Natural | Done |
| S15 | Interpreter | `interpreter` | Promo rules such as `SPEND>100 AND CATEGORY=Shoes` parsed with `Split` and `IndexOf` | A small grammar and expression tree evaluated against a cart | Extension | Not started |
| S16 | Iterator | `iterator` | Callers walk the category tree with manual recursion, copy-pasted in several places | Depth-first and breadth-first iterators; explain `IEnumerable` with `yield` as the built-in form | Natural | Done |
| S17 | Mediator | `mediator` | Cart widgets (item list, promo box, shipping selector, totals) reference and update each other directly | `CartMediator` that the widgets notify | Extension | Done |
| S18 | Memento | `memento` | Undo works by making cart internals public and copying them | `CartMemento` captured and restored by a caretaker | Natural | Done |
| S19 | Observer | `observer` | After a status change, `OrderService` directly calls email, stock, loyalty and analytics | Order events with subscribers; note C# events and `IObservable<T>` | Natural | Done |
| S20 | State | `state` | `order.Status` string checked with if/else in Pay, Ship, Cancel and Refund | One class per order state that owns its transitions | Natural | Done |
| S21 | Strategy | `strategy` | Shipping cost `switch` on method inside the pricing code | `IShippingCostStrategy` per method, chosen at checkout | Natural | Done |
| S22 | Template Method | `template-method` | CSV and JSON exporters duplicate the same load, filter, aggregate, write skeleton | `ReportExporter` base class with abstract steps and hooks | Natural | Done |
| S23 | Visitor | `visitor` | Physical, digital and gift card line items handled with type switches in tax, weight and export code | `ILineItemVisitor` with tax, weight and export visitors | Natural | Done |
