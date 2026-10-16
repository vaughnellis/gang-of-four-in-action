# DIFF: proxy

## File summary

| Status | File | Added | Removed |
| --- | --- | ---: | ---: |
| Modified | CornerStore/Checkout/OrderValidator.cs | 1 | 1 |
| Modified | CornerStore/CornerStore.csproj | 4 | 0 |
| Added | CornerStore/Inventory/CachingInventoryProxy.cs | 55 | 0 |
| Added | CornerStore/Inventory/IInventoryService.cs | 20 | 0 |
| Modified | CornerStore/Inventory/InventoryService.cs | 21 | 9 |
| Added | CornerStore/Inventory/ProtectionInventoryProxy.cs | 45 | 0 |
| Modified | CornerStore/Orders/BackOffice.cs | 4 | 10 |
| Modified | CornerStore/Orders/OrderService.cs | 3 | 2 |
| Modified | CornerStore/StoreConsole.cs | 13 | 4 |
| Added | CornerStore.Tests/ProxyTests.cs | 73 | 0 |

## Full diff

```diff
diff --git a/baseline/src/CornerStore/Checkout/OrderValidator.cs b/pattern/src/CornerStore/Checkout/OrderValidator.cs
index e7357c9..5c53da7 100644
--- a/baseline/src/CornerStore/Checkout/OrderValidator.cs
+++ b/pattern/src/CornerStore/Checkout/OrderValidator.cs
@@ -2,7 +2,7 @@ using System.Globalization;
 namespace CornerStore;
-internal class OrderValidator(InventoryService inventory, FraudScorer fraudScorer)
+internal class OrderValidator(IInventoryService inventory, FraudScorer fraudScorer)
 {
     public string? Validate(Customer customer, Cart cart, Quote quote)
     {
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
diff --git a/pattern/src/CornerStore/Inventory/CachingInventoryProxy.cs b/pattern/src/CornerStore/Inventory/CachingInventoryProxy.cs
new file mode 100644
index 0000000..d633d27
--- /dev/null
+++ b/pattern/src/CornerStore/Inventory/CachingInventoryProxy.cs
@@ -0,0 +1,55 @@
+namespace CornerStore;
+
+internal sealed class CachingInventoryProxy(
+    IInventoryService inner,
+    IBackOfficeInventory backOfficeInventory,
+    IStockRestorer stockRestorer)
+    : IInventoryService, IBackOfficeInventory, IStockRestorer
+{
+    private readonly Dictionary<string, int> stockCache = new(StringComparer.Ordinal);
+
+    public int GetStock(string sku)
+    {
+        if (!stockCache.TryGetValue(sku, out var available))
+        {
+            available = inner.GetStock(sku);
+            stockCache[sku] = available;
+        }
+
+        return available;
+    }
+
+    public void RestoreStock(string sku, int quantity)
+    {
+        stockRestorer.RestoreStock(sku, quantity);
+        stockCache.Remove(sku);
+    }
+
+    public bool ReserveStock(string sku, int quantity)
+    {
+        var reserved = inner.ReserveStock(sku, quantity);
+        if (reserved)
+        {
+            stockCache.Remove(sku);
+        }
+
+        return reserved;
+    }
+
+    public bool TryGetStaffStock(Customer actor, string sku, out int available)
+    {
+        available = GetStock(sku);
+        return true;
+    }
+
+    public bool TryAdjustStaffStock(Customer actor, string sku, int adjustment, out int newStock)
+    {
+        var adjusted = backOfficeInventory.TryAdjustStaffStock(actor, sku, adjustment, out newStock);
+        if (adjusted)
+        {
+            stockCache[sku] = newStock;
+        }
+
+        return adjusted;
+    }
+}
diff --git a/pattern/src/CornerStore/Inventory/IInventoryService.cs b/pattern/src/CornerStore/Inventory/IInventoryService.cs
new file mode 100644
index 0000000..fbc93b3
--- /dev/null
+++ b/pattern/src/CornerStore/Inventory/IInventoryService.cs
@@ -0,0 +1,20 @@
+namespace CornerStore;
+
+internal interface IInventoryService
+{
+    int GetStock(string sku);
+
+    bool ReserveStock(string sku, int quantity);
+}
+
+internal interface IBackOfficeInventory
+{
+    bool TryGetStaffStock(Customer actor, string sku, out int available);
+
+    bool TryAdjustStaffStock(Customer actor, string sku, int adjustment, out int newStock);
+}
+
+internal interface IStockRestorer
+{
+    void RestoreStock(string sku, int quantity);
+}
diff --git a/baseline/src/CornerStore/Inventory/InventoryService.cs b/pattern/src/CornerStore/Inventory/InventoryService.cs
index 28dca1a..67c85c9 100644
--- a/baseline/src/CornerStore/Inventory/InventoryService.cs
+++ b/pattern/src/CornerStore/Inventory/InventoryService.cs
@@ -1,6 +1,6 @@
 namespace CornerStore;
-internal class InventoryService
+internal class InventoryService : IInventoryService, IBackOfficeInventory, IStockRestorer
 {
     private readonly Dictionary<string, int> stock = new(StringComparer.Ordinal)
     {
@@ -20,20 +20,14 @@ internal class InventoryService
     private int lookupCost;
-    // SMELL S12: each uncached stock lookup adds simulated latency cost
+    internal int LookupCost => lookupCost;
+
     public int GetStock(string sku)
     {
         lookupCost += 10;
         return stock.GetValueOrDefault(sku);
     }
-    // SMELL S12: callers can change stock without an authorization check
-    public int AdjustStock(string sku, int adjustment)
-    {
-        stock[sku] = GetStock(sku) + adjustment;
-        return stock[sku];
-    }
-
     public bool ReserveStock(string sku, int quantity)
     {
         var available = GetStock(sku);
@@ -45,4 +39,22 @@ internal class InventoryService
         stock[sku] = available - quantity;
         return true;
     }
+
+    public void RestoreStock(string sku, int quantity)
+    {
+        stock[sku] = GetStock(sku) + quantity;
+    }
+
+    public bool TryGetStaffStock(Customer actor, string sku, out int available)
+    {
+        available = GetStock(sku);
+        return true;
+    }
+
+    public bool TryAdjustStaffStock(Customer actor, string sku, int adjustment, out int newStock)
+    {
+        newStock = GetStock(sku) + adjustment;
+        stock[sku] = newStock;
+        return true;
+    }
 }
\ No newline at end of file
diff --git a/pattern/src/CornerStore/Inventory/ProtectionInventoryProxy.cs b/pattern/src/CornerStore/Inventory/ProtectionInventoryProxy.cs
new file mode 100644
index 0000000..b227ea7
--- /dev/null
+++ b/pattern/src/CornerStore/Inventory/ProtectionInventoryProxy.cs
@@ -0,0 +1,45 @@
+namespace CornerStore;
+
+internal sealed class ProtectionInventoryProxy(
+    IInventoryService inner,
+    IBackOfficeInventory backOfficeInventory,
+    IStockRestorer stockRestorer)
+    : IInventoryService, IBackOfficeInventory, IStockRestorer
+{
+    public int GetStock(string sku)
+    {
+        return inner.GetStock(sku);
+    }
+
+    public void RestoreStock(string sku, int quantity)
+    {
+        stockRestorer.RestoreStock(sku, quantity);
+    }
+
+    public bool ReserveStock(string sku, int quantity)
+    {
+        return inner.ReserveStock(sku, quantity);
+    }
+
+    public bool TryGetStaffStock(Customer actor, string sku, out int available)
+    {
+        if (actor.Role != "staff")
+        {
+            available = 0;
+            return false;
+        }
+
+        return backOfficeInventory.TryGetStaffStock(actor, sku, out available);
+    }
+
+    public bool TryAdjustStaffStock(Customer actor, string sku, int adjustment, out int newStock)
+    {
+        if (actor.Role != "staff")
+        {
+            newStock = 0;
+            return false;
+        }
+
+        return backOfficeInventory.TryAdjustStaffStock(actor, sku, adjustment, out newStock);
+    }
+}
diff --git a/baseline/src/CornerStore/Orders/BackOffice.cs b/pattern/src/CornerStore/Orders/BackOffice.cs
index aff3031..048e3f9 100644
--- a/baseline/src/CornerStore/Orders/BackOffice.cs
+++ b/pattern/src/CornerStore/Orders/BackOffice.cs
@@ -1,17 +1,15 @@
 namespace CornerStore;
-internal class BackOffice(InventoryService inventory, OrderService orderService)
+internal class BackOffice(IBackOfficeInventory inventory, OrderService orderService)
 {
     public string AdjustStock(Customer actor, string sku, int adjustment)
     {
         // SMELL S14: stock adjustment is an inline back-office action
-        // SMELL S12: the caller checks the staff role before changing stock
-        if (actor.Role != "staff")
+        if (!inventory.TryAdjustStaffStock(actor, sku, adjustment, out var stock))
         {
             return "rejected: staff only";
         }
-        var stock = inventory.AdjustStock(sku, adjustment);
         return "stock " + sku + " adjusted by " + adjustment.ToString(System.Globalization.CultureInfo.InvariantCulture)
             + ", now " + stock.ToString(System.Globalization.CultureInfo.InvariantCulture);
     }
@@ -19,21 +17,18 @@ internal class BackOffice(InventoryService inventory, OrderService orderService)
     public string LookupStock(Customer actor, string sku)
     {
         // SMELL S14: stock lookup is an inline back-office action
-        // SMELL S12: the caller checks the staff role before reading stock
-        if (actor.Role != "staff")
+        if (!inventory.TryGetStaffStock(actor, sku, out var available)
+            || !inventory.TryGetStaffStock(actor, sku, out _))
         {
             return "rejected: staff only";
         }
-        var available = inventory.GetStock(sku);
-        inventory.GetStock(sku);
         return "stock " + sku + " " + available.ToString(System.Globalization.CultureInfo.InvariantCulture);
     }
     public string Refund(Customer actor, Order order)
     {
         // SMELL S14: refund runs inline in the back-office service
-        // SMELL S12: the caller checks the staff role before refunding
         if (actor.Role != "staff")
         {
             return "rejected: staff only";
@@ -46,7 +41,6 @@ internal class BackOffice(InventoryService inventory, OrderService orderService)
     public string Cancel(Customer actor, Order order)
     {
         // SMELL S14: cancellation runs inline in the back-office service
-        // SMELL S12: the caller checks the staff role before cancelling
         if (actor.Role != "staff")
         {
             return "rejected: staff only";
diff --git a/baseline/src/CornerStore/Orders/OrderService.cs b/pattern/src/CornerStore/Orders/OrderService.cs
index 87f6ce2..3786348 100644
--- a/baseline/src/CornerStore/Orders/OrderService.cs
+++ b/pattern/src/CornerStore/Orders/OrderService.cs
@@ -2,7 +2,8 @@ namespace CornerStore;
 internal class OrderService(
     IdSequence ids,
-    InventoryService inventory,
+    IInventoryService inventory,
+    IStockRestorer stockRestorer,
     OrderConfirmedEmail confirmedEmail,
     OrderConfirmedSms confirmedSms,
     OrderShippedEmail shippedEmail,
@@ -165,7 +166,7 @@ internal class OrderService(
     {
         foreach (var line in order.Lines)
         {
-            inventory.AdjustStock(line.Product.Sku, line.Quantity);
+            stockRestorer.RestoreStock(line.Product.Sku, line.Quantity);
         }
         order.StockReserved = false;
diff --git a/baseline/src/CornerStore/StoreConsole.cs b/pattern/src/CornerStore/StoreConsole.cs
index a288b4e..5604351 100644
--- a/baseline/src/CornerStore/StoreConsole.cs
+++ b/pattern/src/CornerStore/StoreConsole.cs
@@ -13,7 +13,9 @@ internal class StoreConsole
     private readonly PromoEngine promos = new();
     private readonly PricingService pricing;
     private readonly ListingImporter importer = new();
-    private readonly InventoryService inventory = new();
+    private readonly IInventoryService inventory;
+    private readonly IBackOfficeInventory backOfficeInventory;
+    private readonly IStockRestorer stockRestorer;
     private readonly EmailSender emailSender = new();
     private readonly SmsSender smsSender = new();
     private readonly LoyaltyService loyalty = new();
@@ -36,18 +38,25 @@ internal class StoreConsole
     public StoreConsole(ScenarioClock clock)
     {
         this.clock = clock;
+        var inventoryService = new InventoryService();
+        var cachingProxy = new CachingInventoryProxy(inventoryService, inventoryService, inventoryService);
+        var protectionProxy = new ProtectionInventoryProxy(cachingProxy, cachingProxy, cachingProxy);
+        inventory = protectionProxy;
+        backOfficeInventory = protectionProxy;
+        stockRestorer = protectionProxy;
         cartService = new CartService(catalog);
         pricing = new PricingService(promos);
         orderService = new OrderService(
             Ids,
             inventory,
+            stockRestorer,
             new OrderConfirmedEmail(emailSender),
             new OrderConfirmedSms(smsSender),
             new OrderShippedEmail(emailSender),
             new OrderShippedSms(smsSender),
             loyalty,
             analytics);
-        backOffice = new BackOffice(inventory, orderService);
+        backOffice = new BackOffice(backOfficeInventory, orderService);
         orderValidator = new OrderValidator(inventory, new FraudScorer());
         shipFastClient = new ShipFastClient(Ids);
         reports = new ReportService(orderService);
@@ -426,7 +435,7 @@ internal class StoreConsole
             {
                 foreach (var reservedLine in reservedLines)
                 {
-                    inventory.AdjustStock(reservedLine.Product.Sku, reservedLine.Quantity);
+                    stockRestorer.RestoreStock(reservedLine.Product.Sku, reservedLine.Quantity);
                 }
                 Log.Info("checkout rejected: stock changed for " + line.Product.Sku);
@@ -443,7 +452,7 @@ internal class StoreConsole
         {
             foreach (var reservedLine in reservedLines)
             {
-                inventory.AdjustStock(reservedLine.Product.Sku, reservedLine.Quantity);
+                stockRestorer.RestoreStock(reservedLine.Product.Sku, reservedLine.Quantity);
             }
             order.StockReserved = false;
diff --git a/pattern/tests/CornerStore.Tests/ProxyTests.cs b/pattern/tests/CornerStore.Tests/ProxyTests.cs
new file mode 100644
index 0000000..facc53d
--- /dev/null
+++ b/pattern/tests/CornerStore.Tests/ProxyTests.cs
@@ -0,0 +1,73 @@
+namespace CornerStore.Tests;
+
+public class ProxyTests
+{
+    [Fact]
+    public void CachingProxy_WhenReadingSkuTwice_UsesOneUnderlyingLookup()
+    {
+        var service = new InventoryService();
+        var proxy = new CachingInventoryProxy(service, service, service);
+
+        Assert.Equal(8, proxy.GetStock("SHOE-RUN-42"));
+        Assert.Equal(8, proxy.GetStock("SHOE-RUN-42"));
+        Assert.Equal(10, service.LookupCost);
+    }
+
+    [Fact]
+    public void CachingProxy_WhenStockIsAdjusted_UpdatesCachedValue()
+    {
+        var service = new InventoryService();
+        var proxy = new CachingInventoryProxy(service, service, service);
+        var staff = new Customer { Role = "staff" };
+
+        Assert.Equal(8, proxy.GetStock("SHOE-RUN-42"));
+        Assert.True(proxy.TryAdjustStaffStock(staff, "SHOE-RUN-42", 2, out var newStock));
+        Assert.Equal(10, newStock);
+        Assert.Equal(10, proxy.GetStock("SHOE-RUN-42"));
+        Assert.Equal(20, service.LookupCost);
+    }
+
+    [Fact]
+    public void CachingProxy_WhenStockIsReserved_InvalidatesCachedValue()
+    {
+        var service = new InventoryService();
+        var proxy = new CachingInventoryProxy(service, service, service);
+
+        Assert.Equal(8, proxy.GetStock("SHOE-RUN-42"));
+        Assert.True(proxy.ReserveStock("SHOE-RUN-42", 3));
+        Assert.Equal(5, proxy.GetStock("SHOE-RUN-42"));
+        Assert.Equal(30, service.LookupCost);
+    }
+
+    [Fact]
+    public void ProtectionProxy_WhenActorIsNotStaff_RejectsStockOperations()
+    {
+        var service = new InventoryService();
+        var cache = new CachingInventoryProxy(service, service, service);
+        var proxy = new ProtectionInventoryProxy(cache, cache, cache);
+        var customer = new Customer { Role = "customer" };
+
+        Assert.False(proxy.TryGetStaffStock(customer, "SHOE-RUN-42", out var available));
+        Assert.Equal(0, available);
+        Assert.False(proxy.TryAdjustStaffStock(customer, "SHOE-RUN-42", 2, out var newStock));
+        Assert.Equal(0, newStock);
+        Assert.Equal(0, service.LookupCost);
+    }
+
+    [Fact]
+    public void ProtectionProxy_WhenActorIsStaff_AllowsStockOperations()
+    {
+        var service = new InventoryService();
+        var cache = new CachingInventoryProxy(service, service, service);
+        var proxy = new ProtectionInventoryProxy(cache, cache, cache);
+        var staff = new Customer { Role = "staff" };
+
+        Assert.True(proxy.TryGetStaffStock(staff, "SHOE-RUN-42", out var available));
+        Assert.Equal(8, available);
+        Assert.True(proxy.TryAdjustStaffStock(staff, "SHOE-RUN-42", 2, out var newStock));
+        Assert.Equal(10, newStock);
+        Assert.True(proxy.TryGetStaffStock(staff, "SHOE-RUN-42", out var updatedStock));
+        Assert.Equal(10, updatedStock);
+        Assert.Equal(20, service.LookupCost);
+    }
+}
```
