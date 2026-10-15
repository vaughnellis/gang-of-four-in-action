namespace CornerStore;

internal sealed class DefaultRegionFactory : IRegionFactory
{
    public IRegionalPricing CreatePricing()
    {
        return new RegionalPricing(0m, 0m);
    }

    public IMoneyFormatter CreateMoneyFormatter()
    {
        return new RegionalMoneyFormatter("");
    }
}
