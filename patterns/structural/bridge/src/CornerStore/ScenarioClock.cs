using System.Globalization;

namespace CornerStore;

internal class ScenarioClock(DateTimeOffset start) : TimeProvider
{
    private DateTimeOffset _now = start;

    public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;

    public override DateTimeOffset GetUtcNow() => _now;

    public void Advance(TimeSpan by) => _now += by;

    public string Stamp() => _now.UtcDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture);
}
