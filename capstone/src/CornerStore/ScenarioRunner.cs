namespace CornerStore;

public static class ScenarioRunner
{
    public static void Run(string scenarioPath, TextWriter output)
    {
        var scenario = Scenario.Load(scenarioPath);
        var clock = new ScenarioClock(scenario.StartTime);
        var console = new StoreConsole(clock);

        var config = Config.Instance;
        config.Reset();
        config.SetSeed(scenario.Seed);
        var logger = Log.Instance;
        logger.SetOutput(output);

        try
        {
            foreach (var (key, value) in scenario.Settings)
            {
                console.ApplyConfig(key, value);
            }

            logger.Line("CornerStore trading day: " + scenario.Name + " (start " + clock.Stamp() + ")");
            for (var i = 0; i < scenario.Steps.Count; i++)
            {
                console.Handle(i + 1, scenario.Steps[i]);
            }
        }
        finally
        {
            logger.ResetOutput();
        }
    }
}
