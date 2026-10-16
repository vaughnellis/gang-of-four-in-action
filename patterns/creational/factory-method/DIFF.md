# DIFF: factory-method

## File summary

| Status | File | Added | Removed |
| --- | --- | ---: | ---: |
| Added | CornerStore/Checkout/AfterpayProcessorCreator.cs | 9 | 0 |
| Added | CornerStore/Checkout/CardProcessorCreator.cs | 9 | 0 |
| Added | CornerStore/Checkout/PaymentProcessorCreator.cs | 11 | 0 |
| Added | CornerStore/Checkout/PayPalProcessorCreator.cs | 9 | 0 |
| Modified | CornerStore/CornerStore.csproj | 4 | 0 |
| Modified | CornerStore/StoreConsole.cs | 6 | 7 |
| Added | CornerStore.Tests/FactoryMethodTests.cs | 71 | 0 |

## Full diff

```diff
diff --git a/pattern/src/CornerStore/Checkout/AfterpayProcessorCreator.cs b/pattern/src/CornerStore/Checkout/AfterpayProcessorCreator.cs
new file mode 100644
index 0000000..af8a35f
--- /dev/null
+++ b/pattern/src/CornerStore/Checkout/AfterpayProcessorCreator.cs
@@ -0,0 +1,9 @@
+namespace CornerStore;
+
+internal sealed class AfterpayProcessorCreator : PaymentProcessorCreator
+{
+    protected override IPaymentProcessor CreateProcessor()
+    {
+        return new AfterpayProcessor();
+    }
+}
\ No newline at end of file
diff --git a/pattern/src/CornerStore/Checkout/CardProcessorCreator.cs b/pattern/src/CornerStore/Checkout/CardProcessorCreator.cs
new file mode 100644
index 0000000..4599678
--- /dev/null
+++ b/pattern/src/CornerStore/Checkout/CardProcessorCreator.cs
@@ -0,0 +1,9 @@
+namespace CornerStore;
+
+internal sealed class CardProcessorCreator : PaymentProcessorCreator
+{
+    protected override IPaymentProcessor CreateProcessor()
+    {
+        return new CardProcessor();
+    }
+}
\ No newline at end of file
diff --git a/pattern/src/CornerStore/Checkout/PayPalProcessorCreator.cs b/pattern/src/CornerStore/Checkout/PayPalProcessorCreator.cs
new file mode 100644
index 0000000..b146822
--- /dev/null
+++ b/pattern/src/CornerStore/Checkout/PayPalProcessorCreator.cs
@@ -0,0 +1,9 @@
+namespace CornerStore;
+
+internal sealed class PayPalProcessorCreator : PaymentProcessorCreator
+{
+    protected override IPaymentProcessor CreateProcessor()
+    {
+        return new PayPalProcessor();
+    }
+}
\ No newline at end of file
diff --git a/pattern/src/CornerStore/Checkout/PaymentProcessorCreator.cs b/pattern/src/CornerStore/Checkout/PaymentProcessorCreator.cs
new file mode 100644
index 0000000..f6c8131
--- /dev/null
+++ b/pattern/src/CornerStore/Checkout/PaymentProcessorCreator.cs
@@ -0,0 +1,11 @@
+namespace CornerStore;
+
+internal abstract class PaymentProcessorCreator
+{
+    public void Charge(Customer customer, decimal amount)
+    {
+        CreateProcessor().Charge(customer, amount);
+    }
+
+    protected abstract IPaymentProcessor CreateProcessor();
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
index a288b4e..7ac3a6c 100644
--- a/baseline/src/CornerStore/StoreConsole.cs
+++ b/pattern/src/CornerStore/StoreConsole.cs
@@ -454,29 +454,28 @@ internal class StoreConsole
         Log.Info("checkout placed " + order.Id);
     }
-    // SMELL S03: payment processor construction is selected by a switch in checkout
     private static bool ChargePayment(string paymentType, Customer customer, decimal amount)
     {
-        IPaymentProcessor? processor = null;
+        PaymentProcessorCreator? creator = null;
         switch (paymentType)
         {
             case "card":
-                processor = new CardProcessor();
+                creator = new CardProcessorCreator();
                 break;
             case "paypal":
-                processor = new PayPalProcessor();
+                creator = new PayPalProcessorCreator();
                 break;
             case "afterpay":
-                processor = new AfterpayProcessor();
+                creator = new AfterpayProcessorCreator();
                 break;
         }
-        if (processor is null)
+        if (creator is null)
         {
             return false;
         }
-        processor.Charge(customer, amount);
+        creator.Charge(customer, amount);
         return true;
     }
diff --git a/pattern/tests/CornerStore.Tests/FactoryMethodTests.cs b/pattern/tests/CornerStore.Tests/FactoryMethodTests.cs
new file mode 100644
index 0000000..42bea2f
--- /dev/null
+++ b/pattern/tests/CornerStore.Tests/FactoryMethodTests.cs
@@ -0,0 +1,71 @@
+namespace CornerStore.Tests;
+
+public class FactoryMethodTests : IDisposable
+{
+    private readonly string _path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".json");
+
+    public void Dispose()
+    {
+        File.Delete(_path);
+    }
+
+    [Fact]
+    public void GivenCardCreator_Charge_UsesCardProcessor()
+    {
+        var output = Charge(new CardProcessorCreator());
+
+        Assert.Contains("card charged", output);
+    }
+
+    [Fact]
+    public void GivenPayPalCreator_Charge_UsesPayPalProcessor()
+    {
+        var output = Charge(new PayPalProcessorCreator());
+
+        Assert.Contains("PayPal charged", output);
+    }
+
+    [Fact]
+    public void GivenAfterpayCreator_Charge_UsesAfterpayProcessor()
+    {
+        var output = Charge(new AfterpayProcessorCreator());
+
+        Assert.Contains("Afterpay charged", output);
+    }
+
+    [Fact]
+    public void GivenUnknownPaymentType_Checkout_RejectsWithoutCharging()
+    {
+        File.WriteAllText(
+            _path,
+            """{ "name": "factory-method", "startTime": "2026-03-02T09:00:00Z", "seed": 1, "steps": ["""
+            + """{ "op": "register", "name": "Ava", "region": "AU", "channel": "email", "creditLimit": 500, "address": "1 Harbour St" },"""
+            + """{ "op": "cart.add", "customer": "cu-1", "sku": "MUG-ENAMEL" },"""
+            + """{ "op": "checkout", "customer": "cu-1", "payment": "giftcard" }]}""");
+        var output = new StringWriter();
+
+        ScenarioRunner.Run(_path, output);
+
+        Assert.Contains("checkout rejected: unknown payment type giftcard", output.ToString());
+        Assert.DoesNotContain("charged", output.ToString());
+    }
+
+    private static string Charge(PaymentProcessorCreator creator)
+    {
+        var previousOutput = Log.Out;
+        var output = new StringWriter();
+        Log.Out = output;
+
+        try
+        {
+            creator.Charge(
+                new Customer { Name = "Ava", Region = "AU" },
+                10m);
+            return output.ToString();
+        }
+        finally
+        {
+            Log.Out = previousOutput;
+        }
+    }
+}
\ No newline at end of file
```
