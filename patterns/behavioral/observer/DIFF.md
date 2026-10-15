# DIFF: observer

## File summary

| Status | File | Added | Removed |
| --- | --- | ---: | ---: |
| Modified | CornerStore/CornerStore.csproj | 4 | 0 |
| Added | CornerStore/Orders/AnalyticsOrderStatusObserver.cs | 9 | 0 |
| Added | CornerStore/Orders/InventoryOrderStatusObserver.cs | 12 | 0 |
| Added | CornerStore/Orders/IOrderStatusObserver.cs | 6 | 0 |
| Added | CornerStore/Orders/LoyaltyOrderStatusObserver.cs | 9 | 0 |
| Added | CornerStore/Orders/NotificationOrderStatusObserver.cs | 34 | 0 |
| Modified | CornerStore/Orders/OrderService.cs | 15 | 42 |
| Modified | CornerStore/StoreConsole.cs | 6 | 6 |
| Added | CornerStore.Tests/ObserverTests.cs | 98 | 0 |

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
diff --git a/pattern/src/CornerStore/Orders/AnalyticsOrderStatusObserver.cs b/pattern/src/CornerStore/Orders/AnalyticsOrderStatusObserver.cs
new file mode 100644
index 0000000..8a9ce83
--- /dev/null
+++ b/pattern/src/CornerStore/Orders/AnalyticsOrderStatusObserver.cs
@@ -0,0 +1,9 @@
+namespace CornerStore;
+
+internal sealed class AnalyticsOrderStatusObserver(AnalyticsService analytics) : IOrderStatusObserver
+{
+    public void OnStatusChanged(Order order)
+    {
+        analytics.RecordStatusChange(order);
+    }
+}
\ No newline at end of file
diff --git a/pattern/src/CornerStore/Orders/IOrderStatusObserver.cs b/pattern/src/CornerStore/Orders/IOrderStatusObserver.cs
new file mode 100644
index 0000000..07e8407
--- /dev/null
+++ b/pattern/src/CornerStore/Orders/IOrderStatusObserver.cs
@@ -0,0 +1,6 @@
+namespace CornerStore;
+
+internal interface IOrderStatusObserver
+{
+    void OnStatusChanged(Order order);
+}
\ No newline at end of file
diff --git a/pattern/src/CornerStore/Orders/InventoryOrderStatusObserver.cs b/pattern/src/CornerStore/Orders/InventoryOrderStatusObserver.cs
new file mode 100644
index 0000000..2f8118e
--- /dev/null
+++ b/pattern/src/CornerStore/Orders/InventoryOrderStatusObserver.cs
@@ -0,0 +1,12 @@
+namespace CornerStore;
+
+internal sealed class InventoryOrderStatusObserver(InventoryService inventory) : IOrderStatusObserver
+{
+    public void OnStatusChanged(Order order)
+    {
+        foreach (var line in order.Lines)
+        {
+            inventory.GetStock(line.Product.Sku);
+        }
+    }
+}
\ No newline at end of file
diff --git a/pattern/src/CornerStore/Orders/LoyaltyOrderStatusObserver.cs b/pattern/src/CornerStore/Orders/LoyaltyOrderStatusObserver.cs
new file mode 100644
index 0000000..ffcd66f
--- /dev/null
+++ b/pattern/src/CornerStore/Orders/LoyaltyOrderStatusObserver.cs
@@ -0,0 +1,9 @@
+namespace CornerStore;
+
+internal sealed class LoyaltyOrderStatusObserver(LoyaltyService loyalty) : IOrderStatusObserver
+{
+    public void OnStatusChanged(Order order)
+    {
+        loyalty.RecordStatusChange(order);
+    }
+}
\ No newline at end of file
diff --git a/pattern/src/CornerStore/Orders/NotificationOrderStatusObserver.cs b/pattern/src/CornerStore/Orders/NotificationOrderStatusObserver.cs
new file mode 100644
index 0000000..389b0cb
--- /dev/null
+++ b/pattern/src/CornerStore/Orders/NotificationOrderStatusObserver.cs
@@ -0,0 +1,34 @@
+namespace CornerStore;
+
+internal sealed class NotificationOrderStatusObserver(
+    OrderConfirmedEmail confirmedEmail,
+    OrderConfirmedSms confirmedSms,
+    OrderShippedEmail shippedEmail,
+    OrderShippedSms shippedSms) : IOrderStatusObserver
+{
+    public void OnStatusChanged(Order order)
+    {
+        if (order.Status == "Paid")
+        {
+            if (order.Customer.Channel == "email")
+            {
+                confirmedEmail.Send(order.Customer, order.Id, order.Quote.Total);
+            }
+            else
+            {
+                confirmedSms.Send(order.Customer, order.Id, order.Quote.Total);
+            }
+        }
+        else if (order.Status == "Shipped")
+        {
+            if (order.Customer.Channel == "email")
+            {
+                shippedEmail.Send(order.Customer, order.Id, order.TrackingNumber ?? "");
+            }
+            else
+            {
+                shippedSms.Send(order.Customer, order.Id, order.TrackingNumber ?? "");
+            }
+        }
+    }
+}
\ No newline at end of file
diff --git a/baseline/src/CornerStore/Orders/OrderService.cs b/pattern/src/CornerStore/Orders/OrderService.cs
index 87f6ce2..eeb5b9d 100644
--- a/baseline/src/CornerStore/Orders/OrderService.cs
+++ b/pattern/src/CornerStore/Orders/OrderService.cs
@@ -2,16 +2,16 @@ namespace CornerStore;
 internal class OrderService(
     IdSequence ids,
-    InventoryService inventory,
-    OrderConfirmedEmail confirmedEmail,
-    OrderConfirmedSms confirmedSms,
-    OrderShippedEmail shippedEmail,
-    OrderShippedSms shippedSms,
-    LoyaltyService loyalty,
-    AnalyticsService analytics)
+    InventoryService inventory)
 {
     private readonly Dictionary<string, Order> orders = new(StringComparer.Ordinal);
     private readonly List<Invoice> invoices = [];
+    private readonly List<IOrderStatusObserver> observers = [];
+
+    public void Subscribe(IOrderStatusObserver observer)
+    {
+        observers.Add(observer);
+    }
     public Order Create(Customer customer, Cart cart, Quote quote, string paymentType)
     {
@@ -55,7 +55,7 @@ internal class OrderService(
         }
         order.Status = "Paid";
-        RecordStatusChange(order);
+        NotifyObservers(order);
         return null;
     }
@@ -73,7 +73,7 @@ internal class OrderService(
             order.TrackingNumber = ids.NextShipmentId();
         }
-        RecordStatusChange(order);
+        NotifyObservers(order);
         return null;
     }
