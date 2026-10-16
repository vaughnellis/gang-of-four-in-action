using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

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
    private readonly OrderValidator orderValidator;
    private readonly ShipFastClient shipFastClient;
    private readonly ReportService reports;
    private readonly Dictionary<string, Customer> customers = [];
    private readonly Dictionary<string, Cart> carts = [];
    private readonly Dictionary<string, ItemListWidget> itemListWidgets = [];
    private readonly Dictionary<string, List<CartLine>> savedLines = [];
    private readonly Dictionary<string, string> savedPromo = [];
    private readonly Dictionary<string, string> savedShipping = [];
    private readonly Dictionary<string, bool> savedGiftWrap = [];
    private readonly Dictionary<string, bool> savedInsurance = [];
    private List<Listing> listings = [];

    public StoreConsole(ScenarioClock clock)
    {
        this.clock = clock;
        cartService = new CartService(catalog);
        pricing = new PricingService(promos);
        orderService = new OrderService(
            Ids,
            inventory,
            new OrderConfirmedEmail(emailSender),
            new OrderConfirmedSms(smsSender),
            new OrderShippedEmail(emailSender),
            new OrderShippedSms(smsSender),
            loyalty,
            analytics);
        backOffice = new BackOffice(inventory, orderService);
        orderValidator = new OrderValidator(inventory, new FraudScorer());
        shipFastClient = new ShipFastClient(Ids);
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

        var itemListWidget = new ItemListWidget(cart, customer, cartService);
        var promoBoxWidget = new PromoBoxWidget(cart, customer, cartService, promos);
        var shippingSelectorWidget = new ShippingSelectorWidget(cart, pricing);
        var totalsPanelWidget = new TotalsPanelWidget(cart, customer, pricing);
        itemListWidget.Connect(promoBoxWidget, shippingSelectorWidget, totalsPanelWidget);
        promoBoxWidget.Connect(itemListWidget, shippingSelectorWidget, totalsPanelWidget);
        shippingSelectorWidget.Connect(itemListWidget, promoBoxWidget, totalsPanelWidget);
        totalsPanelWidget.Connect(itemListWidget, promoBoxWidget, shippingSelectorWidget);
        itemListWidgets[id] = itemListWidget;
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

    // SMELL S10: the console coordinates every checkout subsystem in a fixed order
    private void Checkout(JsonElement step)
    {
        var cart = FindCart(step);
        if (cart is null)
        {
            return;
        }

        if (cart.Lines.Count == 0)
        {
            Log.Instance.Info("checkout rejected: cart is empty");
            return;
        }

        var customer = customers[cart.CustomerId];
        var quote = pricing.BuildQuote(cart, customer);
        var error = orderValidator.Validate(customer, cart, quote);
        if (error is not null)
        {
            Log.Instance.Info("checkout rejected: " + error);
            return;
        }

        var paymentType = Text(step, "payment");
        if (!ChargePayment(paymentType, customer, quote.Total))
        {
            Log.Instance.Info("checkout rejected: unknown payment type " + paymentType);
            return;
        }

        var order = orderService.Create(customer, cart, quote, paymentType);
        var reservedLines = new List<CartLine>();
        foreach (var line in order.Lines)
        {
            if (!inventory.ReserveStock(line.Product.Sku, line.Quantity))
            {
                foreach (var reservedLine in reservedLines)
                {
                    inventory.AdjustStock(reservedLine.Product.Sku, reservedLine.Quantity);
                }

                Log.Instance.Info("checkout rejected: stock changed for " + line.Product.Sku);
                return;
            }

            reservedLines.Add(line);
        }

        order.StockReserved = true;
        order.TrackingNumber = BookShipment(order);
        var payError = orderService.Pay(order);
        if (payError is not null)
        {
            foreach (var reservedLine in reservedLines)
            {
                inventory.AdjustStock(reservedLine.Product.Sku, reservedLine.Quantity);
            }

            order.StockReserved = false;
            Log.Instance.Info("checkout rejected: " + payError);
            return;
        }

        Log.Instance.Info("checkout placed " + order.Id);
    }

    // SMELL S03: payment processor construction is selected by a switch in checkout
    private static bool ChargePayment(string paymentType, Customer customer, decimal amount)
    {
        IPaymentProcessor? processor = null;
        switch (paymentType)
        {
            case "card":
                processor = new CardProcessor();
                break;
            case "paypal":
                processor = new PayPalProcessor();
                break;
            case "afterpay":
                processor = new AfterpayProcessor();
                break;
        }

        if (processor is null)
        {
            return false;
        }

        processor.Charge(customer, amount);
        return true;
    }

    // SMELL S06: checkout calls the courier client with XML and parses its response directly
    private string BookShipment(Order order)
    {
        var cents = Math.Round(order.Quote.Total * 100m, 0, MidpointRounding.AwayFromZero)
            .ToString("0", CultureInfo.InvariantCulture);
        var name = System.Security.SecurityElement.Escape(order.Customer.Name) ?? "";
        var address = System.Security.SecurityElement.Escape(order.Customer.Address) ?? "";
        var request = "<shipment><orderId>" + order.Id + "</orderId><recipient>" + name
            + "</recipient><address>" + address + "</address><amountCents>" + cents + "</amountCents></shipment>";
        var response = shipFastClient.CreateConsignmentFromXml(request);
        return XDocument.Parse(response).Root?.Element("tracking")?.Value ?? "";
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

        // SMELL S18: the caller copies the cart's public internals out before the change
        var lines = CopyLines(cart.Lines);
        var promo = cart.PromoCode;
        var shipping = cart.ShippingMethod;
        var giftWrap = cart.GiftWrap;
        var insurance = cart.Insurance;

        var itemListWidget = itemListWidgets[cart.CustomerId];
        var error = itemListWidget.Add(sku, quantity);
        if (error is not null)
        {
            Log.Instance.Info("rejected: " + error);
            return;
        }

        Remember(cart.CustomerId, lines, promo, shipping, giftWrap, insurance);
        itemListWidget.RefreshAfterChange();
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

        // SMELL S18: the caller copies the cart's public internals out before the change
        var lines = CopyLines(cart.Lines);
        var promo = cart.PromoCode;
        var shipping = cart.ShippingMethod;
        var giftWrap = cart.GiftWrap;
        var insurance = cart.Insurance;

        var itemListWidget = itemListWidgets[cart.CustomerId];
        var error = itemListWidget.Remove(sku);
        if (error is not null)
        {
            Log.Instance.Info("rejected: " + error);
            return;
        }

        Remember(cart.CustomerId, lines, promo, shipping, giftWrap, insurance);
        itemListWidget.RefreshAfterChange();
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

        if (!savedLines.TryGetValue(cart.CustomerId, out var lines))
        {
            Log.Instance.Info("nothing to undo");
            return;
        }

        // SMELL S18: the caller writes the saved internals straight back into the cart
        cart.Lines = lines;
        cart.PromoCode = savedPromo[cart.CustomerId];
        cart.ShippingMethod = savedShipping[cart.CustomerId];
        cart.GiftWrap = savedGiftWrap[cart.CustomerId];
        cart.Insurance = savedInsurance[cart.CustomerId];

        savedLines.Remove(cart.CustomerId);
        savedPromo.Remove(cart.CustomerId);
        savedShipping.Remove(cart.CustomerId);
        savedGiftWrap.Remove(cart.CustomerId);
        savedInsurance.Remove(cart.CustomerId);
        itemListWidgets[cart.CustomerId].RefreshAfterChange();
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

        itemListWidgets[cart.CustomerId].RefreshAfterChange();
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

        // SMELL S09: the console has to know each add-on flag by name to set it
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

        itemListWidgets[cart.CustomerId].TotalsPanel.RefreshAfterChange();
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
        var promoBoxWidget = itemListWidgets[cart.CustomerId].PromoBox;
        if (!promoBoxWidget.IsKnown(code))
        {
            Log.Instance.Info("rejected: unknown promo " + code);
            return;
        }

        if (!promoBoxWidget.Evaluate(code))
        {
            Log.Instance.Info("rejected: promo " + code + " does not apply to this cart");
            return;
        }

        promoBoxWidget.Apply(code);
        promoBoxWidget.RefreshAfterChange();
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
        var shippingSelectorWidget = itemListWidgets[cart.CustomerId].ShippingSelector;
        if (!shippingSelectorWidget.CanSelect(method))
        {
            Log.Instance.Info("rejected: unknown shipping method " + method);
            return;
        }

        shippingSelectorWidget.Select(method);
        shippingSelectorWidget.RefreshAfterChange();
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

        itemListWidgets[cart.CustomerId].TotalsPanel.ShowQuote();
    }

    private string Summary(Cart cart)
    {
        return "cart " + cart.CustomerId + " lines=" + cart.Lines.Count.ToString(CultureInfo.InvariantCulture)
            + " subtotal=" + MoneyFormatter.Format(cartService.Subtotal(cart), customers[cart.CustomerId].Region);
    }

    // SMELL S18: the caller has to know every field of a cart line to copy it
    private static List<CartLine> CopyLines(List<CartLine> lines)
    {
        var copy = new List<CartLine>();
        foreach (var line in lines)
        {
            copy.Add(new CartLine
            {
                Product = line.Product,
                Quantity = line.Quantity,
                UnitPrice = line.UnitPrice,
                FromBundle = line.FromBundle,
            });
        }

        return copy;
    }

    private void Remember(string customerId, List<CartLine> lines, string promo, string shipping, bool giftWrap, bool insurance)
    {
        savedLines[customerId] = lines;
        savedPromo[customerId] = promo;
        savedShipping[customerId] = shipping;
        savedGiftWrap[customerId] = giftWrap;
        savedInsurance[customerId] = insurance;
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
