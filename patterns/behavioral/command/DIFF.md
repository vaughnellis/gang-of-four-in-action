# DIFF: command

## File summary

| Status | File | Added | Removed |
| --- | --- | ---: | ---: |
| Modified | CornerStore/CornerStore.csproj | 4 | 0 |
| Added | CornerStore/Orders/AdjustStockCommand.cs | 36 | 0 |
| Modified | CornerStore/Orders/BackOffice.cs | 13 | 14 |
| Added | CornerStore/Orders/CancelCommand.cs | 19 | 0 |
| Added | CornerStore/Orders/CommandAuditEntry.cs | 3 | 0 |
| Added | CornerStore/Orders/CommandInvoker.cs | 35 | 0 |
| Added | CornerStore/Orders/ICommand.cs | 12 | 0 |
| Added | CornerStore/Orders/LookupStockCommand.cs | 22 | 0 |
| Added | CornerStore/Orders/RefundCommand.cs | 19 | 0 |
| Added | CornerStore.Tests/CommandTests.cs | 83 | 0 |

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
diff --git a/pattern/src/CornerStore/Orders/AdjustStockCommand.cs b/pattern/src/CornerStore/Orders/AdjustStockCommand.cs
new file mode 100644
index 0000000..d08ff16
--- /dev/null
+++ b/pattern/src/CornerStore/Orders/AdjustStockCommand.cs
@@ -0,0 +1,36 @@
+using System.Globalization;
+
+namespace CornerStore;
+
+internal sealed class AdjustStockCommand(InventoryService inventory, string sku, int adjustment) : ICommand
+{
+    private bool canUndo;
+
+    public string Name => "AdjustStock";
+
+    public bool CanUndo => canUndo;
+
+    public string Execute()
+    {
+        if (canUndo)
+        {
+            throw new InvalidOperationException("Undo the previous execution before reusing this command.");
+        }
+
+        var stock = inventory.AdjustStock(sku, adjustment);
+        canUndo = true;
+        return "stock " + sku + " adjusted by " + adjustment.ToString(CultureInfo.InvariantCulture)
+            + ", now " + stock.ToString(CultureInfo.InvariantCulture);
+    }
+
+    public void Undo()
+    {
+        if (!canUndo)
+        {
+            throw new InvalidOperationException("This stock adjustment cannot be undone.");
+        }
+
+        inventory.AdjustStock(sku, -adjustment);
+        canUndo = false;
+    }
+}
diff --git a/baseline/src/CornerStore/Orders/BackOffice.cs b/pattern/src/CornerStore/Orders/BackOffice.cs
index aff3031..7e64fbd 100644
--- a/baseline/src/CornerStore/Orders/BackOffice.cs
+++ b/pattern/src/CornerStore/Orders/BackOffice.cs
@@ -2,57 +2,56 @@ namespace CornerStore;
 internal class BackOffice(InventoryService inventory, OrderService orderService)
 {
+    private readonly CommandInvoker commandInvoker = new();
+
+    public IReadOnlyList<CommandAuditEntry> AuditLog => commandInvoker.AuditLog;
+
+    public void UndoLastAction()
+    {
+        commandInvoker.UndoLast();
+    }
+
     public string AdjustStock(Customer actor, string sku, int adjustment)
     {
-        // SMELL S14: stock adjustment is an inline back-office action
         // SMELL S12: the caller checks the staff role before changing stock
         if (actor.Role != "staff")
         {
             return "rejected: staff only";
         }
-        var stock = inventory.AdjustStock(sku, adjustment);
-        return "stock " + sku + " adjusted by " + adjustment.ToString(System.Globalization.CultureInfo.InvariantCulture)
-            + ", now " + stock.ToString(System.Globalization.CultureInfo.InvariantCulture);
+        return commandInvoker.Execute(new AdjustStockCommand(inventory, sku, adjustment));
     }
     public string LookupStock(Customer actor, string sku)
     {
-        // SMELL S14: stock lookup is an inline back-office action
         // SMELL S12: the caller checks the staff role before reading stock
         if (actor.Role != "staff")
         {
             return "rejected: staff only";
         }
-        var available = inventory.GetStock(sku);
-        inventory.GetStock(sku);
-        return "stock " + sku + " " + available.ToString(System.Globalization.CultureInfo.InvariantCulture);
+        return commandInvoker.Execute(new LookupStockCommand(inventory, sku));
     }
     public string Refund(Customer actor, Order order)
     {
-        // SMELL S14: refund runs inline in the back-office service
         // SMELL S12: the caller checks the staff role before refunding
         if (actor.Role != "staff")
         {
             return "rejected: staff only";
         }
-        var error = orderService.Refund(order);
-        return error is null ? "refunded " + order.Id : "rejected: " + error;
+        return commandInvoker.Execute(new RefundCommand(orderService, order));
     }
     public string Cancel(Customer actor, Order order)
     {
-        // SMELL S14: cancellation runs inline in the back-office service
         // SMELL S12: the caller checks the staff role before cancelling
         if (actor.Role != "staff")
         {
             return "rejected: staff only";
         }
-        var error = orderService.Cancel(order);
-        return error is null ? "cancelled " + order.Id : "rejected: " + error;
+        return commandInvoker.Execute(new CancelCommand(orderService, order));
     }
 }
