# Task: proxy

**Task slug:** `proxy`
**Shared context:** `prompts/00-project-brief.md` (sections 6 to 10)
**Pattern:** Proxy (Structural)
**Smell:** S12
**Folder:** `patterns/structural/proxy/`
**Depends on:** `baseline-v1` tag exists.

## Goal

Put caching and staff-only authorization in proxies implementing
`IInventoryService`, preserving the stock behavior and transcript.

## The smell

Inventory lookups add simulated latency and are repeated without caching.
Staff-only stock adjustments and lookups are authorized by callers rather than
the inventory service boundary.

## Requirements

- The unchanged copy was generated and committed as `33063ce`; its Release
  test suite passed before implementation.
- Extract `IInventoryService`; add a caching proxy and a protection proxy.
- Cache stock lookups and keep cached values correct after reserve and
  adjustment operations.
- Move staff authorization for back-office stock lookup and adjustment into the
  protection proxy. Preserve all existing output.
- Wire all app services through the proxies; keep non-stock S14 actions intact.
- Add focused tests proving cache hits, cache coherence after mutations, and
  rejection of non-staff access.
- Complete the README, regenerate `DIFF.md`, and verify all changed files are
  declared.
- Set S12 to Done in `docs/pattern-map.md`. The root README link already exists.
- Run Release tests, review, commit the refactor, and create the local
  `proxy-v1` tag.

## Acceptance criteria

- The golden-master transcript matches byte for byte.
- Pattern-specific tests and Release build pass with warnings as errors.
- Only S12 is changed; all other smell markers remain intact.
- README and `DIFF.md` agree on changed files.

## Boundaries

- Do not edit `baseline/`, `scenarios/`, another pattern folder, or the
  approved transcript.
- Do not rename projects or namespaces.
- Do not push.
