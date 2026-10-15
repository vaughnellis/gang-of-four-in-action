# DIFF: adapter

## File summary

| Status | File | Added | Removed |
| --- | --- | ---: | ---: |
| Added | CornerStore/Checkout/IShippingProvider.cs | 6 | 0 |
| Added | CornerStore/Checkout/ShipFastAdapter.cs | 19 | 0 |
| Modified | CornerStore/CornerStore.csproj | 4 | 0 |
| Modified | CornerStore/StoreConsole.cs | 3 | 17 |
| Added | CornerStore.Tests/AdapterTests.cs | 62 | 0 |

## Full diff

```diff
diff --git a/pattern/src/CornerStore/Checkout/IShippingProvider.cs b/pattern/src/CornerStore/Checkout/IShippingProvider.cs
new file mode 100644
index 0000000..d128355
--- /dev/null
+++ b/pattern/src/CornerStore/Checkout/IShippingProvider.cs
@@ -0,0 +1,6 @@
+namespace CornerStore;
+
+internal interface IShippingProvider
+{
+    string BookShipment(Order order);
+}
\ No newline at end of file
diff --git a/pattern/src/CornerStore/Checkout/ShipFastAdapter.cs b/pattern/src/CornerStore/Checkout/ShipFastAdapter.cs
new file mode 100644
index 0000000..e7acc5d
--- /dev/null
+++ b/pattern/src/CornerStore/Checkout/ShipFastAdapter.cs
@@ -0,0 +1,19 @@
+using System.Globalization;
+using System.Xml.Linq;
+
+namespace CornerStore;
+
+internal sealed class ShipFastAdapter(ShipFastClient client) : IShippingProvider
+{
+    public string BookShipment(Order order)
+    {
+        var cents = Math.Round(order.Quote.Total * 100m, 0, MidpointRounding.AwayFromZero)
+            .ToString("0", CultureInfo.InvariantCulture);
+        var name = System.Security.SecurityElement.Escape(order.Customer.Name) ?? "";
+        var address = System.Security.SecurityElement.Escape(order.Customer.Address) ?? "";
+        var request = "<shipment><orderId>" + order.Id + "</orderId><recipient>" + name
+            + "</recipient><address>" + address + "</address><amountCents>" + cents + "</amountCents></shipment>";
+        var response = client.CreateConsignmentFromXml(request);
+        return XDocument.Parse(response).Root?.Element("tracking")?.Value ?? "";
+    }
+}
\ No newline at end of file
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
diff --git a/baseline/src/CornerStore/StoreConsole.cs b/pattern/src/CornerStore/StoreConsole.cs
index a288b4e..d2f471a 100644
--- a/baseline/src/CornerStore/StoreConsole.cs
+++ b/pattern/src/CornerStore/StoreConsole.cs
@@ -1,7 +1,6 @@
 using System.Globalization;
 using System.Text;
 using System.Text.Json;
-using System.Xml.Linq;

 namespace CornerStore;

@@ -21,7 +20,7 @@ internal class StoreConsole
     private readonly OrderService orderService;
     private readonly BackOffice backOffice;
     private readonly OrderValidator orderValidator;
-    private readonly ShipFastClient shipFastClient;
+    private readonly IShippingProvider shippingProvider;
     private readonly ReportService reports;
     private readonly Dictionary<string, Customer> customers = [];
     private readonly Dictionary<string, Cart> carts = [];
@@ -49,7 +48,7 @@ internal class StoreConsole
             analytics);
         backOffice = new BackOffice(inventory, orderService);
         orderValidator = new OrderValidator(inventory, new FraudScorer());
-        shipFastClient = new ShipFastClient(Ids);
+        shippingProvider = new ShipFastAdapter(new ShipFastClient(Ids));
         reports = new ReportService(orderService);
     }

@@ -437,7 +436,7 @@ internal class StoreConsole
         }

         order.StockReserved = true;
-        order.TrackingNumber = BookShipment(order);
+        order.TrackingNumber = shippingProvider.BookShipment(order);
         var payError = orderService.Pay(order);
         if (payError is not null)
         {
@@ -480,19 +479,6 @@ internal class StoreConsole
         return true;
     }

-    // SMELL S06: checkout calls the courier client with XML and parses its response directly
-    private string BookShipment(Order order)
-    {
-        var cents = Math.Round(order.Quote.Total * 100m, 0, MidpointRounding.AwayFromZero)
-            .ToString("0", CultureInfo.InvariantCulture);
-        var name = System.Security.SecurityElement.Escape(order.Customer.Name) ?? "";
-        var address = System.Security.SecurityElement.Escape(order.Customer.Address) ?? "";
-        var request = "<shipment><orderId>" + order.Id + "</orderId><recipient>" + name
-            + "</recipient><address>" + address + "</address><amountCents>" + cents + "</amountCents></shipment>";
-        var response = shipFastClient.CreateConsignmentFromXml(request);
-        return XDocument.Parse(response).Root?.Element("tracking")?.Value ?? "";
-    }
-
     private void ReportCsv(JsonElement step)
     {
         var status = Text(step, "status");
diff --git a/pattern/tests/CornerStore.Tests/AdapterTests.cs b/pattern/tests/CornerStore.Tests/AdapterTests.cs
new file mode 100644
index 0000000..919eaf3
--- /dev/null
+++ b/pattern/tests/CornerStore.Tests/AdapterTests.cs
@@ -0,0 +1,62 @@
+namespace CornerStore.Tests;
+
+public class AdapterTests
+{
+    [Fact]
+    public void GivenOrder_BookShipment_ConvertsAmountToCentsAndReturnsTracking()
+    {
+        var previousOutput = Log.Out;
+        var output = new StringWriter();
+        Log.Out = output;
+
+        try
+        {
+            var adapter = new ShipFastAdapter(new ShipFastClient(new IdSequence()));
+            var tracking = adapter.BookShipment(CreateOrder("Ava", "1 Harbour St"));
+
+            Assert.Equal("SF-0001", tracking);
+            Assert.Contains("ShipFast booked SF-0001 for o-1001 (1234 cents)", output.ToString());
+        }
+        finally
+        {
+            Log.Out = previousOutput;
+        }
+    }
+
+    [Fact]
+    public void GivenSpecialXmlCharacters_BookShipment_SendsValidRequest()
+    {
+        var previousOutput = Log.Out;
+        var output = new StringWriter();
+        Log.Out = output;
+
+        try
+        {
+            var adapter = new ShipFastAdapter(new ShipFastClient(new IdSequence()));
+
+            var tracking = adapter.BookShipment(CreateOrder("Ava & Co", "1 <Harbour> & 2"));
+
+            Assert.Equal("SF-0001", tracking);
+            Assert.Contains("ShipFast booked SF-0001 for o-1001 (1234 cents)", output.ToString());
+        }
+        finally
+        {
+            Log.Out = previousOutput;
+        }
+    }
+
+    private static Order CreateOrder(string name, string address)
+    {
+        var customer = new Customer
+        {
+            Id = "cu-1",
+            Number = 1,
+            Name = name,
+            Region = "AU",
+            Channel = "email",
+            Address = address,
+            CreditLimit = 500m,
+        };
+        return new Order("o-1001", customer, "Pending", [], new Quote { Total = 12.34m }, "card", null, null, null, null, null);
+    }
+}
\ No newline at end of file
```
