using System.Globalization;
using BenchmarkDotNet.Attributes;
using CornerStore;

namespace CornerStore.Benchmarks;

[MemoryDiagnoser]
public class ListingImportBenchmarks
{
    private const int ListingCount = 100_000;
    private static readonly string[] Skus = ["SHOE-RUN-42", "SOCK-WOOL", "LAMP-DESK"];

    [Benchmark(Baseline = true)]
    public object RepeatedMetadata()
    {
        return RepeatedMetadataImporter.Import(Skus, ListingCount);
    }

    [Benchmark]
    public object SharedProductSpecs()
    {
        return new ListingImporter().Import(Skus, ListingCount);
    }

    private static class RepeatedMetadataImporter
    {
        private static readonly string[] Brands = ["Stridewell", "Hearthly", "Northfield", "Quayside", "Larkspur"];
        private static readonly string[] Materials = ["Mesh", "Canvas", "Wool", "Enamel"];
        private static readonly string[] Origins = ["Portugal", "Vietnam", "Australia"];

        public static List<RepeatedMetadataListing> Import(IReadOnlyList<string> skus, int count)
        {
            var listings = new List<RepeatedMetadataListing>(count);
            for (var i = 0; i < count; i++)
            {
                var brand = Brands[i % Brands.Length];
                var material = Materials[i % Materials.Length];
                var origin = Origins[i % Origins.Length];
                listings.Add(new RepeatedMetadataListing
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

    private sealed class RepeatedMetadataListing
    {
        public string Sku { get; init; } = "";

        public string Brand { get; init; } = "";

        public string Manufacturer { get; init; } = "";

        public string SpecText { get; init; } = "";

        public decimal Price { get; init; }
    }
}
