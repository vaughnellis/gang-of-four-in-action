using System.Globalization;

namespace CornerStore;

internal class Catalog
{
    private readonly Dictionary<string, Product> products = [];
    private readonly Dictionary<string, Bundle> bundles = [];
    private readonly Dictionary<string, Category> categories = [];

    public Catalog()
    {
        Root = AddCategory(null, "Store");
        var footwear = AddCategory(Root, "Footwear");
        var shoes = AddCategory(footwear, "Shoes");
        var kids = AddCategory(shoes, "Kids");
        var socks = AddCategory(footwear, "Socks");
        var home = AddCategory(Root, "Home");
        var kitchen = AddCategory(home, "Kitchen");
        var digital = AddCategory(Root, "Digital");
        var giftCards = AddCategory(Root, "Gift Cards");

        AddPhysical(shoes, "SHOE-RUN-42", "Runner Pro", 89.95m, "Stridewell", 0.8m, "42", "Black", "Mesh", "Lightweight running shoe");
        AddPhysical(shoes, "SHOE-CAS-40", "Canvas Low", 59.50m, "Stridewell", 0.7m, "40", "White", "Canvas", "Everyday canvas sneaker");
        AddPhysical(kids, "SHOE-KID-30", "Junior Runner", 44.00m, "Stridewell", 0.5m, "30", "Red", "Mesh", "Running shoe for kids");
        AddPhysical(socks, "SOCK-WOOL", "Wool Socks", 12.00m, "Northfield", 0.1m, "M", "Grey", "Wool", "Warm merino socks");
        AddPhysical(home, "LAMP-DESK", "Desk Lamp", 35.00m, "Hearthly", 1.2m, "Standard", "Brass", "Metal", "Adjustable desk lamp");
        AddPhysical(home, "TOWEL-SET", "Towel Set", 28.00m, "Hearthly", 0.9m, "Standard", "Blue", "Cotton", "Three piece towel set");
        AddPhysical(kitchen, "MUG-ENAMEL", "Enamel Mug", 14.50m, "Quayside", 0.3m, "350ml", "Cream", "Enamel", "Camp style enamel mug");
        AddPhysical(kitchen, "KETTLE-1L", "Kettle 1L", 49.00m, "Quayside", 1.1m, "1L", "Silver", "Steel", "Stovetop kettle");

        Register(digital, new DigitalProduct { Sku = "EBOOK-FIT", Name = "Fitness Ebook", Price = 9.99m, CategoryName = digital.Name, FileSizeMb = 4 });
        Register(digital, new DigitalProduct { Sku = "EBOOK-GARDEN", Name = "Garden Ebook", Price = 7.50m, CategoryName = digital.Name, FileSizeMb = 6 });
        Register(giftCards, new GiftCardProduct { Sku = "GC-25", Name = "Gift Card 25", Price = 25.00m, CategoryName = giftCards.Name });
        Register(giftCards, new GiftCardProduct { Sku = "GC-50", Name = "Gift Card 50", Price = 50.00m, CategoryName = giftCards.Name });

        var runKit = new Bundle("BUN-RUN-KIT", "Runner Kit");
        runKit.AddComponent(products["SHOE-RUN-42"]);
        runKit.AddComponent(products["SOCK-WOOL"]);
        bundles[runKit.Sku] = runKit;
    }

    public Category Root { get; }

    public static string FormatPrice(decimal amount)
    {
        return amount.ToString("0.00", CultureInfo.InvariantCulture);
    }

    public Category? FindCategory(string name)
    {
        return categories.GetValueOrDefault(name);
    }

    public ICatalogItem? Lookup(string sku)
    {
        if (products.TryGetValue(sku, out var product))
        {
            return product;
        }

        return bundles.GetValueOrDefault(sku);
    }

    public decimal PriceOf(string sku)
    {
        var item = Lookup(sku);
        return item?.Price ?? throw new KeyNotFoundException("unknown sku " + sku);
    }

    public string? CreateBundle(string sku, string name)
    {
        if (Lookup(sku) is not null)
        {
            return "sku " + sku + " already exists";
        }

        bundles[sku] = new Bundle(sku, name);
        return null;
    }

    public string? AddToBundle(string bundleSku, string sku)
    {
        if (!bundles.TryGetValue(bundleSku, out var bundle))
        {
            return "unknown bundle " + bundleSku;
        }

        var item = Lookup(sku);
        if (item is null)
        {
            return "unknown sku " + sku;
        }

        if (item is Bundle)
        {
            return sku + " is a bundle and bundles cannot nest";
        }

        bundle.AddComponent(item);
        return null;
    }

    public string? CreateVariant(string parentSku, string sku, string size, string colour)
    {
        if (!products.TryGetValue(parentSku, out var found))
        {
            return "unknown sku " + parentSku;
        }

        if (found is not PhysicalProduct parent)
        {
            return parentSku + " is not a physical product";
        }

        if (Lookup(sku) is not null)
        {
            return "sku " + sku + " already exists";
        }

        var variant = parent.Clone(sku, size, colour);

        Register(categories[parent.CategoryName], variant);
        return null;
    }

    public void PrintTree(Category category, int depth)
    {
        using var iterator = new DepthFirstCategoryIterator(category, depth);
        while (iterator.MoveNext())
        {
            var current = iterator.Current;
            var pad = new string(' ', iterator.CurrentDepth * 2);
            Log.Instance.Info(pad + current.Name);
            foreach (var product in current.Products)
            {
                Log.Instance.Info(pad + "  - " + product.Sku + " " + product.Name + " " + FormatPrice(PriceOf(product.Sku)));
            }

        }
    }

    public int CountProducts(Category category)
    {
        var count = 0;
        using var iterator = new DepthFirstCategoryIterator(category);
        while (iterator.MoveNext())
        {
            count += iterator.Current.Products.Count;
        }

        return count;
    }

    public void CollectSkus(Category category, List<string> skus)
    {
        using var iterator = new DepthFirstCategoryIterator(category);
        while (iterator.MoveNext())
        {
            foreach (var product in iterator.Current.Products)
            {
                skus.Add(product.Sku);
            }
        }
    }

    private Category AddCategory(Category? parent, string name)
    {
        var category = new Category(name);
        categories[name] = category;
        parent?.Children.Add(category);
        return category;
    }

    private void AddPhysical(
        Category category,
        string sku,
        string name,
        decimal price,
        string brand,
        decimal weightKg,
        string size,
        string colour,
        string material,
        string description)
    {
        Register(category, new PhysicalProduct
        {
            Sku = sku,
            Name = name,
            Price = price,
            CategoryName = category.Name,
            Brand = brand,
            Manufacturer = brand + " Manufacturing",
            WeightKg = weightKg,
            Size = size,
            Colour = colour,
            Material = material,
            Description = description,
        });
    }

    private void Register(Category category, Product product)
    {
        products[product.Sku] = product;
        category.Products.Add(product);
    }
}
