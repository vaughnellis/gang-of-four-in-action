# DIFF: flyweight

## File summary

| Status | File | Added | Removed |
| --- | --- | ---: | ---: |
| Modified | CornerStore/Catalog/Listing.cs | 2 | 7 |
| Modified | CornerStore/Catalog/ListingImporter.cs | 3 | 5 |
| Added | CornerStore/Catalog/ProductSpec.cs | 10 | 0 |
| Added | CornerStore/Catalog/ProductSpecFactory.cs | 22 | 0 |
| Modified | CornerStore/CornerStore.csproj | 5 | 0 |
| Added | CornerStore.Tests/FlyweightTests.cs | 31 | 0 |

## Full diff

```diff
diff --git a/baseline/src/CornerStore/Catalog/Listing.cs b/pattern/src/CornerStore/Catalog/Listing.cs
index f6d9b8d..dd82310 100644
--- a/baseline/src/CornerStore/Catalog/Listing.cs
+++ b/pattern/src/CornerStore/Catalog/Listing.cs
@@ -1,15 +1,10 @@
 namespace CornerStore;
-// SMELL S11: every listing carries its own copy of the brand, manufacturer and spec text
-internal class Listing
+internal class Listing(ProductSpec productSpec)
 {
     public string Sku { get; init; } = "";
-    public string Brand { get; init; } = "";
-
-    public string Manufacturer { get; init; } = "";
-
-    public string SpecText { get; init; } = "";
+    public ProductSpec Spec { get; } = productSpec;
     public decimal Price { get; init; }
 }
diff --git a/baseline/src/CornerStore/Catalog/ListingImporter.cs b/pattern/src/CornerStore/Catalog/ListingImporter.cs
index 08920e1..c540e38 100644
--- a/baseline/src/CornerStore/Catalog/ListingImporter.cs
+++ b/pattern/src/CornerStore/Catalog/ListingImporter.cs
@@ -7,6 +7,7 @@ internal class ListingImporter
     private static readonly string[] Brands = ["Stridewell", "Hearthly", "Northfield", "Quayside", "Larkspur"];
     private static readonly string[] Materials = ["Mesh", "Canvas", "Wool", "Enamel"];
     private static readonly string[] Origins = ["Portugal", "Vietnam", "Australia"];
+    private readonly ProductSpecFactory productSpecFactory = new();
     public List<Listing> Import(IReadOnlyList<string> skus, int count)
     {
@@ -16,14 +17,11 @@ internal class ListingImporter
             var brand = Brands[i % Brands.Length];
             var material = Materials[i % Materials.Length];
             var origin = Origins[i % Origins.Length];
+            var spec = productSpecFactory.Get(brand, material, origin);
-            // SMELL S11: string.Concat builds a fresh copy of the same brand, manufacturer and spec text for every listing
-            listings.Add(new Listing
+            listings.Add(new Listing(spec)
             {
                 Sku = string.Concat(skus[i % skus.Count], "-L", i.ToString(CultureInfo.InvariantCulture)),
-                Brand = string.Concat(brand, " & Co"),
-                Manufacturer = string.Concat(brand, " Manufacturing Pty Ltd"),
-                SpecText = string.Concat("Material: ", material, "; origin: ", origin, "; care: machine wash cold"),
                 Price = 10m + (i % 90),
             });
         }
diff --git a/pattern/src/CornerStore/Catalog/ProductSpec.cs b/pattern/src/CornerStore/Catalog/ProductSpec.cs
new file mode 100644
index 0000000..f05d2c2
--- /dev/null
+++ b/pattern/src/CornerStore/Catalog/ProductSpec.cs
@@ -0,0 +1,10 @@
+namespace CornerStore;
+
+internal sealed class ProductSpec(string brand, string manufacturer, string specText)
+{
+    public string Brand { get; } = brand;
+
+    public string Manufacturer { get; } = manufacturer;
+
+    public string SpecText { get; } = specText;
+}
diff --git a/pattern/src/CornerStore/Catalog/ProductSpecFactory.cs b/pattern/src/CornerStore/Catalog/ProductSpecFactory.cs
new file mode 100644
index 0000000..976c583
--- /dev/null
+++ b/pattern/src/CornerStore/Catalog/ProductSpecFactory.cs
@@ -0,0 +1,22 @@
+namespace CornerStore;
+
+internal sealed class ProductSpecFactory
+{
+    private readonly Dictionary<(string Brand, string Material, string Origin), ProductSpec> specifications = [];
+
+    public ProductSpec Get(string brand, string material, string origin)
+    {
+        var key = (brand, material, origin);
+        if (specifications.TryGetValue(key, out var specification))
+        {
+            return specification;
+        }
+
+        specification = new ProductSpec(
+            string.Concat(brand, " & Co"),
+            string.Concat(brand, " Manufacturing Pty Ltd"),
+            string.Concat("Material: ", material, "; origin: ", origin, "; care: machine wash cold"));
+        specifications.Add(key, specification);
+        return specification;
+    }
+}
diff --git a/baseline/src/CornerStore/CornerStore.csproj b/pattern/src/CornerStore/CornerStore.csproj
index a15a29b..eb50762 100644
--- a/baseline/src/CornerStore/CornerStore.csproj
+++ b/pattern/src/CornerStore/CornerStore.csproj
@@ -5,4 +5,9 @@
     <TargetFramework>net10.0</TargetFramework>
   </PropertyGroup>
+  <ItemGroup>
+    <InternalsVisibleTo Include="CornerStore.Tests" />
+    <InternalsVisibleTo Include="CornerStore.Benchmarks" />
+  </ItemGroup>
+
 </Project>
diff --git a/pattern/tests/CornerStore.Tests/FlyweightTests.cs b/pattern/tests/CornerStore.Tests/FlyweightTests.cs
new file mode 100644
index 0000000..ef38c6f
--- /dev/null
+++ b/pattern/tests/CornerStore.Tests/FlyweightTests.cs
@@ -0,0 +1,31 @@
+namespace CornerStore.Tests;
+
+public class FlyweightTests
+{
+    [Fact]
+    public void Import_ReusesTheSameProductSpecForRepeatedIntrinsicData()
+    {
+        var importer = new ListingImporter();
+
+        var listings = importer.Import(["SHOE-RUN-42"], 120);
+
+        Assert.Same(listings[0].Spec, listings[60].Spec);
+        Assert.Equal("Stridewell & Co", listings[0].Spec.Brand);
+        Assert.Equal("Stridewell Manufacturing Pty Ltd", listings[0].Spec.Manufacturer);
+        Assert.Equal("Material: Mesh; origin: Portugal; care: machine wash cold", listings[0].Spec.SpecText);
+    }
+
+    [Fact]
+    public void Import_KeepsSkuAndPriceSpecificToEachListing()
+    {
+        var importer = new ListingImporter();
+
+        var listings = importer.Import(["SHOE-RUN-42"], 2);
+
+        Assert.NotSame(listings[0].Spec, listings[1].Spec);
+        Assert.Equal("SHOE-RUN-42-L0", listings[0].Sku);
+        Assert.Equal("SHOE-RUN-42-L1", listings[1].Sku);
+        Assert.Equal(10m, listings[0].Price);
+        Assert.Equal(11m, listings[1].Price);
+    }
+}
```
