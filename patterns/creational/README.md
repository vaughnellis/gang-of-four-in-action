# Creational patterns

Creational patterns shape how objects are created and how construction decisions are made. In CornerStore, they help create consistent regional services, valid orders and invoices, payment processors, product variants, and shared application services.

| Pattern | Intent | Smell fixed |
| --- | --- | --- |
| [Abstract Factory](abstract-factory/README.md) | Create matched services for each storefront region. | [S01: regional rules are scattered](../../docs/smells-catalog.md) |
| [Builder](builder/README.md) | Assemble orders and invoices through validated, readable steps. | [S02: positional constructors](../../docs/smells-catalog.md) |
| [Factory Method](factory-method/README.md) | Let payment creator types choose which processor to construct. | [S03: payment construction in a switch](../../docs/smells-catalog.md) |
| [Prototype](prototype/README.md) | Create product variants by copying an existing product. | [S04: fields copied by hand](../../docs/smells-catalog.md) |
| [Singleton](singleton/README.md) | Share one application-wide configuration, logger, or ID sequence instance. | [S05: mutable static state](../../docs/smells-catalog.md) |