@@ -86,7 +86,7 @@ internal class OrderService(
         }
         order.Status = "Delivered";
-        RecordStatusChange(order);
+        NotifyObservers(order);
         return null;
     }
@@ -104,7 +104,7 @@ internal class OrderService(
             RestoreStock(order);
         }
-        RecordStatusChange(order);
+        NotifyObservers(order);
         return null;
     }
@@ -122,43 +122,16 @@ internal class OrderService(
             RestoreStock(order);
         }
-        RecordStatusChange(order);
+        NotifyObservers(order);
         return null;
     }
-    private void RecordStatusChange(Order order)
+    private void NotifyObservers(Order order)
     {
-        // SMELL S19: order transitions directly call stock, notification, loyalty and analytics services
-        foreach (var line in order.Lines)
-        {
-            inventory.GetStock(line.Product.Sku);
-        }
-
-        if (order.Status == "Paid")
+        foreach (var observer in observers)
         {
-            if (order.Customer.Channel == "email")
-            {
-                confirmedEmail.Send(order.Customer, order.Id, order.Quote.Total);
-            }
-            else
-            {
-                confirmedSms.Send(order.Customer, order.Id, order.Quote.Total);
-            }
+            observer.OnStatusChanged(order);
         }
-        else if (order.Status == "Shipped")
-        {
-            if (order.Customer.Channel == "email")
-            {
-                shippedEmail.Send(order.Customer, order.Id, order.TrackingNumber ?? "");
-            }
-            else
-            {
-                shippedSms.Send(order.Customer, order.Id, order.TrackingNumber ?? "");
-            }
-        }
-
-        loyalty.RecordStatusChange(order);
-        analytics.RecordStatusChange(order);
     }
     private void RestoreStock(Order order)
diff --git a/baseline/src/CornerStore/StoreConsole.cs b/pattern/src/CornerStore/StoreConsole.cs
index a288b4e..1f4806a 100644
--- a/baseline/src/CornerStore/StoreConsole.cs
+++ b/pattern/src/CornerStore/StoreConsole.cs
@@ -38,15 +38,15 @@ internal class StoreConsole
         this.clock = clock;
         cartService = new CartService(catalog);
         pricing = new PricingService(promos);
