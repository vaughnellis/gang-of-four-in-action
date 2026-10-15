using System.Globalization;

namespace CornerStore;

internal sealed class RegionalMoneyFormatter(string currencyPrefix) : IMoneyFormatter
{
    public string Format(decimal amount)
    {
        return currencyPrefix + amount.ToString("0.00", CultureInfo.InvariantCulture);
    }
}
