<!-- markdownlint-disable-file -->
# Changes: capstone

Plan: `.copilot-tracking/plans/2026-10-16/capstone-plan.instructions.md`
Implementation date: 2026-10-16

## Summary

Integrated the 23 pattern refactors into one CornerStore copy in the required order. The golden-master transcript remains unchanged. The pattern seams and their resolutions are recorded below.

## Pattern meeting points

* Catalog: Prototype creates product variants, Composite gives products, bundles, and categories a shared catalog-item contract, Flyweight shares immutable specifications across listings, and Iterator provides explicit category traversal. Singleton configuration, logging, and IDs are reused throughout the integrated app.
* Pricing: regional Abstract Factories create matched pricing services. Strategy selects shipping cost, Decorator composes add-on charges, and Interpreter evaluates promotion expressions. Each pattern keeps a separate pricing responsibility.
* Checkout: CheckoutFacade coordinates the operation. It constructs quotes and orders with builders, runs the ordered Chain of Responsibility validators, selects a processor through Factory Method, and calls the courier through the shipping Adapter.
* Cart: Memento captures cart state before successful changes. CartMediator receives widget notifications and refreshes related views without direct widget-to-widget references.
* Order lifecycle: State objects own allowed transitions. Successful transitions notify Observer subscribers through OrderService. The notification observer creates Bridge message abstractions, which deliver through email or SMS channels.
* Back office: Command objects carry staff actions through the audited invoker. The commands use the protected inventory interface, while the caching proxy keeps stock reads current after reservations, adjustments, and restoration. The inventory observer shares that interface for lifecycle updates.
* Reporting: Template Method retains the shared report filtering and aggregation workflow. Visitor dispatches line items to calculate totals by product type, while CSV and JSON exporters retain format-specific rendering. Tax and weight use separate visitors over the same product visitor interface.

## Validation

| Check | Result |
| --- | --- |
| Capstone tests | 131 passed, including the golden master |
| Umbrella Release build | Succeeded with 0 warnings and 0 errors |
| Umbrella Release tests | Passed |
| GoldenMasterTests copy | SHA-256 matches baseline |
| Capstone smell markers | No `SMELL S` markers remain under `capstone/src` |
| README locations | All 23 pattern rows and their file paths and links exist |
| README history | 24 ordered steps documented |
