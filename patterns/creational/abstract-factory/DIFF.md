# DIFF: abstract-factory

## File summary

| Status | File | Added | Removed |
| --- | --- | ---: | ---: |
| Modified | CornerStore/CornerStore.csproj | 4 | 0 |
| Added | CornerStore/Pricing/AustraliaRegionFactory.cs | 14 | 0 |
| Added | CornerStore/Pricing/DefaultRegionFactory.cs | 14 | 0 |
| Added | CornerStore/Pricing/IMoneyFormatter.cs | 6 | 0 |
| Added | CornerStore/Pricing/IRegionalPricing.cs | 8 | 0 |
| Added | CornerStore/Pricing/IRegionFactory.cs | 8 | 0 |
| Modified | CornerStore/Pricing/MoneyFormatter.cs | 1 | 15 |
| Added | CornerStore/Pricing/NewZealandRegionFactory.cs | 14 | 0 |
| Modified | CornerStore/Pricing/PricingService.cs | 3 | 2 |
| Added | CornerStore/Pricing/RegionalMoneyFormatter.cs | 11 | 0 |
| Added | CornerStore/Pricing/RegionalPricing.cs | 3 | 0 |
| Added | CornerStore/Pricing/RegionFactoryProvider.cs | 14 | 0 |
| Removed | CornerStore/Pricing/ShippingCalculator.cs | 0 | 19 |
| Modified | CornerStore/Pricing/TaxCalculator.cs | 0 | 15 |
| Added | CornerStore.Tests/AbstractFactoryTests.cs | 34 | 0 |

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
diff --git a/pattern/src/CornerStore/Pricing/AustraliaRegionFactory.cs b/pattern/src/CornerStore/Pricing/AustraliaRegionFactory.cs
new file mode 100644
index 0000000..434c7bb
--- /dev/null
+++ b/pattern/src/CornerStore/Pricing/AustraliaRegionFactory.cs
@@ -0,0 +1,14 @@
+namespace CornerStore;
+
+internal sealed class AustraliaRegionFactory : IRegionFactory
+{
+    public IRegionalPricing CreatePricing()
+    {
+        return new RegionalPricing(0.10m, 0m);
+    }
+
+    public IMoneyFormatter CreateMoneyFormatter()
+    {
+        return new RegionalMoneyFormatter("A$");
+    }
+}
diff --git a/pattern/src/CornerStore/Pricing/DefaultRegionFactory.cs b/pattern/src/CornerStore/Pricing/DefaultRegionFactory.cs
new file mode 100644
index 0000000..e66dc8f
--- /dev/null
+++ b/pattern/src/CornerStore/Pricing/DefaultRegionFactory.cs
@@ -0,0 +1,14 @@
+namespace CornerStore;
+
+internal sealed class DefaultRegionFactory : IRegionFactory
+{
+    public IRegionalPricing CreatePricing()
+    {
+        return new RegionalPricing(0m, 0m);
+    }
+
+    public IMoneyFormatter CreateMoneyFormatter()
+    {
+        return new RegionalMoneyFormatter("");
+    }
+}
diff --git a/pattern/src/CornerStore/Pricing/IMoneyFormatter.cs b/pattern/src/CornerStore/Pricing/IMoneyFormatter.cs
new file mode 100644
index 0000000..f41b824
--- /dev/null
+++ b/pattern/src/CornerStore/Pricing/IMoneyFormatter.cs
@@ -0,0 +1,6 @@
+namespace CornerStore;
+
+internal interface IMoneyFormatter
+{
+    string Format(decimal amount);
+}
diff --git a/pattern/src/CornerStore/Pricing/IRegionFactory.cs b/pattern/src/CornerStore/Pricing/IRegionFactory.cs
new file mode 100644
index 0000000..9c7b665
--- /dev/null
+++ b/pattern/src/CornerStore/Pricing/IRegionFactory.cs
@@ -0,0 +1,8 @@
+namespace CornerStore;
+
+internal interface IRegionFactory
+{
+    IRegionalPricing CreatePricing();
+
+    IMoneyFormatter CreateMoneyFormatter();
+}
diff --git a/pattern/src/CornerStore/Pricing/IRegionalPricing.cs b/pattern/src/CornerStore/Pricing/IRegionalPricing.cs
new file mode 100644
index 0000000..f1f5484
--- /dev/null
+++ b/pattern/src/CornerStore/Pricing/IRegionalPricing.cs
@@ -0,0 +1,8 @@
+namespace CornerStore;
+
+internal interface IRegionalPricing
+{
+    decimal TaxRate { get; }
+
+    decimal ShippingSurcharge { get; }
+}
diff --git a/baseline/src/CornerStore/Pricing/MoneyFormatter.cs b/pattern/src/CornerStore/Pricing/MoneyFormatter.cs
index 15a8a68..e5646f2 100644
--- a/baseline/src/CornerStore/Pricing/MoneyFormatter.cs
+++ b/pattern/src/CornerStore/Pricing/MoneyFormatter.cs
@@ -1,23 +1,9 @@
-using System.Globalization;
-
 namespace CornerStore;
 internal static class MoneyFormatter
 {
     public static string Format(decimal amount, string region)
     {
-        var number = amount.ToString("0.00", CultureInfo.InvariantCulture);
-
-        // SMELL S01: the currency symbol is picked with its own region branch
-        if (region == "AU")
-        {
-            return "A$" + number;
-        }
-        else if (region == "NZ")
-        {
-            return "NZ$" + number;
-        }
-
-        return number;
+        return RegionFactoryProvider.For(region).CreateMoneyFormatter().Format(amount);
     }
 }
