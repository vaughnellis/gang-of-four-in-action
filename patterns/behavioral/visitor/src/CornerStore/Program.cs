namespace CornerStore;

internal static class Program
{
    private static int Main(string[] args)
    {
        if (args.Length > 1)
        {
            Console.Error.WriteLine("usage: CornerStore [scenario.json]");
            return 2;
        }

        var path = args.Length == 1 ? args[0] : FindDefaultScenario();
        if (!File.Exists(path))
        {
            Console.Error.WriteLine("Scenario not found: " + path);
            return 1;
        }

        ScenarioRunner.Run(path, Console.Out);
        return 0;
    }

    private static string FindDefaultScenario()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "GangOfFour.slnx")))
        {
            directory = directory.Parent;
        }

        var root = directory?.FullName ?? ".";
        return Path.Combine(root, "scenarios", "trading-day.json");
    }
}
