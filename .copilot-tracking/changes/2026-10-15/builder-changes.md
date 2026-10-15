<!-- markdownlint-disable-file -->
# Changes: builder

Plan: `.copilot-tracking/plans/2026-10-15/builder-plan.instructions.md`
Details: `.copilot-tracking/details/2026-10-15/builder-details.md`
Planning log: `.copilot-tracking/plans/logs/2026-10-15/builder-log.md`
Critique: `.copilot-tracking/plans/critiques/2026-10-15/builder-critique.md`
Review: `.copilot-tracking/reviews/2026-10-15/builder-review.md`
Artifact completion: 100%
Implementation date: 2026-10-15

## Summary

Replaced the positional Order and Invoice assembly in `OrderService` with fluent builders that validate required data and preserve null defaults for optional properties. The Release build has zero warnings and all 45 tests pass.

## Files added or modified

* `patterns/creational/builder/src/CornerStore/Orders/OrderBuilder.cs`
* `patterns/creational/builder/src/CornerStore/Orders/InvoiceBuilder.cs`
* `patterns/creational/builder/src/CornerStore/Orders/OrderService.cs`
* `patterns/creational/builder/src/CornerStore/CornerStore.csproj`
* `patterns/creational/builder/tests/CornerStore.Tests/BuilderTests.cs`
* `patterns/creational/builder/README.md`
* `patterns/creational/builder/DIFF.md`
* `docs/pattern-map.md`
* `prompts/patterns/creational-builder.md`

The root README already linked Builder and was left unchanged. `DIFF.md` includes exactly the five source/test paths listed in the pattern README.

## Validation

| Check | Result |
| --- | --- |
| Unchanged copy tests | 41 passed, 0 failed |
| Builder-specific tests | 4 passed, 0 failed |
| Full solution tests | 45 passed, 0 failed |
| Release build with `-warnaserror` | Succeeded, 0 warnings and 0 errors |
| GoldenMasterTests copy | Hash matches baseline |
| S02 marker | Absent |
| S19 and S20 markers | Preserved |
| Editor diagnostics | No errors found |
| Root README link | Present and verified |
| `DIFF.md` summary | Exactly the five README source/test paths |
| Diff whitespace and `git diff --check` | Clean |

## Release summary

Unchanged-copy commit: `2fcb8e5`. Refactor commit: `16bfda1`. Local tag: `builder-v1` at `16bfda1bd6e06aa1289f7a54dc29a437fa0d5486`. No push was made.