diff --git a/pattern/src/CornerStore/Pricing/NewZealandRegionFactory.cs b/pattern/src/CornerStore/Pricing/NewZealandRegionFactory.cs
new file mode 100644
index 0000000..349ee79
--- /dev/null
+++ b/pattern/src/CornerStore/Pricing/NewZealandRegionFactory.cs
@@ -0,0 +1,14 @@
+namespace CornerStore;
+
+internal sealed class NewZealandRegionFactory : IRegionFactory
+{
+    public IRegionalPricing CreatePricing()
+    {
+        return new RegionalPricing(0.15m, 3.00m);
+    }
+
+    public IMoneyFormatter CreateMoneyFormatter()
+    {
+        return new RegionalMoneyFormatter("NZ$");
+    }
+}
diff --git a/baseline/src/CornerStore/Pricing/PricingService.cs b/pattern/src/CornerStore/Pricing/PricingService.cs
index 64d5dd4..bdbb7b8 100644
--- a/baseline/src/CornerStore/Pricing/PricingService.cs
+++ b/pattern/src/CornerStore/Pricing/PricingService.cs
@@ -22,9 +22,10 @@ internal class PricingService(PromoEngine promos)
         var methodCost = ShippingCost(cart.ShippingMethod, weightKg)
             ?? throw new InvalidOperationException("unknown shipping method " + cart.ShippingMethod);
-        var shipping = methodCost + ShippingCalculator.RegionSurcharge(customer.Region);
+        var regionalPricing = RegionFactoryProvider.For(customer.Region).CreatePricing();
+        var shipping = methodCost + regionalPricing.ShippingSurcharge;
-        var rate = TaxCalculator.RateFor(customer.Region);
+        var rate = regionalPricing.TaxRate;
         var tax = 0m;
         var allocated = 0m;
         for (var i = 0; i < cart.Lines.Count; i++)
