<!-- markdownlint-disable-file -->
# Critique: strategy

Plan: `.copilot-tracking/plans/2026-10-15/strategy-plan.instructions.md`
Artifact completion: 100%

## Assessment

The plan isolates S21 in pricing and retains region surcharges and all other pricing rules. The test-only strategy requirement rules out a closed switch or hard-coded type dispatch in the pricing service; favor an injected strategy registry or resolver.

## Risks and mitigations

* Avoid moving region-specific surcharge logic into each method strategy; keep it where it is so AU/NZ behavior remains unchanged.
* Ensure each default method maps to exactly one strategy and unknown methods retain the baseline rejection behavior.
* Make test extensibility prove that pricing consumes the injected strategy rather than only testing implementations in isolation.
* Preserve S01 and S09 markers and behavior.

## Outcome

The plan is sufficient. The exact registry/resolver shape will follow the nearest local constructor and call-site conventions.