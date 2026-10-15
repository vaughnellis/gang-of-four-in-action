<!-- markdownlint-disable-file -->
# Critique: proxy

## Assessment

The cache must not return stale stock after a reservation or adjustment. The
protection proxy must gate back-office stock methods without blocking system
operations used by checkout and order lifecycle.

## Risks and mitigations

* Invalidate or refresh cached values after every stock mutation.
* Keep staff role checks at the protection proxy, not BackOffice.
* Preserve BackOffice's duplicate lookup so the cache hit is exercised.
* Preserve existing checkout reservation and rollback behavior.
* Keep S14 inline action code and markers unchanged.

## Outcome

Compose protection outside caching around `InventoryService`, update all
service dependencies to `IInventoryService`, and test cache and access
behavior directly.
