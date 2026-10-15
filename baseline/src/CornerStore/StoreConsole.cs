using System.Globalization;
using System.Text;
using System.Text.Json;

namespace CornerStore;

internal class StoreConsole(ScenarioClock clock)
{
    public IdSequence Ids { get; } = new();

    public void Handle(int number, JsonElement step)
    {
        var op = step.TryGetProperty("op", out var opValue) ? opValue.GetString() ?? "" : "";
        Log.Line("[" + number.ToString("D3", CultureInfo.InvariantCulture) + "] " + op + Describe(step));

        switch (op)
        {
            case "advance":
                Advance(step);
                break;
            case "config.set":
                SetConfig(step);
                break;
            default:
                Log.Info("unknown op");
                break;
        }
    }

    public bool ApplyConfig(string key, int value)
    {
        // SMELL S05: any caller can overwrite the global Config fields
        switch (key)
        {
            case "fraudThreshold":
                Config.FraudThreshold = value;
                return true;
            case "lowStockLevel":
                Config.LowStockLevel = value;
                return true;
            default:
                return false;
        }
    }

    private static string Describe(JsonElement step)
    {
        var text = new StringBuilder();
        foreach (var property in step.EnumerateObject())
        {
            if (property.Name == "op")
            {
                continue;
            }

            var value = property.Value.ValueKind == JsonValueKind.String
                ? property.Value.GetString()
                : property.Value.GetRawText();
            text.Append(' ').Append(property.Name).Append('=').Append(value);
        }

        return text.ToString();
    }

    private void Advance(JsonElement step)
    {
        if (!step.TryGetProperty("minutes", out var minutes) || !minutes.TryGetInt32(out var value))
        {
            Log.Info("advance needs minutes");
            return;
        }

        clock.Advance(TimeSpan.FromMinutes(value));
        Log.Info("clock " + clock.Stamp());
    }

    private void SetConfig(JsonElement step)
    {
        if (!step.TryGetProperty("key", out var key)
            || !step.TryGetProperty("value", out var value)
            || !value.TryGetInt32(out var number))
        {
            Log.Info("config.set needs key and value");
            return;
        }

        var name = key.GetString() ?? "";
        if (ApplyConfig(name, number))
        {
            Log.Info(name + " set to " + number.ToString(CultureInfo.InvariantCulture));
        }
        else
        {
            Log.Info("unknown setting " + name);
        }
    }
}
