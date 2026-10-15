using Xunit.Sdk;
using Xunit.v3;

// Config and Log are static, so test runs must not overlap.
[assembly: Parallelization(Mode = ParallelMode.None)]
