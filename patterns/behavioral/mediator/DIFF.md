# DIFF: mediator

## File summary

| Status | File | Added | Removed |
| --- | --- | ---: | ---: |
| Modified | CornerStore/CornerStore.csproj | 4 | 0 |
| Modified | CornerStore/StoreConsole.cs | 18 | 29 |
| Added | CornerStore/Widgets/CartMediator.cs | 97 | 0 |
| Modified | CornerStore/Widgets/ItemListWidget.cs | 14 | 27 |
| Modified | CornerStore/Widgets/PromoBoxWidget.cs | 2 | 20 |
| Modified | CornerStore/Widgets/ShippingSelectorWidget.cs | 2 | 20 |
| Modified | CornerStore/Widgets/TotalsPanelWidget.cs | 0 | 20 |
| Added | CornerStore.Tests/MediatorTests.cs | 98 | 0 |

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
diff --git a/baseline/src/CornerStore/StoreConsole.cs b/pattern/src/CornerStore/StoreConsole.cs
index a288b4e..e7a0197 100644
--- a/baseline/src/CornerStore/StoreConsole.cs
+++ b/pattern/src/CornerStore/StoreConsole.cs
@@ -25,7 +25,7 @@ internal class StoreConsole
     private readonly ReportService reports;
     private readonly Dictionary<string, Customer> customers = [];
     private readonly Dictionary<string, Cart> carts = [];
-    private readonly Dictionary<string, ItemListWidget> itemListWidgets = [];
+    private readonly Dictionary<string, CartMediator> cartMediators = [];
     private readonly Dictionary<string, List<CartLine>> savedLines = [];
     private readonly Dictionary<string, string> savedPromo = [];
     private readonly Dictionary<string, string> savedShipping = [];
@@ -267,15 +267,8 @@ internal class StoreConsole
         var cart = new Cart(id);
         carts[id] = cart;
-        var itemListWidget = new ItemListWidget(cart, customer, cartService);
-        var promoBoxWidget = new PromoBoxWidget(cart, customer, cartService, promos);
-        var shippingSelectorWidget = new ShippingSelectorWidget(cart, pricing);
-        var totalsPanelWidget = new TotalsPanelWidget(cart, customer, pricing);
-        itemListWidget.Connect(promoBoxWidget, shippingSelectorWidget, totalsPanelWidget);
-        promoBoxWidget.Connect(itemListWidget, shippingSelectorWidget, totalsPanelWidget);
-        shippingSelectorWidget.Connect(itemListWidget, promoBoxWidget, totalsPanelWidget);
-        totalsPanelWidget.Connect(itemListWidget, promoBoxWidget, shippingSelectorWidget);
-        itemListWidgets[id] = itemListWidget;
+        var mediator = new CartMediator(cart, customer, cartService, promos, pricing);
+        cartMediators[id] = mediator;
         Log.Info("registered " + id + " " + customer.Name + " (" + customer.Region + ", " + customer.Channel + ")");
     }
@@ -533,8 +526,8 @@ internal class StoreConsole
         var giftWrap = cart.GiftWrap;
         var insurance = cart.Insurance;
-        var itemListWidget = itemListWidgets[cart.CustomerId];
-        var error = itemListWidget.Add(sku, quantity);
+        var mediator = cartMediators[cart.CustomerId];
+        var error = mediator.Add(sku, quantity);
         if (error is not null)
         {
             Log.Info("rejected: " + error);
@@ -542,7 +535,6 @@ internal class StoreConsole
         }
         Remember(cart.CustomerId, lines, promo, shipping, giftWrap, insurance);
-        itemListWidget.RefreshAfterChange();
         Log.Info("added " + quantity.ToString(CultureInfo.InvariantCulture) + " x " + sku);
         Log.Info(Summary(cart));
     }
@@ -564,8 +556,8 @@ internal class StoreConsole
         var giftWrap = cart.GiftWrap;
         var insurance = cart.Insurance;
-        var itemListWidget = itemListWidgets[cart.CustomerId];
-        var error = itemListWidget.Remove(sku);
+        var mediator = cartMediators[cart.CustomerId];
+        var error = mediator.Remove(sku);
         if (error is not null)
         {
             Log.Info("rejected: " + error);
@@ -573,7 +565,6 @@ internal class StoreConsole
         }
         Remember(cart.CustomerId, lines, promo, shipping, giftWrap, insurance);
