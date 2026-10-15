<!-- markdownlint-disable-file -->
# Changes: baseline

Plan: `.copilot-tracking/plans/2026-10-15/baseline-plan.instructions.md`
Details: `.copilot-tracking/details/2026-10-15/baseline-details.md`
Planning log: `.copilot-tracking/plans/logs/2026-10-15/baseline-log.md`
Date started: 2026-10-15

## Summary

P01 of 11 complete: app shell, entry point, deterministic clock and ID sequence, transcript writer, `Config` and `Log` (S05), the xUnit v3 test project and the baseline solution. Nothing committed, pushed or tagged.

## Phase P01: App shell and determinism

Added

* `baseline/CornerStore.Baseline.slnx`
* `baseline/src/CornerStore/CornerStore.csproj` (Exe, `net10.0`, no packages)
* `baseline/src/CornerStore/Program.cs` (parses at most one path argument, defaults to `scenarios/trading-day.json` found by walking up to `GangOfFour.slnx`, calls `ScenarioRunner.Run`)
* `baseline/src/CornerStore/ScenarioRunner.cs` (the one public type; loads the scenario, resets and sets `Config` and `Log`, writes the header, dispatches steps)
* `baseline/src/CornerStore/Scenario.cs` (JSON loader: name, startTime, seed, optional config block, steps)
* `baseline/src/CornerStore/ScenarioClock.cs` (`TimeProvider` fixed at `startTime`, UTC, `Advance`)
* `baseline/src/CornerStore/IdSequence.cs` (`cu-1`, `o-1001`, `SF-0001`; instance counters; not used until P03)
* `baseline/src/CornerStore/StoreConsole.cs` (step dispatcher with ops `advance` and `config.set`; unknown ops print `unknown op` and continue)
* `baseline/src/CornerStore/Config.cs`, `Log.cs` (S05)
* `baseline/tests/CornerStore.Tests/CornerStore.Tests.csproj` (xUnit v3 Exe; `xunit.v3`, `xunit.runner.visualstudio`, `Microsoft.NET.Test.Sdk`)
* `baseline/tests/CornerStore.Tests/AssemblyAttributes.cs` (test parallelisation off, PD-7)
* `baseline/tests/CornerStore.Tests/ScenarioRunnerTests.cs` (3 tests: header and step blocks, determinism, unknown setting continues)

Modified

* `global.json`: added `"test": { "runner": "Microsoft.Testing.Platform" }` (PD-2). Edits a foundations file; no boundary forbids it
* `Directory.Packages.props`: removed `Verify.XunitV3` (PD-3)
* `.gitignore`: replaced the Verify block with `*.received.txt`; appended `!.copilot-tracking/plans/logs/` so planning logs are committed (PD-14). Both rules checked with `git check-ignore`
* `GangOfFour.slnx`: both baseline projects added under the `baseline` folder

Removed

* None

Deviations and decisions

* `xunit.v3` 4.0.2 needs `<Using Include="Xunit" />` in the test project (research E-11 said implicit usings were enough; it was wrong for this package set).
* `CollectionBehavior(DisableTestParallelization = true)` is an error in xunit.v3 4.0.2. The replacement is `[assembly: Parallelization(Mode = ParallelMode.None)]` (namespaces `Xunit.Sdk` and `Xunit.v3`).
* `Program.Main` also guards the argument count and a missing file (exit codes 2 and 1). This is argument handling at the boundary, not scenario logic.
* S05 markers sit in four files: `Config.cs`, `Log.cs`, `ScenarioRunner.cs` and `StoreConsole.cs`, because the smell is "set from anywhere".
* All types except `ScenarioRunner` are `internal`, so the single public entry point (FR-002) is the only public surface.
* hve-core C# instruction conflicts, repo rules win: no XML docs on members; `LangVersion` stays `latest` from foundations; behaviour-describing test names instead of `GivenContext_WhenAction_ExpectedResult`; no NSubstitute (baseline tests use only `ScenarioRunner.Run`).
* `baseline/.gitkeep` is left in place (foundations file, harmless).
* The pattern-leak scan matched `StringBuilder` in `StoreConsole.cs`: a BCL type, not the Builder pattern.
* `Microsoft.Extensions.TimeProvider.Testing` stays pinned in `Directory.Packages.props` but is not referenced yet.

Validation (CI flags, Release)

* `dotnet restore GangOfFour.slnx`: exit 0
* `dotnet build GangOfFour.slnx --no-restore -c Release`: 0 warnings, 0 errors; also clean with `--no-incremental`
* `dotnet test GangOfFour.slnx --no-build -c Release`: 3 passed, 0 failed
* `dotnet test baseline/CornerStore.Baseline.slnx -c Release`: 3 passed
* AC-006 search in `baseline/src`: 0 hits
* Running the app twice on a temp scenario: identical output
* `baseline/src`: 8 files, 280 lines (ceiling 60 files and 2,800 lines)
* Files: LF, no BOM, ASCII only, no em dashes

## Follow-on and open items

* `pwsh` still not found on this machine (critique C-14). P11-S1a re-checks.
* After this commit the human can also commit `foundations-log.md` and `baseline-log.md`, now visible to git.