\ No newline at end of file
diff --git a/pattern/src/CornerStore/Orders/CancelCommand.cs b/pattern/src/CornerStore/Orders/CancelCommand.cs
new file mode 100644
index 0000000..16ed290
--- /dev/null
+++ b/pattern/src/CornerStore/Orders/CancelCommand.cs
@@ -0,0 +1,19 @@
+namespace CornerStore;
+
+internal sealed class CancelCommand(OrderService orderService, Order order) : ICommand
+{
+    public string Name => "Cancel";
+
+    public bool CanUndo => false;
+
+    public string Execute()
+    {
+        var error = orderService.Cancel(order);
+        return error is null ? "cancelled " + order.Id : "rejected: " + error;
+    }
+
+    public void Undo()
+    {
+        throw new NotSupportedException("A cancellation has effects that cannot be undone.");
+    }
+}
diff --git a/pattern/src/CornerStore/Orders/CommandAuditEntry.cs b/pattern/src/CornerStore/Orders/CommandAuditEntry.cs
new file mode 100644
index 0000000..3f7df86
--- /dev/null
+++ b/pattern/src/CornerStore/Orders/CommandAuditEntry.cs
@@ -0,0 +1,3 @@
+namespace CornerStore;
+
+internal sealed record CommandAuditEntry(string CommandName, string Result);
diff --git a/pattern/src/CornerStore/Orders/CommandInvoker.cs b/pattern/src/CornerStore/Orders/CommandInvoker.cs
new file mode 100644
index 0000000..027bb60
--- /dev/null
+++ b/pattern/src/CornerStore/Orders/CommandInvoker.cs
@@ -0,0 +1,35 @@
+namespace CornerStore;
+
+internal sealed class CommandInvoker
+{
+    private readonly List<CommandAuditEntry> auditLog = [];
+    private ICommand? lastCommand;
+
+    public IReadOnlyList<CommandAuditEntry> AuditLog => auditLog.AsReadOnly();
+
+    public string Execute(ICommand command)
+    {
+        var result = command.Execute();
+        auditLog.Add(new CommandAuditEntry(command.Name, result));
+        lastCommand = command;
+        return result;
+    }
+
+    public void UndoLast()
+    {
+        if (lastCommand is null)
+        {
+            throw new InvalidOperationException("No command is available to undo.");
+        }
+
+        if (!lastCommand.CanUndo)
+        {
+            throw new InvalidOperationException("The most recently executed command cannot be undone.");
+        }
+
+        var command = lastCommand;
+        command.Undo();
+        auditLog.Add(new CommandAuditEntry(command.Name, "undone"));
+        lastCommand = null;
+    }
+}
diff --git a/pattern/src/CornerStore/Orders/ICommand.cs b/pattern/src/CornerStore/Orders/ICommand.cs
new file mode 100644
index 0000000..063b56f
--- /dev/null
+++ b/pattern/src/CornerStore/Orders/ICommand.cs
@@ -0,0 +1,12 @@
+namespace CornerStore;
+
+internal interface ICommand
+{
+    string Name { get; }
+
+    bool CanUndo { get; }
+
+    string Execute();
+
+    void Undo();
+}
diff --git a/pattern/src/CornerStore/Orders/LookupStockCommand.cs b/pattern/src/CornerStore/Orders/LookupStockCommand.cs
new file mode 100644
index 0000000..469a4c9
--- /dev/null
+++ b/pattern/src/CornerStore/Orders/LookupStockCommand.cs
@@ -0,0 +1,22 @@
+using System.Globalization;
+
+namespace CornerStore;
+
+internal sealed class LookupStockCommand(InventoryService inventory, string sku) : ICommand
+{
+    public string Name => "LookupStock";
+
+    public bool CanUndo => false;
+
+    public string Execute()
+    {
+        var available = inventory.GetStock(sku);
+        inventory.GetStock(sku);
+        return "stock " + sku + " " + available.ToString(CultureInfo.InvariantCulture);
+    }
+
+    public void Undo()
+    {
+        throw new NotSupportedException("A stock lookup has no state to undo.");
+    }
+}
diff --git a/pattern/src/CornerStore/Orders/RefundCommand.cs b/pattern/src/CornerStore/Orders/RefundCommand.cs
new file mode 100644
index 0000000..56d65fd
--- /dev/null
+++ b/pattern/src/CornerStore/Orders/RefundCommand.cs
@@ -0,0 +1,19 @@
+namespace CornerStore;
+
+internal sealed class RefundCommand(OrderService orderService, Order order) : ICommand
+{
+    public string Name => "Refund";
+
+    public bool CanUndo => false;
+
+    public string Execute()
+    {
+        var error = orderService.Refund(order);
+        return error is null ? "refunded " + order.Id : "rejected: " + error;
+    }
+
+    public void Undo()
+    {
+        throw new NotSupportedException("A refund has effects that cannot be undone.");
+    }
+}
diff --git a/pattern/tests/CornerStore.Tests/CommandTests.cs b/pattern/tests/CornerStore.Tests/CommandTests.cs
new file mode 100644
index 0000000..dfd1a4d
--- /dev/null
+++ b/pattern/tests/CornerStore.Tests/CommandTests.cs
@@ -0,0 +1,83 @@
+namespace CornerStore.Tests;
+
+public class CommandTests
+{
+    [Fact]
+    public void GivenStaff_WhenAdjustingStock_RecordsResultAndCanUndo()
+    {
+        var inventory = new InventoryService();
+        var backOffice = CreateBackOffice(inventory);
+        var staff = CreateCustomer("staff");
+        var initialStock = inventory.GetStock("MUG-ENAMEL");
+
+        var result = backOffice.AdjustStock(staff, "MUG-ENAMEL", 3);
+
+        Assert.Equal("stock MUG-ENAMEL adjusted by 3, now 13", result);
+        Assert.Equal(new CommandAuditEntry("AdjustStock", result), Assert.Single(backOffice.AuditLog));
+
+        backOffice.UndoLastAction();
+
+        Assert.Equal(initialStock, inventory.GetStock("MUG-ENAMEL"));
+        Assert.Equal(new CommandAuditEntry("AdjustStock", "undone"), backOffice.AuditLog[^1]);
+    }
+
+    [Fact]
+    public void GivenCustomer_WhenAdjustingStock_ReturnsRejectionWithoutAuditing()
+    {
+        var inventory = new InventoryService();
+        var backOffice = CreateBackOffice(inventory);
+        var customer = CreateCustomer("customer");
+        var initialStock = inventory.GetStock("MUG-ENAMEL");
+
+        var result = backOffice.AdjustStock(customer, "MUG-ENAMEL", 3);
+
+        Assert.Equal("rejected: staff only", result);
+        Assert.Equal(initialStock, inventory.GetStock("MUG-ENAMEL"));
+        Assert.Empty(backOffice.AuditLog);
+    }
+
+    [Fact]
+    public void GivenStockLookup_WhenExecuted_RecordsResultAndDoesNotCreateUndo()
+    {
+        var backOffice = CreateBackOffice(new InventoryService());
+        var staff = CreateCustomer("staff");
+
+        var result = backOffice.LookupStock(staff, "MUG-ENAMEL");
+
+        Assert.Equal("stock MUG-ENAMEL 10", result);
+        Assert.Equal(new CommandAuditEntry("LookupStock", result), Assert.Single(backOffice.AuditLog));
+        Assert.Throws<InvalidOperationException>(backOffice.UndoLastAction);
+    }
+
+    private static BackOffice CreateBackOffice(InventoryService inventory)
+    {
+        var emailSender = new EmailSender();
+        var smsSender = new SmsSender();
+        var orderService = new OrderService(
+            new IdSequence(),
+            inventory,
+            new OrderConfirmedEmail(emailSender),
+            new OrderConfirmedSms(smsSender),
+            new OrderShippedEmail(emailSender),
+            new OrderShippedSms(smsSender),
+            new LoyaltyService(),
+            new AnalyticsService());
+
+        return new BackOffice(inventory, orderService);
+    }
+
+    private static Customer CreateCustomer(string role)
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
+            Role = role,
+        };
+    }
+}
```
