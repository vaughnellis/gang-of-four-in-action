using System.Globalization;

namespace CornerStore;

internal class PromoEngine
{
    private static readonly Dictionary<string, (string Rule, decimal Percent, decimal Amount)> Promos = new()
    {
        ["SHOES10"] = ("SPEND>100 AND CATEGORY=Shoes", 10m, 0m),
        ["BIGSPEND"] = ("SPEND>250", 0m, 20m),
        ["NZFRIEND"] = ("REGION=NZ AND QTY>2", 5m, 0m),
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
        // SMELL S15: the rule text is parsed by hand with Split and IndexOf on every call
        var terms = Promos[code].Rule.Split(" AND ");
        foreach (var term in terms)
        {
            var greater = term.IndexOf('>');
            if (greater > 0)
            {
                var name = term.Substring(0, greater);
                var limit = decimal.Parse(term.Substring(greater + 1), CultureInfo.InvariantCulture);
                if (name == "SPEND")
                {
                    if (subtotal <= limit)
                    {
                        return false;
                    }
                }
                else if (name == "QTY")
                {
                    var quantity = 0;
                    foreach (var line in cart.Lines)
                    {
                        quantity += line.Quantity;
                    }

                    if (quantity <= limit)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }

                continue;
            }

            var equals = term.IndexOf('=');
            if (equals > 0)
            {
                var name = term.Substring(0, equals);
                var value = term.Substring(equals + 1);
                if (name == "CATEGORY")
                {
                    var found = false;
                    foreach (var line in cart.Lines)
                    {
                        if (line.Product.CategoryName == value)
                        {
                            found = true;
                        }
                    }

                    if (!found)
                    {
                        return false;
                    }
                }
                else if (name == "REGION")
                {
                    if (customer.Region != value)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        return true;
    }
}
