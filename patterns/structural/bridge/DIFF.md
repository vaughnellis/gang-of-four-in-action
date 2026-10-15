# DIFF: bridge

## File summary

| Status | File | Added | Removed |
| --- | --- | ---: | ---: |
| Modified | CornerStore/CornerStore.csproj | 4 | 0 |
| Modified | CornerStore/Notifications/EmailSender.cs | 1 | 1 |
| Added | CornerStore/Notifications/IChannel.cs | 6 | 0 |
| Removed | CornerStore/Notifications/OrderConfirmedEmail.cs | 0 | 10 |
| Added | CornerStore/Notifications/OrderConfirmedMessage.cs | 9 | 0 |
| Removed | CornerStore/Notifications/OrderConfirmedSms.cs | 0 | 10 |
| Added | CornerStore/Notifications/OrderMessage.cs | 9 | 0 |
| Removed | CornerStore/Notifications/OrderShippedEmail.cs | 0 | 10 |
| Added | CornerStore/Notifications/OrderShippedMessage.cs | 9 | 0 |
| Removed | CornerStore/Notifications/OrderShippedSms.cs | 0 | 10 |
| Modified | CornerStore/Notifications/SmsSender.cs | 1 | 1 |
| Modified | CornerStore/Orders/OrderService.cs | 11 | 20 |
| Modified | CornerStore/StoreConsole.cs | 2 | 4 |
| Added | CornerStore.Tests/BridgeTests.cs | 51 | 0 |

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
diff --git a/baseline/src/CornerStore/Notifications/EmailSender.cs b/pattern/src/CornerStore/Notifications/EmailSender.cs
index 4a1ea8d..306e814 100644
--- a/baseline/src/CornerStore/Notifications/EmailSender.cs
+++ b/pattern/src/CornerStore/Notifications/EmailSender.cs
@@ -1,6 +1,6 @@
 namespace CornerStore;
