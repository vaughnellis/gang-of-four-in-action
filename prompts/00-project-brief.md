---
title: Gang of Four in Action - Project Brief
purpose: Shared context for every RPI task in this repository. Read this first; the numbered briefs define the task.
---

# Gang of Four in Action: Project Brief

This file is the shared context for every HVE Core RPI task in this repository. It is not a task by itself. Each numbered brief in `prompts/` defines one bounded task and references this file.

Treat everything under **Decisions** as settled. Do not re-research or re-open them unless a brief explicitly asks.

## 1. Purpose

A showcase repository for the 23 Gang of Four design patterns, built as homage to *Design Patterns: Elements of Reusable Object-Oriented Software* (Gamma, Helm, Johnson, Vlissides).

The repository contains:

- **One baseline solution**: a working but deliberately messy console app (CornerStore) with 23 catalogued code smells.
- **One solution per pattern**: a copy of the baseline in which exactly one smell is refactored using that pattern.
- **A capstone** (later): every pattern applied together.

A companion blog will explain each pattern using its folder in this repo. The repo is also a showcase of building with Microsoft HVE Core: every task is planned, implemented and reviewed through RPI.

## 2. Guiding principles

1. **Realistic mess, not absurd mess.** The baseline looks like code people inherit: a god class, switches on strings, `new` everywhere. Never a strawman.
2. **One pattern, one seam.** A pattern solution changes only the code its pattern fixes. Everything else stays byte-identical to the baseline.
3. **Same behaviour, better shape.** Every solution produces the same transcript as the baseline for a fixed scenario.
4. **In-domain first.** Patterns are applied to the baseline wherever the fit is honest. Where it would be forced, the baseline carries a small, realistic feature that needs the pattern (marked Extension).
5. **Honest trade-offs.** Every pattern write-up says when not to use it.

## 3. Decisions (settled)

| ID | Decision |
| --- | --- |
| D-01 | Language and runtime: C# on .NET 10 (LTS). Pin the SDK with `global.json`. |
| D-02 | Domain: fictional online store named **CornerStore**. |
| D-03 | Host: console app with in-memory data. No database, web host or authentication. |
| D-04 | Layout: monorepo on `main`. Each pattern folder holds a full copy of the baseline as its own solution. No branch-per-pattern. |
| D-05 | Naming: project and namespace names are identical in every copy (`CornerStore`, `CornerStore.Tests`). Only the solution file name differs (`Facade.slnx`, `Strategy.slnx`). |
| D-06 | Solution format: `.slnx`. Shared settings in `Directory.Build.props`; package versions in `Directory.Packages.props` (Central Package Management). |
| D-07 | Build settings: nullable enabled, warnings as errors, implicit usings enabled. |
| D-08 | Tests: xUnit. Golden master via Verify, with the approved transcript stored at the shared path `scenarios/expected/baseline.txt`. |
| D-09 | Determinism: no `DateTime.Now`, `Guid.NewGuid()` or unseeded `Random` in app code. Time comes from a `TimeProvider` set to the scenario's `startTime`; IDs are sequential; `Random` is seeded from the scenario; formatting uses invariant culture. |
| D-10 | Workflow per pattern: two commits. Commit 1 copies the baseline unchanged; commit 2 applies the refactor. The human makes both commits; the agent stops after the copy step so the first commit can be made. |
| D-11 | Baseline is frozen at tag `baseline-v1` before any pattern folder is created. |
| D-12 | No text or diagrams reproduced from the GoF book. Paraphrase intents and cite the chapter. |

## 4. Repository structure

```text
gang-of-four-in-action/
├── README.md
├── LICENSE
├── global.json
├── Directory.Build.props
├── Directory.Packages.props
├── GangOfFour.slnx                # umbrella solution used by CI
├── prompts/                       # RPI briefs (this folder)
├── .copilot-tracking/             # HVE Core artifacts: plans, changes, reviews
├── docs/
│   ├── smells-catalog.md
│   ├── pattern-map.md
│   └── comparisons.md
├── baseline/
│   ├── CornerStore.Baseline.slnx
│   ├── src/CornerStore/
│   └── tests/CornerStore.Tests/
├── patterns/
│   ├── creational/<pattern>/
│   ├── structural/<pattern>/
│   └── behavioral/<pattern>/
├── capstone/
├── scenarios/
│   ├── trading-day.json
│   └── expected/baseline.txt
├── tools/
│   ├── new-pattern.ps1
│   └── diff-baseline.ps1
└── .github/
    ├── copilot-instructions.md    # repo-wide rules Copilot applies automatically
    ├── instructions/
    │   ├── baseline.instructions.md   # applies to baseline/**
    │   └── patterns.instructions.md   # applies to patterns/**
    └── workflows/ci.yml
```

The files under `.github/` other than `workflows/` are maintained by the human. They restate the most important rules from this brief so Copilot follows them even outside an RPI task.

A pattern folder looks like this:

```text
patterns/structural/facade/
├── Facade.slnx
├── README.md
├── DIFF.md                        # generated by tools/diff-baseline.ps1
├── src/CornerStore/
└── tests/CornerStore.Tests/
    ├── GoldenMasterTests.cs       # identical to the baseline copy
    └── FacadeTests.cs
```

