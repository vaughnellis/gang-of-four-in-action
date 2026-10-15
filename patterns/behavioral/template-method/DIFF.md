# DIFF: template-method

## File summary

| Status | File | Added | Removed |
| --- | --- | ---: | ---: |
| Modified | CornerStore/CornerStore.csproj | 4 | 0 |
| Modified | CornerStore/Reporting/CsvReportExporter.cs | 20 | 29 |
| Modified | CornerStore/Reporting/JsonReportExporter.cs | 20 | 29 |
| Added | CornerStore/Reporting/ReportExporter.cs | 23 | 0 |
| Added | CornerStore/Reporting/ReportTotals.cs | 10 | 0 |
| Added | CornerStore.Tests/TemplateMethodTests.cs | 87 | 0 |

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
diff --git a/baseline/src/CornerStore/Reporting/CsvReportExporter.cs b/pattern/src/CornerStore/Reporting/CsvReportExporter.cs
index 3e49cdf..7df3c3f 100644
--- a/baseline/src/CornerStore/Reporting/CsvReportExporter.cs
+++ b/pattern/src/CornerStore/Reporting/CsvReportExporter.cs
@@ -1,43 +1,34 @@
-using System.Globalization;
 using System.Text;

 namespace CornerStore;

-internal class CsvReportExporter
+internal sealed class CsvReportExporter : ReportExporter
 {
-    public string Export(List<Order> source, string status)
+    protected override void Accumulate(ReportTotals totals, CartLine line)
     {
-        // SMELL S22: CSV export duplicates the load, filter, aggregate and write skeleton
-        var orders = source.Where(order => status == "all" || order.Status == status).ToList();
-        var physical = 0m;
-        var digital = 0m;
-        var giftCard = 0m;
-        foreach (var order in orders)
+        var amount = line.UnitPrice * line.Quantity;
+        // SMELL S23: CSV rows switch on each concrete line item type
+        switch (line.Product)
         {
-            foreach (var line in order.Lines)
-            {
-                var amount = line.UnitPrice * line.Quantity;
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
-            }
+            case PhysicalProduct:
+                totals.Physical += amount;
+                break;
+            case DigitalProduct:
+                totals.Digital += amount;
+                break;
+            case GiftCardProduct:
+                totals.GiftCard += amount;
+                break;
         }
+    }

+    protected override string WriteReport(ReportTotals totals)
+    {
         var builder = new StringBuilder();
         builder.Append("type,total\n");
-        builder.Append("Physical,").Append(physical.ToString("0.00", CultureInfo.InvariantCulture)).Append('\n');
-        builder.Append("Digital,").Append(digital.ToString("0.00", CultureInfo.InvariantCulture)).Append('\n');
-        builder.Append("GiftCard,").Append(giftCard.ToString("0.00", CultureInfo.InvariantCulture)).Append('\n');
+        builder.Append("Physical,").Append(totals.Physical.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)).Append('\n');
+        builder.Append("Digital,").Append(totals.Digital.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)).Append('\n');
+        builder.Append("GiftCard,").Append(totals.GiftCard.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)).Append('\n');
         return builder.ToString();
     }
 }
\ No newline at end of file
diff --git a/baseline/src/CornerStore/Reporting/JsonReportExporter.cs b/pattern/src/CornerStore/Reporting/JsonReportExporter.cs
index 028f70e..8dca46b 100644
--- a/baseline/src/CornerStore/Reporting/JsonReportExporter.cs
+++ b/pattern/src/CornerStore/Reporting/JsonReportExporter.cs
@@ -1,43 +1,34 @@
-using System.Globalization;
 using System.Text;

 namespace CornerStore;

-internal class JsonReportExporter
+internal sealed class JsonReportExporter : ReportExporter
 {
-    public string Export(List<Order> source, string status)
+    protected override void Accumulate(ReportTotals totals, CartLine line)
     {
-        // SMELL S22: JSON export duplicates the load, filter, aggregate and write skeleton
-        var orders = source.Where(order => status == "all" || order.Status == status).ToList();
-        var physical = 0m;
-        var digital = 0m;
-        var giftCard = 0m;
-        foreach (var order in orders)
+        var amount = line.UnitPrice * line.Quantity;
+        // SMELL S23: JSON rows switch on each concrete line item type
+        switch (line.Product)
         {
-            foreach (var line in order.Lines)
-            {
-                var amount = line.UnitPrice * line.Quantity;
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
-            }
+            case PhysicalProduct:
+                totals.Physical += amount;
+                break;
+            case DigitalProduct:
+                totals.Digital += amount;
+                break;
+            case GiftCardProduct:
+                totals.GiftCard += amount;
+                break;
         }
+    }

+    protected override string WriteReport(ReportTotals totals)
+    {
         var builder = new StringBuilder();
         builder.Append("{\n  \"totals\": [\n");
-        builder.Append("    { \"type\": \"Physical\", \"total\": ").Append(physical.ToString("0.00", CultureInfo.InvariantCulture)).Append(" },\n");
-        builder.Append("    { \"type\": \"Digital\", \"total\": ").Append(digital.ToString("0.00", CultureInfo.InvariantCulture)).Append(" },\n");
-        builder.Append("    { \"type\": \"GiftCard\", \"total\": ").Append(giftCard.ToString("0.00", CultureInfo.InvariantCulture)).Append(" }\n");
+        builder.Append("    { \"type\": \"Physical\", \"total\": ").Append(totals.Physical.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)).Append(" },\n");
+        builder.Append("    { \"type\": \"Digital\", \"total\": ").Append(totals.Digital.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)).Append(" },\n");
+        builder.Append("    { \"type\": \"GiftCard\", \"total\": ").Append(totals.GiftCard.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)).Append(" }\n");
         builder.Append("  ]\n}\n");
         return builder.ToString();
     }
