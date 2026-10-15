<!-- markdownlint-disable-file -->
# Changes: singleton

Plan: `.copilot-tracking/plans/2026-10-16/singleton-plan.instructions.md`
Critique: `.copilot-tracking/plans/critiques/2026-10-16/singleton-critique.md`
Review: `.copilot-tracking/reviews/2026-10-16/singleton-review.md`
Implementation date: 2026-10-16

## Summary

Replaced the static `Config` and `Log` classes with sealed, thread-safe instances created by `Lazy<T>`. Configuration reads and writes are synchronized, as are log writer changes and output. Callers use the singleton instances. `ScenarioRunner` resets configuration and output for each run. The existing transcript remains unchanged.

The `CornerStore.csproj` change grants `CornerStore.Tests` internal visibility so focused tests can exercise the singleton instances without making them public. The README shows how a DI host can register these same instances; no DI dependency was added.

## Files added or modified

* `patterns/creational/singleton/src/CornerStore/Config.cs`
* `patterns/creational/singleton/src/CornerStore/Log.cs`
* `patterns/creational/singleton/src/CornerStore/ScenarioRunner.cs`
* `patterns/creational/singleton/src/CornerStore/StoreConsole.cs`
* `patterns/creational/singleton/src/CornerStore/Checkout/FraudScorer.cs`
* `patterns/creational/singleton/src/CornerStore/Checkout/OrderValidator.cs`
* `patterns/creational/singleton/src/CornerStore/Checkout/AfterpayProcessor.cs`
* `patterns/creational/singleton/src/CornerStore/Checkout/CardProcessor.cs`
* `patterns/creational/singleton/src/CornerStore/Checkout/PayPalProcessor.cs`
* `patterns/creational/singleton/src/CornerStore/Checkout/ShipFastClient.cs`
* `patterns/creational/singleton/src/CornerStore/Catalog/Catalog.cs`
* `patterns/creational/singleton/src/CornerStore/Notifications/EmailSender.cs`
* `patterns/creational/singleton/src/CornerStore/Notifications/SmsSender.cs`
* `patterns/creational/singleton/src/CornerStore/Orders/AnalyticsService.cs`
* `patterns/creational/singleton/src/CornerStore/Orders/LoyaltyService.cs`
* `patterns/creational/singleton/src/CornerStore/Widgets/ItemListWidget.cs`
* `patterns/creational/singleton/src/CornerStore/Widgets/PromoBoxWidget.cs`
* `patterns/creational/singleton/src/CornerStore/Widgets/ShippingSelectorWidget.cs`
* `patterns/creational/singleton/src/CornerStore/Widgets/TotalsPanelWidget.cs`
* `patterns/creational/singleton/src/CornerStore/CornerStore.csproj`
* `patterns/creational/singleton/tests/CornerStore.Tests/AssemblyAttributes.cs`
* `patterns/creational/singleton/tests/CornerStore.Tests/SingletonTests.cs`
* `patterns/creational/singleton/README.md`
* `patterns/creational/singleton/DIFF.md`
* `GangOfFour.slnx`
* `docs/pattern-map.md`
* `prompts/patterns/creational-singleton.md`

The root solution update was made by the required copy generator, consistent with prior pattern copy commits. The root README already contained the Singleton link and was left unchanged. `DIFF.md` contains exactly the 22 source and test paths listed under Files changed in the pattern README.

## Validation

| Check | Result |
| --- | --- |
| Unchanged-copy suite | 41 passed, 0 failed |
| Final Release solution tests | 45 passed, 0 failed |
| Release build with warnings as errors | Succeeded, 0 warnings and 0 errors |
| Golden master | Passed in the full suite; `GoldenMasterTests.cs` SHA-256 matches baseline |
| Focused Singleton tests | 4 tests passed, covering concurrent identity, configuration reset, and configured output |
| S05 and other smell markers | S05 removed; all other marker IDs and counts match baseline |
| Root README link | Present and verified |
| `DIFF.md` generation | Two successive runs produced identical SHA-256 `91528D5065C8B881DF76E8948D104136ADFF6A9DB19F02B79D0CCBA52F662C3B` |
| README and diff path list | 22 matching paths |
| Whitespace | `git diff --check` clean |

The generated diff reports 277 additions and 101 removals across 22 files.

## Release summary

Unchanged-copy commit: `496cfa0`. Refactor commit: `f1c190b`. Local tag: `singleton-v1`. No push was made.
