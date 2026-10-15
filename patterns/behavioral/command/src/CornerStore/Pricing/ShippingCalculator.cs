namespace CornerStore;

internal static class ShippingCalculator
{
    public static decimal RegionSurcharge(string region)
    {
        // SMELL S01: the regional shipping surcharge is picked with its own region branch
        if (region == "AU")
        {
            return 0m;
        }
        else if (region == "NZ")
        {
            return 3.00m;
        }

        return 0m;
    }
}
