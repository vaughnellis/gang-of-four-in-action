<!-- markdownlint-disable-file -->
# Changes: mediator

Plan: `.copilot-tracking/plans/2026-10-16/mediator-plan.instructions.md`
Critique: `.copilot-tracking/plans/critiques/2026-10-16/mediator-critique.md`
Review: `.copilot-tracking/reviews/2026-10-16/mediator-review.md`
Implementation date: 2026-10-16

## Summary

Added `CartMediator` as owner and coordinator of each cart's widgets. Widgets no longer reference peers directly. Item, promo and shipping changes notify the mediator, which preserves each existing refresh order. `StoreConsole` routes widget actions through the mediator. Golden-master behavior remains unchanged.

The `CornerStore.csproj` change adds `InternalsVisibleTo` for `CornerStore.Tests`, so focused tests can verify mediator participants without widening the production API.

## Files added or modified

* `patterns/behavioral/mediator/src/CornerStore/CornerStore.csproj`
* `patterns/behavioral/mediator/src/CornerStore/StoreConsole.cs`
* `patterns/behavioral/mediator/src/CornerStore/Widgets/CartMediator.cs`
* `patterns/behavioral/mediator/src/CornerStore/Widgets/ItemListWidget.cs`
* `patterns/behavioral/mediator/src/CornerStore/Widgets/PromoBoxWidget.cs`
* `patterns/behavioral/mediator/src/CornerStore/Widgets/ShippingSelectorWidget.cs`
* `patterns/behavioral/mediator/src/CornerStore/Widgets/TotalsPanelWidget.cs`
* `patterns/behavioral/mediator/tests/CornerStore.Tests/MediatorTests.cs`
* `patterns/behavioral/mediator/README.md`
* `patterns/behavioral/mediator/DIFF.md`
* `docs/pattern-map.md`
* `prompts/patterns/behavioral-mediator.md`

The root README already contained the Mediator link and was left unchanged. `DIFF.md` covers the eight source and test paths listed in the README.

## Validation

| Check | Result |
| --- | --- |
| Unchanged copy tests | 41 passed, 0 failed |
| Full Release solution tests | 45 passed, 0 failed |
| Release build with `-warnaserror` | Succeeded, 0 warnings and 0 errors |
| Golden master | Passed; `GoldenMasterTests.cs` SHA-256 matches baseline |
| Mediator tests | 4 tests passed, covering refresh order for item, promo, shipping and totals changes |
| S17 markers | Removed |
| Other smell markers | Preserved |
| Root README link | Present and verified |
| `DIFF.md` generation | Repeated generation produced identical SHA-256 `FC2B6223220F04C006206B6C90E5B3C45EEF2FD3B762CD6CBB0B18E50895DB59` |
| README and diff path list | 8 matching paths |
| Whitespace | `git diff --check` clean |

## Release summary

Unchanged-copy commit: `b651a9c`. Refactor commit: `9ee5fb6`. Local tag: `mediator-v1` at `9ee5fb6`. No push was made.
