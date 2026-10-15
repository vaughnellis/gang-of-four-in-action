<!-- markdownlint-disable-file -->
# Planning Log: adapter

Plan: `.copilot-tracking/plans/2026-10-15/adapter-plan.instructions.md`
Artifact completion: 100%

## Decisions

* Keep `ShipFastClient` unchanged as the adaptee.
* `ShipFastAdapter` owns XML escaping/serialization, currency conversion to cents, client invocation, and response parsing.
* Checkout depends on `IShippingProvider`, not the SDK-specific client.
* Do not edit frozen baseline tooling; use the documented local diff-generator workaround.