-        orderService = new OrderService(
-            Ids,
-            inventory,
+        orderService = new OrderService(Ids, inventory);
+        orderService.Subscribe(new InventoryOrderStatusObserver(inventory));
+        orderService.Subscribe(new NotificationOrderStatusObserver(
             new OrderConfirmedEmail(emailSender),
             new OrderConfirmedSms(smsSender),
             new OrderShippedEmail(emailSender),
-            new OrderShippedSms(smsSender),
-            loyalty,
-            analytics);
+            new OrderShippedSms(smsSender)));
+        orderService.Subscribe(new LoyaltyOrderStatusObserver(loyalty));
+        orderService.Subscribe(new AnalyticsOrderStatusObserver(analytics));
         backOffice = new BackOffice(inventory, orderService);
         orderValidator = new OrderValidator(inventory, new FraudScorer());
         shipFastClient = new ShipFastClient(Ids);
diff --git a/pattern/tests/CornerStore.Tests/ObserverTests.cs b/pattern/tests/CornerStore.Tests/ObserverTests.cs
new file mode 100644
index 0000000..2489ee5
--- /dev/null
+++ b/pattern/tests/CornerStore.Tests/ObserverTests.cs
@@ -0,0 +1,98 @@
+namespace CornerStore.Tests;
+
+public class ObserverTests
+{
+    [Fact]
+    public void GivenRegisteredObservers_Pay_NotifiesEachObserverInOrder()
+    {
+        var service = new OrderService(new IdSequence(), new InventoryService());
+        var firstObserver = new RecordingObserver();
+        var secondObserver = new RecordingObserver();
+        service.Subscribe(firstObserver);
+        service.Subscribe(secondObserver);
+        var order = CreateOrder(service);
+
+        var error = service.Pay(order);
+
+        Assert.Null(error);
+        Assert.Equal([order], firstObserver.Orders);
+        Assert.Equal([order], secondObserver.Orders);
+    }
+
+    [Fact]
+    public void GivenInvalidTransition_Ship_DoesNotNotifyObservers()
+    {
+        var service = new OrderService(new IdSequence(), new InventoryService());
+        var observer = new RecordingObserver();
+        service.Subscribe(observer);
+        var order = CreateOrder(service);
+
+        var error = service.Ship(order);
+
+        Assert.Equal("cannot ship from Pending", error);
+        Assert.Empty(observer.Orders);
+    }
+
+    [Fact]
+    public void GivenBuiltInObservers_Pay_PreservesConfirmationAndReportingEffects()
+    {
+        var previousOutput = Log.Out;
+        var output = new StringWriter();
+        Log.Out = output;
+
+        try
+        {
+            var inventory = new InventoryService();
+            var emailSender = new EmailSender();
+            var smsSender = new SmsSender();
+            var service = new OrderService(new IdSequence(), inventory);
+            service.Subscribe(new InventoryOrderStatusObserver(inventory));
+            service.Subscribe(new NotificationOrderStatusObserver(
+                new OrderConfirmedEmail(emailSender),
+                new OrderConfirmedSms(smsSender),
+                new OrderShippedEmail(emailSender),
+                new OrderShippedSms(smsSender)));
+            service.Subscribe(new LoyaltyOrderStatusObserver(new LoyaltyService()));
+            service.Subscribe(new AnalyticsOrderStatusObserver(new AnalyticsService()));
+            var order = CreateOrder(service);
+
+            var error = service.Pay(order);
+
+            Assert.Null(error);
+            Assert.Contains("email to Ava: order o-1001 confirmed", output.ToString());
+            Assert.Contains("loyalty updated for Ava (Paid)", output.ToString());
+            Assert.Contains("analytics order o-1001 Paid", output.ToString());
+        }
+        finally
+        {
+            Log.Out = previousOutput;
+        }
+    }
+
+    private static Order CreateOrder(OrderService service)
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
+        var cart = new Cart(customer.Id);
+        var quote = new Quote { Total = 10m };
+        return service.Create(customer, cart, quote, "card");
+    }
+
+    private sealed class RecordingObserver : IOrderStatusObserver
+    {
+        public List<Order> Orders { get; } = [];
+
+        public void OnStatusChanged(Order order)
+        {
+            Orders.Add(order);
+        }
+    }
+}
\ No newline at end of file
```
