<!-- markdownlint-disable-file -->
# Changes: memento

Plan: `.copilot-tracking/plans/2026-10-16/memento-plan.instructions.md`
Critique: `.copilot-tracking/plans/critiques/2026-10-16/memento-critique.md`
Review: `.copilot-tracking/reviews/2026-10-16/memento-review.md`
Implementation date: 2026-10-16

## Summary

Added an opaque `CartMemento` that stores cart state privately. `Cart` owns capture, restore and cart-line copying; `StoreConsole` keeps one pre-change memento per customer and replaces it only after a successful item change. Undo restores the prior state once, preserving existing behavior and the golden-master transcript.

The `CornerStore.csproj` change grants `CornerStore.Tests` internal visibility so focused tests can exercise the internal cart and memento without making application types public.

## Files added or modified

* `patterns/behavioral/memento/src/CornerStore/Cart/Cart.cs`
* `patterns/behavioral/memento/src/CornerStore/Cart/CartMemento.cs`
* `patterns/behavioral/memento/src/CornerStore/CornerStore.csproj`
* `patterns/behavioral/memento/src/CornerStore/StoreConsole.cs`
* `patterns/behavioral/memento/tests/CornerStore.Tests/MementoTests.cs`
* `patterns/behavioral/memento/README.md`
* `patterns/behavioral/memento/DIFF.md`
* `docs/pattern-map.md`
* `prompts/patterns/behavioral-memento.md`

The root README already contained the Memento pattern index link and was left unchanged. `DIFF.md` contains exactly the five source and test paths listed under Files changed in the pattern README.

## Validation

| Check | Result |
| --- | --- |
| Unchanged-copy suite | 41 passed, 0 failed |
| Final Release solution tests | 43 passed, 0 failed |
| Release build with warnings as errors | Succeeded, 0 warnings and 0 errors |
| Golden master | Passed in the full suite; `GoldenMasterTests.cs` SHA-256 matches baseline |
| Focused Memento tests | 2 tests passed, covering value restoration and independent mutable line state |
| S18 and other smell markers | S18 removed; all other marker IDs and counts match baseline |
| Root README link | Present and verified |
| `DIFF.md` generation | Two successive runs produced identical SHA-256 `F8AE007FAD7AFEC3426301B6DC6E5484F1A7FEE9555B2D52CC58D55E901E8DD2` |
| README and diff path list | 5 matching paths |
| Whitespace | `git diff --check` clean |

## Release summary

Unchanged-copy commit: `b454ddc`. Refactor commit: `a0a70b1`. Local tag: `memento-v1` at `a0a70b1`. No push was made.
