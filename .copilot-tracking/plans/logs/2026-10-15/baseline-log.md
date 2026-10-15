<!-- markdownlint-disable-file -->
# Planning log: baseline

Plan: `.copilot-tracking/plans/2026-10-15/baseline-plan.instructions.md`

## Discrepancy log

| ID | Item | Resolution |
| --- | --- | --- |
| DL-1 | D-08 and the briefs said the approved transcript is `scenarios/expected/baseline.txt` while Verify can only read `*.verified.txt` (research E-8) | Resolved. The user dropped Verify and updated D-08, FR-013 and FR-014. The approved file is `baseline.txt` (PD-4). No wording fix is pending for the human |
| DL-2 | Verify v33 and later carries a build-time sponsorship check that fails the build without a declaration (research E-1 to E-4) | Resolved. Verify is removed (PD-3). No fee status is declared. Research E-1 to E-5, E-7 to E-10 are historical |
| DL-3 | `dotnet test` on SDK 10.0.302 rejects VSTest for xUnit v3 (research E-6) | `global.json` gains the `test.runner` setting (PD-2). This edits a foundations file, which no boundary forbids. CI needs no change |
| DL-4 | Brief phase order puts checkout (P03) before pricing, payments (P04) and notifications, inventory, orders (P05). Cart widgets (S17) need pricing, promos and shipping | Re-sequenced into 11 dependency-ordered phases (PD-1, mapping table in the plan). Smell content is unchanged |
| DL-5 | The brief's P07 writes the whole scenario. A phase stop with no scenario means no transcript to show for P02 to P09 | Scenario grows per phase (PD-8). P10 still owns completion, the golden master and approval |
| DL-6 | FR-016 and FR-017 ask for PowerShell 7. `pwsh` was not installed (research E-12) | The user accepted cross-edition scripts (PD-12) and said PowerShell 7 is now installed. Re-checked: `pwsh` is still not found (DL-15). P11 re-checks and tests under both `pwsh` and `powershell.exe` |
| DL-7 | The brief assigns S05 to P05, but the `Log` class is the transcript writer from P01 (FR-004) and every later phase uses it | S05 is planted in P01 (`Config.cs`, `Log.cs`). The brief's P05 list loses S05, nothing else changes |
| DL-8 | The brief's Reporting row lists S22 and S23 together. Tax and weight by line type are needed by pricing and shipping long before reports | S23 sites are planted in P04 (tax, weight) and P09 (export). The smells catalog lists all sites |
| DL-9 | The brief does not name the shipping methods | `standard`, `express`, `pickup` |
| DL-10 | The brief's 55-file estimate is mine; the brief gives no budget beyond "a handful of classes per feature" (AC-008) | Budget in PD-13. Features that need more than the budget are simplified and recorded in the changes log |
| DL-11 | hve-core `csharp.instructions.md`, `csharp-tests.instructions.md` and `powershell.instructions.md` apply to the files created here | The two C# files were read at the start of round 2. Conflicts and the repo-rule decision: XML docs on every public member (skipped, the baseline is a short teaching sample with plain names); omit `LangVersion` (kept, set by foundations); `GivenContext_WhenAction_ExpectedResult` test names and NSubstitute (repo naming and no mocks win, PD-9); `src/` at the repository root (the layout is fixed by D-04). The PowerShell file is read at P11-S1a |
| DL-12 | `prompts/README.md` still shows `<task slug>-plan.md` (foundations DL-1) | Unchanged. Still the human's edit |
| DL-13 | `.gitignore` (from `dotnet new gitignore`) contains `[Ll]ogs/`, which ignores `.copilot-tracking/plans/logs/`. `git ls-files` shows `foundations-log.md` is not tracked, and neither will `baseline-log.md` be. `prompts/README.md` says the tracking artifacts are committed | Accepted (C-13). P01-S1 adds `!.copilot-tracking/plans/logs/` (PD-14). The human then commits both logs |
| DL-14 | Round 2 user decisions on 2026-10-15: C-1 and C-2 drop Verify; C-3 11 phases with a stop after each; C-4 cross-edition scripts; C-5 scenario grows per phase, only the final scenario phase approves `baseline.txt`; C-6 size budget is a ceiling; C-13 accepted plus `*.received.txt` ignored; `global.json` runner setting accepted | Applied to plan (UR-11, PD-2, PD-3, PD-4, PD-8, PD-12, PD-13, PD-14, P01, P10, P11), details (golden master design, test project references, tool notes) and this log. The research file got a superseded banner |
| DL-15 | The user said PowerShell 7 is now installed. `pwsh` is not on PATH, `C:\Program Files\PowerShell\7` does not exist, and no `pwsh*` is in `WindowsApps` or the machine and user PATH (checked in this terminal session) | Not a P01 blocker. A terminal started before the install may have a stale PATH, but the folder check says it is absent. P11-S1a re-checks and stops to ask if still missing (critique C-14) |
| DL-16 | `.gitattributes` still has `*.received.* -diff` with a Verify comment | Left as is (PD-15). It is harmless and matches the `*.received.txt` ignore |
| DL-17 | The existing Verify block is the last block of `.gitignore`, after the Windows entries, and the `[Ll]ogs/` rule is earlier in the file | P01-S1 replaces the Verify block in place and appends the logs negation at the end, after the rule it overrides. Checked with `git check-ignore` |
| DL-18 | P03 user instruction overrides the details wording for S18 (public `Saved*` fields on `Cart`, snapshot in `CartService`) | The saved copies live in `StoreConsole`, written and read by the caller, so `Cart` holds no snapshot. No snapshot or history type, no command objects (S14). Cart widgets (S17) not touched. P04 must extend the copy code for the add-on flags |
| DL-19 | P04 design choices not fixed by the details: (a) `StoreConsole.CartAddOn` sets each add-on flag by name, which is a third S09 site next to `Cart` and `AddOnCost`; (b) `MoneyFormatter` is used for customer-facing cart and quote output, while regionless catalog output (`browse`, bundle price) keeps `Catalog.FormatPrice`; (c) `PricingService.ShippingCost` returns `decimal?` (null for an unknown method) so the method list lives only in that switch (S21); (d) the pricing entry point is `BuildQuote`, not `Quote`, to avoid a method named like its return type; (e) NZ surcharge 3.00 applies to every method, pickup included, because `ShippingCost` knows no region (S01 and S21 stay separate); (f) add-ons and shipping are not taxed |

