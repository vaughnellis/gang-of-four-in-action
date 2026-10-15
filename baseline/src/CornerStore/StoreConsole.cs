using System.Globalization;
using System.Text;
using System.Text.Json;

namespace CornerStore;

internal class StoreConsole
{
    private readonly ScenarioClock clock;
    private readonly Catalog catalog = new();
    private readonly CartService cartService;
    private readonly ListingImporter importer = new();
    private readonly Dictionary<string, Customer> customers = [];
    private readonly Dictionary<string, Cart> carts = [];
    private readonly Dictionary<string, List<CartLine>> savedLines = [];
    private readonly Dictionary<string, string> savedPromo = [];
    private readonly Dictionary<string, string> savedShipping = [];
    private List<Listing> listings = [];

    public StoreConsole(ScenarioClock clock)
    {
        this.clock = clock;
        cartService = new CartService(catalog);
    }

    public IdSequence Ids { get; } = new();

    public void Handle(int number, JsonElement step)
    {
        var op = step.TryGetProperty("op", out var opValue) ? opValue.GetString() ?? "" : "";
        Log.Line("[" + number.ToString("D3", CultureInfo.InvariantCulture) + "] " + op + Describe(step));

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
            default:
                Log.Info("unknown op");
                break;
        }
    }

    public bool ApplyConfig(string key, int value)
    {
        // SMELL S05: any caller can overwrite the global Config fields
        switch (key)
        {
            case "fraudThreshold":
                Config.FraudThreshold = value;
                return true;
            case "lowStockLevel":
                Config.LowStockLevel = value;
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
            Log.Info("advance needs minutes");
            return;
        }

        clock.Advance(TimeSpan.FromMinutes(value));
        Log.Info("clock " + clock.Stamp());
    }

    private void Browse(JsonElement step)
    {
        var name = step.TryGetProperty("category", out _) ? Text(step, "category") : catalog.Root.Name;
        var category = catalog.FindCategory(name);
        if (category is null)
        {
            Log.Info("unknown category " + name);
            return;
        }

        catalog.PrintTree(category, 0);
        Log.Info(catalog.CountProducts(category).ToString(CultureInfo.InvariantCulture) + " products under " + name);
    }

    private void Variant(JsonElement step)
    {
        var parent = Text(step, "parent");
        var sku = Text(step, "sku");
        var error = catalog.CreateVariant(parent, sku, Text(step, "size"), Text(step, "colour"));
        Log.Info(error is null ? "created variant " + sku + " of " + parent : "rejected: " + error);
    }

    private void BundleCreate(JsonElement step)
    {
        var sku = Text(step, "sku");
        var error = catalog.CreateBundle(sku, Text(step, "name"));
        Log.Info(error is null ? "created bundle " + sku : "rejected: " + error);
    }

    private void BundleAdd(JsonElement step)
    {
        var bundle = Text(step, "bundle");
        var sku = Text(step, "sku");
        var error = catalog.AddToBundle(bundle, sku);
        if (error is not null)
        {
            Log.Info("rejected: " + error);
            return;
        }

        Log.Info("added " + sku + " to " + bundle);
        Log.Info("bundle " + bundle + " price " + Catalog.FormatPrice(catalog.PriceOf(bundle)));
    }

    private void Import(JsonElement step)
    {
        var count = step.TryGetProperty("count", out var value) && value.TryGetInt32(out var number) ? number : 100000;
        var skus = new List<string>();
        catalog.CollectSkus(catalog.Root, skus);
        listings = importer.Import(skus, count);
        Log.Info("imported " + listings.Count.ToString(CultureInfo.InvariantCulture) + " listings");
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
        carts[id] = new Cart(id);
        Log.Info("registered " + id + " " + customer.Name + " (" + customer.Region + ", " + customer.Channel + ")");
    }

    private Cart? FindCart(JsonElement step)
    {
        var id = Text(step, "customer");
        if (!customers.ContainsKey(id))
        {
            Log.Info("rejected: unknown customer " + id);
            return null;
        }

        return carts[id];
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

        var error = cartService.Add(cart, sku, quantity);
        if (error is not null)
        {
            Log.Info("rejected: " + error);
            return;
        }

        Remember(cart.CustomerId, lines, promo, shipping);
        Log.Info("added " + quantity.ToString(CultureInfo.InvariantCulture) + " x " + sku);
        Log.Info(Summary(cart));
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

        var error = cartService.Remove(cart, sku);
        if (error is not null)
        {
            Log.Info("rejected: " + error);
            return;
        }

        Remember(cart.CustomerId, lines, promo, shipping);
        Log.Info("removed " + sku);
        Log.Info(Summary(cart));
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
            Log.Info("nothing to undo");
            return;
        }

        // SMELL S18: the caller writes the saved internals straight back into the cart
        cart.Lines = lines;
        cart.PromoCode = savedPromo[cart.CustomerId];
        cart.ShippingMethod = savedShipping[cart.CustomerId];

        savedLines.Remove(cart.CustomerId);
        savedPromo.Remove(cart.CustomerId);
        savedShipping.Remove(cart.CustomerId);
        Log.Info("undone");
        Log.Info(Summary(cart));
    }

    private void CartShow(JsonElement step)
    {
        var cart = FindCart(step);
        if (cart is null)
        {
            return;
        }

        foreach (var line in cart.Lines)
        {
            var origin = line.FromBundle == "" ? "" : " (from " + line.FromBundle + ")";
            var amount = line.UnitPrice * line.Quantity;
            Log.Info("- " + line.Product.Sku + " " + line.Product.Name + " x" + line.Quantity.ToString(CultureInfo.InvariantCulture)
                + " @ " + Catalog.FormatPrice(line.UnitPrice) + " = " + Catalog.FormatPrice(amount) + origin);
        }

        Log.Info(Summary(cart));
    }

    private string Summary(Cart cart)
    {
        return "cart " + cart.CustomerId + " lines=" + cart.Lines.Count.ToString(CultureInfo.InvariantCulture)
            + " subtotal=" + Catalog.FormatPrice(cartService.Subtotal(cart));
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

    private void Remember(string customerId, List<CartLine> lines, string promo, string shipping)
    {
        savedLines[customerId] = lines;
        savedPromo[customerId] = promo;
        savedShipping[customerId] = shipping;
    }

    private void SetConfig(JsonElement step)
    {
        if (!step.TryGetProperty("key", out var key)
            || !step.TryGetProperty("value", out var value)
            || !value.TryGetInt32(out var number))
        {
            Log.Info("config.set needs key and value");
            return;
        }

        var name = key.GetString() ?? "";
        if (ApplyConfig(name, number))
        {
            Log.Info(name + " set to " + number.ToString(CultureInfo.InvariantCulture));
        }
        else
        {
            Log.Info("unknown setting " + name);
        }
    }
}
