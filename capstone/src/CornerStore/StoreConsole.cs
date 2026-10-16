using System.Globalization;
using System.Text;
using System.Text.Json;

namespace CornerStore;

internal class StoreConsole
{
    private readonly ScenarioClock clock;
    private readonly Catalog catalog = new();
    private readonly CartService cartService;
    private readonly PromoEngine promos = new();
    private readonly PricingService pricing;
    private readonly ListingImporter importer = new();
    private readonly InventoryService inventory = new();
    private readonly EmailSender emailSender = new();
    private readonly SmsSender smsSender = new();
    private readonly LoyaltyService loyalty = new();
    private readonly AnalyticsService analytics = new();
    private readonly OrderService orderService;
    private readonly BackOffice backOffice;
    private readonly CheckoutFacade checkoutFacade;
    private readonly ReportService reports;
    private readonly Dictionary<string, Customer> customers = [];
    private readonly Dictionary<string, Cart> carts = [];
    private readonly Dictionary<string, CartMediator> cartMediators = [];
    private readonly Dictionary<string, CartMemento> savedMementos = [];
    private List<Listing> listings = [];

    public StoreConsole(ScenarioClock clock)
    {
        this.clock = clock;
        cartService = new CartService(catalog);
        pricing = new PricingService(promos);
        orderService = new OrderService(Ids, inventory);
        orderService.Subscribe(new InventoryOrderStatusObserver(inventory));
        orderService.Subscribe(new NotificationOrderStatusObserver(emailSender, smsSender));
        orderService.Subscribe(new LoyaltyOrderStatusObserver(loyalty));
        orderService.Subscribe(new AnalyticsOrderStatusObserver(analytics));
        backOffice = new BackOffice(inventory, orderService);
        checkoutFacade = new CheckoutFacade(
            pricing,
            new OrderValidator(inventory, new FraudScorer()),
            orderService,
            inventory,
            new ShipFastAdapter(new ShipFastClient(Ids)));
        reports = new ReportService(orderService);
    }

    public IdSequence Ids { get; } = new();

    public void Handle(int number, JsonElement step)
    {
        var op = step.TryGetProperty("op", out var opValue) ? opValue.GetString() ?? "" : "";
        Log.Instance.Line("[" + number.ToString("D3", CultureInfo.InvariantCulture) + "] " + op + Describe(step));

        switch (op)
        {
            case "advance":
                Advance(step);
                break;
            case "config.set":
                SetConfig(step);
                break;
            case "browse":
                Browse(step);
                break;
            case "variant":
                Variant(step);
                break;
            case "bundle.create":
                BundleCreate(step);
                break;
            case "bundle.add":
                BundleAdd(step);
                break;
            case "import":
                Import(step);
                break;
            case "register":
                Register(step);
                break;
            case "cart.add":
                CartAdd(step);
                break;
            case "cart.remove":
                CartRemove(step);
                break;
            case "cart.undo":
                CartUndo(step);
                break;
            case "cart.show":
                CartShow(step);
                break;
            case "cart.addon":
                CartAddOn(step);
                break;
            case "cart.promo":
                CartPromo(step);
                break;
            case "cart.shipping":
                CartShipping(step);
                break;
            case "quote":
                ShowQuote(step);
                break;
            case "order.ship":
                OrderShip(step);
                break;
            case "order.deliver":
                OrderDeliver(step);
                break;
            case "admin.stock.adjust":
                AdminStockAdjust(step);
                break;
            case "admin.stock.lookup":
                AdminStockLookup(step);
                break;
            case "admin.refund":
                AdminRefund(step);
                break;
            case "admin.cancel":
                AdminCancel(step);
                break;
            case "checkout":
                Checkout(step);
                break;
            case "report.csv":
                ReportCsv(step);
                break;
            case "report.json":
                ReportJson(step);
                break;
            default:
                Log.Instance.Info("unknown op");
                break;
        }
    }

    public bool ApplyConfig(string key, int value)
    {
        switch (key)
        {
            case "fraudThreshold":
                Config.Instance.SetFraudThreshold(value);
                return true;
            case "lowStockLevel":
                Config.Instance.SetLowStockLevel(value);
                return true;
            default:
                return false;
        }
    }

