using BenchmarkDotNet.Running;

namespace CornerStore.Benchmarks;

internal static class Program
{
    private static void Main(string[] args)
    {
        BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
    }
}
