# Behavioral patterns

Behavioral patterns describe how objects communicate, divide work, and respond to changes. In CornerStore, they separate validation and pricing choices, coordinate cart and order behavior, manage undo, and share report workflows.

| Pattern | Intent | Smell fixed |
| --- | --- | --- |
| [Chain of Responsibility](chain-of-responsibility/README.md) | Pass checkout through an ordered set of validation handlers. | [S13: nested validation method](../../docs/smells-catalog.md) |
| [Command](command/README.md) | Represent back-office actions as executable, auditable objects. | [S14: inline back-office actions](../../docs/smells-catalog.md) |
| [Interpreter](interpreter/README.md) | Parse and evaluate promotion rules as expressions. | [S15: string-based promotion parsing](../../docs/smells-catalog.md) |
| [Iterator](iterator/README.md) | Traverse the category tree through reusable iterators. | [S16: repeated recursive walks](../../docs/smells-catalog.md) |
| [Mediator](mediator/README.md) | Coordinate updates among cart widgets through a mediator. | [S17: direct widget-to-widget updates](../../docs/smells-catalog.md) |
| [Memento](memento/README.md) | Capture and restore cart state without exposing its internals. | [S18: public cart snapshots](../../docs/smells-catalog.md) |
| [Observer](observer/README.md) | Notify subscribed services after an order status change. | [S19: direct lifecycle side effects](../../docs/smells-catalog.md) |
| [State](state/README.md) | Move legal order transitions into state-specific behavior. | [S20: status-string conditionals](../../docs/smells-catalog.md) |
| [Strategy](strategy/README.md) | Select a shipping-cost algorithm for the chosen method. | [S21: shipping-cost switch](../../docs/smells-catalog.md) |
| [Template Method](template-method/README.md) | Share report processing steps while allowing format-specific output. | [S22: duplicated exporter workflow](../../docs/smells-catalog.md) |
| [Visitor](visitor/README.md) | Add tax, weight, and report operations across line-item types. | [S23: repeated line-item type switches](../../docs/smells-catalog.md) |
