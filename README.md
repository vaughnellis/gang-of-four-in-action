---
title: Gang of Four in Action
description: A C# showcase of the 23 Gang of Four design patterns, from baseline refactors to an integrated capstone.
---

# Gang of Four in Action

This repository is a C# and .NET 10 homage to *Design Patterns: Elements of Reusable Object-Oriented Software* by Gamma, Helm, Johnson and Vlissides. It uses CornerStore, a fictional online shop, to show the 23 patterns against the same working baseline. Each standalone pattern solution changes one code seam, and the capstone brings all 23 refactors together.

## Start here

Follow the code from its original shape through each refactor:

1. Read the [CornerStore baseline](baseline/README.md) and its 23 smells.
2. Explore the [creational](patterns/creational/README.md), [structural](patterns/structural/README.md) and [behavioral](patterns/behavioral/README.md) patterns in book order.
3. Compare commonly confused patterns in [Pattern Comparisons](docs/comparisons.md).
4. See how the patterns meet in the [capstone](capstone/README.md).

## Quick start

Clone the repository, then restore, build and test the umbrella solution from its root:

```powershell
git clone https://github.com/vaughnellis/gang-of-four-in-action.git
cd gang-of-four-in-action
dotnet restore GangOfFour.slnx
dotnet build GangOfFour.slnx --no-restore -c Release
dotnet test GangOfFour.slnx --no-build -c Release
```

Install the .NET SDK version pinned in [`global.json`](global.json). The umbrella solution includes the baseline and all 23 standalone pattern solutions. To run the scripted trading day, use:

```powershell
dotnet run --project baseline/src/CornerStore/CornerStore.csproj -- scenarios/trading-day.json
```

Each individual pattern and the capstone also has its own solution file. The relevant README gives its build, test and scenario commands.

## Pattern index

The smell IDs refer to the entries in the [smells catalog](docs/smells-catalog.md). Intents are paraphrased for this CornerStore example.

### Creational

| Smell | Pattern | Intent |
| --- | --- | --- |
| S01 | [Abstract Factory](patterns/creational/abstract-factory/README.md) | Create matched services for each storefront region. |
| S02 | [Builder](patterns/creational/builder/README.md) | Assemble orders and invoices through validated steps. |
| S03 | [Factory Method](patterns/creational/factory-method/README.md) | Let payment creators choose which processor to construct. |
| S04 | [Prototype](patterns/creational/prototype/README.md) | Create product variants by copying an existing product. |
| S05 | [Singleton](patterns/creational/singleton/README.md) | Share one instance of application-wide services. |

### Structural

| Smell | Pattern | Intent |
| --- | --- | --- |
| S06 | [Adapter](patterns/structural/adapter/README.md) | Translate the courier API to the store's shipping contract. |
| S07 | [Bridge](patterns/structural/bridge/README.md) | Vary order messages separately from delivery channels. |
| S08 | [Composite](patterns/structural/composite/README.md) | Treat products and nested bundles through one contract. |
| S09 | [Decorator](patterns/structural/decorator/README.md) | Compose optional charges around a base price. |
| S10 | [Facade](patterns/structural/facade/README.md) | Give checkout one entry point for coordinating services. |
| S11 | [Flyweight](patterns/structural/flyweight/README.md) | Share repeated product specifications across listings. |
| S12 | [Proxy](patterns/structural/proxy/README.md) | Cache inventory reads and guard stock changes. |

### Behavioral

| Smell | Pattern | Intent |
| --- | --- | --- |
| S13 | [Chain of Responsibility](patterns/behavioral/chain-of-responsibility/README.md) | Pass checkout through ordered validation handlers. |
| S14 | [Command](patterns/behavioral/command/README.md) | Represent back-office actions as executable objects. |
| S15 | [Interpreter](patterns/behavioral/interpreter/README.md) | Parse and evaluate promotion rules as expressions. |
| S16 | [Iterator](patterns/behavioral/iterator/README.md) | Traverse the category tree through reusable iterators. |
| S17 | [Mediator](patterns/behavioral/mediator/README.md) | Coordinate cart widget updates through a mediator. |
| S18 | [Memento](patterns/behavioral/memento/README.md) | Capture and restore cart state without exposing internals. |
| S19 | [Observer](patterns/behavioral/observer/README.md) | Notify subscribers when an order status changes. |
| S20 | [State](patterns/behavioral/state/README.md) | Delegate legal order transitions to state-specific behavior. |
| S21 | [Strategy](patterns/behavioral/strategy/README.md) | Select a shipping-cost algorithm for the chosen method. |
| S22 | [Template Method](patterns/behavioral/template-method/README.md) | Share report processing while customizing output formats. |
| S23 | [Visitor](patterns/behavioral/visitor/README.md) | Add operations across the store's line-item types. |

## How this was built

The repository was planned, implemented and reviewed with Microsoft HVE Core's Research, Plan, Implement, Review workflow. The [task briefs](prompts/README.md) describe the work. The `.copilot-tracking/` records document the plans, changes and reviews. The run-all progress record lists 24 completed tasks, the baseline and its 23 pattern solutions. The capstone progress record lists 24 completed steps and reaches 131 tests.

## License

MIT. See [LICENSE](LICENSE).
