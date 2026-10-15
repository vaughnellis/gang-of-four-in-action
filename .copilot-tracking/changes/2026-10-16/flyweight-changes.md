<!-- markdownlint-disable-file -->
# Changes: flyweight

Plan: `.copilot-tracking/plans/2026-10-16/flyweight-plan.instructions.md`
Critique: `.copilot-tracking/plans/critiques/2026-10-16/flyweight-critique.md`
Review: `.copilot-tracking/reviews/2026-10-16/flyweight-review.md`
Implementation date: 2026-10-16

## Summary

Moved repeated listing metadata into immutable `ProductSpec` flyweights. `ProductSpecFactory` caches one instance per brand, material and origin combination; each listing retains its own SKU and price. Added focused sharing tests and a separate BenchmarkDotNet project comparing the original per-listing string construction with the Flyweight importer.

BenchmarkDotNet 0.15.8 was added to the repository's central package version file. The benchmark project is included in the pattern solution, while the production executable remains free of a BenchmarkDotNet reference. `CornerStore.csproj` grants internal access to the test and benchmark assemblies so neither requires widening production types.

## Files added or modified

* `patterns/structural/flyweight/src/CornerStore/Catalog/Listing.cs`
* `patterns/structural/flyweight/src/CornerStore/Catalog/ListingImporter.cs`
* `patterns/structural/flyweight/src/CornerStore/Catalog/ProductSpec.cs`
* `patterns/structural/flyweight/src/CornerStore/Catalog/ProductSpecFactory.cs`
* `patterns/structural/flyweight/src/CornerStore/CornerStore.csproj`
* `patterns/structural/flyweight/tests/CornerStore.Tests/FlyweightTests.cs`
* `patterns/structural/flyweight/benchmarks/CornerStore.Benchmarks/CornerStore.Benchmarks.csproj`
* `patterns/structural/flyweight/benchmarks/CornerStore.Benchmarks/Program.cs`
* `patterns/structural/flyweight/benchmarks/CornerStore.Benchmarks/ListingImportBenchmarks.cs`
* `patterns/structural/flyweight/Flyweight.slnx`
* `Directory.Packages.props`
* `docs/pattern-map.md`
* `prompts/patterns/structural-flyweight.md`
* `patterns/structural/flyweight/README.md`
* `patterns/structural/flyweight/DIFF.md`
* `GangOfFour.slnx` was updated by the required copy generator to include this pattern's app and test projects.

The root README already contained the Flyweight index link and was left unchanged. The generated `DIFF.md` covers six source and test paths, all of which are listed in the pattern README. It does not include benchmark project files or the central package file because the diff generator compares the baseline and pattern source/test trees.

## Validation

| Check | Result |
| --- | --- |
| Unchanged-copy Release tests | 41 passed, 0 failed |
| Final Flyweight Release tests | 43 passed, 0 failed |
| Flyweight solution Release build | Succeeded, 0 warnings and 0 errors; includes benchmark project |
| BenchmarkDotNet ShortRun | Completed both methods over 100,000 listings |
| Allocated memory | Repeated metadata: 42.23 MB; shared product specs: 13.98 MB, ratio 0.33 |
| Benchmark environment | Windows 11, .NET 10.0.10, SDK 10.0.302, AMD Ryzen 7 4800H; BenchmarkDotNet 0.15.8, three iterations |
| Golden master | Passed in the test suite; `GoldenMasterTests.cs` SHA-256 matches baseline |
| S11 and other smell markers | S11 removed; all 42 non-S11 marker/file combinations match baseline |
| Root README link | Present and verified |
| README and diff paths | All six generated diff paths are listed in the README |
| `DIFF.md` | 157 lines; 73 additions and 12 removals across six files; SHA-256 `F3B140F4EC671E05C834A14C761CB7AC83F5670F34861B6B222ED791984E3BC9` |
| Whitespace | `git diff --check` clean |

The root `GangOfFour.slnx` build fails with duplicate `CornerStore` project names in `/patterns/behavioral/`. The same duplicate names (10 app projects and 10 test projects) are present in the solution before the Flyweight copy commit. This pre-existing solution-organization issue is unrelated to S11 and was not changed; the Flyweight-specific solution builds successfully.

## Release summary

Unchanged-copy commit: `b8459d3`. Refactor commit: `74b9d1d`. Local tag: `flyweight-v1`. No push was made.
