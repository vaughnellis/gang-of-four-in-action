namespace CornerStore;

internal sealed class AustraliaRegionFactory : IRegionFactory
{
    public IRegionalPricing CreatePricing()
    {
        return new RegionalPricing(0.10m, 0m);
    }

    public IMoneyFormatter CreateMoneyFormatter()
    {
        return new RegionalMoneyFormatter("A$");
    }
}