diff --git a/pattern/src/CornerStore/Pricing/RegionFactoryProvider.cs b/pattern/src/CornerStore/Pricing/RegionFactoryProvider.cs
new file mode 100644
index 0000000..007e849
--- /dev/null
+++ b/pattern/src/CornerStore/Pricing/RegionFactoryProvider.cs
@@ -0,0 +1,14 @@
+namespace CornerStore;
+
+internal static class RegionFactoryProvider
+{
+    public static IRegionFactory For(string region)
+    {
+        return region switch
+        {
+            "AU" => new AustraliaRegionFactory(),
+            "NZ" => new NewZealandRegionFactory(),
+            _ => new DefaultRegionFactory(),
+        };
+    }
+}
diff --git a/pattern/src/CornerStore/Pricing/RegionalMoneyFormatter.cs b/pattern/src/CornerStore/Pricing/RegionalMoneyFormatter.cs
new file mode 100644
index 0000000..98b4df3
--- /dev/null
+++ b/pattern/src/CornerStore/Pricing/RegionalMoneyFormatter.cs
@@ -0,0 +1,11 @@
+using System.Globalization;
+
+namespace CornerStore;
+
+internal sealed class RegionalMoneyFormatter(string currencyPrefix) : IMoneyFormatter
+{
+    public string Format(decimal amount)
+    {
+        return currencyPrefix + amount.ToString("0.00", CultureInfo.InvariantCulture);
+    }
+}
diff --git a/pattern/src/CornerStore/Pricing/RegionalPricing.cs b/pattern/src/CornerStore/Pricing/RegionalPricing.cs
new file mode 100644
index 0000000..edfc941
--- /dev/null
+++ b/pattern/src/CornerStore/Pricing/RegionalPricing.cs
@@ -0,0 +1,3 @@
+namespace CornerStore;
+
+internal sealed record RegionalPricing(decimal TaxRate, decimal ShippingSurcharge) : IRegionalPricing;
diff --git a/baseline/src/CornerStore/Pricing/ShippingCalculator.cs b/baseline/src/CornerStore/Pricing/ShippingCalculator.cs
deleted file mode 100644
index e02f92d..0000000
--- a/baseline/src/CornerStore/Pricing/ShippingCalculator.cs
+++ /dev/null
@@ -1,19 +0,0 @@
-namespace CornerStore;
-
-internal static class ShippingCalculator
-{
-    public static decimal RegionSurcharge(string region)
-    {
-        // SMELL S01: the regional shipping surcharge is picked with its own region branch
-        if (region == "AU")
-        {
-            return 0m;
-        }
-        else if (region == "NZ")
-        {
-            return 3.00m;
-        }
-
-        return 0m;
-    }
-}
diff --git a/baseline/src/CornerStore/Pricing/TaxCalculator.cs b/pattern/src/CornerStore/Pricing/TaxCalculator.cs
index 609c520..48fb1d6 100644
--- a/baseline/src/CornerStore/Pricing/TaxCalculator.cs
+++ b/pattern/src/CornerStore/Pricing/TaxCalculator.cs
@@ -2,21 +2,6 @@ namespace CornerStore;
 internal static class TaxCalculator
 {
-    public static decimal RateFor(string region)
-    {
-        // SMELL S01: the tax rate is picked with its own region branch
-        if (region == "AU")
-        {
-            return 0.10m;
-        }
-        else if (region == "NZ")
-        {
-            return 0.15m;
-        }
-
-        return 0m;
-    }
-
     public static decimal LineTax(Product product, decimal amount, decimal rate)
     {
         // SMELL S23: tax depends on the line type, picked with a type switch
diff --git a/pattern/tests/CornerStore.Tests/AbstractFactoryTests.cs b/pattern/tests/CornerStore.Tests/AbstractFactoryTests.cs
new file mode 100644
index 0000000..65d9da1
--- /dev/null
+++ b/pattern/tests/CornerStore.Tests/AbstractFactoryTests.cs
@@ -0,0 +1,34 @@
+namespace CornerStore.Tests;
+
+public class AbstractFactoryTests
+{
+    [Fact]
+    public void RegionFactory_ForAustralia_CreatesAustralianProductFamily()
+    {
+        var factory = RegionFactoryProvider.For("AU");
+
+        Assert.Equal(0.10m, factory.CreatePricing().TaxRate);
+        Assert.Equal(0m, factory.CreatePricing().ShippingSurcharge);
+        Assert.Equal("A$12.50", factory.CreateMoneyFormatter().Format(12.50m));
+    }
+
+    [Fact]
+    public void RegionFactory_ForNewZealand_CreatesNewZealandProductFamily()
+    {
+        var factory = RegionFactoryProvider.For("NZ");
+
+        Assert.Equal(0.15m, factory.CreatePricing().TaxRate);
+        Assert.Equal(3.00m, factory.CreatePricing().ShippingSurcharge);
+        Assert.Equal("NZ$12.50", factory.CreateMoneyFormatter().Format(12.50m));
+    }
+
+    [Fact]
+    public void RegionFactory_ForUnknownRegion_PreservesDefaultProductFamily()
+    {
+        var factory = RegionFactoryProvider.For("XX");
+
+        Assert.Equal(0m, factory.CreatePricing().TaxRate);
+        Assert.Equal(0m, factory.CreatePricing().ShippingSurcharge);
+        Assert.Equal("12.50", factory.CreateMoneyFormatter().Format(12.50m));
+    }
+}
```
