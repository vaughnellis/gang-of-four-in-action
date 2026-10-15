<!-- markdownlint-disable-file -->
# Review: interpreter

Plan: `.copilot-tracking/plans/2026-10-16/interpreter-plan.instructions.md`
Changes: `.copilot-tracking/changes/2026-10-16/interpreter-changes.md`
Reviewer: RPI Agent (Phase 4)
Date: 2026-10-16
Outcome: Conformant with justified divergence

## Executive outcome

The S15 path now parses the configured promo strings once into a small expression tree and evaluates that tree against cart, customer and subtotal context. Supported syntax remains limited to the four existing predicates joined by `AND`. Promo display text, strict threshold behavior, cart quantity aggregation, any-line category matching and region equality are preserved.

## Findings

No unresolved findings remain in the Interpreter scope. The root README already linked the Interpreter folder, so no index edit was required. The umbrella solution's duplicate behavioral project names are a pre-existing issue recorded during earlier run-all work; the Interpreter solution itself builds successfully with zero warnings and errors and passes all tests. This is the only justified divergence.

## Request fulfillment

| Request | Result | Evidence |
| --- | --- | --- |
| Create and verify unchanged copy | Complete | 41/41 Release tests passed; copy commit `0cd28fb` |
| Replace repeated rule parsing with Interpreter | Complete | Rules parsed once into expressions; evaluation no longer uses `Split` or `IndexOf` |
| Preserve existing behavior and transcript | Complete | 54/54 tests, including golden master |
| Add focused tests | Complete | Tests cover conjunction, strict thresholds, culture-invariant decimals, quantity aggregation, category, region and malformed rules |
| Complete README and generated diff | Complete | Mermaid grammar diagram, trade-offs, and six listed paths matching the generated diff |
| Update map, commit, tag and progress | Complete | S15 marked Done; refactor commit `71d1cea`; tag `interpreter-v1`; progress record updated |

## Validation

* Unchanged-copy suite: 41/41 passed.
* Final Interpreter suite: 54/54 passed.
* Interpreter Release solution build: 0 warnings, 0 errors.
* `GoldenMasterTests.cs` SHA-256 matches baseline.
* S15 removed; all 61 other smell markers match baseline.
* README Files changed and `DIFF.md` summary contain the same six paths.
* Two consecutive diff-generator runs produced the same file hash.
* `git diff --check` passed.

## Release record

The unchanged copy is `0cd28fb`; the refactor is `71d1cea`, tagged `interpreter-v1`. Progress and tracking records are committed separately. No push was made.
