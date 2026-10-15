using System.Globalization;

namespace CornerStore;

internal class TotalsPanelWidget(Cart cart, Customer customer, PricingService pricing)
{
    public void Refresh()
    {
        var total = cart.Lines.Count == 0 ? 0m : pricing.BuildQuote(cart, customer).Total;
        Log.Info("totals " + MoneyFormatter.Format(total, customer.Region));
    }

    public void ShowQuote()
    {
        var quote = pricing.BuildQuote(cart, customer);
        Log.Info("quote " + cart.CustomerId + " (" + customer.Region + ", " + cart.ShippingMethod + ", "
            + quote.WeightKg.ToString("0.00", CultureInfo.InvariantCulture) + " kg)");
        Log.Info("subtotal " + MoneyFormatter.Format(quote.Subtotal, customer.Region));
        if (cart.PromoCode != "")
        {
            Log.Info(quote.Discount > 0m
                ? "discount -" + MoneyFormatter.Format(quote.Discount, customer.Region) + " (" + cart.PromoCode + ")"
                : "promo " + cart.PromoCode + " does not apply");
        }

        Log.Info("add-ons " + MoneyFormatter.Format(quote.AddOns, customer.Region));
        Log.Info("shipping " + MoneyFormatter.Format(quote.Shipping, customer.Region));
        Log.Info("tax " + MoneyFormatter.Format(quote.Tax, customer.Region));
        Log.Info("total " + MoneyFormatter.Format(quote.Total, customer.Region));
    }
}