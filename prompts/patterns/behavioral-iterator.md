# Task: iterator

**Task slug:** `iterator`
**Shared context:** `prompts/00-project-brief.md` (sections 6 to 10)
**Pattern:** Iterator (Behavioral)
**Smell:** S16
**Folder:** `patterns/behavioral/iterator/`
**Depends on:** `baseline-v1` tag exists.

## Goal

Replace the duplicated recursive category-tree walks with explicit
depth-first and breadth-first iterators while preserving traversal behavior.

## The smell

Callers manually recurse over the category tree in several places.

## Requirements

- The unchanged copy was generated and committed as `102e099`; its Release
  test suite passed before implementation.
- Add depth-first and breadth-first category iterators.
- Use depth-first traversal for current catalog operations and preserve
  existing pre-order category/product output and SKU order.
- Add focused tests for both traversal orders and depth.
- Explain `IEnumerable<T>` with `yield` as the built-in alternative in the
  README.
- Complete the README, regenerate `DIFF.md`, and verify all changed files are
  declared.
- Set S16 to Done in `docs/pattern-map.md`. The root README link already exists.
- Run Release tests, review, commit the refactor, and create the local
  `iterator-v1` tag.

## Acceptance criteria

- The golden-master transcript matches byte for byte.
- Pattern-specific tests and Release build pass with warnings as errors.
- Only S16 is changed; all other smell markers remain intact.
- README and `DIFF.md` agree on changed files.

## Boundaries

- Do not edit `baseline/`, `scenarios/`, another pattern folder, or the
  approved transcript.
- Do not rename projects or namespaces.
- Do not push.
