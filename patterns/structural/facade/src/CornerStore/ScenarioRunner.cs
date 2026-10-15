namespace CornerStore;

public static class ScenarioRunner
{
    public static void Run(string scenarioPath, TextWriter output)
    {
        var scenario = Scenario.Load(scenarioPath);
        var clock = new ScenarioClock(scenario.StartTime);
        var console = new StoreConsole(clock);

        // SMELL S05: the runner sets the global Config and Log fields directly
        Config.Reset();
        Config.Seed = scenario.Seed;
        Log.Out = output;

        try
        {
            foreach (var (key, value) in scenario.Settings)
            {
                console.ApplyConfig(key, value);
            }

            Log.Line("CornerStore trading day: " + scenario.Name + " (start " + clock.Stamp() + ")");
            for (var i = 0; i < scenario.Steps.Count; i++)
            {
                console.Handle(i + 1, scenario.Steps[i]);
            }
        }
        finally
        {
            Log.Out = TextWriter.Null;
        }
    }
}
