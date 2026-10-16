namespace CornerStore;

internal static class RegionFactoryProvider
{
    public static IRegionFactory For(string region)
    {
        return region switch
        {
            "AU" => new AustraliaRegionFactory(),
            "NZ" => new NewZealandRegionFactory(),
            _ => new DefaultRegionFactory(),
        };
    }
}
