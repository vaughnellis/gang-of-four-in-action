using System.Globalization;

namespace CornerStore;

internal static class MoneyFormatter
{
    public static string Format(decimal amount, string region)
    {
        var number = amount.ToString("0.00", CultureInfo.InvariantCulture);

        // SMELL S01: the currency symbol is picked with its own region branch
        if (region == "AU")
        {
            return "A$" + number;
        }
        else if (region == "NZ")
        {
            return "NZ$" + number;
        }

        return number;
    }
}
