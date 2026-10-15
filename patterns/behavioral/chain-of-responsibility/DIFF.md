# DIFF: chain-of-responsibility

## File summary

| Status | File | Added | Removed |
| --- | --- | ---: | ---: |
| Added | CornerStore/Checkout/AddressValidationHandler.cs | 9 | 0 |
| Added | CornerStore/Checkout/CreditLimitValidationHandler.cs | 11 | 0 |
| Added | CornerStore/Checkout/FraudValidationHandler.cs | 15 | 0 |
| Added | CornerStore/Checkout/OrderValidationContext.cs | 3 | 0 |
| Added | CornerStore/Checkout/OrderValidationHandler.cs | 11 | 0 |
| Modified | CornerStore/Checkout/OrderValidator.cs | 14 | 41 |
| Added | CornerStore/Checkout/StockValidationHandler.cs | 24 | 0 |
| Modified | CornerStore/CornerStore.csproj | 4 | 0 |
| Added | CornerStore.Tests/ChainOfResponsibilityTests.cs | 104 | 0 |

## Full diff

```diff
diff --git a/pattern/src/CornerStore/Checkout/AddressValidationHandler.cs b/pattern/src/CornerStore/Checkout/AddressValidationHandler.cs
new file mode 100644
index 0000000..83c19e0
--- /dev/null
+++ b/pattern/src/CornerStore/Checkout/AddressValidationHandler.cs
@@ -0,0 +1,9 @@
+namespace CornerStore;
+
+internal sealed class AddressValidationHandler(OrderValidationHandler? next) : OrderValidationHandler(next)
+{
+    protected override string? Validate(OrderValidationContext context)
+    {
+        return context.Customer.Address == "" ? "address required" : null;
+    }
+}
\ No newline at end of file
diff --git a/pattern/src/CornerStore/Checkout/CreditLimitValidationHandler.cs b/pattern/src/CornerStore/Checkout/CreditLimitValidationHandler.cs
new file mode 100644
index 0000000..aeb5cbc
--- /dev/null
+++ b/pattern/src/CornerStore/Checkout/CreditLimitValidationHandler.cs
@@ -0,0 +1,11 @@
+namespace CornerStore;
+
+internal sealed class CreditLimitValidationHandler(OrderValidationHandler? next) : OrderValidationHandler(next)
+{
+    protected override string? Validate(OrderValidationContext context)
+    {
+        return context.Quote.Total > context.Customer.CreditLimit
+            ? "total exceeds credit limit"
+            : null;
+    }
+}
\ No newline at end of file
diff --git a/pattern/src/CornerStore/Checkout/FraudValidationHandler.cs b/pattern/src/CornerStore/Checkout/FraudValidationHandler.cs
new file mode 100644
index 0000000..90e2db3
--- /dev/null
+++ b/pattern/src/CornerStore/Checkout/FraudValidationHandler.cs
@@ -0,0 +1,15 @@
+using System.Globalization;
+
+namespace CornerStore;
+
+internal sealed class FraudValidationHandler(FraudScorer fraudScorer, OrderValidationHandler? next)
+    : OrderValidationHandler(next)
+{
+    protected override string? Validate(OrderValidationContext context)
+    {
+        var fraudScore = fraudScorer.Score(context.Customer);
+        return fraudScore > Config.FraudThreshold
+            ? "fraud score " + fraudScore.ToString(CultureInfo.InvariantCulture) + " exceeds threshold"
+            : null;
+    }
+}
\ No newline at end of file
diff --git a/pattern/src/CornerStore/Checkout/OrderValidationContext.cs b/pattern/src/CornerStore/Checkout/OrderValidationContext.cs
new file mode 100644
index 0000000..ee11d5e
--- /dev/null
+++ b/pattern/src/CornerStore/Checkout/OrderValidationContext.cs
@@ -0,0 +1,3 @@
+namespace CornerStore;
+
+internal sealed record OrderValidationContext(Customer Customer, Cart Cart, Quote Quote);
\ No newline at end of file
diff --git a/pattern/src/CornerStore/Checkout/OrderValidationHandler.cs b/pattern/src/CornerStore/Checkout/OrderValidationHandler.cs
new file mode 100644
index 0000000..fe89687
--- /dev/null
+++ b/pattern/src/CornerStore/Checkout/OrderValidationHandler.cs
@@ -0,0 +1,11 @@
+namespace CornerStore;
+
+internal abstract class OrderValidationHandler(OrderValidationHandler? next)
+{
+    public string? Handle(OrderValidationContext context)
+    {
+        return Validate(context) ?? next?.Handle(context);
+    }
+
+    protected abstract string? Validate(OrderValidationContext context);
+}
\ No newline at end of file
diff --git a/baseline/src/CornerStore/Checkout/OrderValidator.cs b/pattern/src/CornerStore/Checkout/OrderValidator.cs
index e7357c9..bee4911 100644
--- a/baseline/src/CornerStore/Checkout/OrderValidator.cs
+++ b/pattern/src/CornerStore/Checkout/OrderValidator.cs
@@ -1,48 +1,21 @@
-using System.Globalization;
-
 namespace CornerStore;

-internal class OrderValidator(InventoryService inventory, FraudScorer fraudScorer)
+internal sealed class OrderValidator
 {
-    public string? Validate(Customer customer, Cart cart, Quote quote)
-    {
-        // SMELL S13: checkout validation is one nested method with ordered stock, address, fraud and credit checks
-        string? error = null;
-        var requestedStock = new Dictionary<string, int>(StringComparer.Ordinal);
-        foreach (var line in cart.Lines)
-        {
-            requestedStock[line.Product.Sku] = requestedStock.GetValueOrDefault(line.Product.Sku) + line.Quantity;
-        }
-
-        foreach (var (sku, quantity) in requestedStock.OrderBy(entry => entry.Key, StringComparer.Ordinal))
-        {
-            if (inventory.GetStock(sku) < quantity)
-            {
-                error = "out of stock " + sku;
-                break;
-            }
-        }
+    private readonly OrderValidationHandler firstHandler;

-        if (error is null)
-        {
-            if (customer.Address == "")
-            {
-                error = "address required";
-            }
-            else
-            {
-                var fraudScore = fraudScorer.Score(customer);
-                if (fraudScore > Config.FraudThreshold)
-                {
-                    error = "fraud score " + fraudScore.ToString(CultureInfo.InvariantCulture) + " exceeds threshold";
-                }
-                else if (quote.Total > customer.CreditLimit)
-                {
-                    error = "total exceeds credit limit";
-                }
-            }
-        }
+    public OrderValidator(InventoryService inventory, FraudScorer fraudScorer)
+    {
+        firstHandler = new StockValidationHandler(
+            inventory,
+            new AddressValidationHandler(
+                new FraudValidationHandler(
+                    fraudScorer,
+                    new CreditLimitValidationHandler(null))));
+    }

-        return error;
+    public string? Validate(Customer customer, Cart cart, Quote quote)
+    {
+        return firstHandler.Handle(new OrderValidationContext(customer, cart, quote));
     }
 }
\ No newline at end of file
diff --git a/pattern/src/CornerStore/Checkout/StockValidationHandler.cs b/pattern/src/CornerStore/Checkout/StockValidationHandler.cs
new file mode 100644
index 0000000..a3ed8f7
--- /dev/null
+++ b/pattern/src/CornerStore/Checkout/StockValidationHandler.cs
@@ -0,0 +1,24 @@
+namespace CornerStore;
+
+internal sealed class StockValidationHandler(InventoryService inventory, OrderValidationHandler? next)
+    : OrderValidationHandler(next)
+{
+    protected override string? Validate(OrderValidationContext context)
+    {
+        var requestedStock = new Dictionary<string, int>(StringComparer.Ordinal);
+        foreach (var line in context.Cart.Lines)
+        {
+            requestedStock[line.Product.Sku] = requestedStock.GetValueOrDefault(line.Product.Sku) + line.Quantity;
+        }
+
+        foreach (var (sku, quantity) in requestedStock.OrderBy(entry => entry.Key, StringComparer.Ordinal))
+        {
+            if (inventory.GetStock(sku) < quantity)
+            {
+                return "out of stock " + sku;
+            }
+        }
+
+        return null;
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
diff --git a/pattern/tests/CornerStore.Tests/ChainOfResponsibilityTests.cs b/pattern/tests/CornerStore.Tests/ChainOfResponsibilityTests.cs
new file mode 100644
index 0000000..1451516
--- /dev/null
+++ b/pattern/tests/CornerStore.Tests/ChainOfResponsibilityTests.cs
@@ -0,0 +1,104 @@
+namespace CornerStore.Tests;
+
+public class ChainOfResponsibilityTests
+{
+    [Fact]
+    public void GivenDuplicateSkuLinesAndMissingAddress_Validate_ReturnsSortedAggregatedStockFailureFirst()
+    {
+        var validator = new OrderValidator(new InventoryService(), new FraudScorer());
+        var customer = CreateCustomer(address: "");
+        var cart = new Cart(customer.Id);
+        cart.Lines.Add(CreateLine("MUG-ENAMEL", 9));
+        cart.Lines.Add(CreateLine("KETTLE-1L", 2));
+        cart.Lines.Add(CreateLine("KETTLE-1L", 3));
+
+        var error = validator.Validate(customer, cart, new Quote { Total = 1m });
+
+        Assert.Equal("out of stock KETTLE-1L", error);
+    }
+
+    [Fact]
+    public void GivenMissingAddress_Validate_ReturnsAddressFailure()
+    {
+        var oldThreshold = Config.FraudThreshold;
+        Config.FraudThreshold = 100;
+        try
+        {
+            var validator = new OrderValidator(new InventoryService(), new FraudScorer());
+            var customer = CreateCustomer(address: "");
+
+            var error = validator.Validate(customer, new Cart(customer.Id), new Quote { Total = 1m });
+
+            Assert.Equal("address required", error);
+        }
+        finally
+        {
+            Config.FraudThreshold = oldThreshold;
+        }
+    }
+
+    [Fact]
+    public void GivenFraudScoreAboveThreshold_Validate_ReturnsFraudFailureBeforeCredit()
+    {
+        var oldThreshold = Config.FraudThreshold;
+        Config.FraudThreshold = -1;
+        try
+        {
+            var validator = new OrderValidator(new InventoryService(), new FraudScorer());
+            var customer = CreateCustomer(creditLimit: 0m);
+
+            var error = validator.Validate(customer, new Cart(customer.Id), new Quote { Total = 1m });
+
+            Assert.StartsWith("fraud score ", error);
+            Assert.EndsWith(" exceeds threshold", error);
+        }
+        finally
+        {
+            Config.FraudThreshold = oldThreshold;
+        }
+    }
+
+    [Fact]
+    public void GivenTotalAboveCreditLimit_Validate_ReturnsCreditFailure()
+    {
+        var oldThreshold = Config.FraudThreshold;
+        Config.FraudThreshold = 100;
+        try
+        {
+            var validator = new OrderValidator(new InventoryService(), new FraudScorer());
+            var customer = CreateCustomer(creditLimit: 1m);
+
+            var error = validator.Validate(customer, new Cart(customer.Id), new Quote { Total = 2m });
+
+            Assert.Equal("total exceeds credit limit", error);
+        }
+        finally
+        {
+            Config.FraudThreshold = oldThreshold;
+        }
+    }
+
+    private static CartLine CreateLine(string sku, int quantity)
+    {
+        return new CartLine
+        {
+            Product = new PhysicalProduct { Sku = sku },
+            Quantity = quantity,
+            UnitPrice = 10m,
+        };
+    }
+
+    private static Customer CreateCustomer(string address = "1 Harbour St", decimal creditLimit = 500m)
+    {
+        return new Customer
+        {
+            Id = "cu-1",
+            Number = 1,
+            Name = "Ava",
+            Region = "AU",
+            Channel = "email",
+            Address = address,
+            CreditLimit = creditLimit,
+        };
+    }
+}
\ No newline at end of file
```
