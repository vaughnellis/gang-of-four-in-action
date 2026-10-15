<!-- markdownlint-disable-file -->
# Critique: command

## Assessment

Command objects should own the back-office action results while an invoker
records execution and maintains undo history. The current scenario transcript
is fixed, so audit entries must remain internal and must not add output.

## Risks and mitigations

* Preserve the S12 staff checks and the intentional duplicate `GetStock` call.
* Reverse stock adjustments only; refund and cancellation trigger additional
  status and service effects that cannot be safely compensated here.
* Make unsupported undo explicit and test it rather than presenting a silent
  no-op as a successful reversal.
* Keep audit ordering deterministic and omit timestamps or runtime metrics.

## Outcome

Proceed with a small invoker, one command per existing action, and tests for
successful execution, stock undo, unsupported undo, and audit records.
