<!-- markdownlint-disable-file -->
# Details: foundations

Plan: `.copilot-tracking/plans/2026-10-15/foundations-plan.instructions.md`
Log: `.copilot-tracking/plans/logs/2026-10-15/foundations-log.md`

## Requirement trace

| Requirement | Satisfied by |
| --- | --- |
| FR-001 | P01-S1, P01-S5 |
| FR-002 | P01-S2 |
| FR-003 | P01-S3 |
| FR-004 | P01-S4 |
| FR-005 | P02-S1 |
| FR-006 | P02-S2 to P02-S4 |
| FR-007 | P03-S1, P03-S2 |
| FR-008 | P03-S3 |
| AC-001 | P01-S6, P03-S4 |
| AC-002 | P03-S2 |
| AC-003 | P02-S1 plus existing `prompts/` and `.github/` (see critique C-4) |
| AC-004 | P02-S5 |
| AC-005 | P04-S1 |

## P01 file content

### global.json

```json
{
  "sdk": {
    "version": "10.0.302",
    "rollForward": "latestFeature",
    "allowPrerelease": false
  }
}
```

### Directory.Build.props

Properties in one `PropertyGroup`: `Nullable` enable, `TreatWarningsAsErrors` true, `ImplicitUsings` enable, `LangVersion` latest. No `TargetFramework` (PD-3).

### Directory.Packages.props

`ManagePackageVersionsCentrally` true, and `PackageVersion` items:

| Package | Version |
| --- | --- |
| `xunit.v3` | 4.0.2 |
| `xunit.runner.visualstudio` | 4.0.1 |
| `Microsoft.NET.Test.Sdk` | 18.11.0 |
| `Verify.XunitV3` | 33.3.3 |
| `Microsoft.Extensions.TimeProvider.Testing` | 10.11.0 |

If the user chooses xUnit v2 (critique C-1): `xunit` 2.9.3 and `Verify.Xunit` 31.12.5 replace the v3 pair; the other three stay.

### GangOfFour.slnx

```xml
<Solution>
  <Folder Name="/baseline/" />
  <Folder Name="/patterns/creational/" />
  <Folder Name="/patterns/structural/" />
  <Folder Name="/patterns/behavioral/" />
  <Folder Name="/capstone/" />
</Solution>
```

Empty `Folder` elements and nested folder paths without a declared parent were accepted by `dotnet build` in the probe; the implementation re-checks this with the exact file.

### .gitignore

Output of `dotnet new gitignore`, then append:

* `*.received.*` and `*.received/` for Verify output
* nothing that matches `.copilot-tracking/`, `scenarios/` or `*.slnx`

After generation, confirm with a path check that `.copilot-tracking/`, `scenarios/expected/baseline.txt` and `GangOfFour.slnx` are not ignored.

### .editorconfig

* `root = true`
* all files: `charset = utf-8`, `end_of_line = lf`, `insert_final_newline = true`, `trim_trailing_whitespace = true`
* `*.cs`: 4-space indent, `csharp_style_namespace_declarations = file_scoped:suggestion`
* `*.{csproj,props,slnx,json,yml,yaml}`: 2-space indent
* `*.md`: `trim_trailing_whitespace = false`
* No `dotnet_diagnostic.*.severity` entries and no `:warning` or `:error` severities

### LICENSE

MIT text, year 2026, holder from critique C-2. Not written until the holder is confirmed.

### .gitattributes (if PA-1 approved)

`* text=auto eol=lf`, plus `*.txt text eol=lf` and `*.received.* -diff`.

### P01-S6 commands

```text
dotnet --version
dotnet restore GangOfFour.slnx
dotnet build GangOfFour.slnx --no-restore -c Release
dotnet test GangOfFour.slnx --no-build -c Release
```

Success: each exits 0. The "Unable to find a project to restore" warning is expected and acceptable.

## P02 file content

### .gitkeep

Empty file in each of the seven folders in FR-005. `docs/` has real files and needs none.

### docs/smells-catalog.md

Frontmatter (`title: Smells catalog`, `description`), then a short sentence saying the baseline task fills it, then a table header only: Smell, Pattern, Location, What is wrong.

### docs/pattern-map.md

Frontmatter, one intro sentence, then `## Creational`, `## Structural`, `## Behavioral` sections, each with the table from project brief section 6 plus a final `Status` column set to `Not started`. 5, 7 and 11 rows. Content is copied from the brief; these are project facts, not GoF book text.

### docs/comparisons.md

Frontmatter, then four H2 headings only: Strategy vs State, Facade vs Mediator, Decorator vs Proxy, Command vs Memento for undo.

## P03 file content

### .github/workflows/ci.yml

* `name: CI`
* `on: push` and `on: pull_request`, no branch filters
* `permissions: contents: read`
* one job `build-and-test` on `ubuntu-latest`
* env `DOTNET_NOLOGO: 1` and `DOTNET_CLI_TELEMETRY_OPTOUT: 1`
* steps: `actions/checkout@v4`; `actions/setup-dotnet@v4` with `global-json-file: global.json`; `dotnet restore GangOfFour.slnx`; `dotnet build GangOfFour.slnx --no-restore -c Release`; `dotnet test GangOfFour.slnx --no-build -c Release`
* No `continue-on-error`, so any later build or test failure fails the job

Validation for AC-002: parse with a YAML parser (Python `yaml` if available, otherwise another installed parser) and assert the step list.

### README.md

Frontmatter (`title`, `description`), then sections:

* What this is: homage to the Gang of Four book, one baseline app, one solution per pattern, a capstone later
* How the repo is organised: baseline, patterns by category, capstone, scenarios, tools, docs, with the section 4 tree
* How to run a solution: prerequisites (SDK from `global.json`), `dotnet build` and `dotnet test` for `GangOfFour.slnx` and for a single solution; note that projects arrive with the baseline task
* Pattern index: table with Category, Pattern, Smell, folder link to `patterns/<category>/<slug>/`, 23 rows, folder links will resolve as patterns are added
* Built with HVE Core: links to `prompts/README.md` and `.copilot-tracking/`
* Plain prose, no em dashes, no bolded-prefix lists, no text from the GoF book

## Boundaries check list

* No `.csproj`, `.cs`, `.sln` files created
* `prompts/`, `.github/copilot-instructions.md`, `.github/instructions/` unchanged
* `scenarios/expected/` contains only `.gitkeep`
* No git init, commit, push or tag by the agent