Conventions:

- Pattern folder names are kebab-case: `abstract-factory`, `chain-of-responsibility`, `template-method`.
- Commit messages start with the pattern slug or area: `facade: extract CheckoutFacade from OrderService`.
- Tags: `baseline-v1`, then `<pattern>-v1` when a pattern meets its definition of done. Tags are created by the human, not the agent.

## 5. The baseline: CornerStore

CornerStore runs a scripted trading day from `scenarios/trading-day.json`: customers browse, fill carts, apply promo codes, check out and pay, orders ship, notifications go out, back-office staff adjust stock and issue refunds, and the shop runs reports. Every action writes a line to a transcript.

| Feature area | What the app does | Smells |
| --- | --- | --- |
| Catalog | Products, nested categories, bundles, size and colour variants, bulk listing import | S04, S08, S11, S16 |
| Cart | Add and remove items, apply promo codes, undo the last change | S17, S18 |
| Checkout | Validate, price, charge, reserve stock, ship, confirm | S02, S10, S13 |
| Pricing and promotions | Shipping methods, add-ons (gift wrap, insurance), rule-based promos | S09, S15, S21 |
| Payments and shipping | Card, PayPal and Afterpay payments; a third-party courier SDK | S03, S06 |
| Regions | AU and NZ storefronts with their own tax, shipping and currency formatting | S01 |
| Order lifecycle | Pending, Paid, Shipped, Delivered, Cancelled, Refunded | S19, S20 |
| Notifications | Order and shipping messages over email and SMS (written to the transcript) | S07 |
| Back office | Stock adjustments, refunds, cancellations, slow inventory lookups | S12, S14 |
| Reporting | Sales reports exported as CSV and JSON; tax and weight totals by line item type | S22, S23 |
| Cross-cutting | Configuration and logging | S05 |

Rules for the baseline:

- Every smell has a marker comment at its location: `// SMELL S10: caller orchestrates checkout subsystems`. A smell spanning several files has a marker in each.
- The code is wrong in shape, never wrong in behaviour. No intentional bugs.
- The baseline must not already use any of the 23 patterns at the seams listed below.
- Keep it small enough to skim in one sitting. If a feature needs more than a handful of classes to show its smell, simplify the feature.
- External systems (payment providers, courier SDK, email, SMS) are in-process fakes that write to the transcript.

## 6. Pattern map

Fit: **Natural** means the pattern fixes a smell the domain has anyway. **Extension** means the baseline carries a small feature mainly to host the pattern.

### Creational (`patterns/creational/`)

| Smell | Pattern | Folder | Baseline smell | Refactor target | Fit |
| --- | --- | --- | --- | --- | --- |
| S01 | Abstract Factory | `abstract-factory` | `if (region == "AU")` branches scattered across tax, shipping and currency code; nothing stops AU tax mixing with NZ shipping | `IRegionFactory` with AU and NZ factories producing matched families | Natural |
| S02 | Builder | `builder` | `Order` and `Invoice` built through 11-parameter constructors with nulls for optional parts | Fluent `OrderBuilder` and `InvoiceBuilder` that validate on `Build()` | Natural |
| S03 | Factory Method | `factory-method` | `switch (paymentType)` with `new CardProcessor()` and friends inside checkout | Creator classes that override `CreateProcessor()` | Natural |
| S04 | Prototype | `prototype` | Size and colour variants built by copying every field of the parent product by hand | `Clone()` on products; note records and `with` as the modern form | Natural |
| S05 | Singleton | `singleton` | Static `Config` and `Log` classes with public mutable fields, set from anywhere | Thread-safe Singleton via `Lazy<T>`, then the same object as a DI singleton lifetime, with trade-offs | Natural |

### Structural (`patterns/structural/`)

| Smell | Pattern | Folder | Baseline smell | Refactor target | Fit |
| --- | --- | --- | --- | --- | --- |
| S06 | Adapter | `adapter` | Checkout calls the `ShipFastClient` SDK directly: XML strings, prices in cents, mismatched method names | `IShippingProvider` with a `ShipFastAdapter` | Natural |
| S07 | Bridge | `bridge` | Class explosion: `OrderConfirmedEmail`, `OrderConfirmedSms`, `OrderShippedEmail`, `OrderShippedSms` | Message abstraction bridged to an `IChannel` implementation | Natural |
| S08 | Composite | `composite` | Bundles priced through a separate path with `is Bundle` checks; bundles cannot nest | `ICatalogItem` shared by `Product` and `Bundle` | Natural |
| S09 | Decorator | `decorator` | Add-ons as boolean flags and an if-chain in the price calculation | `GiftWrap` and `Insurance` decorators around a base price component | Natural |
| S10 | Facade | `facade` | The console flow calls inventory, pricing, payment, shipping and notifications directly in a fixed order | `CheckoutFacade.PlaceOrder()` | Natural |
| S11 | Flyweight | `flyweight` | Bulk import of 100,000 listings, each holding its own copy of brand, manufacturer and spec text | Shared `ProductSpec` flyweights from a factory, with a BenchmarkDotNet memory comparison | Extension |
| S12 | Proxy | `proxy` | Slow `InventoryService` called repeatedly with no caching; admin-only stock changes unchecked | Caching proxy and protection proxy behind `IInventoryService` | Natural |

