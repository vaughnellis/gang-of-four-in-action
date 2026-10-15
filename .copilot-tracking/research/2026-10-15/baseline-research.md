<!-- markdownlint-disable-file -->
# Research: baseline

Task slug: `baseline`
Date: 2026-10-15
Difficulty: Challenging (about 55 source files, 23 planted smells, copied 23 times, two unresolved tooling issues found by probe)

Plan: `.copilot-tracking/plans/2026-10-15/baseline-plan.instructions.md`

> Superseded in part (round 2, 2026-10-15): the user dropped Verify. Evidence E-1 to E-5 and E-7 to E-10 and the Verify rows in the alternatives table are historical. E-6 (Microsoft.Testing.Platform runner), E-11 (xUnit v3 project shape, minus the Verify package) and E-13 still apply. E-12 is out of date: see log DL-15.

## Scope

* In scope: `prompts/02-baseline.md` FR-001 to FR-018 and AC-001 to AC-008.
* Settled and not re-researched: project brief D-01 to D-12, and the foundations decisions (xUnit v3 with `Verify.XunitV3`, LF endings, plain H1 markdown).
* Research was limited to the two things the briefs cannot answer: how Verify is configured to read a shared approved file, and what the .NET 10 SDK needs for `dotnet test` with an xUnit v3 project.

## Evidence log

| ID | Evidence | Source |
| --- | --- | --- |
| E-1 | With only the foundations files in place, a test project that references `Verify.XunitV3` 33.3.3 fails `dotnet build` with SponsorCheck error SC021: "No license specified. Package 'Verify' requires a SponsorCheck license property." `TreatWarningsAsErrors` is not the cause; SC021 is an error. | Probe in a temp directory, build output |
| E-2 | Verify v33 and later is covered by the Open Source Maintenance Fee. Every version released after 2026-09-01 is covered. Versions released on or before that date are not. Individuals and non-revenue open source projects are exempt, but the exemption must be declared. | `github.com/VerifyTests/Verify` readme and `docs/maintenance-fee.md` |
| E-3 | Declaration options (one MSBuild property group in `Directory.Build.props`): sponsor with `Verify_GitHubSponsorAccount`; time-bounded license with `Verify_SponsorshipLicensedUntil`; exemption with `Verify_SponsorshipExemption` set to `SmallRevenue` or `OpenSource` plus `Verify_SponsorshipExemptionUntil` (yyyy-MM, at most 12 months ahead); or `Verify_SponsorshipLicenseIgnored`, which logs warning SC023 and therefore fails a warnings-as-errors build. | Same docs, and the SC021 message text from the probe |
| E-4 | Exemptions are time-bounded. After the declared month the build fails until the claim is renewed. An exemption is logged as message SC031, so it does not trip warnings-as-errors. | `docs/maintenance-fee.md` |
| E-5 | With the exemption declared, the probe built with zero warnings and zero errors. | Probe |
| E-6 | On SDK 10.0.302 `dotnet test GangOfFour.slnx --no-build -c Release` fails for an xUnit v3 project with "Testing with VSTest target is no longer supported by Microsoft.Testing.Platform on .NET 10 SDK and later." (the runner then tried to start a missing `.exe` because the build had failed on E-1). Adding `"test": { "runner": "Microsoft.Testing.Platform" }` to `global.json` makes `dotnet test` run the xUnit v3 project through Microsoft.Testing.Platform. | Probe |
| E-7 | With that `global.json` setting the probe test ran, failed on the first run because no approved file existed, and passed after the received file was copied over the verified file. `dotnet test --list-tests` works. | Probe |
| E-8 | Verify always names approved files `{name}.verified.{ext}`. There is no setting that drops `.verified`. `Verify(text).UseDirectory(<absolute path>).UseFileName("baseline")` wrote `baseline.received.txt` and read `baseline.verified.txt` in the absolute directory. A file named `baseline.txt` was ignored; Verify reported a new snapshot and wrote a fresh received file. | Probe, and `docs/naming.md` |
| E-9 | On the first run Verify leaves an empty `baseline.verified.txt` placeholder next to `baseline.received.txt`. Approval means copying the received content over the verified file and deleting the received file. | Probe |
| E-10 | Verify text snapshots are UTF-8 with a byte order mark, LF line endings and no trailing newline. `.gitattributes` from foundations already forces LF, so the approved file needs no extra attribute. | `readme.md` conventions section |
| E-11 | The xUnit v3 test project builds with `OutputType` Exe, `xunit.v3`, `xunit.runner.visualstudio`, `Microsoft.NET.Test.Sdk`, `Verify.XunitV3` and `Microsoft.Extensions.TimeProvider.Testing`. `[Fact]` and `Verify(...)` resolve with implicit usings only. | Probe |
| E-12 | `pwsh` is not installed on this machine. Windows PowerShell 5.1 is. The GitHub `ubuntu-latest` runner includes `pwsh`. | `pwsh --version`, runner image knowledge |
| E-13 | Git is on `main` at `e0726ab`, working tree clean, no tags. SDKs 8.0.302, 9.0.316 and 10.0.302 are installed. | `git status`, `git log`, `dotnet --list-sdks` |

