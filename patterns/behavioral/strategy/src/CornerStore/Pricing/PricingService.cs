namespace CornerStore;

internal class PricingService
{
    private readonly PromoEngine promos;
    private readonly IReadOnlyDictionary<string, IShippingCostStrategy> shippingStrategies;

    public PricingService(
        PromoEngine promos,
        IReadOnlyDictionary<string, IShippingCostStrategy>? shippingStrategies = null)
    {
        this.promos = promos;
        this.shippingStrategies = shippingStrategies ?? new Dictionary<string, IShippingCostStrategy>(StringComparer.Ordinal)
        {
            ["standard"] = new StandardShippingCostStrategy(),
            ["express"] = new ExpressShippingCostStrategy(),
            ["pickup"] = new PickupShippingCostStrategy(),
        };
    }

    public Quote BuildQuote(Cart cart, Customer customer)
    {
        var subtotal = 0m;
        var weightKg = 0m;
        foreach (var line in cart.Lines)
        {
            subtotal += line.UnitPrice * line.Quantity;
            weightKg += WeightCalculator.WeightOf(line.Product, line.Quantity);
        }

        var discount = 0m;
        if (cart.PromoCode != "" && promos.IsKnown(cart.PromoCode) && promos.Evaluate(cart.PromoCode, cart, customer, subtotal))
        {
            discount = promos.DiscountFor(cart.PromoCode, subtotal);
        }

        var addOns = AddOnCost(cart, subtotal - discount);

        var methodCost = ShippingCost(cart.ShippingMethod, weightKg)
            ?? throw new InvalidOperationException("unknown shipping method " + cart.ShippingMethod);
        var shipping = methodCost + ShippingCalculator.RegionSurcharge(customer.Region);

        var rate = TaxCalculator.RateFor(customer.Region);
        var tax = 0m;
        var allocated = 0m;
        for (var i = 0; i < cart.Lines.Count; i++)
        {
            var line = cart.Lines[i];
            var amount = line.UnitPrice * line.Quantity;
            var share = 0m;
            if (discount > 0m)
            {
                share = i == cart.Lines.Count - 1
                    ? discount - allocated
                    : Math.Round(discount * amount / subtotal, 2, MidpointRounding.AwayFromZero);
            }

            allocated += share;
            tax += TaxCalculator.LineTax(line.Product, amount - share, rate);
        }

        return new Quote
        {
            Subtotal = subtotal,
            Discount = discount,
            AddOns = addOns,
            Shipping = shipping,
            Tax = tax,
            Total = subtotal - discount + addOns + shipping + tax,
            WeightKg = weightKg,
        };
    }

    public decimal? ShippingCost(string method, decimal weightKg)
    {
        return shippingStrategies.TryGetValue(method, out var strategy)
            ? strategy.Calculate(weightKg)
            : null;
    }

    private static decimal AddOnCost(Cart cart, decimal discountedSubtotal)
    {
        // SMELL S09: each add-on is a boolean flag checked in an if-chain
        var cost = 0m;
        if (cart.GiftWrap)
        {
            cost += 4.50m;
        }

        if (cart.Insurance)
        {
            cost += Math.Round(discountedSubtotal * 0.02m, 2, MidpointRounding.AwayFromZero);
        }

        return cost;
    }
}
