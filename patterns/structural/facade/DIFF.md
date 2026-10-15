# DIFF: facade

## File summary

| Status | File | Added | Removed |
| --- | --- | ---: | ---: |
| Added | CornerStore/Checkout/CheckoutFacade.cs | 104 | 0 |
| Modified | CornerStore/CornerStore.csproj | 4 | 0 |
| Modified | CornerStore/StoreConsole.cs | 8 | 100 |
| Added | CornerStore.Tests/CheckoutFacadeTests.cs | 130 | 0 |

## Full diff

```diff
diff --git a/pattern/src/CornerStore/Checkout/CheckoutFacade.cs b/pattern/src/CornerStore/Checkout/CheckoutFacade.cs
new file mode 100644
index 0000000..294ba4b
--- /dev/null
+++ b/pattern/src/CornerStore/Checkout/CheckoutFacade.cs
@@ -0,0 +1,104 @@
+using System.Globalization;
+using System.Xml.Linq;
+
+namespace CornerStore;
+
+internal sealed class CheckoutFacade(
+    PricingService pricing,
+    OrderValidator orderValidator,
+    OrderService orderService,
+    InventoryService inventory,
+    ShipFastClient shipFastClient)
+{
+    public string PlaceOrder(Cart cart, Customer customer, string paymentType)
+    {
+        if (cart.Lines.Count == 0)
+        {
+            return "checkout rejected: cart is empty";
+        }
+
+        var quote = pricing.BuildQuote(cart, customer);
+        var error = orderValidator.Validate(customer, cart, quote);
+        if (error is not null)
+        {
+            return "checkout rejected: " + error;
+        }
+
+        if (!ChargePayment(paymentType, customer, quote.Total))
+        {
+            return "checkout rejected: unknown payment type " + paymentType;
+        }
+
+        var order = orderService.Create(customer, cart, quote, paymentType);
+        var reservedLines = new List<CartLine>();
+        foreach (var line in order.Lines)
+        {
+            if (!inventory.ReserveStock(line.Product.Sku, line.Quantity))
+            {
+                foreach (var reservedLine in reservedLines)
+                {
+                    inventory.AdjustStock(reservedLine.Product.Sku, reservedLine.Quantity);
+                }
+
+                return "checkout rejected: stock changed for " + line.Product.Sku;
+            }
+
+            reservedLines.Add(line);
+        }
+
+        order.StockReserved = true;
+        order.TrackingNumber = BookShipment(order);
+        var payError = orderService.Pay(order);
+        if (payError is not null)
+        {
+            foreach (var reservedLine in reservedLines)
+            {
+                inventory.AdjustStock(reservedLine.Product.Sku, reservedLine.Quantity);
+            }
+
+            order.StockReserved = false;
+            return "checkout rejected: " + payError;
+        }
+
+        return "checkout placed " + order.Id;
+    }
+
+    // SMELL S03: payment processor construction is selected by a switch in checkout
+    private static bool ChargePayment(string paymentType, Customer customer, decimal amount)
+    {
+        IPaymentProcessor? processor = null;
+        switch (paymentType)
+        {
+            case "card":
+                processor = new CardProcessor();
+                break;
+            case "paypal":
+                processor = new PayPalProcessor();
+                break;
+            case "afterpay":
+                processor = new AfterpayProcessor();
+                break;
+        }
+
+        if (processor is null)
+        {
+            return false;
+        }
+
+        processor.Charge(customer, amount);
+        return true;
+    }
+
+    // SMELL S06: checkout calls the courier client with XML and parses its response directly
+    private string BookShipment(Order order)
+    {
+        var cents = Math.Round(order.Quote.Total * 100m, 0, MidpointRounding.AwayFromZero)
+            .ToString("0", CultureInfo.InvariantCulture);
+        var name = System.Security.SecurityElement.Escape(order.Customer.Name) ?? "";
+        var address = System.Security.SecurityElement.Escape(order.Customer.Address) ?? "";
+        var request = "<shipment><orderId>" + order.Id + "</orderId><recipient>" + name
+            + "</recipient><address>" + address + "</address><amountCents>" + cents + "</amountCents></shipment>";
+        var response = shipFastClient.CreateConsignmentFromXml(request);
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
index a288b4e..1aa6675 100644
--- a/baseline/src/CornerStore/StoreConsole.cs
+++ b/pattern/src/CornerStore/StoreConsole.cs
@@ -1,7 +1,6 @@
 using System.Globalization;
 using System.Text;
 using System.Text.Json;
-using System.Xml.Linq;

 namespace CornerStore;

@@ -20,8 +19,7 @@ internal class StoreConsole
     private readonly AnalyticsService analytics = new();
     private readonly OrderService orderService;
     private readonly BackOffice backOffice;
-    private readonly OrderValidator orderValidator;
-    private readonly ShipFastClient shipFastClient;
+    private readonly CheckoutFacade checkoutFacade;
     private readonly ReportService reports;
     private readonly Dictionary<string, Customer> customers = [];
     private readonly Dictionary<string, Cart> carts = [];
@@ -48,8 +46,12 @@ internal class StoreConsole
             loyalty,
             analytics);
         backOffice = new BackOffice(inventory, orderService);
-        orderValidator = new OrderValidator(inventory, new FraudScorer());
-        shipFastClient = new ShipFastClient(Ids);
+        checkoutFacade = new CheckoutFacade(
+            pricing,
+            new OrderValidator(inventory, new FraudScorer()),
+            orderService,
+            inventory,
+            new ShipFastClient(Ids));
         reports = new ReportService(orderService);
     }

@@ -387,7 +389,6 @@ internal class StoreConsole
         Log.Info(backOffice.Cancel(customer, order));
     }

-    // SMELL S10: the console coordinates every checkout subsystem in a fixed order
     private void Checkout(JsonElement step)
     {
         var cart = FindCart(step);
@@ -396,101 +397,8 @@ internal class StoreConsole
             return;
         }

-        if (cart.Lines.Count == 0)
-        {
-            Log.Info("checkout rejected: cart is empty");
-            return;
-        }
-
         var customer = customers[cart.CustomerId];
-        var quote = pricing.BuildQuote(cart, customer);
-        var error = orderValidator.Validate(customer, cart, quote);
-        if (error is not null)
-        {
-            Log.Info("checkout rejected: " + error);
-            return;
-        }
-
-        var paymentType = Text(step, "payment");
-        if (!ChargePayment(paymentType, customer, quote.Total))
-        {
-            Log.Info("checkout rejected: unknown payment type " + paymentType);
-            return;
-        }
-
-        var order = orderService.Create(customer, cart, quote, paymentType);
-        var reservedLines = new List<CartLine>();
-        foreach (var line in order.Lines)
-        {
-            if (!inventory.ReserveStock(line.Product.Sku, line.Quantity))
-            {
-                foreach (var reservedLine in reservedLines)
-                {
-                    inventory.AdjustStock(reservedLine.Product.Sku, reservedLine.Quantity);
-                }
-
-                Log.Info("checkout rejected: stock changed for " + line.Product.Sku);
-                return;
-            }
-
-            reservedLines.Add(line);
-        }
-
-        order.StockReserved = true;
-        order.TrackingNumber = BookShipment(order);
-        var payError = orderService.Pay(order);
-        if (payError is not null)
-        {
-            foreach (var reservedLine in reservedLines)
-            {
-                inventory.AdjustStock(reservedLine.Product.Sku, reservedLine.Quantity);
-            }
-
-            order.StockReserved = false;
-            Log.Info("checkout rejected: " + payError);
-            return;
-        }
-
-        Log.Info("checkout placed " + order.Id);
-    }
-
-    // SMELL S03: payment processor construction is selected by a switch in checkout
-    private static bool ChargePayment(string paymentType, Customer customer, decimal amount)
-    {
-        IPaymentProcessor? processor = null;
-        switch (paymentType)
-        {
-            case "card":
-                processor = new CardProcessor();
-                break;
-            case "paypal":
-                processor = new PayPalProcessor();
-                break;
-            case "afterpay":
-                processor = new AfterpayProcessor();
-                break;
-        }
-
-        if (processor is null)
-        {
-            return false;
-        }
-
-        processor.Charge(customer, amount);
-        return true;
-    }
-
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
+        Log.Info(checkoutFacade.PlaceOrder(cart, customer, Text(step, "payment")));
     }

     private void ReportCsv(JsonElement step)
diff --git a/pattern/tests/CornerStore.Tests/CheckoutFacadeTests.cs b/pattern/tests/CornerStore.Tests/CheckoutFacadeTests.cs
new file mode 100644
index 0000000..106d2b4
--- /dev/null
+++ b/pattern/tests/CornerStore.Tests/CheckoutFacadeTests.cs
@@ -0,0 +1,130 @@
+using System.Reflection;
+
+namespace CornerStore.Tests;
+
+public class CheckoutFacadeTests
+{
+    [Fact]
+    public void GivenValidCart_PlaceOrder_PaysReservesBooksAndConfirms()
+    {
+        var previousOutput = Log.Out;
+        var output = new StringWriter();
+        Log.Out = output;
+
+        try
+        {
+            var fixture = CreateFixture();
+            var result = fixture.Facade.PlaceOrder(CreateCart(), CreateCustomer(), "card");
+            var order = Assert.Single(fixture.OrderService.GetAll());
+
+            Assert.Equal("checkout placed o-1001", result);
+            Assert.Equal("Paid", order.Status);
+            Assert.True(order.StockReserved);
+            Assert.Equal("SF-0001", order.TrackingNumber);
+            Assert.Equal(9, fixture.Inventory.GetStock("MUG-ENAMEL"));
+            Assert.Contains("card charged", output.ToString());
+            Assert.Contains("ShipFast booked SF-0001 for o-1001", output.ToString());
+            Assert.Contains("email to Ava: order o-1001 confirmed", output.ToString());
+        }
+        finally
+        {
+            Log.Out = previousOutput;
+        }
+    }
+
+    [Fact]
+    public void GivenMissingAddress_PlaceOrder_DoesNotChargeOrReserveStock()
+    {
+        var previousOutput = Log.Out;
+        var output = new StringWriter();
+        Log.Out = output;
+
+        try
+        {
+            var fixture = CreateFixture();
+            var stockBeforeCheckout = fixture.Inventory.GetStock("MUG-ENAMEL");
+            var customer = CreateCustomer("");
+
+            var result = fixture.Facade.PlaceOrder(CreateCart(), customer, "card");
+
+            Assert.Equal("checkout rejected: address required", result);
+            Assert.Empty(fixture.OrderService.GetAll());
+            Assert.Equal(stockBeforeCheckout, fixture.Inventory.GetStock("MUG-ENAMEL"));
+            Assert.DoesNotContain("card charged", output.ToString());
+        }
+        finally
+        {
+            Log.Out = previousOutput;
+        }
+    }
+
+    [Fact]
+    public void GivenConsoleCheckout_UsesCheckoutFacade()
+    {
+        var facadeField = typeof(StoreConsole).GetField("checkoutFacade", BindingFlags.Instance | BindingFlags.NonPublic);
+
+        Assert.NotNull(facadeField);
+        Assert.Equal(typeof(CheckoutFacade), facadeField.FieldType);
+    }
+
+    private static Cart CreateCart()
+    {
+        var cart = new Cart("cu-1");
+        cart.Lines.Add(new CartLine
+        {
+            Product = new PhysicalProduct
+            {
+                Sku = "MUG-ENAMEL",
+                Name = "Enamel Mug",
+                CategoryName = "Home",
+                Price = 10m,
+                WeightKg = 0.3m,
+            },
+            Quantity = 1,
+            UnitPrice = 10m,
+        });
+        return cart;
+    }
+
+    private static Customer CreateCustomer(string address = "1 Harbour St")
+    {
+        return new Customer
+        {
+            Id = "cu-1",
+            Number = 1,
+            Name = "Ava",
+            Region = "AU",
+            Channel = "email",
+            Address = address,
+            CreditLimit = 500m,
+        };
+    }
+
+    private static FacadeFixture CreateFixture()
+    {
+        var inventory = new InventoryService();
+        var ids = new IdSequence();
+        var orderService = new OrderService(
+            ids,
+            inventory,
+            new OrderConfirmedEmail(new EmailSender()),
+            new OrderConfirmedSms(new SmsSender()),
+            new OrderShippedEmail(new EmailSender()),
+            new OrderShippedSms(new SmsSender()),
+            new LoyaltyService(),
+            new AnalyticsService());
+        var facade = new CheckoutFacade(
+            new PricingService(new PromoEngine()),
+            new OrderValidator(inventory, new FraudScorer()),
+            orderService,
+            inventory,
+            new ShipFastClient(ids));
+
+        return new FacadeFixture(facade, orderService, inventory);
+    }
+
+    private sealed record FacadeFixture(
+        CheckoutFacade Facade,
+        OrderService OrderService,
+        InventoryService Inventory);
+}
\ No newline at end of file
```
