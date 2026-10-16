using System.Globalization;

namespace CornerStore;

internal class TotalsPanelWidget(Cart cart, Customer customer, PricingService pricing)
{
    private ItemListWidget? itemList;
    private PromoBoxWidget? promoBox;
    private ShippingSelectorWidget? shippingSelector;

    public void Connect(ItemListWidget itemListWidget, PromoBoxWidget promoBoxWidget, ShippingSelectorWidget shippingSelectorWidget)
    {
        itemList = itemListWidget;
        promoBox = promoBoxWidget;
        shippingSelector = shippingSelectorWidget;
    }

    public void Refresh()
    {
        var total = cart.Lines.Count == 0 ? 0m : pricing.BuildQuote(cart, customer).Total;
        Log.Instance.Info("totals " + MoneyFormatter.Format(total, customer.Region));
    }

    public void RefreshAfterChange()
    {
        // SMELL S17: this widget refreshes its peers directly after a totals change
        Refresh();
        itemList?.Refresh();
        promoBox?.Refresh();
        shippingSelector?.Refresh();
    }

    public void ShowQuote()
    {
        var quote = pricing.BuildQuote(cart, customer);
        Log.Instance.Info("quote " + cart.CustomerId + " (" + customer.Region + ", " + cart.ShippingMethod + ", "
            + quote.WeightKg.ToString("0.00", CultureInfo.InvariantCulture) + " kg)");
        Log.Instance.Info("subtotal " + MoneyFormatter.Format(quote.Subtotal, customer.Region));
        if (cart.PromoCode != "")
        {
            Log.Instance.Info(quote.Discount > 0m
                ? "discount -" + MoneyFormatter.Format(quote.Discount, customer.Region) + " (" + cart.PromoCode + ")"
                : "promo " + cart.PromoCode + " does not apply");
        }

        Log.Instance.Info("add-ons " + MoneyFormatter.Format(quote.AddOns, customer.Region));
        Log.Instance.Info("shipping " + MoneyFormatter.Format(quote.Shipping, customer.Region));
        Log.Instance.Info("tax " + MoneyFormatter.Format(quote.Tax, customer.Region));
        Log.Instance.Info("total " + MoneyFormatter.Format(quote.Total, customer.Region));
    }
}