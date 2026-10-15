<!-- markdownlint-disable-file -->
# Changes: proxy

## Summary

Added `IInventoryService` with a caching proxy and an outer protection proxy.
Separate `IBackOfficeInventory` and `IStockRestorer` capability contracts
ensure that `BackOffice` receives only protected staff operations, while
trusted order workflows can restore stock. The cache preserves lookup results
until reservations invalidate them and staff adjustments update them. Stock
lookup and adjustment authorization now occurs at the protection proxy.

`BackOffice` continues to check staff authorization for refund and
cancellation because those actions are not inventory-service operations.
Their S12 marker comments were removed with the inventory smell, but the
behavior and S14 inline action code remain unchanged.

## Files changed

* `patterns/structural/proxy/src/CornerStore/Checkout/OrderValidator.cs`
* `patterns/structural/proxy/src/CornerStore/CornerStore.csproj`
* `patterns/structural/proxy/src/CornerStore/Inventory/CachingInventoryProxy.cs`
* `patterns/structural/proxy/src/CornerStore/Inventory/IInventoryService.cs`
* `patterns/structural/proxy/src/CornerStore/Inventory/InventoryService.cs`
* `patterns/structural/proxy/src/CornerStore/Inventory/ProtectionInventoryProxy.cs`
* `patterns/structural/proxy/src/CornerStore/Orders/BackOffice.cs`
* `patterns/structural/proxy/src/CornerStore/Orders/OrderService.cs`
* `patterns/structural/proxy/src/CornerStore/StoreConsole.cs`
* `patterns/structural/proxy/tests/CornerStore.Tests/ProxyTests.cs`
* `patterns/structural/proxy/README.md`
* `patterns/structural/proxy/DIFF.md`
* `docs/pattern-map.md`
* Root README link verified; no change was needed.

## Validation

* Unchanged copy commit `33063ce`: 41 tests passed.
* Final Proxy Release suite: 46 tests passed, including the golden master.
* Focused tests verify cache hits, invalidation after reservation, cache
  refresh after stock adjustment, and staff-only access.
* `DIFF.md` regenerated twice with matching SHA-256
  `DE0BA0C64EE0F579C9B87009E82DF3581F8FD58132B23D9F4C6C73606BEC433E`.
* README file list matches the generated source/test diff.

## Commits and tag

* Copy: `33063ce`.
* Refactor: `7eb18eb`.
* Tag: `proxy-v1`.
