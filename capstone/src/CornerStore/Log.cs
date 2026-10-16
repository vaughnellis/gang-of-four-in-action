namespace CornerStore;

internal sealed class Log
{
    private static readonly Lazy<Log> singleton =
        new(static () => new Log(), LazyThreadSafetyMode.ExecutionAndPublication);

    private readonly object sync = new();
    private TextWriter output = TextWriter.Null;

    private Log()
    {
    }

    public static Log Instance => singleton.Value;

    public void SetOutput(TextWriter writer)
    {
        ArgumentNullException.ThrowIfNull(writer);
        lock (sync)
        {
            output = writer;
        }
    }

    public void ResetOutput()
    {
        SetOutput(TextWriter.Null);
    }

    public void Line(string text)
    {
        lock (sync)
        {
            output.Write(text + "\n");
        }
    }

    public void Info(string text)
    {
        Line("  " + text);
    }
}
