<!-- markdownlint-disable-file -->
# Changes: template-method

Plan: `.copilot-tracking/plans/2026-10-15/template-method-plan.instructions.md`
Details: `.copilot-tracking/details/2026-10-15/template-method-details.md`
Planning log: `.copilot-tracking/plans/logs/2026-10-15/template-method-log.md`
Critique: `.copilot-tracking/plans/critiques/2026-10-15/template-method-critique.md`
Review: `.copilot-tracking/reviews/2026-10-15/template-method-review.md`
Artifact completion: 100%
Implementation date: 2026-10-15

## Summary

Extracted the shared order filtering, traversal, and report algorithm into `ReportExporter`. CSV and JSON subclasses retain their own line-item switches and serialization hooks. The Release build is warning-free and all 45 tests pass.

## Files added or modified

* `patterns/behavioral/template-method/src/CornerStore/Reporting/ReportExporter.cs`
* `patterns/behavioral/template-method/src/CornerStore/Reporting/ReportTotals.cs`
* `patterns/behavioral/template-method/src/CornerStore/Reporting/CsvReportExporter.cs`
* `patterns/behavioral/template-method/src/CornerStore/Reporting/JsonReportExporter.cs`
* `patterns/behavioral/template-method/src/CornerStore/CornerStore.csproj`
* `patterns/behavioral/template-method/tests/CornerStore.Tests/TemplateMethodTests.cs`
* `patterns/behavioral/template-method/README.md`
* `patterns/behavioral/template-method/DIFF.md`
* `docs/pattern-map.md`
* `prompts/patterns/behavioral-template-method.md`

The root README already linked Template Method and was left unchanged. `DIFF.md` contains exactly the six source/test paths listed in the pattern README.

## Validation

| Check | Result |
| --- | --- |
| Unchanged copy tests | 41 passed, 0 failed |
| Template Method-specific tests | 4 passed, 0 failed |
| Full solution tests | 45 passed, 0 failed |
| Release build with `-warnaserror` | Succeeded, 0 warnings and 0 errors |
| GoldenMasterTests copy | Hash matches baseline |
| S22 marker | Absent |
| Both exporter S23 markers | Preserved |
| Editor diagnostics | No errors found |
| Root README link | Present and verified |
| `DIFF.md` summary | Exactly the six README source/test paths |
| Diff whitespace and `git diff --check` | Clean |

## Release summary

Unchanged-copy commit: `c812e07`. The refactor commit and local `template-method-v1` tag are recorded in run-all progress after review.