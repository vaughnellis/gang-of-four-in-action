using System.Globalization;

namespace CornerStore;

internal class OrderValidator(InventoryService inventory, FraudScorer fraudScorer)
{
    public string? Validate(Customer customer, Cart cart, Quote quote)
    {
        // SMELL S13: checkout validation is one nested method with ordered stock, address, fraud and credit checks
        string? error = null;
        var requestedStock = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (var line in cart.Lines)
        {
            requestedStock[line.Product.Sku] = requestedStock.GetValueOrDefault(line.Product.Sku) + line.Quantity;
        }

        foreach (var (sku, quantity) in requestedStock.OrderBy(entry => entry.Key, StringComparer.Ordinal))
        {
            if (inventory.GetStock(sku) < quantity)
            {
                error = "out of stock " + sku;
                break;
            }
        }

        if (error is null)
        {
            if (customer.Address == "")
            {
                error = "address required";
            }
            else
            {
                var fraudScore = fraudScorer.Score(customer);
                if (fraudScore > Config.FraudThreshold)
                {
                    error = "fraud score " + fraudScore.ToString(CultureInfo.InvariantCulture) + " exceeds threshold";
                }
                else if (quote.Total > customer.CreditLimit)
                {
                    error = "total exceeds credit limit";
                }
            }
        }

        return error;
    }
}