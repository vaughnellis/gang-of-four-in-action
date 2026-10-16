using System.Globalization;

namespace CornerStore;

internal class ListingImporter
{
    private static readonly string[] Brands = ["Stridewell", "Hearthly", "Northfield", "Quayside", "Larkspur"];
    private static readonly string[] Materials = ["Mesh", "Canvas", "Wool", "Enamel"];
    private static readonly string[] Origins = ["Portugal", "Vietnam", "Australia"];
    private readonly ProductSpecFactory productSpecFactory = new();

    public List<Listing> Import(IReadOnlyList<string> skus, int count)
    {
        var listings = new List<Listing>(count);
        for (var i = 0; i < count; i++)
        {
            var brand = Brands[i % Brands.Length];
            var material = Materials[i % Materials.Length];
            var origin = Origins[i % Origins.Length];
            var spec = productSpecFactory.Get(brand, material, origin);

            listings.Add(new Listing(spec)
            {
                Sku = string.Concat(skus[i % skus.Count], "-L", i.ToString(CultureInfo.InvariantCulture)),
                Price = 10m + (i % 90),
            });
        }

        return listings;
    }
}
