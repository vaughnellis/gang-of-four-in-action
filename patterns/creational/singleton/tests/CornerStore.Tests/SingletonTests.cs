namespace CornerStore.Tests;

public class SingletonTests
{
    [Fact]
    public void Config_WhenAccessedConcurrently_ReturnsSameInstance()
    {
        var instances = new Config[32];

        Parallel.For(0, instances.Length, index =>
        {
            instances[index] = Config.Instance;
        });

        Assert.All(instances, instance => Assert.Same(instances[0], instance));
    }

    [Fact]
    public void Config_WhenChangedAndReset_RestoresDefaults()
    {
        var config = Config.Instance;
        config.Reset();

        try
        {
            config.SetSeed(17);
            config.SetFraudThreshold(42);
            config.SetLowStockLevel(5);

            Assert.Equal(17, config.Seed);
            Assert.Equal(42, config.FraudThreshold);
            Assert.Equal(5, config.LowStockLevel);

            config.Reset();

            Assert.Equal(0, config.Seed);
            Assert.Equal(80, config.FraudThreshold);
            Assert.Equal(3, config.LowStockLevel);
        }
        finally
        {
            config.Reset();
        }
    }

    [Fact]
    public void Log_WhenAccessedConcurrently_ReturnsSameInstance()
    {
        var instances = new Log[32];

        Parallel.For(0, instances.Length, index =>
        {
            instances[index] = Log.Instance;
        });

        Assert.All(instances, instance => Assert.Same(instances[0], instance));
    }

    [Fact]
    public void Log_WhenConfigured_WritesFormattedLines()
    {
        var logger = Log.Instance;
        using var output = new StringWriter();
        logger.SetOutput(output);

        try
        {
            logger.Info("message");

            Assert.Equal("  message\n", output.ToString());
        }
        finally
        {
            logger.ResetOutput();
        }
    }
}
