namespace CornerStore;

internal interface IRegionFactory
{
    IRegionalPricing CreatePricing();

    IMoneyFormatter CreateMoneyFormatter();
}
