namespace CornerStore.Tests;

public class DeterminismTests
{
    [Fact]
    public void TradingDay_RunTwice_IsIdentical()
    {
        var scenarioPath = Path.Combine(RepositoryRoot.Find(), "scenarios", "trading-day.json");
        var first = new StringWriter();
        var second = new StringWriter();

        ScenarioRunner.Run(scenarioPath, first);
        ScenarioRunner.Run(scenarioPath, second);

        Assert.Equal(first.ToString(), second.ToString());
    }
}