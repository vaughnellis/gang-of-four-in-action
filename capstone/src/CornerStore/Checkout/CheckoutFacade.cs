namespace CornerStore;

internal sealed class CheckoutFacade(
    PricingService pricing,
    OrderValidator orderValidator,
    OrderService orderService,
    InventoryService inventory,
    IShippingProvider shippingProvider)
{
    public string PlaceOrder(Cart cart, Customer customer, string paymentType)
    {
        if (cart.Lines.Count == 0)
        {
            return "checkout rejected: cart is empty";
        }

        var quote = pricing.BuildQuote(cart, customer);
        var error = orderValidator.Validate(customer, cart, quote);
        if (error is not null)
        {
            return "checkout rejected: " + error;
        }

        if (!ChargePayment(paymentType, customer, quote.Total))
        {
            return "checkout rejected: unknown payment type " + paymentType;
        }

        var order = orderService.Create(customer, cart, quote, paymentType);
        var reservedLines = new List<CartLine>();
        foreach (var line in order.Lines)
        {
            if (!inventory.ReserveStock(line.Product.Sku, line.Quantity))
            {
                foreach (var reservedLine in reservedLines)
                {
                    inventory.AdjustStock(reservedLine.Product.Sku, reservedLine.Quantity);
                }

                return "checkout rejected: stock changed for " + line.Product.Sku;
            }

            reservedLines.Add(line);
        }

        order.StockReserved = true;
        order.TrackingNumber = shippingProvider.BookShipment(order);
        var payError = orderService.Pay(order);
        if (payError is not null)
        {
            foreach (var reservedLine in reservedLines)
            {
                inventory.AdjustStock(reservedLine.Product.Sku, reservedLine.Quantity);
            }

            order.StockReserved = false;
            return "checkout rejected: " + payError;
        }

        return "checkout placed " + order.Id;
    }

    private static bool ChargePayment(string paymentType, Customer customer, decimal amount)
    {
        PaymentProcessorCreator? creator = null;
        switch (paymentType)
        {
            case "card":
                creator = new CardProcessorCreator();
                break;
            case "paypal":
                creator = new PayPalProcessorCreator();
                break;
            case "afterpay":
                creator = new AfterpayProcessorCreator();
                break;
        }

        if (creator is null)
        {
            return false;
        }

        creator.Charge(customer, amount);
        return true;
    }
}