-        itemListWidget.RefreshAfterChange();
         Log.Info("removed " + sku);
         Log.Info(Summary(cart));
     }
@@ -604,7 +595,7 @@ internal class StoreConsole
         savedShipping.Remove(cart.CustomerId);
         savedGiftWrap.Remove(cart.CustomerId);
         savedInsurance.Remove(cart.CustomerId);
-        itemListWidgets[cart.CustomerId].RefreshAfterChange();
+        cartMediators[cart.CustomerId].ItemListChanged();
         Log.Info("undone");
         Log.Info(Summary(cart));
     }
@@ -617,7 +608,7 @@ internal class StoreConsole
             return;
         }
-        itemListWidgets[cart.CustomerId].RefreshAfterChange();
+        cartMediators[cart.CustomerId].ItemListChanged();
         Log.Info(Summary(cart));
     }
@@ -647,7 +638,7 @@ internal class StoreConsole
             return;
         }
-        itemListWidgets[cart.CustomerId].TotalsPanel.RefreshAfterChange();
+        cartMediators[cart.CustomerId].TotalsChanged();
         Log.Info(name + (on ? " added" : " removed"));
     }
@@ -660,21 +651,20 @@ internal class StoreConsole
         }
         var code = Text(step, "code");
-        var promoBoxWidget = itemListWidgets[cart.CustomerId].PromoBox;
-        if (!promoBoxWidget.IsKnown(code))
+        var mediator = cartMediators[cart.CustomerId];
+        if (!mediator.IsKnownPromo(code))
         {
             Log.Info("rejected: unknown promo " + code);
             return;
         }
-        if (!promoBoxWidget.Evaluate(code))
+        if (!mediator.PromoApplies(code))
         {
             Log.Info("rejected: promo " + code + " does not apply to this cart");
             return;
         }
-        promoBoxWidget.Apply(code);
-        promoBoxWidget.RefreshAfterChange();
+        mediator.ApplyPromo(code);
         Log.Info("promo " + code + " applied (" + promos.RuleOf(code) + ")");
     }
@@ -687,15 +677,14 @@ internal class StoreConsole
         }
         var method = Text(step, "method");
-        var shippingSelectorWidget = itemListWidgets[cart.CustomerId].ShippingSelector;
-        if (!shippingSelectorWidget.CanSelect(method))
+        var mediator = cartMediators[cart.CustomerId];
+        if (!mediator.CanSelectShipping(method))
         {
             Log.Info("rejected: unknown shipping method " + method);
             return;
         }
-        shippingSelectorWidget.Select(method);
-        shippingSelectorWidget.RefreshAfterChange();
+        mediator.SelectShipping(method);
         Log.Info("shipping set to " + method);
     }
@@ -713,7 +702,7 @@ internal class StoreConsole
             return;
         }
-        itemListWidgets[cart.CustomerId].TotalsPanel.ShowQuote();
+        cartMediators[cart.CustomerId].ShowQuote();
     }
     private string Summary(Cart cart)
