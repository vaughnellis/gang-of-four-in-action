---
title: Pattern Comparisons
description: Compare commonly confused Gang of Four patterns through the CornerStore implementations.
---

Each comparison highlights the decision that separates two patterns. The examples link to the implementations and the smells they address.

- [Strategy vs State](#strategy-vs-state)
- [Facade vs Mediator](#facade-vs-mediator)
- [Decorator vs Proxy](#decorator-vs-proxy)
- [Command vs Memento for undo](#command-vs-memento-for-undo)
- [Factory Method vs Abstract Factory](#factory-method-vs-abstract-factory)
- [Adapter vs Bridge](#adapter-vs-bridge)
- [Composite vs Decorator](#composite-vs-decorator)
- [Template Method vs Strategy](#template-method-vs-strategy)

## Strategy vs State

### The one-line difference

Strategy swaps an algorithm chosen for an operation; State changes an object's behavior as its own lifecycle advances.

### Side-by-side

| Question | Strategy | State |
| --- | --- | --- |
| Intent | Select one of several algorithms for a task. | Represent lifecycle-dependent behavior and transitions. |
| What varies | The calculation used for a shipping method. | The operations permitted in an order's current state. |
| Who chooses the variation and when | Pricing selects a strategy by the cart's shipping method while building a quote. | The order's current state handles an operation and can transition the order to another state. |
| How the participants relate | A service uses one interchangeable strategy to perform a calculation. | An order delegates lifecycle operations to its current state object. |
| CornerStore smell fixed | S21: shipping cost was selected by a switch in pricing code. | S20: order status strings were checked in lifecycle operations. |

### In CornerStore

**Strategy**: [StandardShippingCostStrategy.cs](../patterns/behavioral/strategy/src/CornerStore/Pricing/StandardShippingCostStrategy.cs)

```csharp
internal sealed class StandardShippingCostStrategy : IShippingCostStrategy
{
    public decimal Calculate(decimal weightKg)
    {
        return Math.Round(5.00m + 2.00m * weightKg, 2, MidpointRounding.AwayFromZero);
    }
}
```

**State**: [PendingOrderState.cs](../patterns/behavioral/state/src/CornerStore/Orders/PendingOrderState.cs)

```csharp
    public override string? Pay(OrderService service, Order order)
    {
        order.State = new PaidOrderState();
        service.RecordStatusChange(order);
        return null;
    }
```

### How to tell which you need

- Are you choosing between algorithms for the same operation? Strategy fits.
- Does an object's current lifecycle stage determine which operations it accepts and what transition follows? State fits.

### Common mistake

Treating every replaceable behavior as Strategy misses the lifecycle transitions that make State useful.

## Facade vs Mediator

### The one-line difference

Facade gives a caller a simpler entry to a subsystem; Mediator coordinates communication among peer objects.

### Side-by-side

| Question | Facade | Mediator |
| --- | --- | --- |
| Intent | Provide a focused entry point to a set of subsystem services. | Centralize interaction rules among a group of collaborators. |
| What varies | The sequence of subsystem work required by a higher-level operation. | The reactions and updates that follow an action between peer objects. |
| Who chooses the variation and when | The caller chooses to use the facade when it needs the subsystem operation. | Collaborators notify the mediator during interaction; the mediator routes or orders the responses. |
| How the participants relate | The facade calls subsystem services; those services need not communicate through it. | Colleagues notify a mediator rather than directly coordinating with each other. |
| CornerStore smell fixed | S10: the console flow called checkout services directly in a fixed sequence. | S17: cart widgets referenced and updated each other directly. |

### In CornerStore

**Facade**: [CheckoutFacade.cs](../patterns/structural/facade/src/CornerStore/Checkout/CheckoutFacade.cs)

```csharp
        var quote = pricing.BuildQuote(cart, customer);
        var error = orderValidator.Validate(customer, cart, quote);
        if (error is not null)
        {
            return "checkout rejected: " + error;
        }

        if (!ChargePayment(paymentType, customer, quote.Total))
        {
            return "checkout rejected: unknown payment type " + paymentType;
        }

        var order = orderService.Create(customer, cart, quote, paymentType);
```

**Mediator**: [CartMediator.cs](../patterns/behavioral/mediator/src/CornerStore/Widgets/CartMediator.cs)

```csharp
    public void ItemListChanged()
    {
        ItemList.Refresh();
        PromoBox.Refresh();
        ShippingSelector.Refresh();
        TotalsPanel.Refresh();
    }
```

### How to tell which you need

- Does one caller need a simpler way to invoke several services? Use a facade.
- Do peer objects need to react to one another without holding direct references? Use a mediator.

### Common mistake

Both patterns centralize calls, but a facade simplifies subsystem access while a mediator defines colleague interactions.

## Decorator vs Proxy

### The one-line difference

Decorator adds a responsibility while preserving an object's interface; Proxy controls access to the object behind that interface.

### Side-by-side

| Question | Decorator | Proxy |
| --- | --- | --- |
| Intent | Add behavior around a component without changing its contract. | Stand in for a subject to control or optimize access to it. |
| What varies | The combination of optional responsibilities applied to a price component. | When access reaches the inventory service, including whether a cached value can be used. |
| Who chooses the variation and when | Pricing composes the decorators when it calculates add-on costs. | The application wires a proxy in place of the service; the proxy applies its access policy on each call. |
| How the participants relate | A wrapper implements the same component interface and delegates to the wrapped component. | A substitute implements the subject interface and forwards permitted or uncached calls to the real service. |
| CornerStore smell fixed | S09: add-ons were boolean flags handled by a price-calculation if-chain. | S12: inventory lookups repeated without caching and stock changes lacked an access check. |

### In CornerStore

**Decorator**: [GiftWrapPriceDecorator.cs](../patterns/structural/decorator/src/CornerStore/Pricing/GiftWrapPriceDecorator.cs)

```csharp
internal sealed class GiftWrapPriceDecorator(IAddOnPriceComponent component, bool enabled) : IAddOnPriceComponent
{
    public decimal Cost => component.Cost + (enabled ? 4.50m : 0m);
}
```

**Proxy**: [CachingInventoryProxy.cs](../patterns/structural/proxy/src/CornerStore/Inventory/CachingInventoryProxy.cs)

```csharp
    public int GetStock(string sku)
    {
        if (!stockCache.TryGetValue(sku, out var available))
        {
            available = inner.GetStock(sku);
            stockCache[sku] = available;
        }

        return available;
    }
```

### How to tell which you need

- Are you composing optional behavior that should accumulate around a component? Use Decorator.
- Must a substitute manage access, caching, protection, or deferred creation? Use Proxy.

### Common mistake

Both wrap an object, but adding a responsibility is not the same design decision as controlling access to a subject.

## Command vs Memento for undo

### The one-line difference

Command records an action that can be executed or reversed; Memento records state that can later be restored.

### Side-by-side

| Question | Command | Memento |
| --- | --- | --- |
| Intent | Package a request as an object so it can be dispatched, audited, queued, or undone. | Preserve an object's state for later restoration without exposing snapshot details. |
| What varies | The operation and its execution or reversal logic. | The values captured from an object's state. |
| Who chooses the variation and when | An invoker receives a command when the action is requested, then invokes it. | The originator creates a snapshot at a point in time; a caretaker decides when to keep or restore it. |
| How the participants relate | An invoker calls a command, which performs work on its receivers and may implement an inverse action. | The originator creates and restores a memento; a caretaker holds it without interpreting its contents. |
| CornerStore smell fixed | S14: back-office actions ran inline without an undoable command or audit record. | S18: cart undo exposed internal data so callers could copy and restore it. |

### In CornerStore

**Command**: [AdjustStockCommand.cs](../patterns/behavioral/command/src/CornerStore/Orders/AdjustStockCommand.cs)

```csharp
    public void Undo()
    {
        if (!canUndo)
        {
            throw new InvalidOperationException("This stock adjustment cannot be undone.");
        }

        inventory.AdjustStock(sku, -adjustment);
        canUndo = false;
    }
```

**Memento**: [CartMemento.cs](../patterns/behavioral/memento/src/CornerStore/Cart/CartMemento.cs)

```csharp
    internal void Restore(Cart cart)
    {
        cart.RestoreState(lines, promoCode, shippingMethod, giftWrap, insurance);
    }
```

### How to tell which you need

- Do you need to represent and manage a request, including its execution and reversal? Use Command.
- Do you need to restore an object's earlier values while keeping snapshot details encapsulated? Use Memento.

### Common mistake

Undo does not make these patterns interchangeable: a command describes an action, while a memento preserves the state an action may need to recover.

## Factory Method vs Abstract Factory

### The one-line difference

Factory Method lets a creator subclass choose one product; Abstract Factory creates a compatible family of products.

### Side-by-side

| Question | Factory Method | Abstract Factory |
| --- | --- | --- |
| Intent | Defer creation of one product to a creator's specialized implementation. | Create several related products through one family-specific factory. |
| What varies | The payment processor created by a payment creator. | The regional pricing rules and money formatter that must match. |
| Who chooses the variation and when | A concrete creator overrides the creation method; its caller uses the creator's operation. | The application selects a concrete factory for a region, then asks it for products in that family. |
| How the participants relate | A creator defines an operation that calls an overridable product-creation method. | A client uses a factory interface with methods for the related product types. |
| CornerStore smell fixed | S03: checkout selected payment processors with a switch that constructed concrete types. | S01: region branches could mix tax, shipping, and currency rules from different regions. |

### In CornerStore

**Factory Method**: [PaymentProcessorCreator.cs](../patterns/creational/factory-method/src/CornerStore/Checkout/PaymentProcessorCreator.cs)

```csharp
    public void Charge(Customer customer, decimal amount)
    {
        CreateProcessor().Charge(customer, amount);
    }

    protected abstract IPaymentProcessor CreateProcessor();
```

**Abstract Factory**: [IRegionFactory.cs](../patterns/creational/abstract-factory/src/CornerStore/Pricing/IRegionFactory.cs)

```csharp
internal interface IRegionFactory
{
    IRegionalPricing CreatePricing();

    IMoneyFormatter CreateMoneyFormatter();
}
```

### How to tell which you need

- Does one product type vary inside an otherwise shared creation workflow? Use Factory Method.
- Must several created products remain compatible as a set? Use Abstract Factory.

### Common mistake

Factory Method is not just a smaller Abstract Factory: its key variation point is one overridable product-creation method.

## Adapter vs Bridge

### The one-line difference

Adapter translates an existing incompatible interface; Bridge separates two dimensions that need to vary independently.

### Side-by-side

| Question | Adapter | Bridge |
| --- | --- | --- |
| Intent | Make an existing service usable through the interface a client expects. | Keep an abstraction and its implementation independently extensible. |
| What varies | The translation between CornerStore's shipping request and the courier SDK's protocol. | The kinds of order message and the channel used to deliver each message. |
| Who chooses the variation and when | The application supplies an adapter at composition time so the client can use the SDK. | The application combines a message abstraction with a channel implementation when it creates the message. |
| How the participants relate | The adapter implements the target contract and calls the incompatible adaptee. | The abstraction holds an implementation interface and delegates the implementation-specific work to it. |
| CornerStore smell fixed | S06: checkout called the courier SDK directly using its XML and cents-based contract. | S07: separate message classes multiplied for every message-and-channel combination. |

### In CornerStore

**Adapter**: [ShipFastAdapter.cs](../patterns/structural/adapter/src/CornerStore/Checkout/ShipFastAdapter.cs)

```csharp
internal sealed class ShipFastAdapter(ShipFastClient client) : IShippingProvider
{
    public string BookShipment(Order order)
    {
        var cents = Math.Round(order.Quote.Total * 100m, 0, MidpointRounding.AwayFromZero)
            .ToString("0", CultureInfo.InvariantCulture);
        var name = System.Security.SecurityElement.Escape(order.Customer.Name) ?? "";
```

**Bridge**: [OrderMessage.cs](../patterns/structural/bridge/src/CornerStore/Notifications/OrderMessage.cs)

```csharp
internal abstract class OrderMessage(IChannel channel)
{
    protected void Deliver(Customer customer, string message)
    {
        channel.Send(customer.Name, message);
    }
}
```

### How to tell which you need

- Are you adapting an existing service whose interface does not fit the client? Use Adapter.
- Do two dimensions need their own implementations and combinations? Use Bridge.

### Common mistake

An Adapter may also wrap another object, but its purpose is interface translation rather than independent variation.

## Composite vs Decorator

### The one-line difference

Composite represents a part or a group of parts; Decorator wraps one component to add a responsibility.

### Side-by-side

| Question | Composite | Decorator |
| --- | --- | --- |
| Intent | Let callers treat individual items and nested groups through one component contract. | Add behavior to one component while retaining that contract. |
| What varies | The items contained in a bundle and the group's aggregate behavior. | The optional price adjustments applied around a base price component. |
| Who chooses the variation and when | Catalog construction adds child items to a bundle; operations apply to the component tree. | Pricing composes decorators for the add-ons enabled on a quote. |
| How the participants relate | A composite owns zero or more child components; a leaf has no children. | A decorator holds one component and delegates through it. |
| CornerStore smell fixed | S08: bundles used a separate pricing path, could not nest, and required type checks. | S09: add-ons were boolean flags handled by a price-calculation if-chain. |

### In CornerStore

**Composite**: [Bundle.cs](../patterns/structural/composite/src/CornerStore/Catalog/Bundle.cs)

```csharp
    public void AddComponent(ICatalogItem component)
    {
        components.Add(component);
    }
```

**Decorator**: [GiftWrapPriceDecorator.cs](../patterns/structural/decorator/src/CornerStore/Pricing/GiftWrapPriceDecorator.cs)

```csharp
internal sealed class GiftWrapPriceDecorator(IAddOnPriceComponent component, bool enabled) : IAddOnPriceComponent
{
    public decimal Cost => component.Cost + (enabled ? 4.50m : 0m);
}
```

### How to tell which you need

- Do you need a tree of parts and groups that callers can use uniformly? Use Composite.
- Do you need to add optional behavior around one object without changing its interface? Use Decorator.

### Common mistake

Both can contain another component, but a Composite models children while a Decorator adds behavior to its wrapped component.

## Template Method vs Strategy

### The one-line difference

Template Method fixes an algorithm's outline in a base class; Strategy delegates the whole algorithm to a replaceable object.

### Side-by-side

| Question | Template Method | Strategy |
| --- | --- | --- |
| Intent | Share a stable algorithm sequence while subclasses fill in selected steps. | Select among complete, interchangeable algorithms for an operation. |
| What varies | The report-specific accumulation and output steps. | The calculation for the selected shipping method. |
| Who chooses the variation and when | A subclass defines the steps when the exporter type is chosen. | Pricing looks up the shipping strategy when it builds the quote. |
| How the participants relate | An abstract base class owns the sequence and calls subclass hooks. | A service holds a strategy interface and delegates the varying operation. |
| CornerStore smell fixed | S22: CSV and JSON exporters duplicated the same report-processing skeleton. | S21: shipping cost was selected by a switch in pricing code. |

### In CornerStore

**Template Method**: [ReportExporter.cs](../patterns/behavioral/template-method/src/CornerStore/Reporting/ReportExporter.cs)

```csharp
    public string Export(List<Order> source, string status)
    {
        var orders = source.Where(order => status == "all" || order.Status == status).ToList();
        var totals = new ReportTotals();
        foreach (var order in orders)
        {
            foreach (var line in order.Lines)
            {
                Accumulate(totals, line);
            }
        }
```

**Strategy**: [IShippingCostStrategy.cs](../patterns/behavioral/strategy/src/CornerStore/Pricing/IShippingCostStrategy.cs)

```csharp
internal interface IShippingCostStrategy
{
    decimal Calculate(decimal weightKg);
}
```

### How to tell which you need

- Does the overall sequence stay fixed while a few steps differ by subtype? Use Template Method.
- Should a caller be able to select or replace a complete algorithm independently? Use Strategy.

### Common mistake

Template Method and Strategy both vary behavior, but inheritance is built into the former while composition is central to the latter.
