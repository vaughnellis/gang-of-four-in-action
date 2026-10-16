# DIFF: interpreter

## File summary

| Status | File | Added | Removed |
| --- | --- | ---: | ---: |
| Modified | CornerStore/CornerStore.csproj | 4 | 0 |
| Modified | CornerStore/Pricing/PromoEngine.cs | 12 | 78 |
| Added | CornerStore/Pricing/PromoEvaluationContext.cs | 3 | 0 |
| Added | CornerStore/Pricing/PromoExpression.cs | 60 | 0 |
| Added | CornerStore/Pricing/PromoRuleParser.cs | 165 | 0 |
| Added | CornerStore.Tests/InterpreterTests.cs | 104 | 0 |

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
diff --git a/baseline/src/CornerStore/Pricing/PromoEngine.cs b/pattern/src/CornerStore/Pricing/PromoEngine.cs
index 39964ed..e850fcb 100644
--- a/baseline/src/CornerStore/Pricing/PromoEngine.cs
+++ b/pattern/src/CornerStore/Pricing/PromoEngine.cs
@@ -1,14 +1,14 @@
-using System.Globalization;
-
 namespace CornerStore;
 internal class PromoEngine
 {
-    private static readonly Dictionary<string, (string Rule, decimal Percent, decimal Amount)> Promos = new()
+    private sealed record PromoDefinition(string Rule, decimal Percent, decimal Amount, PromoExpression Expression);
+
+    private static readonly Dictionary<string, PromoDefinition> Promos = new()
     {
-        ["SHOES10"] = ("SPEND>100 AND CATEGORY=Shoes", 10m, 0m),
-        ["BIGSPEND"] = ("SPEND>250", 0m, 20m),
-        ["NZFRIEND"] = ("REGION=NZ AND QTY>2", 5m, 0m),
+        ["SHOES10"] = CreatePromo("SPEND>100 AND CATEGORY=Shoes", 10m, 0m),
+        ["BIGSPEND"] = CreatePromo("SPEND>250", 0m, 20m),
+        ["NZFRIEND"] = CreatePromo("REGION=NZ AND QTY>2", 5m, 0m),
     };
     public bool IsKnown(string code)
@@ -29,78 +29,12 @@ internal class PromoEngine
     public bool Evaluate(string code, Cart cart, Customer customer, decimal subtotal)
     {
-        // SMELL S15: the rule text is parsed by hand with Split and IndexOf on every call
-        var terms = Promos[code].Rule.Split(" AND ");
-        foreach (var term in terms)
-        {
-            var greater = term.IndexOf('>');
-            if (greater > 0)
-            {
-                var name = term.Substring(0, greater);
-                var limit = decimal.Parse(term.Substring(greater + 1), CultureInfo.InvariantCulture);
-                if (name == "SPEND")
-                {
-                    if (subtotal <= limit)
-                    {
-                        return false;
-                    }
-                }
-                else if (name == "QTY")
-                {
-                    var quantity = 0;
-                    foreach (var line in cart.Lines)
-                    {
-                        quantity += line.Quantity;
-                    }
-
-                    if (quantity <= limit)
-                    {
-                        return false;
-                    }
-                }
-                else
-                {
-                    return false;
-                }
-
-                continue;
-            }
-
-            var equals = term.IndexOf('=');
-            if (equals > 0)
-            {
-                var name = term.Substring(0, equals);
-                var value = term.Substring(equals + 1);
-                if (name == "CATEGORY")
-                {
-                    var found = false;
-                    foreach (var line in cart.Lines)
-                    {
-                        if (line.Product.CategoryName == value)
-                        {
-                            found = true;
-                        }
-                    }
-
-                    if (!found)
-                    {
-                        return false;
-                    }
-                }
-                else if (name == "REGION")
-                {
-                    if (customer.Region != value)
-                    {
-                        return false;
-                    }
-                }
-                else
-                {
-                    return false;
-                }
-            }
-        }
+        var context = new PromoEvaluationContext(cart, customer, subtotal);
+        return Promos[code].Expression.Interpret(context);
+    }
-        return true;
+    private static PromoDefinition CreatePromo(string rule, decimal percent, decimal amount)
+    {
+        return new PromoDefinition(rule, percent, amount, PromoRuleParser.Parse(rule));
     }
 }
