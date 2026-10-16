namespace CornerStore;

internal static class MoneyFormatter
{
    public static string Format(decimal amount, string region)
    {
        return RegionFactoryProvider.For(region).CreateMoneyFormatter().Format(amount);
    }
}
