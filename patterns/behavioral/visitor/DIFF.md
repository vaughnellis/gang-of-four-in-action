# DIFF: visitor

## File summary

| Status | File | Added | Removed |
| --- | --- | ---: | ---: |
| Modified | CornerStore/Catalog/DigitalProduct.cs | 5 | 0 |
| Modified | CornerStore/Catalog/GiftCardProduct.cs | 4 | 0 |
| Added | CornerStore/Catalog/ILineItemVisitor.cs | 10 | 0 |
| Modified | CornerStore/Catalog/PhysicalProduct.cs | 5 | 0 |
| Modified | CornerStore/Catalog/Product.cs | 2 | 0 |
| Modified | CornerStore/CornerStore.csproj | 4 | 0 |
| Modified | CornerStore/Pricing/TaxCalculator.cs | 3 | 12 |
| Added | CornerStore/Pricing/TaxVisitor.cs | 21 | 0 |
| Modified | CornerStore/Pricing/WeightCalculator.cs | 3 | 12 |
| Added | CornerStore/Pricing/WeightVisitor.cs | 21 | 0 |
| Modified | CornerStore/Reporting/CsvReportExporter.cs | 5 | 19 |
| Modified | CornerStore/Reporting/JsonReportExporter.cs | 5 | 19 |
| Added | CornerStore/Reporting/ReportTotalsVisitor.cs | 33 | 0 |
| Added | CornerStore.Tests/VisitorTests.cs | 43 | 0 |

## Full diff

