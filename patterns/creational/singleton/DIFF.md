# DIFF: singleton

## File summary

| Status | File | Added | Removed |
| --- | --- | ---: | ---: |
| Modified | CornerStore/Catalog/Catalog.cs | 2 | 2 |
| Modified | CornerStore/Checkout/AfterpayProcessor.cs | 1 | 1 |
| Modified | CornerStore/Checkout/CardProcessor.cs | 1 | 1 |
| Modified | CornerStore/Checkout/FraudScorer.cs | 1 | 1 |
| Modified | CornerStore/Checkout/OrderValidator.cs | 1 | 1 |
| Modified | CornerStore/Checkout/PayPalProcessor.cs | 1 | 1 |
| Modified | CornerStore/Checkout/ShipFastClient.cs | 1 | 1 |
| Modified | CornerStore/Config.cs | 78 | 9 |
| Modified | CornerStore/CornerStore.csproj | 4 | 0 |
| Modified | CornerStore/Log.cs | 32 | 6 |
| Modified | CornerStore/Notifications/EmailSender.cs | 1 | 1 |
| Modified | CornerStore/Notifications/SmsSender.cs | 1 | 1 |
| Modified | CornerStore/Orders/AnalyticsService.cs | 1 | 1 |
| Modified | CornerStore/Orders/LoyaltyService.cs | 1 | 1 |
| Modified | CornerStore/ScenarioRunner.cs | 7 | 6 |
| Modified | CornerStore/StoreConsole.cs | 55 | 56 |
| Modified | CornerStore/Widgets/ItemListWidget.cs | 1 | 1 |
| Modified | CornerStore/Widgets/PromoBoxWidget.cs | 1 | 1 |
| Modified | CornerStore/Widgets/ShippingSelectorWidget.cs | 1 | 1 |
| Modified | CornerStore/Widgets/TotalsPanelWidget.cs | 8 | 8 |
| Modified | CornerStore.Tests/AssemblyAttributes.cs | 1 | 1 |
| Added | CornerStore.Tests/SingletonTests.cs | 77 | 0 |

## Full diff

