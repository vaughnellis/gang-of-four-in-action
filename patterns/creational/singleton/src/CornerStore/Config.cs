namespace CornerStore;

internal sealed class Config
{
    private static readonly Lazy<Config> singleton =
        new(static () => new Config(), LazyThreadSafetyMode.ExecutionAndPublication);

    private readonly object sync = new();
    private int seed;
    private int fraudThreshold = 80;
    private int lowStockLevel = 3;

    private Config()
    {
    }

    public static Config Instance => singleton.Value;

    public int Seed
    {
        get
        {
            lock (sync)
            {
                return seed;
            }
        }
    }

    public int FraudThreshold
    {
        get
        {
            lock (sync)
            {
                return fraudThreshold;
            }
        }
    }

    public int LowStockLevel
    {
        get
        {
            lock (sync)
            {
                return lowStockLevel;
            }
        }
    }

    public void SetSeed(int value)
    {
        lock (sync)
        {
            seed = value;
        }
    }

    public void SetFraudThreshold(int value)
    {
        lock (sync)
        {
            fraudThreshold = value;
        }
    }

    public void SetLowStockLevel(int value)
    {
        lock (sync)
        {
            lowStockLevel = value;
        }
    }

    public void Reset()
    {
        lock (sync)
        {
            seed = 0;
            fraudThreshold = 80;
            lowStockLevel = 3;
        }
    }
}
