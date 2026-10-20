<!-- markdownlint-disable-file -->
# README release audit

Date: 2026-10-20

Status: Pass after scoped documentation fixes. No commit, push or tag was made.

## Scope and method

Audited the current working tree, including uncommitted changes, against [the README standard](../../../prompts/08-readmes.md) and [the project brief](../../../prompts/00-project-brief.md).
The working tree was clean at audit entry. The inventory contains 30 READMEs: the 29 required learning pages and the existing prompts guide.
The prompts guide was inspected but not modified.

The historical boundary is `df248cac3b5be56c0cc4f3a4a69b637ed1ef2938`, the parent of the README implementation commit `2b1ab874af70af1659f32d26ee4d0c1eeb8f36a4`.
That boundary includes the README brief's setup but precedes its implementation.

The checks covered:

1. Required README inventory.
2. The ten exact FR-001 headings in order, smell ID, changed-file list and count against each DIFF, book-order navigation, and exactly one blog placeholder per pattern.
3. Every fenced source excerpt against the file linked immediately before it. Comparison permits common indentation and line-ending differences, but preserves internal whitespace and blank lines.
4. Every relative Markdown link, including generated heading anchors, comparison anchors and directory links.
5. Every runnable command in Run it and Quick start sections. Baseline and capstone build/test commands and the root's additional scenario command were also executed.
6. Every Mermaid block parsed using Mermaid 11 locally.
7. Category indexes and all 23 baseline smell-to-pattern mappings.
8. Root learning path, grouped book-order index, provenance links and counts.
9. Full scaffold creation in both PowerShell hosts, using isolated copies of the current generator and baseline.
10. No em dashes or YAML frontmatter in any README.
11. Historical and current-worktree scope.
12. The exact restore, build and test commands from CI.

## Failures found and fixed

| Finding | Fix | Recheck |
| --- | --- | --- |
| All 23 pattern tables listed changed files but omitted a numeric count. | Added a File count row derived from each folder's DIFF summary, preserving the complete file lists. | Pass |
| Abstract Factory linked backward to a category page; Visitor wrapped forward to Abstract Factory. | Explicitly state that the first and last patterns have no previous or next pattern. | Pass |
| Composite and Flyweight excerpts omitted blank lines present in the linked source. | Restored those blank lines without changing excerpt content. | Pass |
| Two Singleton excerpts relied on an earlier source link. | Added an immediate Config.cs link for each excerpt. The code itself was already accurate. | Pass |
| Root provenance used unlinked tracking references. | Linked HVE Core, the tracking folder and both progress records. Verified the existing counts. | Pass |
| Root and capstone READMEs had YAML frontmatter. | Removed the frontmatter and retained their plain H1 headings and content. | Pass |

## Updated per-README matrix

Numbers correspond to the checks above. A dash means the check does not apply.
Check 2 includes headings, metadata, navigation and the blog line.
Check 7 includes category order or baseline smell mappings.

| README | 1 Inventory | 2 Pattern | 3 Excerpts | 4 Links | 5 Commands | 6 Mermaid | 7 Index/smells | 8 Root | 10 Writing |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| [Root](../../../README.md) | Pass | - | - | Pass | Pass | - | Pass | Pass | Pass |
| [Baseline](../../../baseline/README.md) | Pass | - | - | Pass | Pass | - | Pass | - | Pass |
| [Creational category](../../../patterns/creational/README.md) | Pass | - | - | Pass | - | - | Pass | - | Pass |
| [Abstract Factory](../../../patterns/creational/abstract-factory/README.md) | Pass | Pass | Pass | Pass | Pass | Pass | - | - | Pass |
| [Builder](../../../patterns/creational/builder/README.md) | Pass | Pass | Pass | Pass | Pass | Pass | - | - | Pass |
| [Factory Method](../../../patterns/creational/factory-method/README.md) | Pass | Pass | Pass | Pass | Pass | Pass | - | - | Pass |
| [Prototype](../../../patterns/creational/prototype/README.md) | Pass | Pass | Pass | Pass | Pass | Pass | - | - | Pass |
| [Singleton](../../../patterns/creational/singleton/README.md) | Pass | Pass | Pass | Pass | Pass | Pass | - | - | Pass |
| [Structural category](../../../patterns/structural/README.md) | Pass | - | - | Pass | - | - | Pass | - | Pass |
| [Adapter](../../../patterns/structural/adapter/README.md) | Pass | Pass | Pass | Pass | Pass | Pass | - | - | Pass |
| [Bridge](../../../patterns/structural/bridge/README.md) | Pass | Pass | Pass | Pass | Pass | Pass | - | - | Pass |
| [Composite](../../../patterns/structural/composite/README.md) | Pass | Pass | Pass | Pass | Pass | Pass | - | - | Pass |
| [Decorator](../../../patterns/structural/decorator/README.md) | Pass | Pass | Pass | Pass | Pass | Pass | - | - | Pass |
| [Facade](../../../patterns/structural/facade/README.md) | Pass | Pass | Pass | Pass | Pass | Pass | - | - | Pass |
| [Flyweight](../../../patterns/structural/flyweight/README.md) | Pass | Pass | Pass | Pass | Pass | Pass | - | - | Pass |
| [Proxy](../../../patterns/structural/proxy/README.md) | Pass | Pass | Pass | Pass | Pass | Pass | - | - | Pass |
| [Behavioral category](../../../patterns/behavioral/README.md) | Pass | - | - | Pass | - | - | Pass | - | Pass |
| [Chain of Responsibility](../../../patterns/behavioral/chain-of-responsibility/README.md) | Pass | Pass | Pass | Pass | Pass | Pass | - | - | Pass |
| [Command](../../../patterns/behavioral/command/README.md) | Pass | Pass | Pass | Pass | Pass | Pass | - | - | Pass |
| [Interpreter](../../../patterns/behavioral/interpreter/README.md) | Pass | Pass | Pass | Pass | Pass | Pass | - | - | Pass |
| [Iterator](../../../patterns/behavioral/iterator/README.md) | Pass | Pass | Pass | Pass | Pass | Pass | - | - | Pass |
| [Mediator](../../../patterns/behavioral/mediator/README.md) | Pass | Pass | Pass | Pass | Pass | Pass | - | - | Pass |
| [Memento](../../../patterns/behavioral/memento/README.md) | Pass | Pass | Pass | Pass | Pass | Pass | - | - | Pass |
| [Observer](../../../patterns/behavioral/observer/README.md) | Pass | Pass | Pass | Pass | Pass | Pass | - | - | Pass |
| [State](../../../patterns/behavioral/state/README.md) | Pass | Pass | Pass | Pass | Pass | Pass | - | - | Pass |
| [Strategy](../../../patterns/behavioral/strategy/README.md) | Pass | Pass | Pass | Pass | Pass | Pass | - | - | Pass |
| [Template Method](../../../patterns/behavioral/template-method/README.md) | Pass | Pass | Pass | Pass | Pass | Pass | - | - | Pass |
| [Visitor](../../../patterns/behavioral/visitor/README.md) | Pass | Pass | Pass | Pass | Pass | Pass | - | - | Pass |
| [Capstone](../../../capstone/README.md) | Pass | - | - | Pass | Pass | Pass | - | - | Pass |
| [Prompts guide](../../../prompts/README.md) | Pass | - | - | Pass | - | - | - | - | Pass |

