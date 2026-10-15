<!-- markdownlint-disable-file -->
# Planning Log: strategy

Plan: `.copilot-tracking/plans/2026-10-15/strategy-plan.instructions.md`
Artifact completion: 100%

## Sequencing note

The run-all brief lists Strategy before applying both pilots' notes, but the Strategy pilot declares Facade's prompt edits as a dependency. The Facade notes were therefore applied before starting this pilot, and the final post-pilot template pass will integrate Strategy's own notes. This is a narrow sequencing deviation to satisfy both briefs as far as possible.

## Decisions

* Preserve AU/NZ surcharge calculations outside shipping-method strategies.
* Keep the default method implementations individually testable and accept an injected test-only implementation without changing `PricingService`.
* Leave the root README alone if its Strategy link already exists.
* Do not modify baseline tooling for the known diff-generator scalar issue; use the documented non-invasive workaround if needed.