namespace CornerStore;

internal abstract record PromoExpression
{
    public abstract bool Interpret(PromoEvaluationContext context);
}

internal sealed record AndExpression(PromoExpression Left, PromoExpression Right) : PromoExpression
{
    public override bool Interpret(PromoEvaluationContext context)
    {
        return Left.Interpret(context) && Right.Interpret(context);
    }
}

internal sealed record SpendGreaterThanExpression(decimal Limit) : PromoExpression
{
    public override bool Interpret(PromoEvaluationContext context)
    {
        return context.Subtotal > Limit;
    }
}

internal sealed record QuantityGreaterThanExpression(decimal Limit) : PromoExpression
{
    public override bool Interpret(PromoEvaluationContext context)
    {
        var quantity = 0;
        foreach (var line in context.Cart.Lines)
        {
            quantity += line.Quantity;
        }

        return quantity > Limit;
    }
}

internal sealed record CategoryEqualsExpression(string Value) : PromoExpression
{
    public override bool Interpret(PromoEvaluationContext context)
    {
        foreach (var line in context.Cart.Lines)
        {
            if (line.Product.CategoryName == Value)
            {
                return true;
            }
        }

        return false;
    }
}

internal sealed record RegionEqualsExpression(string Value) : PromoExpression
{
    public override bool Interpret(PromoEvaluationContext context)
    {
        return context.Customer.Region == Value;
    }
}