## Evaluated alternatives

| Topic | Selected | Alternatives | Why |
| --- | --- | --- | --- |
| Verify fee declaration | Ask the user (default: `OpenSource` exemption until 2027-09) | Sponsor; pin an older `Verify.XunitV3` released on or before 2026-09-01; ignore | The declaration is a statement about the author's situation, so the author decides. Ignore is unusable under warnings-as-errors. Pinning an older release avoids the check, but its xUnit v3 dependency range was not verified |
| `dotnet test` runner | `global.json` `test.runner` set to Microsoft.Testing.Platform | `dotnet run --project` per test project; classic VSTest | The foundations CI command stays unchanged; VSTest is rejected by SDK 10 |
| Approved file name | `scenarios/expected/baseline.verified.txt` (ask the user) | Keep `baseline.txt` with a hand-rolled comparison instead of Verify; keep both files | D-08 requires Verify, and Verify cannot read `baseline.txt`. Two copies of the transcript would drift |
| Scripts | Cross-edition PowerShell, tested on 5.1 locally, run by `pwsh` in CI | Install PowerShell 7 locally; rewrite in C# | Installing software is the user's call; the brief asks for PowerShell |
| Scenario authoring | Grow `scenarios/trading-day.json` phase by phase | Write the whole scenario in the golden master phase | Each phase stop can show a transcript excerpt for its own smells |

## Constraints found in the briefs and instructions

* Tests drive the app only through `ScenarioRunner.Run`, so baseline tests cannot reference internals that a pattern refactor will rename. Early-phase confidence therefore comes from transcript excerpts, not internal unit tests.
* `GoldenMasterTests.cs` must be identical in every copy, so it holds no baseline-specific path. It finds `GangOfFour.slnx` by walking up from `AppContext.BaseDirectory`.
* The briefs list checkout (P03) before pricing, payments and notifications (P04, P05). Checkout cannot compile until those exist, and the cart widgets (S17) need pricing, promos and shipping. The plan therefore reorders phases by dependency (plan decision PD-1).
* `static` mutable `Config` and `Log` (S05) make the app non-reentrant. xUnit runs test classes in parallel by default, so the test project must disable parallelisation (plan decision PD-7).
* Hash codes, `string.GetHashCode()` and unordered enumeration are not allowed to influence the transcript. `Random` must be constructed per use from `seed + customer number` so that evaluation order cannot change a result when a pattern restructures validation.

## Next research topics (none blocking)

* The exact newest `Verify.XunitV3` released on or before 2026-09-01 and its xUnit v3 dependency range, only if the user picks the pin option.
* Whether MTP exits non-zero when a test project discovers zero tests. P01 avoids the question by shipping one test.
