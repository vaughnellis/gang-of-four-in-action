namespace CornerStore.Tests;

public class ScenarioRunnerTests : IDisposable
{
    private const string SmokeScenario = """
        {
          "name": "smoke",
          "startTime": "2026-03-02T09:00:00Z",
          "seed": 7,
          "steps": [
            { "op": "advance", "minutes": 30 },
            { "op": "config.set", "key": "fraudThreshold", "value": 60 },
            { "op": "bogus.op" }
          ]
        }
        """;

    private readonly string _path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".json");

    public void Dispose()
    {
        File.Delete(_path);
    }

    [Fact]
    public void Run_WithSmokeScenario_WritesHeaderThenOneBlockPerStep()
    {
        var transcript = RunScenario(SmokeScenario);

        var expected = string.Join(
            "\n",
            "CornerStore trading day: smoke (start 2026-03-02T09:00:00Z)",
            "[001] advance minutes=30",
            "  clock 2026-03-02T09:30:00Z",
            "[002] config.set key=fraudThreshold value=60",
            "  fraudThreshold set to 60",
            "[003] bogus.op",
            "  unknown op",
            "");
        Assert.Equal(expected, transcript);
    }

    [Fact]
    public void Run_TwiceOnTheSameScenario_ProducesIdenticalTranscripts()
    {
        var first = RunScenario(SmokeScenario);
        var second = RunScenario(SmokeScenario);

        Assert.Equal(first, second);
    }

    [Fact]
    public void Run_WithUnknownSetting_ReportsItAndContinues()
    {
        var transcript = RunScenario("""
            {
              "name": "unknown-setting",
              "startTime": "2026-03-02T09:00:00Z",
              "seed": 1,
              "steps": [
                { "op": "config.set", "key": "nonsense", "value": 1 },
                { "op": "advance", "minutes": 5 }
              ]
            }
            """);

        Assert.Contains("  unknown setting nonsense\n", transcript);
        Assert.Contains("  clock 2026-03-02T09:05:00Z\n", transcript);
    }

    private string RunScenario(string json)
    {
        File.WriteAllText(_path, json);
        var output = new StringWriter();

        ScenarioRunner.Run(_path, output);

        return output.ToString();
    }
}
