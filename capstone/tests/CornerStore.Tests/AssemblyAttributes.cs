using Xunit.Sdk;
using Xunit.v3;

// Config and Log are process-wide singletons, so test runs must not overlap.
[assembly: Parallelization(Mode = ParallelMode.None)]
