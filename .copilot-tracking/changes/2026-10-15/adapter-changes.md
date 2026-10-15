<!-- markdownlint-disable-file -->
# Changes: adapter

Plan: `.copilot-tracking/plans/2026-10-15/adapter-plan.instructions.md`
Details: `.copilot-tracking/details/2026-10-15/adapter-details.md`
Planning log: `.copilot-tracking/plans/logs/2026-10-15/adapter-log.md`
Critique: `.copilot-tracking/plans/critiques/2026-10-15/adapter-critique.md`
Review: `.copilot-tracking/reviews/2026-10-15/adapter-review.md`
Artifact completion: 100%
Implementation date: 2026-10-15

## Summary

Introduced `IShippingProvider` and `ShipFastAdapter`. Checkout now uses the provider interface while the adapter owns XML serialization, cents conversion, client invocation, and tracking response parsing. The Release build is warning-free and all 43 tests pass.

## Files added or modified

* `patterns/structural/adapter/src/CornerStore/Checkout/IShippingProvider.cs`
* `patterns/structural/adapter/src/CornerStore/Checkout/ShipFastAdapter.cs`
* `patterns/structural/adapter/src/CornerStore/StoreConsole.cs`
* `patterns/structural/adapter/src/CornerStore/CornerStore.csproj`
* `patterns/structural/adapter/tests/CornerStore.Tests/AdapterTests.cs`
* `patterns/structural/adapter/README.md`
* `patterns/structural/adapter/DIFF.md`
* `docs/pattern-map.md`
* `prompts/patterns/structural-adapter.md`

The root README already linked the Adapter folder and was left unchanged. `DIFF.md` contains exactly the five source/test paths listed in the pattern README.

## Validation

| Check | Result |
| --- | --- |
| Unchanged copy tests | 41 passed, 0 failed |
| Adapter-specific tests | 2 passed, 0 failed |
| Full solution tests | 43 passed, 0 failed |
| Release build with `-warnaserror` | Succeeded, 0 warnings and 0 errors |
| GoldenMasterTests copy | Hash matches baseline |
| S06 marker | Absent |
| S03 and S10 markers | Preserved |
| Editor diagnostics | No errors found |
| Root README link | Present and verified |
| `DIFF.md` summary | Exactly the five README source/test paths |
| Diff whitespace and `git diff --check` | Clean |

## Release summary

Unchanged-copy commit: `e3af51e`. Refactor commit: `1348680`. Local tag: `adapter-v1` at `1348680d168593693b0fadb82f3721b892d0d211`. No push was made.