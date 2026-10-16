using System.Globalization;

namespace CornerStore.Tests;

public class InterpreterTests
{
    [Fact]
    public void GivenCompoundRule_WhenEveryPredicateMatches_Interpret_ReturnsTrue()
    {
        var expression = PromoRuleParser.Parse("SPEND>100 AND CATEGORY=Shoes AND REGION=NZ");
        var context = CreateContext(100.01m, "NZ", ("Other", 1), ("Shoes", 1));

        var result = expression.Interpret(context);

        Assert.True(result);
    }

    [Fact]
    public void GivenDecimalThreshold_WhenCultureUsesCommaDecimal_ParserUsesInvariantNumber()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
            var expression = PromoRuleParser.Parse("SPEND>100.5");

            Assert.False(expression.Interpret(CreateContext(100.5m)));
            Assert.True(expression.Interpret(CreateContext(100.51m)));
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    [Fact]
    public void GivenCategoryPredicate_WhenAnyLineMatches_InterpretReturnsTrue()
    {
        var expression = PromoRuleParser.Parse("CATEGORY=Shoes");

        var result = expression.Interpret(CreateContext(0m, lines: [("Mugs", 1), ("Shoes", 1)]));

        Assert.True(result);
    }

    [Fact]
    public void GivenQuantityPredicate_WhenCartLinesAreSummed_InterpretUsesStrictGreaterThan()
    {
        var expression = PromoRuleParser.Parse("QTY>2");

        Assert.True(expression.Interpret(CreateContext(0m, lines: [("Mugs", 1), ("Shoes", 2)])));
        Assert.False(expression.Interpret(CreateContext(0m, lines: [("Mugs", 1), ("Shoes", 1)])));
    }

    [Fact]
    public void GivenRegionPredicate_WhenCustomerRegionMatches_InterpretReturnsTrue()
    {
        var expression = PromoRuleParser.Parse("REGION=NZ");

        Assert.True(expression.Interpret(CreateContext(0m, region: "NZ")));
        Assert.False(expression.Interpret(CreateContext(0m, region: "AU")));
    }

    [Fact]
    public void GivenSpendPredicate_WhenSubtotalEqualsLimit_InterpretReturnsFalse()
    {
        var expression = PromoRuleParser.Parse("SPEND>100");

        Assert.False(expression.Interpret(CreateContext(100m)));
        Assert.True(expression.Interpret(CreateContext(100.01m)));
    }

    [Theory]
    [InlineData("")]
    [InlineData("SPEND>")]
    [InlineData("SPEND=100")]
    [InlineData("COLOR=Red")]
    [InlineData("SPEND>100 OR CATEGORY=Shoes")]
    [InlineData("SPEND>100 AND")]
    [InlineData("SPEND>100 AND CATEGORY=Shoes AND")]
    public void GivenMalformedRule_WhenParsed_ThrowsFormatException(string rule)
    {
        Assert.Throws<FormatException>(() => PromoRuleParser.Parse(rule));
    }

    private static PromoEvaluationContext CreateContext(
        decimal subtotal,
        string region = "AU",
        params (string Category, int Quantity)[] lines)
    {
        var cart = new Cart("cu-1");
        foreach (var (category, quantity) in lines)
        {
            cart.Lines.Add(new CartLine
            {
                Product = new PhysicalProduct { CategoryName = category },
                Quantity = quantity,
            });
        }

        var customer = new Customer { Region = region };
        return new PromoEvaluationContext(cart, customer, subtotal);
    }
}