diff --git a/pattern/src/CornerStore/Widgets/CartMediator.cs b/pattern/src/CornerStore/Widgets/CartMediator.cs
new file mode 100644
index 0000000..a18f480
--- /dev/null
+++ b/pattern/src/CornerStore/Widgets/CartMediator.cs
@@ -0,0 +1,97 @@
+namespace CornerStore;
+
+internal sealed class CartMediator
+{
+    private readonly ItemListWidget itemListWidget;
+    private readonly PromoBoxWidget promoBoxWidget;
+    private readonly ShippingSelectorWidget shippingSelectorWidget;
+    private readonly TotalsPanelWidget totalsPanelWidget;
+
+    public CartMediator(Cart cart, Customer customer, CartService cartService, PromoEngine promos, PricingService pricing)
+    {
+        itemListWidget = new ItemListWidget(cart, customer, cartService, this);
+        promoBoxWidget = new PromoBoxWidget(cart, customer, cartService, promos, this);
+        shippingSelectorWidget = new ShippingSelectorWidget(cart, pricing, this);
+        totalsPanelWidget = new TotalsPanelWidget(cart, customer, pricing);
+    }
+
+    public string? Add(string sku, int quantity)
+    {
+        return ItemList.Add(sku, quantity);
+    }
+
+    public string? Remove(string sku)
+    {
+        return ItemList.Remove(sku);
+    }
+
+    public bool IsKnownPromo(string code)
+    {
+        return PromoBox.IsKnown(code);
+    }
+
+    public bool PromoApplies(string code)
+    {
+        return PromoBox.Evaluate(code);
+    }
+
+    public void ApplyPromo(string code)
+    {
+        PromoBox.Apply(code);
+    }
+
+    public bool CanSelectShipping(string method)
+    {
+        return ShippingSelector.CanSelect(method);
+    }
+
+    public void SelectShipping(string method)
+    {
+        ShippingSelector.Select(method);
+    }
+
+    public void ShowQuote()
+    {
+        TotalsPanel.ShowQuote();
+    }
+
+    public void ItemListChanged()
+    {
+        ItemList.Refresh();
+        PromoBox.Refresh();
+        ShippingSelector.Refresh();
+        TotalsPanel.Refresh();
+    }
+
+    public void PromoChanged()
+    {
+        PromoBox.Refresh();
+        ItemList.Refresh();
+        ShippingSelector.Refresh();
+        TotalsPanel.Refresh();
+    }
+
+    public void ShippingChanged()
+    {
+        ShippingSelector.Refresh();
+        ItemList.Refresh();
+        PromoBox.Refresh();
+        TotalsPanel.Refresh();
+    }
+
+    public void TotalsChanged()
+    {
+        TotalsPanel.Refresh();
+        ItemList.Refresh();
+        PromoBox.Refresh();
+        ShippingSelector.Refresh();
+    }
+
+    private ItemListWidget ItemList => itemListWidget;
+
+    private PromoBoxWidget PromoBox => promoBoxWidget;
+
+    private ShippingSelectorWidget ShippingSelector => shippingSelectorWidget;
+
+    private TotalsPanelWidget TotalsPanel => totalsPanelWidget;
+}
diff --git a/baseline/src/CornerStore/Widgets/ItemListWidget.cs b/pattern/src/CornerStore/Widgets/ItemListWidget.cs
index cf06126..87c0343 100644
--- a/baseline/src/CornerStore/Widgets/ItemListWidget.cs
+++ b/pattern/src/CornerStore/Widgets/ItemListWidget.cs
@@ -2,33 +2,28 @@ using System.Globalization;
 namespace CornerStore;
-internal class ItemListWidget(Cart cart, Customer customer, CartService cartService)
+internal class ItemListWidget(Cart cart, Customer customer, CartService cartService, CartMediator mediator)
 {
-    private PromoBoxWidget? promoBox;
-    private ShippingSelectorWidget? shippingSelector;
-    private TotalsPanelWidget? totalsPanel;
-
-    public PromoBoxWidget PromoBox => promoBox ?? throw new InvalidOperationException("widgets are not connected");
-
-    public ShippingSelectorWidget ShippingSelector => shippingSelector ?? throw new InvalidOperationException("widgets are not connected");
-
-    public TotalsPanelWidget TotalsPanel => totalsPanel ?? throw new InvalidOperationException("widgets are not connected");
-
     public string? Add(string sku, int quantity)
     {
-        return cartService.Add(cart, sku, quantity);
+        var error = cartService.Add(cart, sku, quantity);
+        if (error is null)
+        {
+            mediator.ItemListChanged();
+        }
+
+        return error;
     }
     public string? Remove(string sku)
     {
-        return cartService.Remove(cart, sku);
-    }
+        var error = cartService.Remove(cart, sku);
+        if (error is null)
+        {
+            mediator.ItemListChanged();
+        }
-    public void Connect(PromoBoxWidget promoBoxWidget, ShippingSelectorWidget shippingSelectorWidget, TotalsPanelWidget totalsPanelWidget)
-    {
-        promoBox = promoBoxWidget;
-        shippingSelector = shippingSelectorWidget;
-        totalsPanel = totalsPanelWidget;
+        return error;
     }
     public void Refresh()
@@ -43,12 +38,4 @@ internal class ItemListWidget(Cart cart, Customer customer, CartService cartServ
         }
     }
-    public void RefreshAfterChange()
-    {
-        // SMELL S17: this widget refreshes its peers directly after a cart change
-        Refresh();
-        promoBox?.Refresh();
-        shippingSelector?.Refresh();
-        totalsPanel?.Refresh();
-    }
 }
