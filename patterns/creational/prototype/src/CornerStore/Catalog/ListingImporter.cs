using System.Globalization;

namespace CornerStore;

internal class ListingImporter
{
    private static readonly string[] Brands = ["Stridewell", "Hearthly", "Northfield", "Quayside", "Larkspur"];
    private static readonly string[] Materials = ["Mesh", "Canvas", "Wool", "Enamel"];
    private static readonly string[] Origins = ["Portugal", "Vietnam", "Australia"];

    public List<Listing> Import(IReadOnlyList<string> skus, int count)
    {
        var listings = new List<Listing>(count);
        for (var i = 0; i < count; i++)
        {
            var brand = Brands[i % Brands.Length];
            var material = Materials[i % Materials.Length];
            var origin = Origins[i % Origins.Length];

            // SMELL S11: string.Concat builds a fresh copy of the same brand, manufacturer and spec text for every listing
            listings.Add(new Listing
            {
                Sku = string.Concat(skus[i % skus.Count], "-L", i.ToString(CultureInfo.InvariantCulture)),
                Brand = string.Concat(brand, " & Co"),
                Manufacturer = string.Concat(brand, " Manufacturing Pty Ltd"),
                SpecText = string.Concat("Material: ", material, "; origin: ", origin, "; care: machine wash cold"),
                Price = 10m + (i % 90),
            });
        }

        return listings;
    }
}
