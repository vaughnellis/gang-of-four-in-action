using System.Globalization;
using System.Xml.Linq;

namespace CornerStore;

internal sealed class CheckoutFacade(
    PricingService pricing,
    OrderValidator orderValidator,
    OrderService orderService,
    InventoryService inventory,
    ShipFastClient shipFastClient)
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
        order.TrackingNumber = BookShipment(order);
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

    // SMELL S03: payment processor construction is selected by a switch in checkout
    private static bool ChargePayment(string paymentType, Customer customer, decimal amount)
    {
        IPaymentProcessor? processor = null;
        switch (paymentType)
        {
            case "card":
                processor = new CardProcessor();
                break;
            case "paypal":
                processor = new PayPalProcessor();
                break;
            case "afterpay":
                processor = new AfterpayProcessor();
                break;
        }

        if (processor is null)
        {
            return false;
        }

        processor.Charge(customer, amount);
        return true;
    }

    // SMELL S06: checkout calls the courier client with XML and parses its response directly
    private string BookShipment(Order order)
    {
        var cents = Math.Round(order.Quote.Total * 100m, 0, MidpointRounding.AwayFromZero)
            .ToString("0", CultureInfo.InvariantCulture);
        var name = System.Security.SecurityElement.Escape(order.Customer.Name) ?? "";
        var address = System.Security.SecurityElement.Escape(order.Customer.Address) ?? "";
        var request = "<shipment><orderId>" + order.Id + "</orderId><recipient>" + name
            + "</recipient><address>" + address + "</address><amountCents>" + cents + "</amountCents></shipment>";
        var response = shipFastClient.CreateConsignmentFromXml(request);
        return XDocument.Parse(response).Root?.Element("tracking")?.Value ?? "";
    }
}