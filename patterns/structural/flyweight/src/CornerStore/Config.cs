namespace CornerStore;

// SMELL S05: global settings in a static class with public mutable fields, set from anywhere
internal static class Config
{
    public static int Seed = 0;
    public static int FraudThreshold = 80;
    public static int LowStockLevel = 3;

    public static void Reset()
    {
        Seed = 0;
        FraudThreshold = 80;
        LowStockLevel = 3;
    }
}
