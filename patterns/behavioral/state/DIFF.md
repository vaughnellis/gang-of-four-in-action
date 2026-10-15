# DIFF: state

## File summary

| Status | File | Added | Removed |
| --- | --- | ---: | ---: |
| Modified | CornerStore/CornerStore.csproj | 4 | 0 |
| Added | CornerStore/Orders/CancelledOrderState.cs | 6 | 0 |
| Added | CornerStore/Orders/DeliveredOrderState.cs | 22 | 0 |
| Modified | CornerStore/Orders/Order.cs | 4 | 2 |
| Modified | CornerStore/Orders/OrderService.cs | 16 | 63 |
| Added | CornerStore/Orders/OrderState.cs | 21 | 0 |
| Added | CornerStore/Orders/PaidOrderState.cs | 38 | 0 |
| Added | CornerStore/Orders/PendingOrderState.cs | 29 | 0 |
| Added | CornerStore/Orders/RefundedOrderState.cs | 6 | 0 |
| Added | CornerStore/Orders/ShippedOrderState.cs | 29 | 0 |
| Added | CornerStore.Tests/StateTests.cs | 110 | 0 |

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
diff --git a/pattern/src/CornerStore/Orders/CancelledOrderState.cs b/pattern/src/CornerStore/Orders/CancelledOrderState.cs
new file mode 100644
index 0000000..9eceacb
--- /dev/null
+++ b/pattern/src/CornerStore/Orders/CancelledOrderState.cs
@@ -0,0 +1,6 @@
+namespace CornerStore;
+
+internal sealed class CancelledOrderState : OrderState
+{
+    public override string Name => "Cancelled";
+}
\ No newline at end of file
diff --git a/pattern/src/CornerStore/Orders/DeliveredOrderState.cs b/pattern/src/CornerStore/Orders/DeliveredOrderState.cs
new file mode 100644
index 0000000..bdfecae
--- /dev/null
+++ b/pattern/src/CornerStore/Orders/DeliveredOrderState.cs
@@ -0,0 +1,22 @@
+namespace CornerStore;
+
+internal sealed class DeliveredOrderState : OrderState
+{
+    public override string Name => "Delivered";
+
+    public override string? Refund(OrderService service, Order order)
+    {
+        order.State = new RefundedOrderState();
+        RestoreStockIfReserved(service, order);
+        service.RecordStatusChange(order);
+        return null;
+    }
+
+    private static void RestoreStockIfReserved(OrderService service, Order order)
+    {
+        if (order.StockReserved)
+        {
+            service.RestoreStock(order);
+        }
+    }
+}
\ No newline at end of file
diff --git a/baseline/src/CornerStore/Orders/Order.cs b/pattern/src/CornerStore/Orders/Order.cs
index 6344d8b..f758a8d 100644
--- a/baseline/src/CornerStore/Orders/Order.cs
+++ b/pattern/src/CornerStore/Orders/Order.cs
@@ -3,7 +3,7 @@ namespace CornerStore;
 internal class Order(
     string id,
     Customer customer,
-    string status,
+    OrderState state,
     List<CartLine> lines,
     Quote quote,
     string paymentType,
@@ -17,7 +17,9 @@ internal class Order(

     public Customer Customer { get; } = customer;

-    public string Status { get; set; } = status;
+    public OrderState State { get; set; } = state;
+
+    public string Status => State.Name;

     public List<CartLine> Lines { get; } = lines;

diff --git a/baseline/src/CornerStore/Orders/OrderService.cs b/pattern/src/CornerStore/Orders/OrderService.cs
index 87f6ce2..2ac78d3 100644
--- a/baseline/src/CornerStore/Orders/OrderService.cs
+++ b/pattern/src/CornerStore/Orders/OrderService.cs
@@ -17,7 +17,7 @@ internal class OrderService(
     {
         // SMELL S02: the order is assembled with eleven positional arguments and null optionals
         var order = new Order(
-            ids.NextOrderId(), customer, "Pending",
+            ids.NextOrderId(), customer, new PendingOrderState(),
             cart.Lines.Select(CopyLine).ToList(), quote, paymentType,
             cart.PromoCode == "" ? null : cart.PromoCode, null, null, null, null);
         orders[order.Id] = order;
@@ -48,85 +48,30 @@ internal class OrderService(

     public string? Pay(Order order)
     {
-        // SMELL S20: the allowed transition is selected by comparing the status string
-        if (order.Status != "Pending")
-        {
-            return "cannot pay from " + order.Status;
-        }
-
-        order.Status = "Paid";
-        RecordStatusChange(order);
-        return null;
+        return order.State.Pay(this, order);
     }

     public string? Ship(Order order)
     {
-        // SMELL S20: the allowed transition is selected by comparing the status string
-        if (order.Status != "Paid")
-        {
-            return "cannot ship from " + order.Status;
-        }
-
-        order.Status = "Shipped";
-        if (order.TrackingNumber is null)
-        {
-            order.TrackingNumber = ids.NextShipmentId();
-        }
-
-        RecordStatusChange(order);
-        return null;
+        return order.State.Ship(this, order);
     }

     public string? Deliver(Order order)
     {
-        // SMELL S20: the allowed transition is selected by comparing the status string
-        if (order.Status != "Shipped")
-        {
-            return "cannot deliver from " + order.Status;
-        }
-
-        order.Status = "Delivered";
-        RecordStatusChange(order);
-        return null;
+        return order.State.Deliver(this, order);
     }

     public string? Cancel(Order order)
     {
-        // SMELL S20: the allowed transition is selected by comparing the status string
-        if (order.Status != "Pending" && order.Status != "Paid")
-        {
-            return "cannot cancel from " + order.Status;
-        }
-
-        order.Status = "Cancelled";
-        if (order.StockReserved)
-        {
-            RestoreStock(order);
-        }
-
-        RecordStatusChange(order);
-        return null;
+        return order.State.Cancel(this, order);
     }

     public string? Refund(Order order)
     {
-        // SMELL S20: the allowed transition is selected by comparing the status string
-        if (order.Status != "Paid" && order.Status != "Shipped" && order.Status != "Delivered")
-        {
-            return "cannot refund from " + order.Status;
-        }
-
-        order.Status = "Refunded";
-        if (order.StockReserved)
-        {
-            RestoreStock(order);
-        }
-
-        RecordStatusChange(order);
-        return null;
+        return order.State.Refund(this, order);
     }

-    private void RecordStatusChange(Order order)
+    internal void RecordStatusChange(Order order)
     {
         // SMELL S19: order transitions directly call stock, notification, loyalty and analytics services
         foreach (var line in order.Lines)
@@ -161,7 +106,7 @@ internal class OrderService(
         analytics.RecordStatusChange(order);
     }

-    private void RestoreStock(Order order)
+    internal void RestoreStock(Order order)
     {
         foreach (var line in order.Lines)
         {
@@ -171,6 +116,14 @@ internal class OrderService(
         order.StockReserved = false;
     }

+    internal void AssignTrackingNumber(Order order)
+    {
+        if (order.TrackingNumber is null)
+        {
+            order.TrackingNumber = ids.NextShipmentId();
+        }
+    }
+
     private static CartLine CopyLine(CartLine line)
     {
         return new CartLine
diff --git a/pattern/src/CornerStore/Orders/OrderState.cs b/pattern/src/CornerStore/Orders/OrderState.cs
new file mode 100644
index 0000000..06fa83a
--- /dev/null
+++ b/pattern/src/CornerStore/Orders/OrderState.cs
@@ -0,0 +1,21 @@
+namespace CornerStore;
+
+internal abstract class OrderState
+{
+    public abstract string Name { get; }
+
+    public virtual string? Pay(OrderService service, Order order) => Cannot("pay", order);
+
+    public virtual string? Ship(OrderService service, Order order) => Cannot("ship", order);
+
+    public virtual string? Deliver(OrderService service, Order order) => Cannot("deliver", order);
+
+    public virtual string? Cancel(OrderService service, Order order) => Cannot("cancel", order);
+
+    public virtual string? Refund(OrderService service, Order order) => Cannot("refund", order);
+
+    protected static string Cannot(string operation, Order order)
+    {
+        return "cannot " + operation + " from " + order.Status;
+    }
+}
\ No newline at end of file
diff --git a/pattern/src/CornerStore/Orders/PaidOrderState.cs b/pattern/src/CornerStore/Orders/PaidOrderState.cs
new file mode 100644
index 0000000..688e755
--- /dev/null
+++ b/pattern/src/CornerStore/Orders/PaidOrderState.cs
@@ -0,0 +1,38 @@
+namespace CornerStore;
+
+internal sealed class PaidOrderState : OrderState
+{
+    public override string Name => "Paid";
+
+    public override string? Ship(OrderService service, Order order)
+    {
+        order.State = new ShippedOrderState();
+        service.AssignTrackingNumber(order);
+        service.RecordStatusChange(order);
+        return null;
+    }
+
+    public override string? Cancel(OrderService service, Order order)
+    {
+        order.State = new CancelledOrderState();
+        RestoreStockIfReserved(service, order);
+        service.RecordStatusChange(order);
+        return null;
+    }
+
+    public override string? Refund(OrderService service, Order order)
+    {
+        order.State = new RefundedOrderState();
+        RestoreStockIfReserved(service, order);
+        service.RecordStatusChange(order);
+        return null;
+    }
+
+    private static void RestoreStockIfReserved(OrderService service, Order order)
+    {
+        if (order.StockReserved)
+        {
+            service.RestoreStock(order);
+        }
+    }
+}
\ No newline at end of file
diff --git a/pattern/src/CornerStore/Orders/PendingOrderState.cs b/pattern/src/CornerStore/Orders/PendingOrderState.cs
new file mode 100644
index 0000000..684d442
--- /dev/null
+++ b/pattern/src/CornerStore/Orders/PendingOrderState.cs
@@ -0,0 +1,29 @@
+namespace CornerStore;
+
+internal sealed class PendingOrderState : OrderState
+{
+    public override string Name => "Pending";
+
+    public override string? Pay(OrderService service, Order order)
+    {
+        order.State = new PaidOrderState();
+        service.RecordStatusChange(order);
+        return null;
+    }
+
+    public override string? Cancel(OrderService service, Order order)
+    {
+        order.State = new CancelledOrderState();
+        RestoreStockIfReserved(service, order);
+        service.RecordStatusChange(order);
+        return null;
+    }
+
+    private static void RestoreStockIfReserved(OrderService service, Order order)
+    {
+        if (order.StockReserved)
+        {
+            service.RestoreStock(order);
+        }
+    }
+}
\ No newline at end of file
diff --git a/pattern/src/CornerStore/Orders/RefundedOrderState.cs b/pattern/src/CornerStore/Orders/RefundedOrderState.cs
new file mode 100644
index 0000000..5392610
--- /dev/null
+++ b/pattern/src/CornerStore/Orders/RefundedOrderState.cs
@@ -0,0 +1,6 @@
+namespace CornerStore;
+
+internal sealed class RefundedOrderState : OrderState
+{
+    public override string Name => "Refunded";
+}
\ No newline at end of file
diff --git a/pattern/src/CornerStore/Orders/ShippedOrderState.cs b/pattern/src/CornerStore/Orders/ShippedOrderState.cs
new file mode 100644
index 0000000..8fb3cd3
--- /dev/null
+++ b/pattern/src/CornerStore/Orders/ShippedOrderState.cs
@@ -0,0 +1,29 @@
+namespace CornerStore;
+
+internal sealed class ShippedOrderState : OrderState
+{
+    public override string Name => "Shipped";
+
+    public override string? Deliver(OrderService service, Order order)
+    {
+        order.State = new DeliveredOrderState();
+        service.RecordStatusChange(order);
+        return null;
+    }
+
+    public override string? Refund(OrderService service, Order order)
+    {
+        order.State = new RefundedOrderState();
+        RestoreStockIfReserved(service, order);
+        service.RecordStatusChange(order);
+        return null;
+    }
+
+    private static void RestoreStockIfReserved(OrderService service, Order order)
+    {
+        if (order.StockReserved)
+        {
+            service.RestoreStock(order);
+        }
+    }
+}
\ No newline at end of file
diff --git a/pattern/tests/CornerStore.Tests/StateTests.cs b/pattern/tests/CornerStore.Tests/StateTests.cs
new file mode 100644
index 0000000..80c33de
--- /dev/null
+++ b/pattern/tests/CornerStore.Tests/StateTests.cs
@@ -0,0 +1,110 @@
+namespace CornerStore.Tests;
+
+public class StateTests
+{
+    [Fact]
+    public void GivenPendingOrder_ValidTransitions_ReachRefunded()
+    {
+        var fixture = CreateFixture();
+
+        Assert.Null(fixture.OrderService.Pay(fixture.Order));
+        Assert.Equal("Paid", fixture.Order.Status);
+        Assert.Null(fixture.OrderService.Ship(fixture.Order));
+        Assert.Equal("Shipped", fixture.Order.Status);
+        Assert.Null(fixture.OrderService.Deliver(fixture.Order));
+        Assert.Equal("Delivered", fixture.Order.Status);
+        Assert.Null(fixture.OrderService.Refund(fixture.Order));
+        Assert.Equal("Refunded", fixture.Order.Status);
+    }
+
+    [Fact]
+    public void GivenPendingOrder_Ship_IsRejectedWithoutChangingState()
+    {
+        var fixture = CreateFixture();
+
+        var error = fixture.OrderService.Ship(fixture.Order);
+
+        Assert.Equal("cannot ship from Pending", error);
+        Assert.Equal("Pending", fixture.Order.Status);
+    }
+
+    [Fact]
+    public void GivenReservedStock_Cancel_RestoresInventory()
+    {
+        var fixture = CreateFixture(withLine: true);
+        var stockBeforeReservation = fixture.Inventory.GetStock("MUG-ENAMEL");
+        Assert.True(fixture.Inventory.ReserveStock("MUG-ENAMEL", 1));
+        fixture.Order.StockReserved = true;
+
+        var error = fixture.OrderService.Cancel(fixture.Order);
+
+        Assert.Null(error);
+        Assert.Equal("Cancelled", fixture.Order.Status);
+        Assert.Equal(stockBeforeReservation, fixture.Inventory.GetStock("MUG-ENAMEL"));
+    }
+
+    [Fact]
+    public void GivenPaidOrderWithReservedStock_Refund_RestoresInventory()
+    {
+        var fixture = CreateFixture(withLine: true);
+        Assert.Null(fixture.OrderService.Pay(fixture.Order));
+        var stockBeforeReservation = fixture.Inventory.GetStock("MUG-ENAMEL");
+        Assert.True(fixture.Inventory.ReserveStock("MUG-ENAMEL", 1));
+        fixture.Order.StockReserved = true;
+
+        var error = fixture.OrderService.Refund(fixture.Order);
+
+        Assert.Null(error);
+        Assert.Equal("Refunded", fixture.Order.Status);
+        Assert.Equal(stockBeforeReservation, fixture.Inventory.GetStock("MUG-ENAMEL"));
+    }
+
+    private static OrderFixture CreateFixture(bool withLine = false)
+    {
+        var inventory = new InventoryService();
+        var ids = new IdSequence();
+        var emailSender = new EmailSender();
+        var smsSender = new SmsSender();
+        var orderService = new OrderService(
+            ids,
+            inventory,
+            new OrderConfirmedEmail(emailSender),
+            new OrderConfirmedSms(smsSender),
+            new OrderShippedEmail(emailSender),
+            new OrderShippedSms(smsSender),
+            new LoyaltyService(),
+            new AnalyticsService());
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
+        if (withLine)
+        {
+            cart.Lines.Add(new CartLine
+            {
+                Product = new PhysicalProduct
+                {
+                    Sku = "MUG-ENAMEL",
+                    Name = "Enamel Mug",
+                    CategoryName = "Home",
+                    Price = 10m,
+                    WeightKg = 0.3m,
+                },
+                Quantity = 1,
+                UnitPrice = 10m,
+            });
+        }
+
+        var order = orderService.Create(customer, cart, new Quote { Total = 10m }, "card");
+        return new OrderFixture(orderService, inventory, order);
+    }
+
+    private sealed record OrderFixture(OrderService OrderService, InventoryService Inventory, Order Order);
+}
\ No newline at end of file
```