    private static string Text(JsonElement step, string name)
    {
        return step.TryGetProperty(name, out var value) ? value.GetString() ?? "" : "";
    }

    private static string Describe(JsonElement step)
    {
        var text = new StringBuilder();
        foreach (var property in step.EnumerateObject())
        {
            if (property.Name == "op")
            {
                continue;
            }

            var value = property.Value.ValueKind == JsonValueKind.String
                ? property.Value.GetString()
                : property.Value.GetRawText();
            text.Append(' ').Append(property.Name).Append('=').Append(value);
        }

        return text.ToString();
    }

    private void Advance(JsonElement step)
    {
        if (!step.TryGetProperty("minutes", out var minutes) || !minutes.TryGetInt32(out var value))
        {
            Log.Instance.Info("advance needs minutes");
            return;
        }

        clock.Advance(TimeSpan.FromMinutes(value));
        Log.Instance.Info("clock " + clock.Stamp());
    }

    private void Browse(JsonElement step)
    {
        var name = step.TryGetProperty("category", out _) ? Text(step, "category") : catalog.Root.Name;
        var category = catalog.FindCategory(name);
        if (category is null)
        {
            Log.Instance.Info("unknown category " + name);
            return;
        }

        catalog.PrintTree(category, 0);
        Log.Instance.Info(catalog.CountProducts(category).ToString(CultureInfo.InvariantCulture) + " products under " + name);
    }

    private void Variant(JsonElement step)
    {
        var parent = Text(step, "parent");
        var sku = Text(step, "sku");
        var error = catalog.CreateVariant(parent, sku, Text(step, "size"), Text(step, "colour"));
        Log.Instance.Info(error is null ? "created variant " + sku + " of " + parent : "rejected: " + error);
    }

    private void BundleCreate(JsonElement step)
    {
        var sku = Text(step, "sku");
        var error = catalog.CreateBundle(sku, Text(step, "name"));
        Log.Instance.Info(error is null ? "created bundle " + sku : "rejected: " + error);
    }

    private void BundleAdd(JsonElement step)
    {
        var bundle = Text(step, "bundle");
        var sku = Text(step, "sku");
        var error = catalog.AddToBundle(bundle, sku);
        if (error is not null)
        {
            Log.Instance.Info("rejected: " + error);
            return;
        }

        Log.Instance.Info("added " + sku + " to " + bundle);
        Log.Instance.Info("bundle " + bundle + " price " + Catalog.FormatPrice(catalog.PriceOf(bundle)));
    }

    private void Import(JsonElement step)
    {
        var count = step.TryGetProperty("count", out var value) && value.TryGetInt32(out var number) ? number : 100000;
        var skus = new List<string>();
        catalog.CollectSkus(catalog.Root, skus);
        listings = importer.Import(skus, count);
        Log.Instance.Info("imported " + listings.Count.ToString(CultureInfo.InvariantCulture) + " listings");
    }

    private void Register(JsonElement step)
    {
        var id = Ids.NextCustomerId();
        var role = Text(step, "role");
        var customer = new Customer
        {
            Id = id,
            Number = customers.Count + 1,
            Name = Text(step, "name"),
            Region = Text(step, "region"),
            Channel = Text(step, "channel"),
            Address = Text(step, "address"),
            CreditLimit = step.TryGetProperty("creditLimit", out var limit) && limit.TryGetDecimal(out var amount) ? amount : 0m,
            Role = role == "" ? "customer" : role,
        };
        customers[id] = customer;
        var cart = new Cart(id);
        carts[id] = cart;

        cartMediators[id] = new CartMediator(cart, customer, cartService, promos, pricing);
        Log.Instance.Info("registered " + id + " " + customer.Name + " (" + customer.Region + ", " + customer.Channel + ")");
    }

    private Cart? FindCart(JsonElement step)
    {
        var id = Text(step, "customer");
        if (!customers.ContainsKey(id))
        {
            Log.Instance.Info("rejected: unknown customer " + id);
            return null;
        }

        return carts[id];
    }

