namespace CornerStore;

internal class PricingService(PromoEngine promos)
{
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
        // SMELL S21: the shipping cost is picked with a switch on the method string
        switch (method)
        {
            case "standard":
                return Math.Round(5.00m + 2.00m * weightKg, 2, MidpointRounding.AwayFromZero);
            case "express":
                return Math.Round(12.00m + 4.00m * weightKg, 2, MidpointRounding.AwayFromZero);
            case "pickup":
                return 0m;
            default:
                return null;
        }
    }

    private static decimal AddOnCost(Cart cart, decimal discountedSubtotal)
    {
        IAddOnPriceComponent component = new BaseAddOnPriceComponent();
        component = new GiftWrapPriceDecorator(component, cart.GiftWrap);
        component = new InsurancePriceDecorator(component, cart.Insurance, discountedSubtotal);
        return component.Cost;
    }
}
