# Task: composite

**Task slug:** `composite`
**Shared context:** `prompts/00-project-brief.md` (sections 6 to 10)
**Pattern:** Composite (Structural)
**Smell:** S08
**Folder:** `patterns/structural/composite/`
**Depends on:** `baseline-v1` tag exists.

## Goal

Refactor the separate product and bundle pricing/cart paths through a shared
`ICatalogItem` contract while preserving the approved transcript.

## The smell

Bundles are priced through a separate path selected with `is Bundle` checks,
and the catalog rejects nested bundles. The refactor target is an
`ICatalogItem` shared by `Product` and `Bundle`.

## Requirements

- The unchanged copy was generated and committed as `e840c73`; its Release test
  suite passed before implementation.
- Implement `ICatalogItem` on `Product` and `Bundle`. Use the shared contract
  for catalog pricing and cart addition.
- Preserve simple-product and one-level bundle prices and cart lines.
- Preserve the existing nested-bundle rejection. The approved transcript is
  immutable, so the refactor must not enable nesting.
- Add focused tests for shared product/bundle behavior and the retained
  nesting rule.
- Complete the README, regenerate `DIFF.md`, and verify only declared source
  and test files differ from baseline.
- Set S08 to Done in `docs/pattern-map.md`. The root README link already exists.
- Run Release tests and review the diff, then commit the refactor and create
  the local `composite-v1` tag.

## Acceptance criteria

- The golden-master transcript matches byte for byte.
- Pattern-specific tests and Release build pass with warnings as errors.
- Only S08 is changed; other smell markers remain intact.
- README and `DIFF.md` agree on changed files.
- Document the justified divergence: nested bundles remain prohibited to
  preserve approved behavior.

## Boundaries

- Do not edit `baseline/`, `scenarios/`, another pattern folder, or the
  approved transcript.
- Do not rename projects or namespaces.
- Do not push.
