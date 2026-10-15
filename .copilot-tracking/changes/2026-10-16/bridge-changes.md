<!-- markdownlint-disable-file -->
# Changes: bridge

Plan: `.copilot-tracking/plans/2026-10-16/bridge-plan.instructions.md`
Critique: `.copilot-tracking/plans/critiques/2026-10-16/bridge-critique.md`
Review: `.copilot-tracking/reviews/2026-10-16/bridge-review.md`
Implementation date: 2026-10-16

## Summary

Separated order confirmation and shipping messages from their delivery channels. Two message abstractions now delegate through `IChannel`, implemented by email and SMS senders. The customer's existing channel selection and emitted transcript are unchanged.

The `CornerStore.csproj` change adds `InternalsVisibleTo` for `CornerStore.Tests`, allowing focused tests to exercise internal bridge participants without widening the production API.

## Files added or modified

* `patterns/structural/bridge/src/CornerStore/CornerStore.csproj`
* `patterns/structural/bridge/src/CornerStore/Notifications/EmailSender.cs`
* `patterns/structural/bridge/src/CornerStore/Notifications/IChannel.cs`
* `patterns/structural/bridge/src/CornerStore/Notifications/OrderConfirmedEmail.cs` (removed)
* `patterns/structural/bridge/src/CornerStore/Notifications/OrderConfirmedMessage.cs`
* `patterns/structural/bridge/src/CornerStore/Notifications/OrderConfirmedSms.cs` (removed)
* `patterns/structural/bridge/src/CornerStore/Notifications/OrderMessage.cs`
* `patterns/structural/bridge/src/CornerStore/Notifications/OrderShippedEmail.cs` (removed)
* `patterns/structural/bridge/src/CornerStore/Notifications/OrderShippedMessage.cs`
* `patterns/structural/bridge/src/CornerStore/Notifications/OrderShippedSms.cs` (removed)
* `patterns/structural/bridge/src/CornerStore/Notifications/SmsSender.cs`
* `patterns/structural/bridge/src/CornerStore/Orders/OrderService.cs`
* `patterns/structural/bridge/src/CornerStore/StoreConsole.cs`
* `patterns/structural/bridge/tests/CornerStore.Tests/BridgeTests.cs`
* `patterns/structural/bridge/README.md`
* `patterns/structural/bridge/DIFF.md`
* `docs/pattern-map.md`
* `prompts/patterns/structural-bridge.md`

The root README already contained the Bridge link and was left unchanged. `DIFF.md` covers all 14 source and test paths listed in the README.

## Validation

| Check | Result |
| --- | --- |
| Unchanged copy tests | 41 passed, 0 failed |
| Full Release solution tests | 45 passed, 0 failed |
| Release build with `-warnaserror` | Succeeded, 0 warnings and 0 errors |
| Golden master | Passed; `GoldenMasterTests.cs` SHA-256 matches baseline |
| Bridge tests | 4 theory cases passed within the 45-test suite |
| S07 markers | Removed |
| Other smell markers | Preserved, including S02, S19 and S20 in `OrderService` |
| Root README link | Present and verified |
| `DIFF.md` generation | Repeated generation produced identical SHA-256 `26F3C1829EFC97E205D71794E2F36B413E24F06D3E3BD9D7FBF2AA5950CA1655` |
| README and diff path list | 14 matching paths |
| Whitespace | `git diff --check` clean |

## Release summary

Unchanged-copy commit: `6f8c853`. Refactor commit: pending. Local tag: pending.
