namespace CornerStore;

internal sealed class ProductSpecFactory
{
    private readonly Dictionary<(string Brand, string Material, string Origin), ProductSpec> specifications = [];

    public ProductSpec Get(string brand, string material, string origin)
    {
        var key = (brand, material, origin);
        if (specifications.TryGetValue(key, out var specification))
        {
            return specification;
        }

        specification = new ProductSpec(
            string.Concat(brand, " & Co"),
            string.Concat(brand, " Manufacturing Pty Ltd"),
            string.Concat("Material: ", material, "; origin: ", origin, "; care: machine wash cold"));
        specifications.Add(key, specification);
        return specification;
    }
}
