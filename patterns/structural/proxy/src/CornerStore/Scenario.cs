using System.Globalization;
using System.Text.Json;

namespace CornerStore;

internal class Scenario
{
    public string Name { get; init; } = "";

    public DateTimeOffset StartTime { get; init; }

    public int Seed { get; init; }

    public List<(string Key, int Value)> Settings { get; } = [];

    public List<JsonElement> Steps { get; } = [];

    public static Scenario Load(string path)
    {
        using var document = JsonDocument.Parse(File.ReadAllText(path));
        var root = document.RootElement;

        var start = root.GetProperty("startTime").GetString() ?? "";
        var scenario = new Scenario
        {
            Name = root.GetProperty("name").GetString() ?? "",
            StartTime = DateTimeOffset.Parse(
                start,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal),
            Seed = root.GetProperty("seed").GetInt32(),
        };

        if (root.TryGetProperty("config", out var config))
        {
            foreach (var setting in config.EnumerateObject())
            {
                scenario.Settings.Add((setting.Name, setting.Value.GetInt32()));
            }
        }

        foreach (var step in root.GetProperty("steps").EnumerateArray())
        {
            scenario.Steps.Add(step.Clone());
        }

        return scenario;
    }
}