```diff
diff --git a/baseline/src/CornerStore/Catalog/Catalog.cs b/pattern/src/CornerStore/Catalog/Catalog.cs
index 2131b22..a8c718d 100644
--- a/baseline/src/CornerStore/Catalog/Catalog.cs
+++ b/pattern/src/CornerStore/Catalog/Catalog.cs
@@ -162,10 +162,10 @@ internal class Catalog
     {
         // SMELL S16: manual recursion over the category tree, first copy
         var pad = new string(' ', depth * 2);
-        Log.Info(pad + category.Name);
+        Log.Instance.Info(pad + category.Name);
         foreach (var product in category.Products)
         {
-            Log.Info(pad + "  - " + product.Sku + " " + product.Name + " " + FormatPrice(PriceOf(product.Sku)));
+            Log.Instance.Info(pad + "  - " + product.Sku + " " + product.Name + " " + FormatPrice(PriceOf(product.Sku)));
         }
         foreach (var child in category.Children)
diff --git a/baseline/src/CornerStore/Checkout/AfterpayProcessor.cs b/pattern/src/CornerStore/Checkout/AfterpayProcessor.cs
index 6741e37..8cf3018 100644
--- a/baseline/src/CornerStore/Checkout/AfterpayProcessor.cs
+++ b/pattern/src/CornerStore/Checkout/AfterpayProcessor.cs
@@ -4,6 +4,6 @@ internal class AfterpayProcessor : IPaymentProcessor
 {
     public void Charge(Customer customer, decimal amount)
     {
-        Log.Info("Afterpay charged " + MoneyFormatter.Format(amount, customer.Region) + " for " + customer.Name);
+        Log.Instance.Info("Afterpay charged " + MoneyFormatter.Format(amount, customer.Region) + " for " + customer.Name);
     }
 }
\ No newline at end of file
diff --git a/baseline/src/CornerStore/Checkout/CardProcessor.cs b/pattern/src/CornerStore/Checkout/CardProcessor.cs
index 47205c3..e1a0701 100644
--- a/baseline/src/CornerStore/Checkout/CardProcessor.cs
+++ b/pattern/src/CornerStore/Checkout/CardProcessor.cs
@@ -4,6 +4,6 @@ internal class CardProcessor : IPaymentProcessor
 {
     public void Charge(Customer customer, decimal amount)
     {
-        Log.Info("card charged " + MoneyFormatter.Format(amount, customer.Region) + " for " + customer.Name);
+        Log.Instance.Info("card charged " + MoneyFormatter.Format(amount, customer.Region) + " for " + customer.Name);
     }
 }
\ No newline at end of file
diff --git a/baseline/src/CornerStore/Checkout/FraudScorer.cs b/pattern/src/CornerStore/Checkout/FraudScorer.cs
index 02ab7a0..6f576eb 100644
--- a/baseline/src/CornerStore/Checkout/FraudScorer.cs
+++ b/pattern/src/CornerStore/Checkout/FraudScorer.cs
@@ -4,6 +4,6 @@ internal class FraudScorer
 {
     public int Score(Customer customer)
     {
-        return new Random(Config.Seed + customer.Number).Next(0, 100);
+        return new Random(Config.Instance.Seed + customer.Number).Next(0, 100);
     }
 }
\ No newline at end of file
diff --git a/baseline/src/CornerStore/Checkout/OrderValidator.cs b/pattern/src/CornerStore/Checkout/OrderValidator.cs
index e7357c9..4d0c09b 100644
--- a/baseline/src/CornerStore/Checkout/OrderValidator.cs
+++ b/pattern/src/CornerStore/Checkout/OrderValidator.cs
@@ -32,7 +32,7 @@ internal class OrderValidator(InventoryService inventory, FraudScorer fraudScore
             else
             {
                 var fraudScore = fraudScorer.Score(customer);
-                if (fraudScore > Config.FraudThreshold)
+                if (fraudScore > Config.Instance.FraudThreshold)
                 {
                     error = "fraud score " + fraudScore.ToString(CultureInfo.InvariantCulture) + " exceeds threshold";
                 }
diff --git a/baseline/src/CornerStore/Checkout/PayPalProcessor.cs b/pattern/src/CornerStore/Checkout/PayPalProcessor.cs
index 42ecf01..ae6020e 100644
--- a/baseline/src/CornerStore/Checkout/PayPalProcessor.cs
+++ b/pattern/src/CornerStore/Checkout/PayPalProcessor.cs
@@ -4,6 +4,6 @@ internal class PayPalProcessor : IPaymentProcessor
 {
     public void Charge(Customer customer, decimal amount)
     {
-        Log.Info("PayPal charged " + MoneyFormatter.Format(amount, customer.Region) + " for " + customer.Name);
+        Log.Instance.Info("PayPal charged " + MoneyFormatter.Format(amount, customer.Region) + " for " + customer.Name);
     }
 }
\ No newline at end of file
diff --git a/baseline/src/CornerStore/Checkout/ShipFastClient.cs b/pattern/src/CornerStore/Checkout/ShipFastClient.cs
index 1172e8b..111bbc1 100644
--- a/baseline/src/CornerStore/Checkout/ShipFastClient.cs
+++ b/pattern/src/CornerStore/Checkout/ShipFastClient.cs
@@ -11,7 +11,7 @@ internal class ShipFastClient(IdSequence ids)
         var orderId = request.Element("orderId")?.Value ?? "";
         var cents = request.Element("amountCents")?.Value ?? "0";
         var tracking = ids.NextShipmentId();
-        Log.Info("ShipFast booked " + tracking + " for " + orderId + " (" + cents + " cents)");
+        Log.Instance.Info("ShipFast booked " + tracking + " for " + orderId + " (" + cents + " cents)");
         return "<shipment><tracking>" + tracking + "</tracking></shipment>";
     }
 }
\ No newline at end of file
diff --git a/baseline/src/CornerStore/Config.cs b/pattern/src/CornerStore/Config.cs
index ec873a4..15dbe25 100644
--- a/baseline/src/CornerStore/Config.cs
+++ b/pattern/src/CornerStore/Config.cs
@@ -1,16 +1,85 @@
 namespace CornerStore;
-// SMELL S05: global settings in a static class with public mutable fields, set from anywhere
-internal static class Config
+internal sealed class Config
 {
-    public static int Seed = 0;
-    public static int FraudThreshold = 80;
-    public static int LowStockLevel = 3;
+    private static readonly Lazy<Config> singleton =
+        new(static () => new Config(), LazyThreadSafetyMode.ExecutionAndPublication);
-    public static void Reset()
+    private readonly object sync = new();
+    private int seed;
+    private int fraudThreshold = 80;
+    private int lowStockLevel = 3;
+
+    private Config()
+    {
+    }
+
+    public static Config Instance => singleton.Value;
+
+    public int Seed
+    {
+        get
+        {
+            lock (sync)
+            {
+                return seed;
+            }
+        }
+    }
+
+    public int FraudThreshold
+    {
+        get
+        {
+            lock (sync)
+            {
+                return fraudThreshold;
+            }
+        }
+    }
+
+    public int LowStockLevel
+    {
+        get
+        {
+            lock (sync)
+            {
+                return lowStockLevel;
+            }
+        }
+    }
+
+    public void SetSeed(int value)
+    {
+        lock (sync)
+        {
+            seed = value;
+        }
+    }
+
+    public void SetFraudThreshold(int value)
+    {
+        lock (sync)
+        {
+            fraudThreshold = value;
+        }
+    }
+
+    public void SetLowStockLevel(int value)
+    {
+        lock (sync)
+        {
+            lowStockLevel = value;
+        }
+    }
+
+    public void Reset()
     {
-        Seed = 0;
-        FraudThreshold = 80;
-        LowStockLevel = 3;
+        lock (sync)
+        {
+            seed = 0;
+            fraudThreshold = 80;
+            lowStockLevel = 3;
+        }
     }
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
diff --git a/baseline/src/CornerStore/Log.cs b/pattern/src/CornerStore/Log.cs
index b90aa8a..cfed22f 100644
--- a/baseline/src/CornerStore/Log.cs
+++ b/pattern/src/CornerStore/Log.cs
@@ -1,16 +1,42 @@
 namespace CornerStore;
-// SMELL S05: global transcript writer in a static class with a public mutable field
-internal static class Log
+internal sealed class Log
 {
-    public static TextWriter Out = TextWriter.Null;
+    private static readonly Lazy<Log> singleton =
+        new(static () => new Log(), LazyThreadSafetyMode.ExecutionAndPublication);
-    public static void Line(string text)
+    private readonly object sync = new();
+    private TextWriter output = TextWriter.Null;
+
+    private Log()
+    {
+    }
+
+    public static Log Instance => singleton.Value;
+
+    public void SetOutput(TextWriter writer)
+    {
+        ArgumentNullException.ThrowIfNull(writer);
+        lock (sync)
+        {
+            output = writer;
+        }
+    }
+
+    public void ResetOutput()
+    {
+        SetOutput(TextWriter.Null);
+    }
+
+    public void Line(string text)
     {
-        Out.Write(text + "\n");
+        lock (sync)
+        {
+            output.Write(text + "\n");
+        }
     }
-    public static void Info(string text)
+    public void Info(string text)
     {
         Line("  " + text);
     }
diff --git a/baseline/src/CornerStore/Notifications/EmailSender.cs b/pattern/src/CornerStore/Notifications/EmailSender.cs
index 4a1ea8d..5e578d1 100644
--- a/baseline/src/CornerStore/Notifications/EmailSender.cs
+++ b/pattern/src/CornerStore/Notifications/EmailSender.cs
@@ -4,6 +4,6 @@ internal class EmailSender
 {
     public void Send(string recipient, string message)
     {
-        Log.Info("email to " + recipient + ": " + message);
+        Log.Instance.Info("email to " + recipient + ": " + message);
     }
 }
\ No newline at end of file
diff --git a/baseline/src/CornerStore/Notifications/SmsSender.cs b/pattern/src/CornerStore/Notifications/SmsSender.cs
index ec5eb12..2792a0d 100644
--- a/baseline/src/CornerStore/Notifications/SmsSender.cs
+++ b/pattern/src/CornerStore/Notifications/SmsSender.cs
@@ -4,6 +4,6 @@ internal class SmsSender
 {
     public void Send(string recipient, string message)
     {
-        Log.Info("sms to " + recipient + ": " + message);
+        Log.Instance.Info("sms to " + recipient + ": " + message);
     }
 }
\ No newline at end of file
diff --git a/baseline/src/CornerStore/Orders/AnalyticsService.cs b/pattern/src/CornerStore/Orders/AnalyticsService.cs
index 7953c93..a4fa83e 100644
--- a/baseline/src/CornerStore/Orders/AnalyticsService.cs
+++ b/pattern/src/CornerStore/Orders/AnalyticsService.cs
@@ -4,6 +4,6 @@ internal class AnalyticsService
 {
     public void RecordStatusChange(Order order)
     {
-        Log.Info("analytics order " + order.Id + " " + order.Status);
+        Log.Instance.Info("analytics order " + order.Id + " " + order.Status);
     }
 }
\ No newline at end of file
diff --git a/baseline/src/CornerStore/Orders/LoyaltyService.cs b/pattern/src/CornerStore/Orders/LoyaltyService.cs
index 079bd15..34fc8f6 100644
--- a/baseline/src/CornerStore/Orders/LoyaltyService.cs
+++ b/pattern/src/CornerStore/Orders/LoyaltyService.cs
@@ -4,6 +4,6 @@ internal class LoyaltyService
 {
     public void RecordStatusChange(Order order)
     {
-        Log.Info("loyalty updated for " + order.Customer.Name + " (" + order.Status + ")");
+        Log.Instance.Info("loyalty updated for " + order.Customer.Name + " (" + order.Status + ")");
     }
 }
\ No newline at end of file
diff --git a/baseline/src/CornerStore/ScenarioRunner.cs b/pattern/src/CornerStore/ScenarioRunner.cs
index 21d8c7c..4e44487 100644
--- a/baseline/src/CornerStore/ScenarioRunner.cs
+++ b/pattern/src/CornerStore/ScenarioRunner.cs
@@ -8,10 +8,11 @@ public static class ScenarioRunner
         var clock = new ScenarioClock(scenario.StartTime);
         var console = new StoreConsole(clock);
-        // SMELL S05: the runner sets the global Config and Log fields directly
-        Config.Reset();
-        Config.Seed = scenario.Seed;
-        Log.Out = output;
+        var config = Config.Instance;
+        config.Reset();
+        config.SetSeed(scenario.Seed);
+        var logger = Log.Instance;
+        logger.SetOutput(output);
         try
         {
@@ -20,7 +21,7 @@ public static class ScenarioRunner
                 console.ApplyConfig(key, value);
             }
-            Log.Line("CornerStore trading day: " + scenario.Name + " (start " + clock.Stamp() + ")");
+            logger.Line("CornerStore trading day: " + scenario.Name + " (start " + clock.Stamp() + ")");
             for (var i = 0; i < scenario.Steps.Count; i++)
             {
                 console.Handle(i + 1, scenario.Steps[i]);
@@ -28,7 +29,7 @@ public static class ScenarioRunner
         }
         finally
         {
-            Log.Out = TextWriter.Null;
+            logger.ResetOutput();
         }
     }
 }
diff --git a/baseline/src/CornerStore/StoreConsole.cs b/pattern/src/CornerStore/StoreConsole.cs
index a288b4e..bf55025 100644
--- a/baseline/src/CornerStore/StoreConsole.cs
+++ b/pattern/src/CornerStore/StoreConsole.cs
@@ -58,7 +58,7 @@ internal class StoreConsole
     public void Handle(int number, JsonElement step)
     {
         var op = step.TryGetProperty("op", out var opValue) ? opValue.GetString() ?? "" : "";
-        Log.Line("[" + number.ToString("D3", CultureInfo.InvariantCulture) + "] " + op + Describe(step));
+        Log.Instance.Line("[" + number.ToString("D3", CultureInfo.InvariantCulture) + "] " + op + Describe(step));
         switch (op)
         {
@@ -138,21 +138,20 @@ internal class StoreConsole
                 ReportJson(step);
                 break;
             default:
-                Log.Info("unknown op");
+                Log.Instance.Info("unknown op");
                 break;
         }
     }
     public bool ApplyConfig(string key, int value)
     {
-        // SMELL S05: any caller can overwrite the global Config fields
         switch (key)
         {
             case "fraudThreshold":
-                Config.FraudThreshold = value;
+                Config.Instance.SetFraudThreshold(value);
                 return true;
             case "lowStockLevel":
-                Config.LowStockLevel = value;
+                Config.Instance.SetLowStockLevel(value);
                 return true;
             default:
                 return false;
@@ -187,12 +186,12 @@ internal class StoreConsole
     {
         if (!step.TryGetProperty("minutes", out var minutes) || !minutes.TryGetInt32(out var value))
         {
-            Log.Info("advance needs minutes");
+            Log.Instance.Info("advance needs minutes");
             return;
         }
         clock.Advance(TimeSpan.FromMinutes(value));
-        Log.Info("clock " + clock.Stamp());
+        Log.Instance.Info("clock " + clock.Stamp());
     }
     private void Browse(JsonElement step)
@@ -201,12 +200,12 @@ internal class StoreConsole
         var category = catalog.FindCategory(name);
         if (category is null)
         {
-            Log.Info("unknown category " + name);
+            Log.Instance.Info("unknown category " + name);
             return;
         }
         catalog.PrintTree(category, 0);
-        Log.Info(catalog.CountProducts(category).ToString(CultureInfo.InvariantCulture) + " products under " + name);
+        Log.Instance.Info(catalog.CountProducts(category).ToString(CultureInfo.InvariantCulture) + " products under " + name);
     }
     private void Variant(JsonElement step)
@@ -214,14 +213,14 @@ internal class StoreConsole
         var parent = Text(step, "parent");
         var sku = Text(step, "sku");
         var error = catalog.CreateVariant(parent, sku, Text(step, "size"), Text(step, "colour"));
-        Log.Info(error is null ? "created variant " + sku + " of " + parent : "rejected: " + error);
+        Log.Instance.Info(error is null ? "created variant " + sku + " of " + parent : "rejected: " + error);
     }
     private void BundleCreate(JsonElement step)
     {
         var sku = Text(step, "sku");
         var error = catalog.CreateBundle(sku, Text(step, "name"));
-        Log.Info(error is null ? "created bundle " + sku : "rejected: " + error);
+        Log.Instance.Info(error is null ? "created bundle " + sku : "rejected: " + error);
     }
     private void BundleAdd(JsonElement step)
@@ -231,12 +230,12 @@ internal class StoreConsole
         var error = catalog.AddToBundle(bundle, sku);
         if (error is not null)
         {
-            Log.Info("rejected: " + error);
+            Log.Instance.Info("rejected: " + error);
             return;
         }
-        Log.Info("added " + sku + " to " + bundle);
-        Log.Info("bundle " + bundle + " price " + Catalog.FormatPrice(catalog.PriceOf(bundle)));
+        Log.Instance.Info("added " + sku + " to " + bundle);
+        Log.Instance.Info("bundle " + bundle + " price " + Catalog.FormatPrice(catalog.PriceOf(bundle)));
     }
     private void Import(JsonElement step)
@@ -245,7 +244,7 @@ internal class StoreConsole
         var skus = new List<string>();
         catalog.CollectSkus(catalog.Root, skus);
         listings = importer.Import(skus, count);
-        Log.Info("imported " + listings.Count.ToString(CultureInfo.InvariantCulture) + " listings");
+        Log.Instance.Info("imported " + listings.Count.ToString(CultureInfo.InvariantCulture) + " listings");
     }
     private void Register(JsonElement step)
@@ -276,7 +275,7 @@ internal class StoreConsole
         shippingSelectorWidget.Connect(itemListWidget, promoBoxWidget, totalsPanelWidget);
         totalsPanelWidget.Connect(itemListWidget, promoBoxWidget, shippingSelectorWidget);
         itemListWidgets[id] = itemListWidget;
-        Log.Info("registered " + id + " " + customer.Name + " (" + customer.Region + ", " + customer.Channel + ")");
+        Log.Instance.Info("registered " + id + " " + customer.Name + " (" + customer.Region + ", " + customer.Channel + ")");
     }
     private Cart? FindCart(JsonElement step)
@@ -284,7 +283,7 @@ internal class StoreConsole
         var id = Text(step, "customer");
         if (!customers.ContainsKey(id))
         {
-            Log.Info("rejected: unknown customer " + id);
+            Log.Instance.Info("rejected: unknown customer " + id);
             return null;
         }
@@ -296,7 +295,7 @@ internal class StoreConsole
         var id = Text(step, "customer");
         if (!customers.TryGetValue(id, out var customer))
         {
-            Log.Info("rejected: unknown customer " + id);
+            Log.Instance.Info("rejected: unknown customer " + id);
             return null;
         }
@@ -309,7 +308,7 @@ internal class StoreConsole
         var order = orderService.Find(id);
         if (order is null)
         {
-            Log.Info("rejected: unknown order " + id);
+            Log.Instance.Info("rejected: unknown order " + id);
         }
         return order;
@@ -324,7 +323,7 @@ internal class StoreConsole
         }
         var error = orderService.Ship(order);
-        Log.Info(error is null ? "order " + order.Id + " shipped" : "rejected: " + error);
+        Log.Instance.Info(error is null ? "order " + order.Id + " shipped" : "rejected: " + error);
     }
     private void OrderDeliver(JsonElement step)
@@ -336,7 +335,7 @@ internal class StoreConsole
         }
         var error = orderService.Deliver(order);
-        Log.Info(error is null ? "order " + order.Id + " delivered" : "rejected: " + error);
+        Log.Instance.Info(error is null ? "order " + order.Id + " delivered" : "rejected: " + error);
     }
     private void AdminStockAdjust(JsonElement step)
@@ -349,7 +348,7 @@ internal class StoreConsole
         var sku = Text(step, "sku");
         var amount = step.TryGetProperty("delta", out var value) && value.TryGetInt32(out var number) ? number : 0;
-        Log.Info(backOffice.AdjustStock(customer, sku, amount));
+        Log.Instance.Info(backOffice.AdjustStock(customer, sku, amount));
     }
     private void AdminStockLookup(JsonElement step)
@@ -360,7 +359,7 @@ internal class StoreConsole
             return;
         }
-        Log.Info(backOffice.LookupStock(customer, Text(step, "sku")));
+        Log.Instance.Info(backOffice.LookupStock(customer, Text(step, "sku")));
     }
     private void AdminRefund(JsonElement step)
@@ -372,7 +371,7 @@ internal class StoreConsole
             return;
         }
-        Log.Info(backOffice.Refund(customer, order));
+        Log.Instance.Info(backOffice.Refund(customer, order));
     }
     private void AdminCancel(JsonElement step)
@@ -384,7 +383,7 @@ internal class StoreConsole
             return;
         }
-        Log.Info(backOffice.Cancel(customer, order));
+        Log.Instance.Info(backOffice.Cancel(customer, order));
     }
     // SMELL S10: the console coordinates every checkout subsystem in a fixed order
@@ -398,7 +397,7 @@ internal class StoreConsole
         if (cart.Lines.Count == 0)
         {
-            Log.Info("checkout rejected: cart is empty");
+            Log.Instance.Info("checkout rejected: cart is empty");
             return;
         }
@@ -407,14 +406,14 @@ internal class StoreConsole
         var error = orderValidator.Validate(customer, cart, quote);
         if (error is not null)
         {
-            Log.Info("checkout rejected: " + error);
+            Log.Instance.Info("checkout rejected: " + error);
             return;
         }
         var paymentType = Text(step, "payment");
         if (!ChargePayment(paymentType, customer, quote.Total))
         {
-            Log.Info("checkout rejected: unknown payment type " + paymentType);
+            Log.Instance.Info("checkout rejected: unknown payment type " + paymentType);
             return;
         }
@@ -429,7 +428,7 @@ internal class StoreConsole
                     inventory.AdjustStock(reservedLine.Product.Sku, reservedLine.Quantity);
                 }
-                Log.Info("checkout rejected: stock changed for " + line.Product.Sku);
+                Log.Instance.Info("checkout rejected: stock changed for " + line.Product.Sku);
                 return;
             }
@@ -447,11 +446,11 @@ internal class StoreConsole
             }
             order.StockReserved = false;
-            Log.Info("checkout rejected: " + payError);
+            Log.Instance.Info("checkout rejected: " + payError);
             return;
         }
-        Log.Info("checkout placed " + order.Id);
+        Log.Instance.Info("checkout placed " + order.Id);
     }
     // SMELL S03: payment processor construction is selected by a switch in checkout
@@ -497,10 +496,10 @@ internal class StoreConsole
     {
         var status = Text(step, "status");
         status = status == "" ? "all" : status;
-        Log.Info("CSV report (" + status + ")");
+        Log.Instance.Info("CSV report (" + status + ")");
         foreach (var row in reports.ExportCsv(status).Split('\n', StringSplitOptions.RemoveEmptyEntries))
         {
-            Log.Info(row);
+            Log.Instance.Info(row);
         }
     }
@@ -508,10 +507,10 @@ internal class StoreConsole
     {
         var status = Text(step, "status");
         status = status == "" ? "all" : status;
-        Log.Info("JSON report (" + status + ")");
+        Log.Instance.Info("JSON report (" + status + ")");
         foreach (var row in reports.ExportJson(status).Split('\n', StringSplitOptions.RemoveEmptyEntries))
         {
-            Log.Info(row);
+            Log.Instance.Info(row);
         }
     }
@@ -537,14 +536,14 @@ internal class StoreConsole
         var error = itemListWidget.Add(sku, quantity);
         if (error is not null)
         {
-            Log.Info("rejected: " + error);
+            Log.Instance.Info("rejected: " + error);
             return;
         }
         Remember(cart.CustomerId, lines, promo, shipping, giftWrap, insurance);
         itemListWidget.RefreshAfterChange();
-        Log.Info("added " + quantity.ToString(CultureInfo.InvariantCulture) + " x " + sku);
-        Log.Info(Summary(cart));
+        Log.Instance.Info("added " + quantity.ToString(CultureInfo.InvariantCulture) + " x " + sku);
+        Log.Instance.Info(Summary(cart));
     }
     private void CartRemove(JsonElement step)
@@ -568,14 +567,14 @@ internal class StoreConsole
         var error = itemListWidget.Remove(sku);
         if (error is not null)
         {
-            Log.Info("rejected: " + error);
+            Log.Instance.Info("rejected: " + error);
             return;
         }
         Remember(cart.CustomerId, lines, promo, shipping, giftWrap, insurance);
         itemListWidget.RefreshAfterChange();
-        Log.Info("removed " + sku);
-        Log.Info(Summary(cart));
+        Log.Instance.Info("removed " + sku);
+        Log.Instance.Info(Summary(cart));
     }
     private void CartUndo(JsonElement step)
@@ -588,7 +587,7 @@ internal class StoreConsole
         if (!savedLines.TryGetValue(cart.CustomerId, out var lines))
         {
-            Log.Info("nothing to undo");
+            Log.Instance.Info("nothing to undo");
             return;
         }
@@ -605,8 +604,8 @@ internal class StoreConsole
         savedGiftWrap.Remove(cart.CustomerId);
         savedInsurance.Remove(cart.CustomerId);
         itemListWidgets[cart.CustomerId].RefreshAfterChange();
-        Log.Info("undone");
-        Log.Info(Summary(cart));
+        Log.Instance.Info("undone");
+        Log.Instance.Info(Summary(cart));
     }
     private void CartShow(JsonElement step)
@@ -618,7 +617,7 @@ internal class StoreConsole
         }
         itemListWidgets[cart.CustomerId].RefreshAfterChange();
-        Log.Info(Summary(cart));
+        Log.Instance.Info(Summary(cart));
     }
     private void CartAddOn(JsonElement step)
@@ -643,12 +642,12 @@ internal class StoreConsole
         }
         else
         {
-            Log.Info("rejected: unknown add-on " + name);
+            Log.Instance.Info("rejected: unknown add-on " + name);
             return;
         }
         itemListWidgets[cart.CustomerId].TotalsPanel.RefreshAfterChange();
-        Log.Info(name + (on ? " added" : " removed"));
+        Log.Instance.Info(name + (on ? " added" : " removed"));
     }
     private void CartPromo(JsonElement step)
@@ -663,19 +662,19 @@ internal class StoreConsole
         var promoBoxWidget = itemListWidgets[cart.CustomerId].PromoBox;
         if (!promoBoxWidget.IsKnown(code))
         {
-            Log.Info("rejected: unknown promo " + code);
+            Log.Instance.Info("rejected: unknown promo " + code);
             return;
         }
         if (!promoBoxWidget.Evaluate(code))
         {
-            Log.Info("rejected: promo " + code + " does not apply to this cart");
+            Log.Instance.Info("rejected: promo " + code + " does not apply to this cart");
             return;
         }
         promoBoxWidget.Apply(code);
         promoBoxWidget.RefreshAfterChange();
-        Log.Info("promo " + code + " applied (" + promos.RuleOf(code) + ")");
+        Log.Instance.Info("promo " + code + " applied (" + promos.RuleOf(code) + ")");
     }
     private void CartShipping(JsonElement step)
@@ -690,13 +689,13 @@ internal class StoreConsole
         var shippingSelectorWidget = itemListWidgets[cart.CustomerId].ShippingSelector;
         if (!shippingSelectorWidget.CanSelect(method))
         {
-            Log.Info("rejected: unknown shipping method " + method);
+            Log.Instance.Info("rejected: unknown shipping method " + method);
             return;
         }
         shippingSelectorWidget.Select(method);
         shippingSelectorWidget.RefreshAfterChange();
-        Log.Info("shipping set to " + method);
+        Log.Instance.Info("shipping set to " + method);
     }
     private void ShowQuote(JsonElement step)
@@ -709,7 +708,7 @@ internal class StoreConsole
         if (cart.Lines.Count == 0)
         {
-            Log.Info("rejected: cart is empty");
+            Log.Instance.Info("rejected: cart is empty");
             return;
         }
@@ -755,18 +754,18 @@ internal class StoreConsole
             || !step.TryGetProperty("value", out var value)
             || !value.TryGetInt32(out var number))
         {
-            Log.Info("config.set needs key and value");
+            Log.Instance.Info("config.set needs key and value");
             return;
         }
         var name = key.GetString() ?? "";
         if (ApplyConfig(name, number))
         {
-            Log.Info(name + " set to " + number.ToString(CultureInfo.InvariantCulture));
+            Log.Instance.Info(name + " set to " + number.ToString(CultureInfo.InvariantCulture));
         }
         else
         {
-            Log.Info("unknown setting " + name);
+            Log.Instance.Info("unknown setting " + name);
         }
     }
 }
diff --git a/baseline/src/CornerStore/Widgets/ItemListWidget.cs b/pattern/src/CornerStore/Widgets/ItemListWidget.cs
index cf06126..e5ba835 100644
--- a/baseline/src/CornerStore/Widgets/ItemListWidget.cs
+++ b/pattern/src/CornerStore/Widgets/ItemListWidget.cs
@@ -37,7 +37,7 @@ internal class ItemListWidget(Cart cart, Customer customer, CartService cartServ
         {
             var origin = line.FromBundle == "" ? "" : " (from " + line.FromBundle + ")";
             var amount = line.UnitPrice * line.Quantity;
-            Log.Info("- " + line.Product.Sku + " " + line.Product.Name + " x" + line.Quantity.ToString(CultureInfo.InvariantCulture)
+            Log.Instance.Info("- " + line.Product.Sku + " " + line.Product.Name + " x" + line.Quantity.ToString(CultureInfo.InvariantCulture)
                 + " @ " + MoneyFormatter.Format(line.UnitPrice, customer.Region) + " = "
                 + MoneyFormatter.Format(amount, customer.Region) + origin);
         }
diff --git a/baseline/src/CornerStore/Widgets/PromoBoxWidget.cs b/pattern/src/CornerStore/Widgets/PromoBoxWidget.cs
index bbe8b9b..2e2a7a4 100644
--- a/baseline/src/CornerStore/Widgets/PromoBoxWidget.cs
+++ b/pattern/src/CornerStore/Widgets/PromoBoxWidget.cs
@@ -30,7 +30,7 @@ internal class PromoBoxWidget(Cart cart, Customer customer, CartService cartServ
     public void Refresh()
     {
-        Log.Info("promo " + (cart.PromoCode == "" ? "none" : cart.PromoCode));
+        Log.Instance.Info("promo " + (cart.PromoCode == "" ? "none" : cart.PromoCode));
     }
     public void RefreshAfterChange()
diff --git a/baseline/src/CornerStore/Widgets/ShippingSelectorWidget.cs b/pattern/src/CornerStore/Widgets/ShippingSelectorWidget.cs
index 9ea73d0..a0417f6 100644
--- a/baseline/src/CornerStore/Widgets/ShippingSelectorWidget.cs
+++ b/pattern/src/CornerStore/Widgets/ShippingSelectorWidget.cs
@@ -25,7 +25,7 @@ internal class ShippingSelectorWidget(Cart cart, PricingService pricing)
     public void Refresh()
     {
-        Log.Info("shipping " + cart.ShippingMethod);
+        Log.Instance.Info("shipping " + cart.ShippingMethod);
     }
     public void RefreshAfterChange()
diff --git a/baseline/src/CornerStore/Widgets/TotalsPanelWidget.cs b/pattern/src/CornerStore/Widgets/TotalsPanelWidget.cs
index 0c0ab23..7039625 100644
--- a/baseline/src/CornerStore/Widgets/TotalsPanelWidget.cs
+++ b/pattern/src/CornerStore/Widgets/TotalsPanelWidget.cs
@@ -18,7 +18,7 @@ internal class TotalsPanelWidget(Cart cart, Customer customer, PricingService pr
     public void Refresh()
     {
         var total = cart.Lines.Count == 0 ? 0m : pricing.BuildQuote(cart, customer).Total;
-        Log.Info("totals " + MoneyFormatter.Format(total, customer.Region));
+        Log.Instance.Info("totals " + MoneyFormatter.Format(total, customer.Region));
     }
     public void RefreshAfterChange()
@@ -33,19 +33,19 @@ internal class TotalsPanelWidget(Cart cart, Customer customer, PricingService pr
     public void ShowQuote()
     {
         var quote = pricing.BuildQuote(cart, customer);
-        Log.Info("quote " + cart.CustomerId + " (" + customer.Region + ", " + cart.ShippingMethod + ", "
+        Log.Instance.Info("quote " + cart.CustomerId + " (" + customer.Region + ", " + cart.ShippingMethod + ", "
             + quote.WeightKg.ToString("0.00", CultureInfo.InvariantCulture) + " kg)");
-        Log.Info("subtotal " + MoneyFormatter.Format(quote.Subtotal, customer.Region));
+        Log.Instance.Info("subtotal " + MoneyFormatter.Format(quote.Subtotal, customer.Region));
         if (cart.PromoCode != "")
         {
-            Log.Info(quote.Discount > 0m
+            Log.Instance.Info(quote.Discount > 0m
                 ? "discount -" + MoneyFormatter.Format(quote.Discount, customer.Region) + " (" + cart.PromoCode + ")"
                 : "promo " + cart.PromoCode + " does not apply");
         }
-        Log.Info("add-ons " + MoneyFormatter.Format(quote.AddOns, customer.Region));
-        Log.Info("shipping " + MoneyFormatter.Format(quote.Shipping, customer.Region));
-        Log.Info("tax " + MoneyFormatter.Format(quote.Tax, customer.Region));
-        Log.Info("total " + MoneyFormatter.Format(quote.Total, customer.Region));
+        Log.Instance.Info("add-ons " + MoneyFormatter.Format(quote.AddOns, customer.Region));
+        Log.Instance.Info("shipping " + MoneyFormatter.Format(quote.Shipping, customer.Region));
+        Log.Instance.Info("tax " + MoneyFormatter.Format(quote.Tax, customer.Region));
+        Log.Instance.Info("total " + MoneyFormatter.Format(quote.Total, customer.Region));
     }
 }
\ No newline at end of file
diff --git a/baseline/tests/CornerStore.Tests/AssemblyAttributes.cs b/pattern/tests/CornerStore.Tests/AssemblyAttributes.cs
index be7b494..58d5e51 100644
--- a/baseline/tests/CornerStore.Tests/AssemblyAttributes.cs
+++ b/pattern/tests/CornerStore.Tests/AssemblyAttributes.cs
@@ -1,5 +1,5 @@
 using Xunit.Sdk;
 using Xunit.v3;
-// Config and Log are static, so test runs must not overlap.
+// Config and Log are process-wide singletons, so test runs must not overlap.
 [assembly: Parallelization(Mode = ParallelMode.None)]
diff --git a/pattern/tests/CornerStore.Tests/SingletonTests.cs b/pattern/tests/CornerStore.Tests/SingletonTests.cs
new file mode 100644
index 0000000..9210f1b
--- /dev/null
+++ b/pattern/tests/CornerStore.Tests/SingletonTests.cs
@@ -0,0 +1,77 @@
+namespace CornerStore.Tests;
+
+public class SingletonTests
+{
+    [Fact]
+    public void Config_WhenAccessedConcurrently_ReturnsSameInstance()
+    {
+        var instances = new Config[32];
+
+        Parallel.For(0, instances.Length, index =>
+        {
+            instances[index] = Config.Instance;
+        });
+
+        Assert.All(instances, instance => Assert.Same(instances[0], instance));
+    }
+
+    [Fact]
+    public void Config_WhenChangedAndReset_RestoresDefaults()
+    {
+        var config = Config.Instance;
+        config.Reset();
+
+        try
+        {
+            config.SetSeed(17);
+            config.SetFraudThreshold(42);
+            config.SetLowStockLevel(5);
+
+            Assert.Equal(17, config.Seed);
+            Assert.Equal(42, config.FraudThreshold);
+            Assert.Equal(5, config.LowStockLevel);
+
+            config.Reset();
+
+            Assert.Equal(0, config.Seed);
+            Assert.Equal(80, config.FraudThreshold);
+            Assert.Equal(3, config.LowStockLevel);
+        }
+        finally
+        {
+            config.Reset();
+        }
+    }
+
+    [Fact]
+    public void Log_WhenAccessedConcurrently_ReturnsSameInstance()
+    {
+        var instances = new Log[32];
+
+        Parallel.For(0, instances.Length, index =>
+        {
+            instances[index] = Log.Instance;
+        });
+
+        Assert.All(instances, instance => Assert.Same(instances[0], instance));
+    }
+
+    [Fact]
+    public void Log_WhenConfigured_WritesFormattedLines()
+    {
+        var logger = Log.Instance;
+        using var output = new StringWriter();
+        logger.SetOutput(output);
+
+        try
+        {
+            logger.Info("message");
+
+            Assert.Equal("  message\n", output.ToString());
+        }
+        finally
+        {
+            logger.ResetOutput();
+        }
+    }
+}
```