    private Customer? FindCustomer(JsonElement step)
    {
        var id = Text(step, "customer");
        if (!customers.TryGetValue(id, out var customer))
        {
            Log.Instance.Info("rejected: unknown customer " + id);
            return null;
        }

        return customer;
    }

    private Order? FindOrder(JsonElement step)
    {
        var id = Text(step, "order");
        var order = orderService.Find(id);
        if (order is null)
        {
            Log.Instance.Info("rejected: unknown order " + id);
        }

        return order;
    }

    private void OrderShip(JsonElement step)
    {
        var order = FindOrder(step);
        if (order is null)
        {
            return;
        }

        var error = orderService.Ship(order);
        Log.Instance.Info(error is null ? "order " + order.Id + " shipped" : "rejected: " + error);
    }

    private void OrderDeliver(JsonElement step)
    {
        var order = FindOrder(step);
        if (order is null)
        {
            return;
        }

        var error = orderService.Deliver(order);
        Log.Instance.Info(error is null ? "order " + order.Id + " delivered" : "rejected: " + error);
    }

    private void AdminStockAdjust(JsonElement step)
    {
        var customer = FindCustomer(step);
        if (customer is null)
        {
            return;
        }

        var sku = Text(step, "sku");
        var amount = step.TryGetProperty("delta", out var value) && value.TryGetInt32(out var number) ? number : 0;
        Log.Instance.Info(backOffice.AdjustStock(customer, sku, amount));
    }

    private void AdminStockLookup(JsonElement step)
    {
        var customer = FindCustomer(step);
        if (customer is null)
        {
            return;
        }

        Log.Instance.Info(backOffice.LookupStock(customer, Text(step, "sku")));
    }

    private void AdminRefund(JsonElement step)
    {
        var customer = FindCustomer(step);
        var order = FindOrder(step);
        if (customer is null || order is null)
        {
            return;
        }

        Log.Instance.Info(backOffice.Refund(customer, order));
    }

    private void AdminCancel(JsonElement step)
    {
        var customer = FindCustomer(step);
        var order = FindOrder(step);
        if (customer is null || order is null)
        {
            return;
        }

        Log.Instance.Info(backOffice.Cancel(customer, order));
    }

    private void Checkout(JsonElement step)
    {
        var cart = FindCart(step);
        if (cart is null)
        {
            return;
        }

        var customer = customers[cart.CustomerId];
        Log.Instance.Info(checkoutFacade.PlaceOrder(cart, customer, Text(step, "payment")));
    }

    private void ReportCsv(JsonElement step)
    {
        var status = Text(step, "status");
        status = status == "" ? "all" : status;
        Log.Instance.Info("CSV report (" + status + ")");
        foreach (var row in reports.ExportCsv(status).Split('\n', StringSplitOptions.RemoveEmptyEntries))
        {
            Log.Instance.Info(row);
        }
    }

    private void ReportJson(JsonElement step)
    {
        var status = Text(step, "status");
        status = status == "" ? "all" : status;
        Log.Instance.Info("JSON report (" + status + ")");
        foreach (var row in reports.ExportJson(status).Split('\n', StringSplitOptions.RemoveEmptyEntries))
        {
            Log.Instance.Info(row);
        }
    }

    private void CartAdd(JsonElement step)
    {
        var cart = FindCart(step);
        if (cart is null)
        {
            return;
        }

        var sku = Text(step, "sku");
        var quantity = step.TryGetProperty("qty", out var value) && value.TryGetInt32(out var number) ? number : 1;

        var memento = cart.CreateMemento();

        var mediator = cartMediators[cart.CustomerId];
        var error = mediator.Add(sku, quantity);
        if (error is not null)
        {
            Log.Instance.Info("rejected: " + error);
            return;
        }

        savedMementos[cart.CustomerId] = memento;
        Log.Instance.Info("added " + quantity.ToString(CultureInfo.InvariantCulture) + " x " + sku);
        Log.Instance.Info(Summary(cart));
    }