```diff
diff --git a/baseline/src/CornerStore/Catalog/DigitalProduct.cs b/pattern/src/CornerStore/Catalog/DigitalProduct.cs
index be77b7f..948821e 100644
--- a/baseline/src/CornerStore/Catalog/DigitalProduct.cs
+++ b/pattern/src/CornerStore/Catalog/DigitalProduct.cs
@@ -3,4 +3,9 @@ namespace CornerStore;
 internal class DigitalProduct : Product
 {
     public int FileSizeMb { get; init; }
+
+    internal override void Accept(ILineItemVisitor visitor)
+    {
+        visitor.Visit(this);
+    }
 }
diff --git a/baseline/src/CornerStore/Catalog/GiftCardProduct.cs b/pattern/src/CornerStore/Catalog/GiftCardProduct.cs
index 118556b..7fa05e7 100644
--- a/baseline/src/CornerStore/Catalog/GiftCardProduct.cs
+++ b/pattern/src/CornerStore/Catalog/GiftCardProduct.cs
@@ -2,4 +2,8 @@ namespace CornerStore;
 internal class GiftCardProduct : Product
 {
+    internal override void Accept(ILineItemVisitor visitor)
+    {
+        visitor.Visit(this);
+    }
 }
diff --git a/pattern/src/CornerStore/Catalog/ILineItemVisitor.cs b/pattern/src/CornerStore/Catalog/ILineItemVisitor.cs
new file mode 100644
index 0000000..6de2647
--- /dev/null
+++ b/pattern/src/CornerStore/Catalog/ILineItemVisitor.cs
@@ -0,0 +1,10 @@
+namespace CornerStore;
+
+internal interface ILineItemVisitor
+{
+    void Visit(PhysicalProduct product);
+
+    void Visit(DigitalProduct product);
+
+    void Visit(GiftCardProduct product);
+}
diff --git a/baseline/src/CornerStore/Catalog/PhysicalProduct.cs b/pattern/src/CornerStore/Catalog/PhysicalProduct.cs
index 8cc62f4..0dc852f 100644
--- a/baseline/src/CornerStore/Catalog/PhysicalProduct.cs
+++ b/pattern/src/CornerStore/Catalog/PhysicalProduct.cs
@@ -15,4 +15,9 @@ internal class PhysicalProduct : Product
     public string Material { get; init; } = "";
     public string Description { get; init; } = "";
+
+    internal override void Accept(ILineItemVisitor visitor)
+    {
+        visitor.Visit(this);
+    }
 }
diff --git a/baseline/src/CornerStore/Catalog/Product.cs b/pattern/src/CornerStore/Catalog/Product.cs
index a803edc..489b345 100644
--- a/baseline/src/CornerStore/Catalog/Product.cs
+++ b/pattern/src/CornerStore/Catalog/Product.cs
@@ -9,4 +9,6 @@ internal abstract class Product
     public decimal Price { get; init; }
     public string CategoryName { get; init; } = "";
+
+    internal abstract void Accept(ILineItemVisitor visitor);
 }
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
diff --git a/baseline/src/CornerStore/Pricing/TaxCalculator.cs b/pattern/src/CornerStore/Pricing/TaxCalculator.cs
index 609c520..9a3e2d8 100644
--- a/baseline/src/CornerStore/Pricing/TaxCalculator.cs
+++ b/pattern/src/CornerStore/Pricing/TaxCalculator.cs
@@ -19,17 +19,8 @@ internal static class TaxCalculator
     public static decimal LineTax(Product product, decimal amount, decimal rate)
     {
-        // SMELL S23: tax depends on the line type, picked with a type switch
-        switch (product)
-        {
-            case PhysicalProduct:
-                return Math.Round(amount * rate, 2, MidpointRounding.AwayFromZero);
-            case DigitalProduct:
-                return Math.Round(amount * rate, 2, MidpointRounding.AwayFromZero);
-            case GiftCardProduct:
-                return 0m;
-            default:
-                throw new InvalidOperationException("unknown product type for " + product.Sku);
-        }
+        var visitor = new TaxVisitor(amount, rate);
+        product.Accept(visitor);
+        return visitor.Tax;
     }
 }
diff --git a/pattern/src/CornerStore/Pricing/TaxVisitor.cs b/pattern/src/CornerStore/Pricing/TaxVisitor.cs
new file mode 100644
index 0000000..521e701
--- /dev/null
+++ b/pattern/src/CornerStore/Pricing/TaxVisitor.cs
@@ -0,0 +1,21 @@
+namespace CornerStore;
+
+internal sealed class TaxVisitor(decimal amount, decimal rate) : ILineItemVisitor
+{
+    public decimal Tax { get; private set; }
+
+    public void Visit(PhysicalProduct product)
+    {
+        Tax = Math.Round(amount * rate, 2, MidpointRounding.AwayFromZero);
+    }
+
+    public void Visit(DigitalProduct product)
+    {
+        Tax = Math.Round(amount * rate, 2, MidpointRounding.AwayFromZero);
+    }
+
+    public void Visit(GiftCardProduct product)
+    {
+        Tax = 0m;
+    }
+}
diff --git a/baseline/src/CornerStore/Pricing/WeightCalculator.cs b/pattern/src/CornerStore/Pricing/WeightCalculator.cs
index b22a2f7..5c42263 100644
--- a/baseline/src/CornerStore/Pricing/WeightCalculator.cs
+++ b/pattern/src/CornerStore/Pricing/WeightCalculator.cs
@@ -4,17 +4,8 @@ internal static class WeightCalculator
 {
     public static decimal WeightOf(Product product, int quantity)
     {
-        // SMELL S23: weight depends on the line type, picked with a type switch
-        switch (product)
-        {
-            case PhysicalProduct physical:
-                return physical.WeightKg * quantity;
-            case DigitalProduct:
-                return 0m;
-            case GiftCardProduct:
-                return 0m;
-            default:
-                throw new InvalidOperationException("unknown product type for " + product.Sku);
-        }
+        var visitor = new WeightVisitor(quantity);
+        product.Accept(visitor);
+        return visitor.Weight;
     }
 }
diff --git a/pattern/src/CornerStore/Pricing/WeightVisitor.cs b/pattern/src/CornerStore/Pricing/WeightVisitor.cs
new file mode 100644
index 0000000..bb2f296
--- /dev/null
+++ b/pattern/src/CornerStore/Pricing/WeightVisitor.cs
@@ -0,0 +1,21 @@
+namespace CornerStore;
+
+internal sealed class WeightVisitor(int quantity) : ILineItemVisitor
+{
+    public decimal Weight { get; private set; }
+
+    public void Visit(PhysicalProduct product)
+    {
+        Weight = product.WeightKg * quantity;
+    }
+
+    public void Visit(DigitalProduct product)
+    {
+        Weight = 0m;
+    }
+
+    public void Visit(GiftCardProduct product)
+    {
+        Weight = 0m;
+    }
+}
diff --git a/baseline/src/CornerStore/Reporting/CsvReportExporter.cs b/pattern/src/CornerStore/Reporting/CsvReportExporter.cs
index 3e49cdf..8eeaf6f 100644
--- a/baseline/src/CornerStore/Reporting/CsvReportExporter.cs
+++ b/pattern/src/CornerStore/Reporting/CsvReportExporter.cs
@@ -9,35 +9,21 @@ internal class CsvReportExporter
     {
         // SMELL S22: CSV export duplicates the load, filter, aggregate and write skeleton
         var orders = source.Where(order => status == "all" || order.Status == status).ToList();
-        var physical = 0m;
-        var digital = 0m;
-        var giftCard = 0m;
+        var visitor = new ReportTotalsVisitor();
         foreach (var order in orders)
         {
             foreach (var line in order.Lines)
             {
                 var amount = line.UnitPrice * line.Quantity;
-                // SMELL S23: CSV rows switch on each concrete line item type
-                switch (line.Product)
-                {
-                    case PhysicalProduct:
-                        physical += amount;
-                        break;
-                    case DigitalProduct:
-                        digital += amount;
-                        break;
-                    case GiftCardProduct:
-                        giftCard += amount;
-                        break;
-                }
+                visitor.Add(line.Product, amount);
             }
         }
         var builder = new StringBuilder();
         builder.Append("type,total\n");
-        builder.Append("Physical,").Append(physical.ToString("0.00", CultureInfo.InvariantCulture)).Append('\n');
-        builder.Append("Digital,").Append(digital.ToString("0.00", CultureInfo.InvariantCulture)).Append('\n');
-        builder.Append("GiftCard,").Append(giftCard.ToString("0.00", CultureInfo.InvariantCulture)).Append('\n');
+        builder.Append("Physical,").Append(visitor.PhysicalTotal.ToString("0.00", CultureInfo.InvariantCulture)).Append('\n');
+        builder.Append("Digital,").Append(visitor.DigitalTotal.ToString("0.00", CultureInfo.InvariantCulture)).Append('\n');
+        builder.Append("GiftCard,").Append(visitor.GiftCardTotal.ToString("0.00", CultureInfo.InvariantCulture)).Append('\n');
         return builder.ToString();
     }
 }
\ No newline at end of file
diff --git a/baseline/src/CornerStore/Reporting/JsonReportExporter.cs b/pattern/src/CornerStore/Reporting/JsonReportExporter.cs
index 028f70e..731b113 100644
--- a/baseline/src/CornerStore/Reporting/JsonReportExporter.cs
+++ b/pattern/src/CornerStore/Reporting/JsonReportExporter.cs
@@ -9,35 +9,21 @@ internal class JsonReportExporter
     {
         // SMELL S22: JSON export duplicates the load, filter, aggregate and write skeleton
         var orders = source.Where(order => status == "all" || order.Status == status).ToList();
-        var physical = 0m;
-        var digital = 0m;
-        var giftCard = 0m;
+        var visitor = new ReportTotalsVisitor();
         foreach (var order in orders)
         {
             foreach (var line in order.Lines)
             {
                 var amount = line.UnitPrice * line.Quantity;
-                // SMELL S23: JSON rows switch on each concrete line item type
-                switch (line.Product)
-                {
-                    case PhysicalProduct:
-                        physical += amount;
-                        break;
-                    case DigitalProduct:
-                        digital += amount;
-                        break;
-                    case GiftCardProduct:
-                        giftCard += amount;
-                        break;
-                }
+                visitor.Add(line.Product, amount);
             }
         }
         var builder = new StringBuilder();
         builder.Append("{\n  \"totals\": [\n");
-        builder.Append("    { \"type\": \"Physical\", \"total\": ").Append(physical.ToString("0.00", CultureInfo.InvariantCulture)).Append(" },\n");
-        builder.Append("    { \"type\": \"Digital\", \"total\": ").Append(digital.ToString("0.00", CultureInfo.InvariantCulture)).Append(" },\n");
-        builder.Append("    { \"type\": \"GiftCard\", \"total\": ").Append(giftCard.ToString("0.00", CultureInfo.InvariantCulture)).Append(" }\n");
+        builder.Append("    { \"type\": \"Physical\", \"total\": ").Append(visitor.PhysicalTotal.ToString("0.00", CultureInfo.InvariantCulture)).Append(" },\n");
+        builder.Append("    { \"type\": \"Digital\", \"total\": ").Append(visitor.DigitalTotal.ToString("0.00", CultureInfo.InvariantCulture)).Append(" },\n");
+        builder.Append("    { \"type\": \"GiftCard\", \"total\": ").Append(visitor.GiftCardTotal.ToString("0.00", CultureInfo.InvariantCulture)).Append(" }\n");
         builder.Append("  ]\n}\n");
         return builder.ToString();
     }
diff --git a/pattern/src/CornerStore/Reporting/ReportTotalsVisitor.cs b/pattern/src/CornerStore/Reporting/ReportTotalsVisitor.cs
new file mode 100644
index 0000000..1c2df1e
--- /dev/null
+++ b/pattern/src/CornerStore/Reporting/ReportTotalsVisitor.cs
@@ -0,0 +1,33 @@
+namespace CornerStore;
+
+internal sealed class ReportTotalsVisitor : ILineItemVisitor
+{
+    private decimal currentAmount;
+
+    public decimal PhysicalTotal { get; private set; }
+
+    public decimal DigitalTotal { get; private set; }
+
+    public decimal GiftCardTotal { get; private set; }
+
+    public void Add(Product product, decimal amount)
+    {
+        currentAmount = amount;
+        product.Accept(this);
+    }
+
+    public void Visit(PhysicalProduct product)
+    {
+        PhysicalTotal += currentAmount;
+    }
+
+    public void Visit(DigitalProduct product)
+    {
+        DigitalTotal += currentAmount;
+    }
+
+    public void Visit(GiftCardProduct product)
+    {
+        GiftCardTotal += currentAmount;
+    }
+}
diff --git a/pattern/tests/CornerStore.Tests/VisitorTests.cs b/pattern/tests/CornerStore.Tests/VisitorTests.cs
new file mode 100644
index 0000000..4641f73
--- /dev/null
+++ b/pattern/tests/CornerStore.Tests/VisitorTests.cs
@@ -0,0 +1,43 @@
+namespace CornerStore.Tests;
+
+public class VisitorTests
+{
+    [Fact]
+    public void TaxCalculator_LineTax_DispatchesByProductType()
+    {
+        var physical = new PhysicalProduct();
+        var digital = new DigitalProduct();
+        var giftCard = new GiftCardProduct();
+
+        Assert.Equal(1.24m, TaxCalculator.LineTax(physical, 12.35m, 0.10m));
+        Assert.Equal(1.24m, TaxCalculator.LineTax(digital, 12.35m, 0.10m));
+        Assert.Equal(0m, TaxCalculator.LineTax(giftCard, 12.35m, 0.10m));
+    }
+
+    [Fact]
+    public void WeightCalculator_WeightOf_UsesQuantityOnlyForPhysicalProducts()
+    {
+        var physical = new PhysicalProduct { WeightKg = 1.25m };
+        var digital = new DigitalProduct();
+        var giftCard = new GiftCardProduct();
+
+        Assert.Equal(3.75m, WeightCalculator.WeightOf(physical, 3));
+        Assert.Equal(0m, WeightCalculator.WeightOf(digital, 3));
+        Assert.Equal(0m, WeightCalculator.WeightOf(giftCard, 3));
+    }
+
+    [Fact]
+    public void ReportTotalsVisitor_Add_DistributesAmountsByProductType()
+    {
+        var visitor = new ReportTotalsVisitor();
+
+        visitor.Add(new PhysicalProduct(), 14.50m);
+        visitor.Add(new PhysicalProduct(), 5.50m);
+        visitor.Add(new DigitalProduct(), 7.50m);
+        visitor.Add(new GiftCardProduct(), 25m);
+
+        Assert.Equal(20m, visitor.PhysicalTotal);
+        Assert.Equal(7.50m, visitor.DigitalTotal);
+        Assert.Equal(25m, visitor.GiftCardTotal);
+    }
+}
```
