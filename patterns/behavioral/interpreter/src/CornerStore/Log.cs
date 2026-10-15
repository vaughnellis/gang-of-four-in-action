namespace CornerStore;

// SMELL S05: global transcript writer in a static class with a public mutable field
internal static class Log
{
    public static TextWriter Out = TextWriter.Null;

    public static void Line(string text)
    {
        Out.Write(text + "\n");
    }

    public static void Info(string text)
    {
        Line("  " + text);
    }
}
