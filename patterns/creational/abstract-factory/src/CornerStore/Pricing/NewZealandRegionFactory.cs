namespace CornerStore;

internal sealed class NewZealandRegionFactory : IRegionFactory
{
    public IRegionalPricing CreatePricing()
    {
        return new RegionalPricing(0.15m, 3.00m);
    }

    public IMoneyFormatter CreateMoneyFormatter()
    {
        return new RegionalMoneyFormatter("NZ$");
    }
}