diff --git a/pattern/src/CornerStore/Reporting/ReportExporter.cs b/pattern/src/CornerStore/Reporting/ReportExporter.cs
new file mode 100644
index 0000000..69d5452
--- /dev/null
+++ b/pattern/src/CornerStore/Reporting/ReportExporter.cs
@@ -0,0 +1,23 @@
+namespace CornerStore;
+
+internal abstract class ReportExporter
+{
+    public string Export(List<Order> source, string status)
+    {
+        var orders = source.Where(order => status == "all" || order.Status == status).ToList();
+        var totals = new ReportTotals();
+        foreach (var order in orders)
+        {
+            foreach (var line in order.Lines)
+            {
+                Accumulate(totals, line);
+            }
+        }
+
+        return WriteReport(totals);
+    }
+
+    protected abstract void Accumulate(ReportTotals totals, CartLine line);
+
+    protected abstract string WriteReport(ReportTotals totals);
+}
\ No newline at end of file
diff --git a/pattern/src/CornerStore/Reporting/ReportTotals.cs b/pattern/src/CornerStore/Reporting/ReportTotals.cs
new file mode 100644
index 0000000..111879c
--- /dev/null
+++ b/pattern/src/CornerStore/Reporting/ReportTotals.cs
@@ -0,0 +1,10 @@
+namespace CornerStore;
+
+internal sealed class ReportTotals
+{
+    public decimal Physical { get; set; }
+
+    public decimal Digital { get; set; }
+
+    public decimal GiftCard { get; set; }
+}
\ No newline at end of file
diff --git a/pattern/tests/CornerStore.Tests/TemplateMethodTests.cs b/pattern/tests/CornerStore.Tests/TemplateMethodTests.cs
new file mode 100644
index 0000000..ae73a03
--- /dev/null
+++ b/pattern/tests/CornerStore.Tests/TemplateMethodTests.cs
@@ -0,0 +1,87 @@
+using System.Globalization;
+
+namespace CornerStore.Tests;
+
+public class TemplateMethodTests
+{
+    [Fact]
+    public void GivenOrdersWithDifferentStatuses_Export_FiltersBeforeAccumulating()
+    {
+        var paidOrder = CreateOrder("Paid", new PhysicalProduct(), 10m);
+        var shippedOrder = CreateOrder("Shipped", new DigitalProduct(), 20m);
+
+        var csv = new CsvReportExporter().Export([paidOrder, shippedOrder], "Paid");
+
+        Assert.Contains("Physical,10.00\n", csv);
+        Assert.Contains("Digital,0.00\n", csv);
+    }
+
+    [Fact]
+    public void GivenCsvExporter_Export_WritesCsvStructure()
+    {
+        var order = CreateOrder("Paid", new GiftCardProduct(), 25m);
+
+        var report = new CsvReportExporter().Export([order], "all");
+
+        Assert.StartsWith("type,total\n", report);
+        Assert.Contains("GiftCard,25.00\n", report);
+    }
+
+    [Fact]
+    public void GivenJsonExporter_Export_WritesJsonStructure()
+    {
+        var order = CreateOrder("Paid", new DigitalProduct(), 15m);
+
+        var report = new JsonReportExporter().Export([order], "all");
+
+        Assert.StartsWith("{\n  \"totals\": [\n", report);
+        Assert.Contains("{ \"type\": \"Digital\", \"total\": 15.00 }", report);
+        Assert.EndsWith("  ]\n}\n", report);
+    }
+
+    [Fact]
+    public void GivenCustomExporter_Export_UsesSubclassHooks()
+    {
+        var order = CreateOrder("Paid", new PhysicalProduct(), 8.25m);
+
+        var report = new TestReportExporter().Export([order], "all");
+
+        Assert.Equal("custom:8.25", report);
+    }
+
+    private static Order CreateOrder(string status, Product product, decimal price)
+    {
+        var customer = new Customer
+        {
+            Id = "cu-1",
+            Number = 1,
+            Name = "Ava",
+            Region = "AU",
+            Channel = "email",
+            Address = "1 Harbour St",
+            CreditLimit = 500m,
+        };
+        var line = new CartLine
+        {
+            Product = product,
+            Quantity = 1,
+            UnitPrice = price,
+        };
+        return new Order("o-1001", customer, status, [line], new Quote(), "card", null, null, null, null, null);
+    }
+
+    private sealed class TestReportExporter : ReportExporter
+    {
+        private decimal _total;
+
+        protected override void Accumulate(ReportTotals totals, CartLine line)
+        {
+            _total += line.UnitPrice * line.Quantity;
+        }
+
+        protected override string WriteReport(ReportTotals totals)
+        {
+            return "custom:" + _total.ToString("0.00", CultureInfo.InvariantCulture);
+        }
+    }
+}
\ No newline at end of file
```
