# DIFF: memento

## File summary

| Status | File | Added | Removed |
| --- | --- | ---: | ---: |
| Modified | CornerStore/Cart/Cart.cs | 46 | 1 |
| Added | CornerStore/Cart/CartMemento.cs | 29 | 0 |
| Modified | CornerStore/CornerStore.csproj | 4 | 0 |
| Modified | CornerStore/StoreConsole.cs | 8 | 59 |
| Added | CornerStore.Tests/MementoTests.cs | 62 | 0 |

## Full diff

```diff
diff --git a/baseline/src/CornerStore/Cart/Cart.cs b/pattern/src/CornerStore/Cart/Cart.cs
index a08dcb6..d461958 100644
--- a/baseline/src/CornerStore/Cart/Cart.cs
+++ b/pattern/src/CornerStore/Cart/Cart.cs
@@ -1,6 +1,5 @@
 namespace CornerStore;
-// SMELL S18: every internal of the cart is a public field, so a caller can copy the state out and back in
 internal class Cart(string customerId)
 {
     public string CustomerId = customerId;
@@ -15,4 +14,50 @@ internal class Cart(string customerId)
     public bool GiftWrap;
     public bool Insurance;
+
+    public CartMemento CreateMemento()
+    {
+        return new CartMemento(
+            CopyLines(Lines),
+            PromoCode,
+            ShippingMethod,
+            GiftWrap,
+            Insurance);
+    }
+
+    public void Restore(CartMemento memento)
+    {
+        memento.Restore(this);
+    }
+
+    internal void RestoreState(
+        List<CartLine> lines,
+        string promoCode,
+        string shippingMethod,
+        bool giftWrap,
+        bool insurance)
+    {
+        Lines = CopyLines(lines);
+        PromoCode = promoCode;
+        ShippingMethod = shippingMethod;
+        GiftWrap = giftWrap;
+        Insurance = insurance;
+    }
+
+    private static List<CartLine> CopyLines(List<CartLine> lines)
+    {
+        var copy = new List<CartLine>();
+        foreach (var line in lines)
+        {
+            copy.Add(new CartLine
+            {
+                Product = line.Product,
+                Quantity = line.Quantity,
+                UnitPrice = line.UnitPrice,
+                FromBundle = line.FromBundle,
+            });
+        }
+
+        return copy;
+    }
 }
diff --git a/pattern/src/CornerStore/Cart/CartMemento.cs b/pattern/src/CornerStore/Cart/CartMemento.cs
new file mode 100644
index 0000000..d6ccf71
--- /dev/null
+++ b/pattern/src/CornerStore/Cart/CartMemento.cs
@@ -0,0 +1,29 @@
+namespace CornerStore;
+
+internal sealed class CartMemento
+{
+    private readonly List<CartLine> lines;
+    private readonly string promoCode;
+    private readonly string shippingMethod;
+    private readonly bool giftWrap;
+    private readonly bool insurance;
+
+    internal CartMemento(
+        List<CartLine> lines,
+        string promoCode,
+        string shippingMethod,
+        bool giftWrap,
+        bool insurance)
+    {
+        this.lines = lines;
+        this.promoCode = promoCode;
+        this.shippingMethod = shippingMethod;
+        this.giftWrap = giftWrap;
+        this.insurance = insurance;
+    }
+
+    internal void Restore(Cart cart)
+    {
+        cart.RestoreState(lines, promoCode, shippingMethod, giftWrap, insurance);
+    }
+}
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
index a288b4e..193210a 100644
--- a/baseline/src/CornerStore/StoreConsole.cs
+++ b/pattern/src/CornerStore/StoreConsole.cs
@@ -26,11 +26,7 @@ internal class StoreConsole
     private readonly Dictionary<string, Customer> customers = [];
     private readonly Dictionary<string, Cart> carts = [];
     private readonly Dictionary<string, ItemListWidget> itemListWidgets = [];
-    private readonly Dictionary<string, List<CartLine>> savedLines = [];
-    private readonly Dictionary<string, string> savedPromo = [];
-    private readonly Dictionary<string, string> savedShipping = [];
-    private readonly Dictionary<string, bool> savedGiftWrap = [];
-    private readonly Dictionary<string, bool> savedInsurance = [];
+    private readonly Dictionary<string, CartMemento> savedMementos = [];
     private List<Listing> listings = [];
     public StoreConsole(ScenarioClock clock)
@@ -526,12 +522,7 @@ internal class StoreConsole
         var sku = Text(step, "sku");
         var quantity = step.TryGetProperty("qty", out var value) && value.TryGetInt32(out var number) ? number : 1;
-        // SMELL S18: the caller copies the cart's public internals out before the change
-        var lines = CopyLines(cart.Lines);
-        var promo = cart.PromoCode;
-        var shipping = cart.ShippingMethod;
-        var giftWrap = cart.GiftWrap;
-        var insurance = cart.Insurance;
+        var memento = cart.CreateMemento();
         var itemListWidget = itemListWidgets[cart.CustomerId];
         var error = itemListWidget.Add(sku, quantity);
@@ -541,7 +532,7 @@ internal class StoreConsole
             return;
         }
-        Remember(cart.CustomerId, lines, promo, shipping, giftWrap, insurance);
+        savedMementos[cart.CustomerId] = memento;
         itemListWidget.RefreshAfterChange();
         Log.Info("added " + quantity.ToString(CultureInfo.InvariantCulture) + " x " + sku);
         Log.Info(Summary(cart));
@@ -557,12 +548,7 @@ internal class StoreConsole
         var sku = Text(step, "sku");
-        // SMELL S18: the caller copies the cart's public internals out before the change
-        var lines = CopyLines(cart.Lines);
-        var promo = cart.PromoCode;
-        var shipping = cart.ShippingMethod;
-        var giftWrap = cart.GiftWrap;
-        var insurance = cart.Insurance;
+        var memento = cart.CreateMemento();
         var itemListWidget = itemListWidgets[cart.CustomerId];
         var error = itemListWidget.Remove(sku);
@@ -572,7 +558,7 @@ internal class StoreConsole
             return;
         }
-        Remember(cart.CustomerId, lines, promo, shipping, giftWrap, insurance);
+        savedMementos[cart.CustomerId] = memento;
         itemListWidget.RefreshAfterChange();
         Log.Info("removed " + sku);
         Log.Info(Summary(cart));
@@ -586,24 +572,14 @@ internal class StoreConsole
             return;
         }
-        if (!savedLines.TryGetValue(cart.CustomerId, out var lines))
+        if (!savedMementos.TryGetValue(cart.CustomerId, out var memento))
         {
             Log.Info("nothing to undo");
             return;
         }
-        // SMELL S18: the caller writes the saved internals straight back into the cart
-        cart.Lines = lines;
-        cart.PromoCode = savedPromo[cart.CustomerId];
-        cart.ShippingMethod = savedShipping[cart.CustomerId];
-        cart.GiftWrap = savedGiftWrap[cart.CustomerId];
-        cart.Insurance = savedInsurance[cart.CustomerId];
-
-        savedLines.Remove(cart.CustomerId);
-        savedPromo.Remove(cart.CustomerId);
-        savedShipping.Remove(cart.CustomerId);
-        savedGiftWrap.Remove(cart.CustomerId);
-        savedInsurance.Remove(cart.CustomerId);
+        cart.Restore(memento);
+        savedMementos.Remove(cart.CustomerId);
         itemListWidgets[cart.CustomerId].RefreshAfterChange();
         Log.Info("undone");
         Log.Info(Summary(cart));
@@ -722,33 +698,6 @@ internal class StoreConsole
             + " subtotal=" + MoneyFormatter.Format(cartService.Subtotal(cart), customers[cart.CustomerId].Region);
     }
-    // SMELL S18: the caller has to know every field of a cart line to copy it
-    private static List<CartLine> CopyLines(List<CartLine> lines)
-    {
-        var copy = new List<CartLine>();
-        foreach (var line in lines)
-        {
-            copy.Add(new CartLine
-            {
-                Product = line.Product,
-                Quantity = line.Quantity,
-                UnitPrice = line.UnitPrice,
-                FromBundle = line.FromBundle,
-            });
-        }
-
-        return copy;
-    }
-
-    private void Remember(string customerId, List<CartLine> lines, string promo, string shipping, bool giftWrap, bool insurance)
-    {
-        savedLines[customerId] = lines;
-        savedPromo[customerId] = promo;
-        savedShipping[customerId] = shipping;
-        savedGiftWrap[customerId] = giftWrap;
-        savedInsurance[customerId] = insurance;
-    }
-
     private void SetConfig(JsonElement step)
     {
         if (!step.TryGetProperty("key", out var key)
diff --git a/pattern/tests/CornerStore.Tests/MementoTests.cs b/pattern/tests/CornerStore.Tests/MementoTests.cs
new file mode 100644
index 0000000..bed25b8
--- /dev/null
+++ b/pattern/tests/CornerStore.Tests/MementoTests.cs
@@ -0,0 +1,62 @@
+namespace CornerStore.Tests;
+
+public class MementoTests
+{
+    [Fact]
+    public void Cart_Restore_ReturnsAllMutableStateToTheCapturedValues()
+    {
+        var cart = CreateCart();
+        var memento = cart.CreateMemento();
+        cart.Lines[0].Quantity = 5;
+        cart.Lines[0].UnitPrice = 20m;
+        cart.PromoCode = "OTHER";
+        cart.ShippingMethod = "express";
+        cart.GiftWrap = false;
+        cart.Insurance = false;
+
+        cart.Restore(memento);
+
+        Assert.Equal(2, cart.Lines[0].Quantity);
+        Assert.Equal(12.50m, cart.Lines[0].UnitPrice);
+        Assert.Equal("SKU-1", cart.Lines[0].Product.Sku);
+        Assert.Equal("BUN-1", cart.Lines[0].FromBundle);
+        Assert.Equal("SAVE", cart.PromoCode);
+        Assert.Equal("pickup", cart.ShippingMethod);
+        Assert.True(cart.GiftWrap);
+        Assert.True(cart.Insurance);
+    }
+
+    [Fact]
+    public void CartMemento_DoesNotShareMutableLineStateWithTheCart()
+    {
+        var cart = CreateCart();
+        var memento = cart.CreateMemento();
+        cart.Lines[0].Quantity = 5;
+        cart.Restore(memento);
+        cart.Lines[0].Quantity = 9;
+
+        cart.Restore(memento);
+
+        Assert.Equal(2, cart.Lines[0].Quantity);
+    }
+
+    private static Cart CreateCart()
+    {
+        var cart = new Cart("cu-1")
+        {
+            PromoCode = "SAVE",
+            ShippingMethod = "pickup",
+            GiftWrap = true,
+            Insurance = true,
+        };
+        cart.Lines.Add(new CartLine
+        {
+            Product = new PhysicalProduct { Sku = "SKU-1", Name = "Sample" },
+            Quantity = 2,
+            UnitPrice = 12.50m,
+            FromBundle = "BUN-1",
+        });
+
+        return cart;
+    }
+}
```
