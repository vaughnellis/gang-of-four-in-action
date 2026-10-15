<!-- markdownlint-disable-file -->
# Changes: prototype

Plan: `.copilot-tracking/plans/2026-10-16/prototype-plan.instructions.md`
Critique: `.copilot-tracking/plans/critiques/2026-10-16/prototype-critique.md`
Review: `.copilot-tracking/reviews/2026-10-16/prototype-review.md`
Implementation date: 2026-10-16

## Summary

Added a physical-product `Clone` operation that preserves the prototype's product fields and overrides SKU, name, size and colour for a variant. `Catalog.CreateVariant` retains its existing validations and registration behavior while delegating product construction to the prototype.

The project file grants `CornerStore.Tests` internal visibility so focused tests can verify the internal catalog and product types without widening production APIs. The README lists and explains this project-file change.

## Files added or modified

* `patterns/creational/prototype/src/CornerStore/Catalog/Catalog.cs`
* `patterns/creational/prototype/src/CornerStore/Catalog/PhysicalProduct.cs`
* `patterns/creational/prototype/src/CornerStore/CornerStore.csproj`
* `patterns/creational/prototype/tests/CornerStore.Tests/PrototypeTests.cs`
* `patterns/creational/prototype/README.md`
* `patterns/creational/prototype/DIFF.md`
* `GangOfFour.slnx`
* `docs/pattern-map.md`
* `prompts/patterns/creational-prototype.md`

The root solution was updated by the required copy generator. The root README already contained the Prototype index link and was left unchanged. `DIFF.md` covers exactly the four source and test files listed under Files changed in the pattern README.

## Validation

| Check | Result |
| --- | --- |
| Unchanged-copy Release tests | 41 passed, 0 failed |
| Final Release tests | 43 passed, 0 failed |
| Release build with warnings as errors | Succeeded, 0 warnings and 0 errors |
| Golden master | Passed in both test runs; `GoldenMasterTests.cs` SHA-256 matches baseline |
| S04 and other smell markers | S04 removed; other markers in `Catalog.cs` preserved |
| Root README link | Present and verified |
| `DIFF.md` generation | Two successive runs produced identical SHA-256 `D1A5DD11098C3C758585D44068DBCD7EFDDEF6C71E527C5B36E187858EB23B30` |
| README and diff path list | 4 matching paths |
| Whitespace | `git diff --check` clean |

The generated diff reports 92 additions and 16 removals across four files.

## Release summary

Unchanged-copy commit: `afd5fe0`. Refactor commit: `d284c52`. Local tag: `prototype-v1`. No push was made.