## Repo-wide checks

| Check | Result | Evidence |
| --- | --- | --- |
| Inventory and FR-001 | Pass | All 29 required pages exist; all 23 pattern pages have the ten exact sections and one exact blog line. The prompts guide brings the total to 30. |
| Excerpts | Pass | All 74 source excerpts match their linked current files after scoped fixes. |
| Relative links and anchors | Pass | All 392 relative README links resolve, including comparisons anchors and book-order navigation. |
| Documented commands | Pass | All 75 baseline, pattern and capstone commands succeeded. This includes both Flyweight short benchmarks, not just the benchmark process exit code. |
| Scenarios | Pass | Baseline plus all 23 standalone pattern scenarios each emitted 417 transcript lines. Their test runs include the shared golden master. |
| Root Quick start | Pass | Cloned the public repository at `2b1ab87` into a temporary directory and ran restore, Release build and tests, plus the documented baseline scenario. The clone matched the current application code and was removed afterwards. |
| Mermaid | Pass | All 44 blocks passed the local Mermaid 11 parser: 43 in pattern READMEs and one in the capstone. The external validator timed out; no external-rendering success is claimed. |
| Category and baseline indexes | Pass | All category tables follow FR-003 book order. Baseline S01-S23 each link to the corresponding pattern folder and smells catalog. |
| Root counts | Pass | Run-all: 24 completed tasks, including 23 patterns. Capstone: 24 completed steps and 131 tests. Counts agree with both progress records. |
| 9: Scaffold under pwsh | Pass | Full script invocation created the scratch solution, both project copies and README; ten headings and one blog placeholder matched. All 68 source/test files copied byte-identically. |
| 9: Scaffold under powershell.exe | Pass | Same full-script assertions passed under Windows PowerShell. |
| 9: Scratch cleanup | Pass | Both isolated fixture trees, including scratch patterns, were removed. No scratch pattern or umbrella solution change remains in the repository. |
| 10: Writing | Pass | None of the 30 READMEs has an em dash or YAML frontmatter. |
| 11: Historical scope | Pass | Diff from `df248ca` to the working tree has no changes under any src or tests directory, scenarios, prompts or any DIFF.md. Only README files, the earlier README scaffold update and tracking records differ. |
| 11: Audit-only scope | Pass | This audit changes 25 READMEs and adds this report. It does not modify the already-correct generator, application code, tests, DIFFs, scenarios, prompts, workflow or protected instructions. |
| 12: CI restore/build/test | Pass | Exact commands from CI succeeded on the working tree: Release build with zero warnings and errors; all 1,205 tests passed, none failed or skipped. |
| Whitespace and review state | Pass | git diff --check passed. All changes remain uncommitted for maintainer review. |

CI commands executed from the repository root:

```powershell
dotnet restore GangOfFour.slnx
dotnet build GangOfFour.slnx --no-restore -c Release
dotnet test GangOfFour.slnx --no-build -c Release
```

## Review outcome

All requested checks pass after the documentation fixes. No application or test changes were needed.
The existing scaffold already meets FR-008 and passed both full scratch invocations.
There are no remaining failures within this audit's scope.

The next action is maintainer review and commit. Release execution remains outside this task.