-internal class EmailSender
+internal class EmailSender : IChannel
 {
     public void Send(string recipient, string message)
     {
diff --git a/pattern/src/CornerStore/Notifications/IChannel.cs b/pattern/src/CornerStore/Notifications/IChannel.cs
new file mode 100644
index 0000000..4c0b4b6
--- /dev/null
+++ b/pattern/src/CornerStore/Notifications/IChannel.cs
@@ -0,0 +1,6 @@
+namespace CornerStore;
+
+internal interface IChannel
+{
+    void Send(string recipient, string message);
+}
diff --git a/baseline/src/CornerStore/Notifications/OrderConfirmedEmail.cs b/baseline/src/CornerStore/Notifications/OrderConfirmedEmail.cs
deleted file mode 100644
index bb0efbd..0000000
--- a/baseline/src/CornerStore/Notifications/OrderConfirmedEmail.cs
+++ /dev/null
@@ -1,10 +0,0 @@
-namespace CornerStore;
-
-internal class OrderConfirmedEmail(EmailSender sender)
-{
-    // SMELL S07: confirmation email owns the email-specific wording and sender call
-    public void Send(Customer customer, string orderId, decimal total)
-    {
-        sender.Send(customer.Name, "order " + orderId + " confirmed, total " + MoneyFormatter.Format(total, customer.Region));
-    }
-}
\ No newline at end of file
diff --git a/pattern/src/CornerStore/Notifications/OrderConfirmedMessage.cs b/pattern/src/CornerStore/Notifications/OrderConfirmedMessage.cs
new file mode 100644
index 0000000..8a20310
--- /dev/null
+++ b/pattern/src/CornerStore/Notifications/OrderConfirmedMessage.cs
@@ -0,0 +1,9 @@
+namespace CornerStore;
+
+internal sealed class OrderConfirmedMessage(IChannel channel) : OrderMessage(channel)
+{
+    public void Send(Customer customer, string orderId, decimal total)
+    {
+        Deliver(customer, "order " + orderId + " confirmed, total " + MoneyFormatter.Format(total, customer.Region));
+    }
+}
diff --git a/baseline/src/CornerStore/Notifications/OrderConfirmedSms.cs b/baseline/src/CornerStore/Notifications/OrderConfirmedSms.cs
deleted file mode 100644
index 0ab688b..0000000
--- a/baseline/src/CornerStore/Notifications/OrderConfirmedSms.cs
+++ /dev/null
@@ -1,10 +0,0 @@
-namespace CornerStore;
-
-internal class OrderConfirmedSms(SmsSender sender)
-{
-    // SMELL S07: confirmation SMS owns the SMS-specific wording and sender call
-    public void Send(Customer customer, string orderId, decimal total)
-    {
-        sender.Send(customer.Name, "order " + orderId + " confirmed, total " + MoneyFormatter.Format(total, customer.Region));
-    }
-}
\ No newline at end of file
diff --git a/pattern/src/CornerStore/Notifications/OrderMessage.cs b/pattern/src/CornerStore/Notifications/OrderMessage.cs
new file mode 100644
index 0000000..7c916e8
--- /dev/null
+++ b/pattern/src/CornerStore/Notifications/OrderMessage.cs
@@ -0,0 +1,9 @@
+namespace CornerStore;
+
+internal abstract class OrderMessage(IChannel channel)
+{
+    protected void Deliver(Customer customer, string message)
+    {
+        channel.Send(customer.Name, message);
+    }
+}
diff --git a/baseline/src/CornerStore/Notifications/OrderShippedEmail.cs b/baseline/src/CornerStore/Notifications/OrderShippedEmail.cs
deleted file mode 100644
index 1acb063..0000000
--- a/baseline/src/CornerStore/Notifications/OrderShippedEmail.cs
+++ /dev/null
@@ -1,10 +0,0 @@
-namespace CornerStore;
-
-internal class OrderShippedEmail(EmailSender sender)
-{
-    // SMELL S07: shipping email owns the email-specific wording and sender call
-    public void Send(Customer customer, string orderId, string trackingNumber)
-    {
-        sender.Send(customer.Name, "order " + orderId + " shipped, tracking " + trackingNumber);
-    }
-}
\ No newline at end of file
diff --git a/pattern/src/CornerStore/Notifications/OrderShippedMessage.cs b/pattern/src/CornerStore/Notifications/OrderShippedMessage.cs
new file mode 100644
index 0000000..f6347e5
--- /dev/null
+++ b/pattern/src/CornerStore/Notifications/OrderShippedMessage.cs
@@ -0,0 +1,9 @@
+namespace CornerStore;
+
+internal sealed class OrderShippedMessage(IChannel channel) : OrderMessage(channel)
+{
+    public void Send(Customer customer, string orderId, string trackingNumber)
+    {
+        Deliver(customer, "order " + orderId + " shipped, tracking " + trackingNumber);
+    }
+}
diff --git a/baseline/src/CornerStore/Notifications/OrderShippedSms.cs b/baseline/src/CornerStore/Notifications/OrderShippedSms.cs
deleted file mode 100644
index 1fe6562..0000000
--- a/baseline/src/CornerStore/Notifications/OrderShippedSms.cs
+++ /dev/null
@@ -1,10 +0,0 @@
-namespace CornerStore;
-
-internal class OrderShippedSms(SmsSender sender)
-{
-    // SMELL S07: shipping SMS owns the SMS-specific wording and sender call
-    public void Send(Customer customer, string orderId, string trackingNumber)
-    {
-        sender.Send(customer.Name, "order " + orderId + " shipped, tracking " + trackingNumber);
-    }
-}
\ No newline at end of file
diff --git a/baseline/src/CornerStore/Notifications/SmsSender.cs b/pattern/src/CornerStore/Notifications/SmsSender.cs
index ec5eb12..bed9779 100644
--- a/baseline/src/CornerStore/Notifications/SmsSender.cs
+++ b/pattern/src/CornerStore/Notifications/SmsSender.cs
@@ -1,6 +1,6 @@
 namespace CornerStore;
-internal class SmsSender
+internal class SmsSender : IChannel
 {
     public void Send(string recipient, string message)
     {
diff --git a/baseline/src/CornerStore/Orders/OrderService.cs b/pattern/src/CornerStore/Orders/OrderService.cs
index 87f6ce2..16d0a86 100644
--- a/baseline/src/CornerStore/Orders/OrderService.cs
+++ b/pattern/src/CornerStore/Orders/OrderService.cs
@@ -3,10 +3,8 @@ namespace CornerStore;
 internal class OrderService(
     IdSequence ids,
     InventoryService inventory,
-    OrderConfirmedEmail confirmedEmail,
-    OrderConfirmedSms confirmedSms,
-    OrderShippedEmail shippedEmail,
-    OrderShippedSms shippedSms,
+    IChannel emailChannel,
+    IChannel smsChannel,
     LoyaltyService loyalty,
     AnalyticsService analytics)
 {
@@ -136,31 +134,24 @@ internal class OrderService(
         if (order.Status == "Paid")
         {
-            if (order.Customer.Channel == "email")
-            {
-                confirmedEmail.Send(order.Customer, order.Id, order.Quote.Total);
-            }
-            else
-            {
-                confirmedSms.Send(order.Customer, order.Id, order.Quote.Total);
-            }
+            new OrderConfirmedMessage(ChannelFor(order.Customer.Channel))
+                .Send(order.Customer, order.Id, order.Quote.Total);
         }
         else if (order.Status == "Shipped")
         {
-            if (order.Customer.Channel == "email")
-            {
-                shippedEmail.Send(order.Customer, order.Id, order.TrackingNumber ?? "");
-            }
-            else
-            {
-                shippedSms.Send(order.Customer, order.Id, order.TrackingNumber ?? "");
-            }
+            new OrderShippedMessage(ChannelFor(order.Customer.Channel))
+                .Send(order.Customer, order.Id, order.TrackingNumber ?? "");
         }
         loyalty.RecordStatusChange(order);
         analytics.RecordStatusChange(order);
     }
+    private IChannel ChannelFor(string channel)
+    {
+        return channel == "email" ? emailChannel : smsChannel;
+    }
+
     private void RestoreStock(Order order)
     {
         foreach (var line in order.Lines)
diff --git a/baseline/src/CornerStore/StoreConsole.cs b/pattern/src/CornerStore/StoreConsole.cs
index a288b4e..dac3161 100644
--- a/baseline/src/CornerStore/StoreConsole.cs
+++ b/pattern/src/CornerStore/StoreConsole.cs
@@ -41,10 +41,8 @@ internal class StoreConsole
         orderService = new OrderService(
             Ids,
             inventory,
-            new OrderConfirmedEmail(emailSender),
-            new OrderConfirmedSms(smsSender),
-            new OrderShippedEmail(emailSender),
-            new OrderShippedSms(smsSender),
+            emailSender,
+            smsSender,
             loyalty,
             analytics);
         backOffice = new BackOffice(inventory, orderService);
diff --git a/pattern/tests/CornerStore.Tests/BridgeTests.cs b/pattern/tests/CornerStore.Tests/BridgeTests.cs
new file mode 100644
index 0000000..43664d9
--- /dev/null
+++ b/pattern/tests/CornerStore.Tests/BridgeTests.cs
@@ -0,0 +1,51 @@
+namespace CornerStore.Tests;
+
+public class BridgeTests
+{
+    [Theory]
+    [InlineData("email")]
+    [InlineData("sms")]
+    public void OrderConfirmedMessage_SendsThroughConfiguredChannel(string channelName)
+    {
+        var channel = new RecordingChannel(channelName);
+        var message = new OrderConfirmedMessage(channel);
+        var customer = new Customer { Name = "Ava", Region = "AU" };
+
+        message.Send(customer, "ord-1", 25m);
+
+        Assert.Equal(channelName, channel.Name);
+        Assert.Equal("Ava", channel.Recipient);
+        Assert.Equal("order ord-1 confirmed, total A$25.00", channel.Message);
+    }
+
+    [Theory]
+    [InlineData("email")]
+    [InlineData("sms")]
+    public void OrderShippedMessage_SendsThroughConfiguredChannel(string channelName)
+    {
+        var channel = new RecordingChannel(channelName);
+        var message = new OrderShippedMessage(channel);
+        var customer = new Customer { Name = "Ava" };
+
+        message.Send(customer, "ord-1", "ship-1");
+
+        Assert.Equal(channelName, channel.Name);
+        Assert.Equal("Ava", channel.Recipient);
+        Assert.Equal("order ord-1 shipped, tracking ship-1", channel.Message);
+    }
+
+    private sealed class RecordingChannel(string name) : IChannel
+    {
+        public string Name { get; } = name;
+
+        public string Recipient { get; private set; } = "";
+
+        public string Message { get; private set; } = "";
+
+        public void Send(string recipient, string message)
+        {
+            Recipient = recipient;
+            Message = message;
+        }
+    }
+}
```