    private void CartRemove(JsonElement step)
    {
        var cart = FindCart(step);
        if (cart is null)
        {
            return;
        }

        var sku = Text(step, "sku");

        var memento = cart.CreateMemento();

        var mediator = cartMediators[cart.CustomerId];
        var error = mediator.Remove(sku);
        if (error is not null)
        {
            Log.Instance.Info("rejected: " + error);
            return;
        }

        savedMementos[cart.CustomerId] = memento;
        Log.Instance.Info("removed " + sku);
        Log.Instance.Info(Summary(cart));
    }

    private void CartUndo(JsonElement step)
    {
        var cart = FindCart(step);
        if (cart is null)
        {
            return;
        }

        if (!savedMementos.TryGetValue(cart.CustomerId, out var memento))
        {
            Log.Instance.Info("nothing to undo");
            return;
        }

        cart.Restore(memento);
        savedMementos.Remove(cart.CustomerId);
        cartMediators[cart.CustomerId].ItemListChanged();
        Log.Instance.Info("undone");
        Log.Instance.Info(Summary(cart));
    }

    private void CartShow(JsonElement step)
    {
        var cart = FindCart(step);
        if (cart is null)
        {
            return;
        }

        cartMediators[cart.CustomerId].ItemListChanged();
        Log.Instance.Info(Summary(cart));
    }

    private void CartAddOn(JsonElement step)
    {
        var cart = FindCart(step);
        if (cart is null)
        {
            return;
        }

        var name = Text(step, "addon");
        var on = !step.TryGetProperty("on", out var flag) || flag.ValueKind != JsonValueKind.False;

        if (name == "giftwrap")
        {
            cart.GiftWrap = on;
        }
        else if (name == "insurance")
        {
            cart.Insurance = on;
        }
        else
        {
            Log.Instance.Info("rejected: unknown add-on " + name);
            return;
        }

        cartMediators[cart.CustomerId].TotalsChanged();
        Log.Instance.Info(name + (on ? " added" : " removed"));
    }

    private void CartPromo(JsonElement step)
    {
        var cart = FindCart(step);
        if (cart is null)
        {
            return;
        }

        var code = Text(step, "code");
        var mediator = cartMediators[cart.CustomerId];
        if (!mediator.IsKnownPromo(code))
        {
            Log.Instance.Info("rejected: unknown promo " + code);
            return;
        }

        if (!mediator.PromoApplies(code))
        {
            Log.Instance.Info("rejected: promo " + code + " does not apply to this cart");
            return;
        }

        mediator.ApplyPromo(code);
        Log.Instance.Info("promo " + code + " applied (" + promos.RuleOf(code) + ")");
    }

    private void CartShipping(JsonElement step)
    {
        var cart = FindCart(step);
        if (cart is null)
        {
            return;
        }

        var method = Text(step, "method");
        var mediator = cartMediators[cart.CustomerId];
        if (!mediator.CanSelectShipping(method))
        {
            Log.Instance.Info("rejected: unknown shipping method " + method);
            return;
        }

        mediator.SelectShipping(method);
        Log.Instance.Info("shipping set to " + method);
    }

    private void ShowQuote(JsonElement step)
    {
        var cart = FindCart(step);
        if (cart is null)
        {
            return;
        }

        if (cart.Lines.Count == 0)
        {
            Log.Instance.Info("rejected: cart is empty");
            return;
        }

        cartMediators[cart.CustomerId].ShowQuote();
    }

    private string Summary(Cart cart)
    {
        return "cart " + cart.CustomerId + " lines=" + cart.Lines.Count.ToString(CultureInfo.InvariantCulture)
            + " subtotal=" + MoneyFormatter.Format(cartService.Subtotal(cart), customers[cart.CustomerId].Region);
    }

    private void SetConfig(JsonElement step)
    {
        if (!step.TryGetProperty("key", out var key)
            || !step.TryGetProperty("value", out var value)
            || !value.TryGetInt32(out var number))
        {
            Log.Instance.Info("config.set needs key and value");
            return;
        }

        var name = key.GetString() ?? "";
        if (ApplyConfig(name, number))
        {
            Log.Instance.Info(name + " set to " + number.ToString(CultureInfo.InvariantCulture));
        }
        else
        {
            Log.Instance.Info("unknown setting " + name);
        }
    }
}