\ No newline at end of file
diff --git a/baseline/src/CornerStore/Widgets/PromoBoxWidget.cs b/pattern/src/CornerStore/Widgets/PromoBoxWidget.cs
index bbe8b9b..ea74c89 100644
--- a/baseline/src/CornerStore/Widgets/PromoBoxWidget.cs
+++ b/pattern/src/CornerStore/Widgets/PromoBoxWidget.cs
@@ -1,18 +1,7 @@
 namespace CornerStore;
-internal class PromoBoxWidget(Cart cart, Customer customer, CartService cartService, PromoEngine promos)
+internal class PromoBoxWidget(Cart cart, Customer customer, CartService cartService, PromoEngine promos, CartMediator mediator)
 {
-    private ItemListWidget? itemList;
-    private ShippingSelectorWidget? shippingSelector;
-    private TotalsPanelWidget? totalsPanel;
-
-    public void Connect(ItemListWidget itemListWidget, ShippingSelectorWidget shippingSelectorWidget, TotalsPanelWidget totalsPanelWidget)
-    {
-        itemList = itemListWidget;
-        shippingSelector = shippingSelectorWidget;
-        totalsPanel = totalsPanelWidget;
-    }
-
     public bool IsKnown(string code)
     {
         return promos.IsKnown(code);
@@ -26,6 +15,7 @@ internal class PromoBoxWidget(Cart cart, Customer customer, CartService cartServ
     public void Apply(string code)
     {
         cart.PromoCode = code;
+        mediator.PromoChanged();
     }
     public void Refresh()
@@ -33,12 +23,4 @@ internal class PromoBoxWidget(Cart cart, Customer customer, CartService cartServ
         Log.Info("promo " + (cart.PromoCode == "" ? "none" : cart.PromoCode));
     }
-    public void RefreshAfterChange()
-    {
-        // SMELL S17: this widget refreshes its peers directly after a promo change
-        Refresh();
-        itemList?.Refresh();
-        shippingSelector?.Refresh();
-        totalsPanel?.Refresh();
-    }
 }
\ No newline at end of file
diff --git a/baseline/src/CornerStore/Widgets/ShippingSelectorWidget.cs b/pattern/src/CornerStore/Widgets/ShippingSelectorWidget.cs
index 9ea73d0..c97b133 100644
--- a/baseline/src/CornerStore/Widgets/ShippingSelectorWidget.cs
+++ b/pattern/src/CornerStore/Widgets/ShippingSelectorWidget.cs
@@ -1,18 +1,7 @@
 namespace CornerStore;
-internal class ShippingSelectorWidget(Cart cart, PricingService pricing)
+internal class ShippingSelectorWidget(Cart cart, PricingService pricing, CartMediator mediator)
 {
-    private ItemListWidget? itemList;
-    private PromoBoxWidget? promoBox;
-    private TotalsPanelWidget? totalsPanel;
-
-    public void Connect(ItemListWidget itemListWidget, PromoBoxWidget promoBoxWidget, TotalsPanelWidget totalsPanelWidget)
-    {
-        itemList = itemListWidget;
-        promoBox = promoBoxWidget;
-        totalsPanel = totalsPanelWidget;
-    }
-
     public bool CanSelect(string method)
     {
         return pricing.ShippingCost(method, 0m) is not null;
@@ -21,6 +10,7 @@ internal class ShippingSelectorWidget(Cart cart, PricingService pricing)
     public void Select(string method)
     {
         cart.ShippingMethod = method;
+        mediator.ShippingChanged();
     }
     public void Refresh()
@@ -28,12 +18,4 @@ internal class ShippingSelectorWidget(Cart cart, PricingService pricing)
         Log.Info("shipping " + cart.ShippingMethod);
     }
-    public void RefreshAfterChange()
-    {
-        // SMELL S17: this widget refreshes its peers directly after a shipping change
-        Refresh();
-        itemList?.Refresh();
-        promoBox?.Refresh();
-        totalsPanel?.Refresh();
-    }
 }
\ No newline at end of file
diff --git a/baseline/src/CornerStore/Widgets/TotalsPanelWidget.cs b/pattern/src/CornerStore/Widgets/TotalsPanelWidget.cs
index 0c0ab23..b349187 100644
--- a/baseline/src/CornerStore/Widgets/TotalsPanelWidget.cs
+++ b/pattern/src/CornerStore/Widgets/TotalsPanelWidget.cs
@@ -4,32 +4,12 @@ namespace CornerStore;
 internal class TotalsPanelWidget(Cart cart, Customer customer, PricingService pricing)
 {
-    private ItemListWidget? itemList;
-    private PromoBoxWidget? promoBox;
-    private ShippingSelectorWidget? shippingSelector;
-
-    public void Connect(ItemListWidget itemListWidget, PromoBoxWidget promoBoxWidget, ShippingSelectorWidget shippingSelectorWidget)
-    {
-        itemList = itemListWidget;
-        promoBox = promoBoxWidget;
-        shippingSelector = shippingSelectorWidget;
-    }
-
     public void Refresh()
     {
         var total = cart.Lines.Count == 0 ? 0m : pricing.BuildQuote(cart, customer).Total;
         Log.Info("totals " + MoneyFormatter.Format(total, customer.Region));
     }
-    public void RefreshAfterChange()
-    {
-        // SMELL S17: this widget refreshes its peers directly after a totals change
-        Refresh();
-        itemList?.Refresh();
-        promoBox?.Refresh();
-        shippingSelector?.Refresh();
-    }
-
     public void ShowQuote()
     {
         var quote = pricing.BuildQuote(cart, customer);
diff --git a/pattern/tests/CornerStore.Tests/MediatorTests.cs b/pattern/tests/CornerStore.Tests/MediatorTests.cs
new file mode 100644
index 0000000..0c8d6b7
--- /dev/null
+++ b/pattern/tests/CornerStore.Tests/MediatorTests.cs
@@ -0,0 +1,98 @@
+namespace CornerStore.Tests;
+
+public class MediatorTests
+{
+    [Fact]
+    public void ItemListChange_RefreshesWidgetsInTheEstablishedOrder()
+    {
+        var mediator = CreateMediator();
+
+        var output = Capture(() => Assert.Null(mediator.Add("SHOE-RUN-42", 1)));
+
+        Assert.Equal(
+            [
+                "  - SHOE-RUN-42 Runner Pro x1 @ A$89.95 = A$89.95",
+                "  promo none",
+                "  shipping standard",
+                "  totals A$105.55",
+            ],
+            output);
+    }
+
+    [Fact]
+    public void PromoChange_RefreshesWidgetsInTheEstablishedOrder()
+    {
+        var mediator = CreateMediator();
+        Assert.Null(mediator.Add("SHOE-RUN-42", 2));
+
+        var output = Capture(() => mediator.ApplyPromo("SHOES10"));
+
+        Assert.Equal(
+            [
+                "  promo SHOES10",
+                "  - SHOE-RUN-42 Runner Pro x2 @ A$89.95 = A$179.90",
+                "  shipping standard",
+                "  totals A$186.30",
+            ],
+            output);
+    }
+
+    [Fact]
+    public void ShippingChange_RefreshesWidgetsInTheEstablishedOrder()
+    {
+        var mediator = CreateMediator();
+        Assert.Null(mediator.Add("SHOE-RUN-42", 1));
+
+        var output = Capture(() => mediator.SelectShipping("express"));
+
+        Assert.Equal(
+            [
+                "  shipping express",
+                "  - SHOE-RUN-42 Runner Pro x1 @ A$89.95 = A$89.95",
+                "  promo none",
+                "  totals A$114.15",
+            ],
+            output);
+    }
+
+    [Fact]
+    public void TotalsChange_RefreshesWidgetsInTheEstablishedOrder()
+    {
+        var mediator = CreateMediator();
+
+        var output = Capture(mediator.TotalsChanged);
+
+        Assert.Equal(
+            [
+                "  totals A$0.00",
+                "  promo none",
+                "  shipping standard",
+            ],
+            output);
+    }
+
+    private static CartMediator CreateMediator()
+    {
+        var cart = new Cart("cu-1");
+        var customer = new Customer { Id = "cu-1", Name = "Ava", Region = "AU", CreditLimit = 800m };
+        var promos = new PromoEngine();
+
+        return new CartMediator(cart, customer, new CartService(new Catalog()), promos, new PricingService(promos));
+    }
+
+    private static string[] Capture(Action action)
+    {
+        var originalOutput = Log.Out;
+        using var output = new StringWriter();
+        try
+        {
+            Log.Out = output;
+            action();
+            return output.ToString().Split('\n', StringSplitOptions.RemoveEmptyEntries);
+        }
+        finally
+        {
+            Log.Out = originalOutput;
+        }
+    }
+}
```
