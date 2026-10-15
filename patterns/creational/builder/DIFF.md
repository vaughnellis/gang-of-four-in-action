# DIFF: builder

## File summary

| Status | File | Added | Removed |
| --- | --- | ---: | ---: |
| Modified | CornerStore/CornerStore.csproj | 4 | 0 |
| Added | CornerStore/Orders/InvoiceBuilder.cs | 41 | 0 |
| Added | CornerStore/Orders/OrderBuilder.cs | 116 | 0 |
| Modified | CornerStore/Orders/OrderService.cs | 11 | 10 |
| Added | CornerStore.Tests/BuilderTests.cs | 93 | 0 |

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
diff --git a/pattern/src/CornerStore/Orders/InvoiceBuilder.cs b/pattern/src/CornerStore/Orders/InvoiceBuilder.cs
new file mode 100644
index 0000000..0ab040d
--- /dev/null
+++ b/pattern/src/CornerStore/Orders/InvoiceBuilder.cs
@@ -0,0 +1,41 @@
+namespace CornerStore;
+
+internal sealed class InvoiceBuilder
+{
+    private Order? order;
+    private string? note;
+
+    public InvoiceBuilder ForOrder(Order value)
+    {
+        order = value;
+        return this;
+    }
+
+    public InvoiceBuilder WithNote(string? value)
+    {
+        note = value;
+        return this;
+    }
+
+    public Invoice Build()
+    {
+        if (order is null)
+        {
+            throw new InvalidOperationException("invoice order is required");
+        }
+
+        var quote = order.Quote;
+        return new Invoice(
+            order.Id + "-I",
+            order.Id,
+            order.Customer,
+            order.Lines,
+            quote.Subtotal,
+            quote.Discount,
+            quote.AddOns,
+            quote.Shipping,
+            quote.Tax,
+            quote.Total,
+            note);
+    }
+}
\ No newline at end of file
diff --git a/pattern/src/CornerStore/Orders/OrderBuilder.cs b/pattern/src/CornerStore/Orders/OrderBuilder.cs
new file mode 100644
index 0000000..f4e6f42
--- /dev/null
+++ b/pattern/src/CornerStore/Orders/OrderBuilder.cs
@@ -0,0 +1,116 @@
+namespace CornerStore;
+
+internal sealed class OrderBuilder
+{
+    private string? id;
+    private Customer? customer;
+    private List<CartLine>? lines;
+    private Quote? quote;
+    private string? paymentType;
+    private string? promoCode;
+    private string? giftMessage;
+    private string? note;
+    private string? trackingNumber;
+    private string? cancellationReason;
+
+    public OrderBuilder WithId(string value)
+    {
+        id = value;
+        return this;
+    }
+
+    public OrderBuilder WithCustomer(Customer value)
+    {
+        customer = value;
+        return this;
+    }
+
+    public OrderBuilder WithLines(List<CartLine> value)
+    {
+        lines = value;
+        return this;
+    }
+
+    public OrderBuilder WithQuote(Quote value)
+    {
+        quote = value;
+        return this;
+    }
+
+    public OrderBuilder WithPaymentType(string value)
+    {
+        paymentType = value;
+        return this;
+    }
+
+    public OrderBuilder WithPromoCode(string? value)
+    {
+        promoCode = value;
+        return this;
+    }
+
+    public OrderBuilder WithGiftMessage(string? value)
+    {
+        giftMessage = value;
+        return this;
+    }
+
+    public OrderBuilder WithNote(string? value)
+    {
+        note = value;
+        return this;
+    }
+
+    public OrderBuilder WithTrackingNumber(string? value)
+    {
+        trackingNumber = value;
+        return this;
+    }
+
+    public OrderBuilder WithCancellationReason(string? value)
+    {
+        cancellationReason = value;
+        return this;
+    }
+
+    public Order Build()
+    {
+        if (id is null)
+        {
+            throw new InvalidOperationException("order id is required");
+        }
+
+        if (customer is null)
+        {
+            throw new InvalidOperationException("order customer is required");
+        }
+
+        if (lines is null)
+        {
+            throw new InvalidOperationException("order lines are required");
+        }
+
+        if (quote is null)
+        {
+            throw new InvalidOperationException("order quote is required");
+        }
+
+        if (paymentType is null)
+        {
+            throw new InvalidOperationException("order payment type is required");
+        }
+
+        return new Order(
+            id,
+            customer,
+            "Pending",
+            lines,
+            quote,
+            paymentType,
+            promoCode,
+            giftMessage,
+            note,
+            trackingNumber,
+            cancellationReason);
+    }
+}
\ No newline at end of file
diff --git a/baseline/src/CornerStore/Orders/OrderService.cs b/pattern/src/CornerStore/Orders/OrderService.cs
index 87f6ce2..8491766 100644
--- a/baseline/src/CornerStore/Orders/OrderService.cs
+++ b/pattern/src/CornerStore/Orders/OrderService.cs
@@ -15,11 +15,14 @@ internal class OrderService(

     public Order Create(Customer customer, Cart cart, Quote quote, string paymentType)
     {
-        // SMELL S02: the order is assembled with eleven positional arguments and null optionals
-        var order = new Order(
-            ids.NextOrderId(), customer, "Pending",
-            cart.Lines.Select(CopyLine).ToList(), quote, paymentType,
-            cart.PromoCode == "" ? null : cart.PromoCode, null, null, null, null);
+        var order = new OrderBuilder()
+            .WithId(ids.NextOrderId())
+            .WithCustomer(customer)
+            .WithLines(cart.Lines.Select(CopyLine).ToList())
+            .WithQuote(quote)
+            .WithPaymentType(paymentType)
+            .WithPromoCode(cart.PromoCode == "" ? null : cart.PromoCode)
+            .Build();
         orders[order.Id] = order;
         CreateInvoice(order);
         return order;
@@ -27,11 +30,9 @@ internal class OrderService(

     public Invoice CreateInvoice(Order order)
     {
-        var quote = order.Quote;
-        // SMELL S02: invoice parts are assembled with eleven positional arguments and a null note
-        var invoice = new Invoice(
-            order.Id + "-I", order.Id, order.Customer, order.Lines,
-            quote.Subtotal, quote.Discount, quote.AddOns, quote.Shipping, quote.Tax, quote.Total, null);
+        var invoice = new InvoiceBuilder()
+            .ForOrder(order)
+            .Build();
         invoices.Add(invoice);
         return invoice;
     }
diff --git a/pattern/tests/CornerStore.Tests/BuilderTests.cs b/pattern/tests/CornerStore.Tests/BuilderTests.cs
new file mode 100644
index 0000000..e674075
--- /dev/null
+++ b/pattern/tests/CornerStore.Tests/BuilderTests.cs
@@ -0,0 +1,93 @@
+namespace CornerStore.Tests;
+
+public class BuilderTests
+{
+    [Fact]
+    public void GivenRequiredOrderValues_Build_CreatesOrderWithOptionalDefaults()
+    {
+        var customer = CreateCustomer();
+        var quote = new Quote { Subtotal = 10m, Total = 12m };
+        var lines = new List<CartLine>();
+
+        var order = new OrderBuilder()
+            .WithId("o-1001")
+            .WithCustomer(customer)
+            .WithLines(lines)
+            .WithQuote(quote)
+            .WithPaymentType("card")
+            .Build();
+
+        Assert.Equal("o-1001", order.Id);
+        Assert.Equal("Pending", order.Status);
+        Assert.Same(customer, order.Customer);
+        Assert.Same(lines, order.Lines);
+        Assert.Same(quote, order.Quote);
+        Assert.Null(order.PromoCode);
+        Assert.Null(order.GiftMessage);
+        Assert.Null(order.Note);
+        Assert.Null(order.TrackingNumber);
+        Assert.Null(order.CancellationReason);
+    }
+
+    [Fact]
+    public void GivenCompleteOrder_BuildInvoice_CopiesQuoteValues()
+    {
+        var order = new OrderBuilder()
+            .WithId("o-1001")
+            .WithCustomer(CreateCustomer())
+            .WithLines([])
+            .WithQuote(new Quote
+            {
+                Subtotal = 20m,
+                Discount = 2m,
+                AddOns = 4.50m,
+                Shipping = 5m,
+                Tax = 2.25m,
+                Total = 29.75m,
+            })
+            .WithPaymentType("paypal")
+            .Build();
+
+        var invoice = new InvoiceBuilder().ForOrder(order).Build();
+
+        Assert.Equal("o-1001-I", invoice.Id);
+        Assert.Equal(order.Id, invoice.OrderId);
+        Assert.Equal(20m, invoice.Subtotal);
+        Assert.Equal(2m, invoice.Discount);
+        Assert.Equal(4.50m, invoice.AddOns);
+        Assert.Equal(5m, invoice.Shipping);
+        Assert.Equal(2.25m, invoice.Tax);
+        Assert.Equal(29.75m, invoice.Total);
+        Assert.Null(invoice.Note);
+    }
+
+    [Fact]
+    public void GivenMissingOrderId_Build_ThrowsClearValidationError()
+    {
+        var exception = Assert.Throws<InvalidOperationException>(() => new OrderBuilder().Build());
+
+        Assert.Equal("order id is required", exception.Message);
+    }
+
+    [Fact]
+    public void GivenMissingInvoiceOrder_Build_ThrowsClearValidationError()
+    {
+        var exception = Assert.Throws<InvalidOperationException>(() => new InvoiceBuilder().Build());
+
+        Assert.Equal("invoice order is required", exception.Message);
+    }
+
+    private static Customer CreateCustomer()
+    {
+        return new Customer
+        {
+            Id = "cu-1",
+            Number = 1,
+            Name = "Ava",
+            Region = "AU",
+            Channel = "email",
+            Address = "1 Harbour St",
+            CreditLimit = 500m,
+        };
+    }
+}
\ No newline at end of file
```
