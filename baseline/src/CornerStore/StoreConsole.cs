using System.Globalization;
using System.Text;
using System.Text.Json;

namespace CornerStore;

internal class StoreConsole(ScenarioClock clock)
{
    private readonly Catalog catalog = new();
    private readonly ListingImporter importer = new();
    private List<Listing> listings = [];

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
