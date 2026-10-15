# DIFF: decorator

## File summary

| Status | File | Added | Removed |
| --- | --- | ---: | ---: |
| Modified | CornerStore/Cart/Cart.cs | 0 | 1 |
| Modified | CornerStore/CornerStore.csproj | 4 | 0 |
| Added | CornerStore/Pricing/BaseAddOnPriceComponent.cs | 6 | 0 |
| Added | CornerStore/Pricing/GiftWrapPriceDecorator.cs | 6 | 0 |
| Added | CornerStore/Pricing/IAddOnPriceComponent.cs | 6 | 0 |
| Added | CornerStore/Pricing/InsurancePriceDecorator.cs | 11 | 0 |
| Modified | CornerStore/Pricing/PricingService.cs | 4 | 13 |
| Modified | CornerStore/StoreConsole.cs | 0 | 1 |
| Added | CornerStore.Tests/DecoratorTests.cs | 42 | 0 |

## Full diff

```diff
diff --git a/baseline/src/CornerStore/Cart/Cart.cs b/pattern/src/CornerStore/Cart/Cart.cs
index a08dcb6..f758792 100644
--- a/baseline/src/CornerStore/Cart/Cart.cs
+++ b/pattern/src/CornerStore/Cart/Cart.cs
@@ -11,7 +11,6 @@ internal class Cart(string customerId)

     public string ShippingMethod = "standard";

-    // SMELL S09: add-ons are plain boolean flags, one per add-on
     public bool GiftWrap;

     public bool Insurance;
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
diff --git a/pattern/src/CornerStore/Pricing/BaseAddOnPriceComponent.cs b/pattern/src/CornerStore/Pricing/BaseAddOnPriceComponent.cs
new file mode 100644
index 0000000..05716c7
--- /dev/null
+++ b/pattern/src/CornerStore/Pricing/BaseAddOnPriceComponent.cs
@@ -0,0 +1,6 @@
+namespace CornerStore;
+
+internal sealed class BaseAddOnPriceComponent : IAddOnPriceComponent
+{
+    public decimal Cost => 0m;
+}
\ No newline at end of file
diff --git a/pattern/src/CornerStore/Pricing/GiftWrapPriceDecorator.cs b/pattern/src/CornerStore/Pricing/GiftWrapPriceDecorator.cs
new file mode 100644
index 0000000..79c4924
--- /dev/null
+++ b/pattern/src/CornerStore/Pricing/GiftWrapPriceDecorator.cs
@@ -0,0 +1,6 @@
+namespace CornerStore;
+
+internal sealed class GiftWrapPriceDecorator(IAddOnPriceComponent component, bool enabled) : IAddOnPriceComponent
+{
+    public decimal Cost => component.Cost + (enabled ? 4.50m : 0m);
+}
\ No newline at end of file
diff --git a/pattern/src/CornerStore/Pricing/IAddOnPriceComponent.cs b/pattern/src/CornerStore/Pricing/IAddOnPriceComponent.cs
new file mode 100644
index 0000000..977d749
--- /dev/null
+++ b/pattern/src/CornerStore/Pricing/IAddOnPriceComponent.cs
@@ -0,0 +1,6 @@
+namespace CornerStore;
+
+internal interface IAddOnPriceComponent
+{
+    decimal Cost { get; }
+}
\ No newline at end of file
diff --git a/pattern/src/CornerStore/Pricing/InsurancePriceDecorator.cs b/pattern/src/CornerStore/Pricing/InsurancePriceDecorator.cs
new file mode 100644
index 0000000..88051ef
--- /dev/null
+++ b/pattern/src/CornerStore/Pricing/InsurancePriceDecorator.cs
@@ -0,0 +1,11 @@
+namespace CornerStore;
+
+internal sealed class InsurancePriceDecorator(
+    IAddOnPriceComponent component,
+    bool enabled,
+    decimal discountedSubtotal) : IAddOnPriceComponent
+{
+    public decimal Cost => component.Cost + (enabled
+        ? Math.Round(discountedSubtotal * 0.02m, 2, MidpointRounding.AwayFromZero)
+        : 0m);
+}
\ No newline at end of file
diff --git a/baseline/src/CornerStore/Pricing/PricingService.cs b/pattern/src/CornerStore/Pricing/PricingService.cs
index 64d5dd4..363f52a 100644
--- a/baseline/src/CornerStore/Pricing/PricingService.cs
+++ b/pattern/src/CornerStore/Pricing/PricingService.cs
@@ -73,18 +73,9 @@ internal class PricingService(PromoEngine promos)

     private static decimal AddOnCost(Cart cart, decimal discountedSubtotal)
     {
-        // SMELL S09: each add-on is a boolean flag checked in an if-chain
-        var cost = 0m;
-        if (cart.GiftWrap)
-        {
-            cost += 4.50m;
-        }
-
-        if (cart.Insurance)
-        {
-            cost += Math.Round(discountedSubtotal * 0.02m, 2, MidpointRounding.AwayFromZero);
-        }
-
-        return cost;
+        IAddOnPriceComponent component = new BaseAddOnPriceComponent();
+        component = new GiftWrapPriceDecorator(component, cart.GiftWrap);
+        component = new InsurancePriceDecorator(component, cart.Insurance, discountedSubtotal);
+        return component.Cost;
     }
 }
diff --git a/baseline/src/CornerStore/StoreConsole.cs b/pattern/src/CornerStore/StoreConsole.cs
index a288b4e..4d08e3d 100644
--- a/baseline/src/CornerStore/StoreConsole.cs
+++ b/pattern/src/CornerStore/StoreConsole.cs
@@ -632,7 +632,6 @@ internal class StoreConsole
         var name = Text(step, "addon");
         var on = !step.TryGetProperty("on", out var flag) || flag.ValueKind != JsonValueKind.False;

-        // SMELL S09: the console has to know each add-on flag by name to set it
         if (name == "giftwrap")
         {
             cart.GiftWrap = on;
diff --git a/pattern/tests/CornerStore.Tests/DecoratorTests.cs b/pattern/tests/CornerStore.Tests/DecoratorTests.cs
new file mode 100644
index 0000000..4332b52
--- /dev/null
+++ b/pattern/tests/CornerStore.Tests/DecoratorTests.cs
@@ -0,0 +1,42 @@
+namespace CornerStore.Tests;
+
+public class DecoratorTests
+{
+    [Fact]
+    public void GivenBaseComponent_Cost_IsZero()
+    {
+        var component = new BaseAddOnPriceComponent();
+
+        Assert.Equal(0m, component.Cost);
+    }
+
+    [Fact]
+    public void GivenGiftWrapDecorator_Cost_AddsFixedCharge()
+    {
+        IAddOnPriceComponent component = new BaseAddOnPriceComponent();
+
+        component = new GiftWrapPriceDecorator(component, true);
+
+        Assert.Equal(4.50m, component.Cost);
+    }
+
+    [Fact]
+    public void GivenInsuranceDecorator_Cost_AddsRoundedDiscountedSubtotalCharge()
+    {
+        IAddOnPriceComponent component = new BaseAddOnPriceComponent();
+
+        component = new InsurancePriceDecorator(component, true, 179.90m);
+
+        Assert.Equal(3.60m, component.Cost);
+    }
+
+    [Fact]
+    public void GivenBothDecorators_Cost_ExcludesGiftWrapFromInsuranceBasis()
+    {
+        IAddOnPriceComponent component = new BaseAddOnPriceComponent();
+        component = new GiftWrapPriceDecorator(component, true);
+        component = new InsurancePriceDecorator(component, true, 100m);
+
+        Assert.Equal(6.50m, component.Cost);
+    }
+}
\ No newline at end of file
```
