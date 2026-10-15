using System.Text;

namespace CornerStore.Tests;

public class GoldenMasterTests
{
    [Fact]
    public void TradingDay_MatchesApprovedTranscript()
    {
        var root = RepositoryRoot.Find();
        var scenarioPath = Path.Combine(root, "scenarios", "trading-day.json");
        var expectedPath = Path.Combine(root, "scenarios", "expected", "baseline.txt");
        var receivedPath = Path.Combine(root, "scenarios", "expected", "baseline.received.txt");
        var output = new StringWriter();
        ScenarioRunner.Run(scenarioPath, output);
        var actual = output.ToString();

        if (Environment.GetEnvironmentVariable("GOLDEN_MASTER_APPROVE") == "1")
        {
            File.WriteAllText(expectedPath, actual, new UTF8Encoding(false));
            return;
        }

        if (!File.Exists(expectedPath))
        {
            File.WriteAllText(receivedPath, actual, new UTF8Encoding(false));
            Assert.Fail("Approved transcript is missing; actual output was written to baseline.received.txt");
            return;
        }

        var expected = File.ReadAllText(expectedPath, Encoding.UTF8);
        if (!string.Equals(expected, actual, StringComparison.Ordinal))
        {
            File.WriteAllText(receivedPath, actual, new UTF8Encoding(false));
            Assert.Fail(FirstDifference(expected, actual));
        }
    }

    private static string FirstDifference(string expected, string actual)
    {
        var expectedLines = expected.Split('\n');
        var actualLines = actual.Split('\n');
        var index = 0;
        while (index < expectedLines.Length && index < actualLines.Length && expectedLines[index] == actualLines[index])
        {
            index++;
        }

        var expectedLine = index < expectedLines.Length ? expectedLines[index] : "<end>";
        var actualLine = index < actualLines.Length ? actualLines[index] : "<end>";
        return "Transcript differs at line " + (index + 1).ToString(System.Globalization.CultureInfo.InvariantCulture)
            + ": expected '" + Truncate(expectedLine) + "', actual '" + Truncate(actualLine) + "'";
    }

    private static string Truncate(string line)
    {
        return line.Length <= 120 ? line : line[..120] + "...";
    }
}