## Pending user decisions

None. C-1 to C-6 and C-13 were answered on 2026-10-15 (DL-14). C-14 (`pwsh` not found) is informational and is re-checked at P11.

## Research performed

* Read `prompts/00-project-brief.md`, `prompts/02-baseline.md`, `prompts/README.md`, the foundations planning log and critique, both scoped instruction files, and the foundations outputs (`GangOfFour.slnx`, `Directory.Packages.props`, `.gitattributes`, `ci.yml`).
* Probed, in a temp directory outside the repo (cleaned afterwards): an xUnit v3 test project with Verify and the foundations props. Results in research E-1 to E-11.
* Read Verify's naming, maintenance-fee and readme documents.
* Checked git state and installed tooling.
* No Decisions D-01 to D-12 were re-researched.

## Implementation paths considered

| Topic | Selected | Alternatives |
| --- | --- | --- |
| Golden master mechanism | Plain xUnit v3 string comparison, received file on mismatch, `GOLDEN_MASTER_APPROVE=1` to approve (user decision, D-08) | Verify with `baseline.verified.txt`; both files |
| Phase order | 11 dependency-ordered phases | The brief's 8 phases with stubs for missing dependencies; fewer, larger phases |
| Namespaces | One `CornerStore` namespace, folders per area | A namespace per folder |
| Entry flow | `ScenarioRunner` parses; `StoreConsole` runs steps | A single class that parses and executes |
| Catalog data | Built in code | Defined in the scenario JSON |
| Product types | Abstract `Product` with three subclasses; `Bundle` separate | An enum `Kind` on one `Product` class; order lines as the type hierarchy |
| Bundles in carts | Expanded into component lines at add time | Separate bundle lines carried through pricing, tax, weight and export |
| Fraud score | `new Random(Config.Seed + customerNumber)` per call | One shared seeded `Random` |
| Test isolation | Parallelisation off for the assembly | Make `Config` and `Log` thread-local (hides the smell) |
| New-pattern copy tool | Pure PowerShell file copy and XML edit | `dotnet new` template |
| DIFF.md source | `git diff --no-index` on temp copies without `bin` and `obj` | Diff the live folders (build output leaks in) |

## Suggested follow-on work

* Add `.markdownlint.json`, markdown lint in CI and Dependabot (foundations follow-on, still open).
* SHA-pin the two GitHub Actions (foundations follow-on, still open).
* Human: confirm PowerShell 7 is on PATH before P11 (open a fresh terminal after installing; see DL-15).
* Human: commit `foundations-log.md` and `baseline-log.md` after P01 makes them visible to git.
* Pattern tasks: record the fixed order of operations that a refactor must preserve (checkout order, widget refresh order, transition guard messages) in each pattern brief.
