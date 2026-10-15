# DIFF: composite

## File summary

| Status | File | Added | Removed |
| --- | --- | ---: | ---: |
| Modified | CornerStore/Cart/CartService.cs | 3 | 38 |
| Modified | CornerStore/Catalog/Bundle.cs | 56 | 2 |
| Modified | CornerStore/Catalog/Catalog.cs | 10 | 30 |
| Added | CornerStore/Catalog/ICatalogItem.cs | 14 | 0 |
| Modified | CornerStore/Catalog/Product.cs | 9 | 1 |
| Added | CornerStore.Tests/CompositeTests.cs | 59 | 0 |

## Full diff

```diff
diff --git a/baseline/src/CornerStore/Cart/CartService.cs b/pattern/src/CornerStore/Cart/CartService.cs
index 3b1c202..57cf831 100644
--- a/baseline/src/CornerStore/Cart/CartService.cs
+++ b/pattern/src/CornerStore/Cart/CartService.cs
@@ -10,20 +10,12 @@ internal class CartService(Catalog catalog)
         }
         var item = catalog.Lookup(sku);
-
-        // SMELL S08: a bundle is told apart with an is-check and expanded into discounted component lines
-        if (item is Bundle bundle)
-        {
-            return AddBundle(cart, bundle, quantity);
-        }
-
-        if (item is not Product product)
+        if (item is null)
         {
             return "unknown sku " + sku;
         }
-        AddLine(cart, product, quantity, product.Price, "");
-        return null;
+        return item.AddToCart(cart, quantity, "", null);
     }
     public string? Remove(Cart cart, string sku)
@@ -43,34 +35,7 @@ internal class CartService(Catalog catalog)
         return total;
     }
-    private string? AddBundle(Cart cart, Bundle bundle, int quantity)
-    {
-        if (bundle.ComponentSkus.Count == 0)
-        {
-            return bundle.Sku + " has no components";
-        }
-
-        // The last component takes the remainder so the lines add up to the bundle price.
-        var bundlePrice = catalog.PriceOf(bundle.Sku);
-        var allocated = 0m;
-        for (var i = 0; i < bundle.ComponentSkus.Count; i++)
-        {
-            if (catalog.Lookup(bundle.ComponentSkus[i]) is not Product component)
-            {
-                return "unknown sku " + bundle.ComponentSkus[i];
-            }
-
-            var price = i == bundle.ComponentSkus.Count - 1
-                ? bundlePrice - allocated
-                : Math.Round(component.Price * (100 - bundle.DiscountPercent) / 100m, 2, MidpointRounding.AwayFromZero);
-            allocated += price;
-            AddLine(cart, component, quantity, price, bundle.Sku);
-        }
-
-        return null;
-    }
-
-    private static void AddLine(Cart cart, Product product, int quantity, decimal unitPrice, string fromBundle)
+    internal static void AddLine(Cart cart, Product product, int quantity, decimal unitPrice, string fromBundle)
     {
         foreach (var line in cart.Lines)
         {
diff --git a/baseline/src/CornerStore/Catalog/Bundle.cs b/pattern/src/CornerStore/Catalog/Bundle.cs
index e0444c9..aaed060 100644
--- a/baseline/src/CornerStore/Catalog/Bundle.cs
+++ b/pattern/src/CornerStore/Catalog/Bundle.cs
@@ -1,12 +1,66 @@
 namespace CornerStore;
-internal class Bundle(string sku, string name)
+internal class Bundle(string sku, string name) : ICatalogItem
 {
+    private readonly List<ICatalogItem> components = [];
+
     public string Sku { get; } = sku;
     public string Name { get; } = name;
     public int DiscountPercent { get; init; } = 10;
-    public List<string> ComponentSkus { get; } = [];
+    public IReadOnlyList<ICatalogItem> Components => components.AsReadOnly();
+
+    public decimal Price
+    {
+        get
+        {
+            var total = 0m;
+            foreach (var component in components)
+            {
+                total += component.Price;
+            }
+
+            return Math.Round(total * (100 - DiscountPercent) / 100m, 2, MidpointRounding.AwayFromZero);
+        }
+    }
+
+    public void AddComponent(ICatalogItem component)
+    {
+        components.Add(component);
+    }
+
+    public string? AddToCart(Cart cart, int quantity, string fromBundle, decimal? unitPrice)
+    {
+        if (components.Count == 0)
+        {
+            return Sku + " has no components";
+        }
+
+        var rootBundleSku = fromBundle == "" ? Sku : fromBundle;
+        var targetPrice = unitPrice ?? Price;
+        var scale = Price == 0m ? 0m : targetPrice / Price;
+        var allocated = 0m;
+        for (var i = 0; i < components.Count; i++)
+        {
+            var component = components[i];
+            var discountedPrice = Math.Round(
+                component.Price * (100 - DiscountPercent) / 100m,
+                2,
+                MidpointRounding.AwayFromZero);
+            var componentPrice = i == components.Count - 1
+                ? targetPrice - allocated
+                : Math.Round(discountedPrice * scale, 2, MidpointRounding.AwayFromZero);
+            allocated += componentPrice;
+
+            var error = component.AddToCart(cart, quantity, rootBundleSku, componentPrice);
+            if (error is not null)
+            {
+                return error;
+            }
+        }
+
+        return null;
+    }
 }
diff --git a/baseline/src/CornerStore/Catalog/Catalog.cs b/pattern/src/CornerStore/Catalog/Catalog.cs
index 2131b22..d75a960 100644
--- a/baseline/src/CornerStore/Catalog/Catalog.cs
+++ b/pattern/src/CornerStore/Catalog/Catalog.cs
@@ -35,8 +35,8 @@ internal class Catalog
         Register(giftCards, new GiftCardProduct { Sku = "GC-50", Name = "Gift Card 50", Price = 50.00m, CategoryName = giftCards.Name });
         var runKit = new Bundle("BUN-RUN-KIT", "Runner Kit");
-        runKit.ComponentSkus.Add("SHOE-RUN-42");
-        runKit.ComponentSkus.Add("SOCK-WOOL");
+        runKit.AddComponent(products["SHOE-RUN-42"]);
+        runKit.AddComponent(products["SOCK-WOOL"]);
         bundles[runKit.Sku] = runKit;
     }
@@ -52,7 +52,7 @@ internal class Catalog
         return categories.GetValueOrDefault(name);
     }
-    public object? Lookup(string sku)
+    public ICatalogItem? Lookup(string sku)
     {
         if (products.TryGetValue(sku, out var product))
         {
@@ -65,25 +65,7 @@ internal class Catalog
     public decimal PriceOf(string sku)
     {
         var item = Lookup(sku);
-
-        // SMELL S08: bundles take a separate pricing path picked with an is-check
-        if (item is Bundle bundle)
-        {
-            var total = 0m;
-            foreach (var componentSku in bundle.ComponentSkus)
-            {
-                total += products[componentSku].Price;
-            }
-
-            return Math.Round(total * (100 - bundle.DiscountPercent) / 100m, 2, MidpointRounding.AwayFromZero);
-        }
-
-        if (item is Product product)
-        {
-            return product.Price;
-        }
-
-        throw new KeyNotFoundException("unknown sku " + sku);
+        return item?.Price ?? throw new KeyNotFoundException("unknown sku " + sku);
     }
     public string? CreateBundle(string sku, string name)
@@ -99,25 +81,23 @@ internal class Catalog
     public string? AddToBundle(string bundleSku, string sku)
     {
-        if (Lookup(bundleSku) is not Bundle bundle)
+        if (!bundles.TryGetValue(bundleSku, out var bundle))
         {
             return "unknown bundle " + bundleSku;
         }
         var item = Lookup(sku);
-
-        // SMELL S08: a bundle is told apart from a product with is-checks and cannot hold another bundle
-        if (item is Bundle)
+        if (item is null)
         {
-            return sku + " is a bundle and bundles cannot nest";
+            return "unknown sku " + sku;
         }
-        if (item is not Product)
+        if (item is Bundle)
         {
-            return "unknown sku " + sku;
+            return sku + " is a bundle and bundles cannot nest";
         }
-        bundle.ComponentSkus.Add(sku);
+        bundle.AddComponent(item);
         return null;
     }
diff --git a/pattern/src/CornerStore/Catalog/ICatalogItem.cs b/pattern/src/CornerStore/Catalog/ICatalogItem.cs
new file mode 100644
index 0000000..8d5d0f3
--- /dev/null
+++ b/pattern/src/CornerStore/Catalog/ICatalogItem.cs
@@ -0,0 +1,14 @@
+namespace CornerStore;
+
+internal interface ICatalogItem
+{
+    string Sku { get; }
+
+    string Name { get; }
+
+    decimal Price { get; }
+
+    IReadOnlyList<ICatalogItem> Components { get; }
+
+    string? AddToCart(Cart cart, int quantity, string fromBundle, decimal? unitPrice);
+}
diff --git a/baseline/src/CornerStore/Catalog/Product.cs b/pattern/src/CornerStore/Catalog/Product.cs
index a803edc..f11d3ae 100644
--- a/baseline/src/CornerStore/Catalog/Product.cs
+++ b/pattern/src/CornerStore/Catalog/Product.cs
@@ -1,6 +1,6 @@
 namespace CornerStore;
-internal abstract class Product
+internal abstract class Product : ICatalogItem
 {
     public string Sku { get; init; } = "";
@@ -9,4 +9,12 @@ internal abstract class Product
     public decimal Price { get; init; }
     public string CategoryName { get; init; } = "";
+
+    public IReadOnlyList<ICatalogItem> Components => Array.Empty<ICatalogItem>();
+
+    public string? AddToCart(Cart cart, int quantity, string fromBundle, decimal? unitPrice)
+    {
+        CartService.AddLine(cart, this, quantity, unitPrice ?? Price, fromBundle);
+        return null;
+    }
 }
diff --git a/pattern/tests/CornerStore.Tests/CompositeTests.cs b/pattern/tests/CornerStore.Tests/CompositeTests.cs
new file mode 100644
index 0000000..06ea1ae
--- /dev/null
+++ b/pattern/tests/CornerStore.Tests/CompositeTests.cs
@@ -0,0 +1,59 @@
+namespace CornerStore.Tests;
+
+public class CompositeTests : IDisposable
+{
+    private readonly string _path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".json");
+
+    public void Dispose()
+    {
+        File.Delete(_path);
+    }
+
+    [Fact]
+    public void GivenNestedBundle_WhenAddedToBundle_PreservesTheNestingRule()
+    {
+        var transcript = RunSteps(
+            """{ "op": "bundle.create", "sku": "BUN-OUTER", "name": "Outer kit" }""",
+            """{ "op": "bundle.add", "bundle": "BUN-OUTER", "sku": "BUN-RUN-KIT" }""");
+
+        Assert.Contains("  rejected: BUN-RUN-KIT is a bundle and bundles cannot nest\n", transcript);
+    }
+
+    [Fact]
+    public void GivenEmptyBundle_WhenAddedToCart_ReportsNoComponents()
+    {
+        var transcript = RunSteps(
+            """{ "op": "register", "name": "Ava", "region": "AU", "channel": "email", "creditLimit": 800 }""",
+            """{ "op": "bundle.create", "sku": "BUN-EMPTY", "name": "Empty kit" }""",
+            """{ "op": "cart.add", "customer": "cu-1", "sku": "BUN-EMPTY" }""");
+
+        Assert.Contains("  rejected: BUN-EMPTY has no components\n", transcript);
+    }
+
+    [Fact]
+    public void GivenSimpleProductAndOneLevelBundle_WhenAddedToCart_PreservesPrices()
+    {
+        var transcript = RunSteps(
+            """{ "op": "register", "name": "Ava", "region": "AU", "channel": "email", "creditLimit": 800 }""",
+            """{ "op": "cart.add", "customer": "cu-1", "sku": "MUG-ENAMEL" }""",
+            """{ "op": "cart.add", "customer": "cu-1", "sku": "BUN-RUN-KIT" }""",
+            """{ "op": "cart.show", "customer": "cu-1" }""");
+
+        Assert.Contains("  - MUG-ENAMEL Enamel Mug x1 @ A$14.50 = A$14.50\n", transcript);
+        Assert.Contains("  - SHOE-RUN-42 Runner Pro x1 @ A$80.96 = A$80.96 (from BUN-RUN-KIT)\n", transcript);
+        Assert.Contains("  - SOCK-WOOL Wool Socks x1 @ A$10.80 = A$10.80 (from BUN-RUN-KIT)\n", transcript);
+        Assert.EndsWith("  cart cu-1 lines=3 subtotal=A$106.26\n", transcript);
+    }
+
+    private string RunSteps(params string[] steps)
+    {
+        File.WriteAllText(
+            _path,
+            """{ "name": "composite", "startTime": "2026-03-02T09:00:00Z", "seed": 1, "steps": [""" + string.Join(",", steps) + "] }");
+        var output = new StringWriter();
+
+        ScenarioRunner.Run(_path, output);
+
+        return output.ToString();
+    }
+}
```
