# Structural patterns

Structural patterns organize objects and classes into larger arrangements. In CornerStore, they provide ways to adapt external APIs, combine catalog items, compose pricing, coordinate checkout and share repeated product data.

| Pattern | Intent | Smell fixed |
| --- | --- | --- |
| [Adapter](adapter/README.md) | Translate CornerStore's shipping contract to the courier API. | [S06: direct courier protocol dependency](../../docs/smells-catalog.md) |
| [Bridge](bridge/README.md) | Vary order messages separately from their delivery channels. | [S07: duplicate message and channel classes](../../docs/smells-catalog.md) |
| [Composite](composite/README.md) | Treat products and nested bundles through one catalog-item contract. | [S08: separate bundle paths](../../docs/smells-catalog.md) |
| [Decorator](decorator/README.md) | Add optional charges by composing price components. | [S09: add-on flags and branches](../../docs/smells-catalog.md) |
| [Facade](facade/README.md) | Provide one entry point for coordinating checkout services. | [S10: checkout orchestration in the console](../../docs/smells-catalog.md) |
| [Flyweight](flyweight/README.md) | Share repeated product specifications across imported listings. | [S11: duplicated listing specifications](../../docs/smells-catalog.md) |
| [Proxy](proxy/README.md) | Cache inventory reads and guard stock changes behind a shared contract. | [S12: uncached reads and scattered access checks](../../docs/smells-catalog.md) |
