namespace CornerStore;

internal class PromoEngine
{
    private sealed record PromoDefinition(string Rule, decimal Percent, decimal Amount, PromoExpression Expression);

    private static readonly Dictionary<string, PromoDefinition> Promos = new()
    {
        ["SHOES10"] = CreatePromo("SPEND>100 AND CATEGORY=Shoes", 10m, 0m),
        ["BIGSPEND"] = CreatePromo("SPEND>250", 0m, 20m),
        ["NZFRIEND"] = CreatePromo("REGION=NZ AND QTY>2", 5m, 0m),
    };

    public bool IsKnown(string code)
    {
        return Promos.ContainsKey(code);
    }

    public string RuleOf(string code)
    {
        return Promos[code].Rule;
    }

    public decimal DiscountFor(string code, decimal subtotal)
    {
        var promo = Promos[code];
        return Math.Round(subtotal * promo.Percent / 100m, 2, MidpointRounding.AwayFromZero) + promo.Amount;
    }

    public bool Evaluate(string code, Cart cart, Customer customer, decimal subtotal)
    {
        var context = new PromoEvaluationContext(cart, customer, subtotal);
        return Promos[code].Expression.Interpret(context);
    }

    private static PromoDefinition CreatePromo(string rule, decimal percent, decimal amount)
    {
        return new PromoDefinition(rule, percent, amount, PromoRuleParser.Parse(rule));
    }
}
