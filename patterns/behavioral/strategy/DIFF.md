# DIFF: strategy

## File summary

| Status | File | Added | Removed |
| --- | --- | ---: | ---: |
| Modified | CornerStore/CornerStore.csproj | 4 | 0 |
| Added | CornerStore/Pricing/ExpressShippingCostStrategy.cs | 9 | 0 |
| Added | CornerStore/Pricing/IShippingCostStrategy.cs | 6 | 0 |
| Added | CornerStore/Pricing/PickupShippingCostStrategy.cs | 9 | 0 |
| Modified | CornerStore/Pricing/PricingService.cs | 20 | 13 |
| Added | CornerStore/Pricing/StandardShippingCostStrategy.cs | 9 | 0 |
| Added | CornerStore.Tests/StrategyTests.cs | 81 | 0 |

## Full diff

```diff
diff --git a/baseline/src/CornerStore/CornerStore.csproj b/pattern/src/CornerStore/CornerStore.csproj
index a15a29b..f8dc6c1 100644
--- a/baseline/src/CornerStore/CornerStore.csproj
+++ b/pattern/src/CornerStore/CornerStore.csproj
@@ -5,4 +5,8 @@
     <TargetFramework>net10.0</TargetFramework>
   </PropertyGroup>
+  <ItemGroup>
+    <InternalsVisibleTo Include="CornerStore.Tests" />
+  </ItemGroup>
+
 </Project>
diff --git a/pattern/src/CornerStore/Pricing/ExpressShippingCostStrategy.cs b/pattern/src/CornerStore/Pricing/ExpressShippingCostStrategy.cs
new file mode 100644
index 0000000..e960bc0
--- /dev/null
+++ b/pattern/src/CornerStore/Pricing/ExpressShippingCostStrategy.cs
@@ -0,0 +1,9 @@
+namespace CornerStore;
+
+internal sealed class ExpressShippingCostStrategy : IShippingCostStrategy
+{
+    public decimal Calculate(decimal weightKg)
+    {
+        return Math.Round(12.00m + 4.00m * weightKg, 2, MidpointRounding.AwayFromZero);
+    }
+}
\ No newline at end of file
diff --git a/pattern/src/CornerStore/Pricing/IShippingCostStrategy.cs b/pattern/src/CornerStore/Pricing/IShippingCostStrategy.cs
new file mode 100644
index 0000000..6a4bc26
--- /dev/null
+++ b/pattern/src/CornerStore/Pricing/IShippingCostStrategy.cs
@@ -0,0 +1,6 @@
+namespace CornerStore;
+
+internal interface IShippingCostStrategy
+{
+    decimal Calculate(decimal weightKg);
+}
\ No newline at end of file
diff --git a/pattern/src/CornerStore/Pricing/PickupShippingCostStrategy.cs b/pattern/src/CornerStore/Pricing/PickupShippingCostStrategy.cs
new file mode 100644
index 0000000..3ddf709
--- /dev/null
+++ b/pattern/src/CornerStore/Pricing/PickupShippingCostStrategy.cs
@@ -0,0 +1,9 @@
+namespace CornerStore;
+
+internal sealed class PickupShippingCostStrategy : IShippingCostStrategy
+{
+    public decimal Calculate(decimal weightKg)
+    {
+        return 0m;
+    }
+}
\ No newline at end of file
diff --git a/baseline/src/CornerStore/Pricing/PricingService.cs b/pattern/src/CornerStore/Pricing/PricingService.cs
index 64d5dd4..7492998 100644
--- a/baseline/src/CornerStore/Pricing/PricingService.cs
+++ b/pattern/src/CornerStore/Pricing/PricingService.cs
@@ -1,7 +1,23 @@
 namespace CornerStore;
-internal class PricingService(PromoEngine promos)
+internal class PricingService
 {
+    private readonly PromoEngine promos;
+    private readonly IReadOnlyDictionary<string, IShippingCostStrategy> shippingStrategies;
+
+    public PricingService(
+        PromoEngine promos,
+        IReadOnlyDictionary<string, IShippingCostStrategy>? shippingStrategies = null)
+    {
+        this.promos = promos;
+        this.shippingStrategies = shippingStrategies ?? new Dictionary<string, IShippingCostStrategy>(StringComparer.Ordinal)
+        {
+            ["standard"] = new StandardShippingCostStrategy(),
+            ["express"] = new ExpressShippingCostStrategy(),
+            ["pickup"] = new PickupShippingCostStrategy(),
+        };
+    }
+
     public Quote BuildQuote(Cart cart, Customer customer)
     {
         var subtotal = 0m;
@@ -57,18 +73,9 @@ internal class PricingService(PromoEngine promos)
     public decimal? ShippingCost(string method, decimal weightKg)
     {
-        // SMELL S21: the shipping cost is picked with a switch on the method string
-        switch (method)
-        {
-            case "standard":
-                return Math.Round(5.00m + 2.00m * weightKg, 2, MidpointRounding.AwayFromZero);
-            case "express":
-                return Math.Round(12.00m + 4.00m * weightKg, 2, MidpointRounding.AwayFromZero);
-            case "pickup":
-                return 0m;
-            default:
-                return null;
-        }
+        return shippingStrategies.TryGetValue(method, out var strategy)
+            ? strategy.Calculate(weightKg)
+            : null;
     }
     private static decimal AddOnCost(Cart cart, decimal discountedSubtotal)
diff --git a/pattern/src/CornerStore/Pricing/StandardShippingCostStrategy.cs b/pattern/src/CornerStore/Pricing/StandardShippingCostStrategy.cs
new file mode 100644
index 0000000..17fc5ab
--- /dev/null
+++ b/pattern/src/CornerStore/Pricing/StandardShippingCostStrategy.cs
@@ -0,0 +1,9 @@
+namespace CornerStore;
+
+internal sealed class StandardShippingCostStrategy : IShippingCostStrategy
+{
+    public decimal Calculate(decimal weightKg)
+    {
+        return Math.Round(5.00m + 2.00m * weightKg, 2, MidpointRounding.AwayFromZero);
+    }
+}
\ No newline at end of file
diff --git a/pattern/tests/CornerStore.Tests/StrategyTests.cs b/pattern/tests/CornerStore.Tests/StrategyTests.cs
new file mode 100644
index 0000000..1f81647
--- /dev/null
+++ b/pattern/tests/CornerStore.Tests/StrategyTests.cs
@@ -0,0 +1,81 @@
+namespace CornerStore.Tests;
+
+public class StrategyTests
+{
+    [Theory]
+    [InlineData("standard", "AU", 6.60)]
+    [InlineData("standard", "NZ", 9.60)]
+    [InlineData("express", "AU", 15.20)]
+    [InlineData("express", "NZ", 18.20)]
+    [InlineData("pickup", "AU", 0.00)]
+    [InlineData("pickup", "NZ", 3.00)]
+    public void GivenShippingMethodAndRegion_BuildQuote_UsesExpectedShippingCost(
+        string method,
+        string region,
+        decimal expectedShipping)
+    {
+        var pricing = new PricingService(new PromoEngine());
+
+        var quote = pricing.BuildQuote(CreateCart(method), CreateCustomer(region));
+
+        Assert.Equal(expectedShipping, quote.Shipping);
+    }
+
+    [Fact]
+    public void GivenTestOnlyStrategy_BuildQuote_UsesInjectedStrategy()
+    {
+        var strategies = new Dictionary<string, IShippingCostStrategy>(StringComparer.Ordinal)
+        {
+            ["test"] = new TestShippingCostStrategy(),
+        };
+        var pricing = new PricingService(new PromoEngine(), strategies);
+
+        var quote = pricing.BuildQuote(CreateCart("test"), CreateCustomer("AU"));
+
+        Assert.Equal(17m, quote.Shipping);
+    }
+
+    private static Cart CreateCart(string method)
+    {
+        var cart = new Cart("cu-1")
+        {
+            ShippingMethod = method,
+        };
+        cart.Lines.Add(new CartLine
+        {
+            Product = new PhysicalProduct
+            {
+                Sku = "MUG-ENAMEL",
+                Name = "Enamel Mug",
+                CategoryName = "Home",
+                Price = 10m,
+                WeightKg = 0.8m,
+            },
+            Quantity = 1,
+            UnitPrice = 10m,
+        });
+        return cart;
+    }
+
+    private static Customer CreateCustomer(string region)
+    {
+        return new Customer
+        {
+            Id = "cu-1",
+            Number = 1,
+            Name = "Ava",
+            Region = region,
+            Channel = "email",
+            Address = "1 Harbour St",
+            CreditLimit = 500m,
+        };
+    }
+
+    private sealed class TestShippingCostStrategy : IShippingCostStrategy
+    {
+        public decimal Calculate(decimal weightKg)
+        {
+            return 17m;
+        }
+    }
+}
\ No newline at end of file
```
