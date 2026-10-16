---
title: Gang of Four in Action
description: A C# showcase of 23 Gang of Four design patterns, from baseline refactors to an integrated capstone.
---

## Overview

A showcase of the 23 Gang of Four design patterns in C# on .NET 10, built as an homage to *Design Patterns: Elements of Reusable Object-Oriented Software* by Gamma, Helm, Johnson and Vlissides.

The repository holds one deliberately messy console app and one solution per pattern. The app is CornerStore, a small fictional online store with 23 catalogued code smells. Each pattern solution is a copy of the baseline in which exactly one smell is refactored with that pattern, and every solution produces the same transcript as the baseline. The [capstone](capstone/README.md) applies all 23 patterns together and describes how their seams fit.

A companion blog explains each pattern using its folder in this repository.

## Status

The baseline, approved trading-day transcript and copy/diff tooling are complete. Pattern solutions are added one at a time by their tasks; the index below tracks all 23.

## How the repository is organised

```text
gang-of-four-in-action/
├── baseline/        the messy CornerStore app and its tests
├── patterns/
│   ├── creational/  one folder per creational pattern
│   ├── structural/  one folder per structural pattern
│   └── behavioral/  one folder per behavioral pattern
├── capstone/        integrated CornerStore app and pattern walkthrough
├── scenarios/       the scripted trading day and the approved transcript
├── tools/           scripts to create a pattern folder and diff it against the baseline
├── docs/            smells catalog, pattern map and [pattern comparisons](docs/comparisons.md)
├── prompts/         the task briefs used to build this repository
└── GangOfFour.slnx  umbrella solution used by CI
```

Shared settings live in `Directory.Build.props` and package versions in `Directory.Packages.props`. The SDK version is pinned in `global.json`.

## How to run a solution

You need the .NET SDK version named in `global.json`.

```powershell
dotnet build GangOfFour.slnx
dotnet test GangOfFour.slnx
```

Each pattern folder holds its own solution, such as `patterns/structural/facade/Facade.slnx`. Build and test it the same way by passing that file to `dotnet build` and `dotnet test`.

The umbrella solution includes the baseline and all 23 patterns (24 test projects). Each copy has its own nested solution folder, such as `/baseline/` or `/patterns/behavioral/strategy/`, so project and namespace names stay `CornerStore` and `CornerStore.Tests` in every copy. `tools/new-pattern.ps1` adds new patterns to their own solution folders. Build outputs remain in each project's local `bin` and `obj` directories.

## Pattern index

Smell IDs match [docs/pattern-map.md](docs/pattern-map.md), which also tracks progress.

### Creational

| Smell | Pattern | Folder |
| --- | --- | --- |
| S01 | Abstract Factory | [patterns/creational/abstract-factory](patterns/creational/abstract-factory/) |
| S02 | Builder | [patterns/creational/builder](patterns/creational/builder/) |
| S03 | Factory Method | [patterns/creational/factory-method](patterns/creational/factory-method/) |
| S04 | Prototype | [patterns/creational/prototype](patterns/creational/prototype/) |
| S05 | Singleton | [patterns/creational/singleton](patterns/creational/singleton/) |

### Structural

| Smell | Pattern | Folder |
| --- | --- | --- |
| S06 | Adapter | [patterns/structural/adapter](patterns/structural/adapter/) |
| S07 | Bridge | [patterns/structural/bridge](patterns/structural/bridge/) |
| S08 | Composite | [patterns/structural/composite](patterns/structural/composite/) |
| S09 | Decorator | [patterns/structural/decorator](patterns/structural/decorator/) |
| S10 | Facade | [patterns/structural/facade](patterns/structural/facade/) |
| S11 | Flyweight | [patterns/structural/flyweight](patterns/structural/flyweight/) |
| S12 | Proxy | [patterns/structural/proxy](patterns/structural/proxy/) |

### Behavioral

| Smell | Pattern | Folder |
| --- | --- | --- |
| S13 | Chain of Responsibility | [patterns/behavioral/chain-of-responsibility](patterns/behavioral/chain-of-responsibility/) |
| S14 | Command | [patterns/behavioral/command](patterns/behavioral/command/) |
| S15 | Interpreter | [patterns/behavioral/interpreter](patterns/behavioral/interpreter/) |
| S16 | Iterator | [patterns/behavioral/iterator](patterns/behavioral/iterator/) |
| S17 | Mediator | [patterns/behavioral/mediator](patterns/behavioral/mediator/) |
| S18 | Memento | [patterns/behavioral/memento](patterns/behavioral/memento/) |
| S19 | Observer | [patterns/behavioral/observer](patterns/behavioral/observer/) |
| S20 | State | [patterns/behavioral/state](patterns/behavioral/state/) |
| S21 | Strategy | [patterns/behavioral/strategy](patterns/behavioral/strategy/) |
| S22 | Template Method | [patterns/behavioral/template-method](patterns/behavioral/template-method/) |
| S23 | Visitor | [patterns/behavioral/visitor](patterns/behavioral/visitor/) |

## Built with HVE Core

Every task in this repository is planned, implemented and reviewed with Microsoft HVE Core using the Research, Plan, Implement, Review loop. The task briefs are in [prompts/README.md](prompts/README.md), and the plans, change records and reviews they produce are kept in [.copilot-tracking/](.copilot-tracking/).

## License

MIT. See [LICENSE](LICENSE).