### Behavioral (`patterns/behavioral/`)

| Smell | Pattern | Folder | Baseline smell | Refactor target | Fit |
| --- | --- | --- | --- | --- | --- |
| S13 | Chain of Responsibility | `chain-of-responsibility` | One long validation method: stock, address, fraud score and credit limit in nested ifs | Ordered validation handlers | Natural |
| S14 | Command | `command` | Back-office actions run inline with no undo, audit trail or queue | `ICommand` objects with `Execute` and `Undo`, plus an audit log | Natural |
| S15 | Interpreter | `interpreter` | Promo rules such as `SPEND>100 AND CATEGORY=Shoes` parsed with `Split` and `IndexOf` | A small grammar and expression tree evaluated against a cart | Extension |
| S16 | Iterator | `iterator` | Callers walk the category tree with manual recursion, copy-pasted in several places | Depth-first and breadth-first iterators; explain `IEnumerable` with `yield` as the built-in form | Natural |
| S17 | Mediator | `mediator` | Cart widgets (item list, promo box, shipping selector, totals) reference and update each other directly | `CartMediator` that the widgets notify | Extension |
| S18 | Memento | `memento` | Undo works by making cart internals public and copying them | `CartMemento` captured and restored by a caretaker | Natural |
| S19 | Observer | `observer` | After a status change, `OrderService` directly calls email, stock, loyalty and analytics | Order events with subscribers; note C# events and `IObservable<T>` | Natural |
| S20 | State | `state` | `order.Status` string checked with if/else in Pay, Ship, Cancel and Refund | One class per order state that owns its transitions | Natural |
| S21 | Strategy | `strategy` | Shipping cost `switch` on method inside the pricing code | `IShippingCostStrategy` per method, chosen at checkout | Natural |
| S22 | Template Method | `template-method` | CSV and JSON exporters duplicate the same load, filter, aggregate, write skeleton | `ReportExporter` base class with abstract steps and hooks | Natural |
| S23 | Visitor | `visitor` | Physical, digital and gift card line items handled with type switches in tax, weight and export code | `ILineItemVisitor` with tax, weight and export visitors | Natural |

## 7. Behaviour safety net

**Golden master.** `scenarios/trading-day.json` scripts the whole day. The baseline produces a transcript, approved once and stored at `scenarios/expected/baseline.txt`. Every solution's `GoldenMasterTests` runs the same scenario through the app's entry point and must produce an identical transcript.

- Tests locate the repository root by walking up to `GangOfFour.slnx`, so every copy compares against the same shared files.
- Tests drive the app only through its public entry point (`ScenarioRunner.Run(scenarioPath, TextWriter)`), never internal classes, so `GoldenMasterTests.cs` is identical in every copy.
- A pattern that adds behaviour (Command's undo, Flyweight's benchmark) proves it with extra tests. The shared transcript must still match.

**Diff against baseline.** `tools/diff-baseline.ps1 -Category <category> -Name <pattern>` runs `git diff --no-index` between `baseline/src` and the pattern's `src` and writes `DIFF.md`. Each pattern README declares the files it expects to change.

**Baseline freeze.** After `baseline-v1`, any baseline fix becomes `baseline-v2`, applied to finished patterns as a patch.

**CI.** GitHub Actions builds and tests every solution, runs the golden master, and fails if any `DIFF.md` is stale.

## 8. Pattern README template

1. **Intent.** In our own words, citing the GoF chapter.
2. **The smell.** Smell ID and a short excerpt from the baseline.
3. **The refactor.** What was introduced, with a Mermaid class diagram.
4. **Diff highlights.** The changes that matter; link to `DIFF.md`.
5. **Trade-offs.** Costs, and when not to use the pattern.
6. **Related patterns and common confusions.**
7. **Modern .NET notes.** Where the platform already provides the pattern.
8. **Blog post link.** Placeholder until published.

## 9. Definition of done (every pattern)

- Builds with zero warnings.
- Golden master transcript matches `scenarios/expected/baseline.txt` byte for byte.
- Pattern-specific unit tests pass.
- Changes stay inside the files declared in the README.
- The `SMELL Sxx` marker for this pattern's smell is removed; every other marker is untouched.
- README complete, including the Mermaid diagram and trade-offs.
- `DIFF.md` regenerated and committed.
- Ready for two commits: unchanged copy, then refactor (made by the human).

## 10. Global boundaries (apply to every task)

- Never modify `baseline/` after `baseline-v1` unless the brief is a baseline fix.
- Never modify another pattern's folder.
- Never change project or namespace names in a copy.
- Never edit `scenarios/expected/baseline.txt` from a pattern task.
- Never edit `prompts/`, `.github/copilot-instructions.md` or `.github/instructions/`. Propose changes in the change record.
- Never push, tag, or create releases. Stop and hand back to the human for anything externally visible.
- Prose in READMEs and docs: plain, direct, no em dashes.