diff --git a/pattern/src/CornerStore/Pricing/PromoEvaluationContext.cs b/pattern/src/CornerStore/Pricing/PromoEvaluationContext.cs
new file mode 100644
index 0000000..0480a91
--- /dev/null
+++ b/pattern/src/CornerStore/Pricing/PromoEvaluationContext.cs
@@ -0,0 +1,3 @@
+namespace CornerStore;
+
+internal sealed record PromoEvaluationContext(Cart Cart, Customer Customer, decimal Subtotal);
diff --git a/pattern/src/CornerStore/Pricing/PromoExpression.cs b/pattern/src/CornerStore/Pricing/PromoExpression.cs
new file mode 100644
index 0000000..332a811
--- /dev/null
+++ b/pattern/src/CornerStore/Pricing/PromoExpression.cs
@@ -0,0 +1,60 @@
+namespace CornerStore;
+
+internal abstract record PromoExpression
+{
+    public abstract bool Interpret(PromoEvaluationContext context);
+}
+
+internal sealed record AndExpression(PromoExpression Left, PromoExpression Right) : PromoExpression
+{
+    public override bool Interpret(PromoEvaluationContext context)
+    {
+        return Left.Interpret(context) && Right.Interpret(context);
+    }
+}
+
+internal sealed record SpendGreaterThanExpression(decimal Limit) : PromoExpression
+{
+    public override bool Interpret(PromoEvaluationContext context)
+    {
+        return context.Subtotal > Limit;
+    }
+}
+
+internal sealed record QuantityGreaterThanExpression(decimal Limit) : PromoExpression
+{
+    public override bool Interpret(PromoEvaluationContext context)
+    {
+        var quantity = 0;
+        foreach (var line in context.Cart.Lines)
+        {
+            quantity += line.Quantity;
+        }
+
+        return quantity > Limit;
+    }
+}
+
+internal sealed record CategoryEqualsExpression(string Value) : PromoExpression
+{
+    public override bool Interpret(PromoEvaluationContext context)
+    {
+        foreach (var line in context.Cart.Lines)
+        {
+            if (line.Product.CategoryName == Value)
+            {
+                return true;
+            }
+        }
+
+        return false;
+    }
+}
+
+internal sealed record RegionEqualsExpression(string Value) : PromoExpression
+{
+    public override bool Interpret(PromoEvaluationContext context)
+    {
+        return context.Customer.Region == Value;
+    }
+}
diff --git a/pattern/src/CornerStore/Pricing/PromoRuleParser.cs b/pattern/src/CornerStore/Pricing/PromoRuleParser.cs
new file mode 100644
index 0000000..ca5ddf1
--- /dev/null
+++ b/pattern/src/CornerStore/Pricing/PromoRuleParser.cs
@@ -0,0 +1,165 @@
+using System.Globalization;
+
+namespace CornerStore;
+
+internal sealed class PromoRuleParser(string rule)
+{
+    private int position;
+
+    public static PromoExpression Parse(string rule)
+    {
+        if (string.IsNullOrWhiteSpace(rule))
+        {
+            throw new FormatException("A promo rule cannot be empty.");
+        }
+
+        return new PromoRuleParser(rule).ParseRule();
+    }
+
+    private PromoExpression ParseRule()
+    {
+        SkipWhitespace();
+        var expression = ParsePredicate();
+
+        while (true)
+        {
+            var whitespace = SkipWhitespace();
+            if (position == rule.Length)
+            {
+                return expression;
+            }
+
+            if (whitespace == 0 || !TryReadKeyword("AND"))
+            {
+                throw Error("Expected AND or the end of the rule");
+            }
+
+            if (position == rule.Length || !char.IsWhiteSpace(rule[position]))
+            {
+                throw Error("Expected whitespace after AND");
+            }
+
+            SkipWhitespace();
+            expression = new AndExpression(expression, ParsePredicate());
+        }
+    }
+
+    private PromoExpression ParsePredicate()
+    {
+        var name = ReadIdentifier();
+        SkipWhitespace();
+        if (position == rule.Length)
+        {
+            throw Error("Expected > or =");
+        }
+
+        var operation = rule[position++];
+        SkipWhitespace();
+
+        return (name, operation) switch
+        {
+            ("SPEND", '>') => new SpendGreaterThanExpression(ReadNumber()),
+            ("QTY", '>') => new QuantityGreaterThanExpression(ReadNumber()),
+            ("CATEGORY", '=') => new CategoryEqualsExpression(ReadIdentifier()),
+            ("REGION", '=') => new RegionEqualsExpression(ReadIdentifier()),
+            _ => throw Error($"Unsupported predicate {name}{operation}"),
+        };
+    }
+
+    private string ReadIdentifier()
+    {
+        if (position == rule.Length || !IsIdentifierStart(rule[position]))
+        {
+            throw Error("Expected an identifier");
+        }
+
+        var start = position++;
+        while (position < rule.Length && IsIdentifierPart(rule[position]))
+        {
+            position++;
+        }
+
+        return rule[start..position];
+    }
+
+    private decimal ReadNumber()
+    {
+        var start = position;
+        while (position < rule.Length && char.IsAsciiDigit(rule[position]))
+        {
+            position++;
+        }
+
+        if (position < rule.Length && rule[position] == '.')
+        {
+            position++;
+            var fractionalStart = position;
+            while (position < rule.Length && char.IsAsciiDigit(rule[position]))
+            {
+                position++;
+            }
+
+            if (fractionalStart == position)
+            {
+                throw Error("Expected digits after the decimal point");
+            }
+        }
+
+        if (start == position || !decimal.TryParse(
+                rule.AsSpan(start, position - start),
+                NumberStyles.AllowDecimalPoint,
+                CultureInfo.InvariantCulture,
+                out var value))
+        {
+            throw Error("Expected an invariant-culture number");
+        }
+
+        return value;
+    }
+
+    private bool TryReadKeyword(string keyword)
+    {
+        if (!rule.AsSpan(position).StartsWith(keyword.AsSpan(), StringComparison.Ordinal))
+        {
+            return false;
+        }
+
+        var end = position + keyword.Length;
+        if (end < rule.Length && IsIdentifierPart(rule[end]))
+        {
+            return false;
+        }
+
+        position = end;
+        return true;
+    }
+
+    private int SkipWhitespace()
+    {
+        var start = position;
+        while (position < rule.Length && char.IsWhiteSpace(rule[position]))
+        {
+            position++;
+        }
+
+        return position - start;
+    }
+
+    private FormatException Error(string message)
+    {
+        return new FormatException($"Invalid promo rule '{rule}' at position {position}: {message}.");
+    }
+
+    private static bool IsIdentifierStart(char value)
+    {
+        return value is >= 'A' and <= 'Z'
+            or >= 'a' and <= 'z';
+    }
+
+    private static bool IsIdentifierPart(char value)
+    {
+        return IsIdentifierStart(value)
+            || char.IsAsciiDigit(value)
+            || value is '_' or '-';
+    }
+}
diff --git a/pattern/tests/CornerStore.Tests/InterpreterTests.cs b/pattern/tests/CornerStore.Tests/InterpreterTests.cs
new file mode 100644
index 0000000..92a3876
--- /dev/null
+++ b/pattern/tests/CornerStore.Tests/InterpreterTests.cs
@@ -0,0 +1,104 @@
+using System.Globalization;
+
+namespace CornerStore.Tests;
+
+public class InterpreterTests
+{
+    [Fact]
+    public void GivenCompoundRule_WhenEveryPredicateMatches_Interpret_ReturnsTrue()
+    {
+        var expression = PromoRuleParser.Parse("SPEND>100 AND CATEGORY=Shoes AND REGION=NZ");
+        var context = CreateContext(100.01m, "NZ", ("Other", 1), ("Shoes", 1));
+
+        var result = expression.Interpret(context);
+
+        Assert.True(result);
+    }
+
+    [Fact]
+    public void GivenDecimalThreshold_WhenCultureUsesCommaDecimal_ParserUsesInvariantNumber()
+    {
+        var originalCulture = CultureInfo.CurrentCulture;
+        try
+        {
+            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
+            var expression = PromoRuleParser.Parse("SPEND>100.5");
+
+            Assert.False(expression.Interpret(CreateContext(100.5m)));
+            Assert.True(expression.Interpret(CreateContext(100.51m)));
+        }
+        finally
+        {
+            CultureInfo.CurrentCulture = originalCulture;
+        }
+    }
+
+    [Fact]
+    public void GivenCategoryPredicate_WhenAnyLineMatches_InterpretReturnsTrue()
+    {
+        var expression = PromoRuleParser.Parse("CATEGORY=Shoes");
+
+        var result = expression.Interpret(CreateContext(0m, lines: [("Mugs", 1), ("Shoes", 1)]));
+
+        Assert.True(result);
+    }
+
+    [Fact]
+    public void GivenQuantityPredicate_WhenCartLinesAreSummed_InterpretUsesStrictGreaterThan()
+    {
+        var expression = PromoRuleParser.Parse("QTY>2");
+
+        Assert.True(expression.Interpret(CreateContext(0m, lines: [("Mugs", 1), ("Shoes", 2)])));
+        Assert.False(expression.Interpret(CreateContext(0m, lines: [("Mugs", 1), ("Shoes", 1)])));
+    }
+
+    [Fact]
+    public void GivenRegionPredicate_WhenCustomerRegionMatches_InterpretReturnsTrue()
+    {
+        var expression = PromoRuleParser.Parse("REGION=NZ");
+
+        Assert.True(expression.Interpret(CreateContext(0m, region: "NZ")));
+        Assert.False(expression.Interpret(CreateContext(0m, region: "AU")));
+    }
+
+    [Fact]
+    public void GivenSpendPredicate_WhenSubtotalEqualsLimit_InterpretReturnsFalse()
+    {
+        var expression = PromoRuleParser.Parse("SPEND>100");
+
+        Assert.False(expression.Interpret(CreateContext(100m)));
+        Assert.True(expression.Interpret(CreateContext(100.01m)));
+    }
+
+    [Theory]
+    [InlineData("")]
+    [InlineData("SPEND>")]
+    [InlineData("SPEND=100")]
+    [InlineData("COLOR=Red")]
+    [InlineData("SPEND>100 OR CATEGORY=Shoes")]
+    [InlineData("SPEND>100 AND")]
+    [InlineData("SPEND>100 AND CATEGORY=Shoes AND")]
+    public void GivenMalformedRule_WhenParsed_ThrowsFormatException(string rule)
+    {
+        Assert.Throws<FormatException>(() => PromoRuleParser.Parse(rule));
+    }
+
+    private static PromoEvaluationContext CreateContext(
+        decimal subtotal,
+        string region = "AU",
+        params (string Category, int Quantity)[] lines)
+    {
+        var cart = new Cart("cu-1");
+        foreach (var (category, quantity) in lines)
+        {
+            cart.Lines.Add(new CartLine
+            {
+                Product = new PhysicalProduct { CategoryName = category },
+                Quantity = quantity,
+            });
+        }
+
+        var customer = new Customer { Region = region };
+        return new PromoEvaluationContext(cart, customer, subtotal);
+    }
+}
```
