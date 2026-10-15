<!-- markdownlint-disable-file -->
# Critique: facade

Plan: `.copilot-tracking/plans/2026-10-15/facade-plan.instructions.md`
Artifact completion: 100%

## Assessment

The plan follows the pilot's explicit phases and isolates S10 to the console's checkout orchestration. The untouched-copy test is a useful discriminator before code changes, and direct tests cover the facade outcomes separately from the shared golden master.

## Risks and mitigations

* Payment is charged before stock reservation in the existing behavior. Preserve this ordering and preserve its existing compensation behavior rather than changing unrelated business semantics.
* Payment processor selection and courier XML handling are nearby S03 and S06 smells. Keep both markers and their implementation behavior intact when moving those helpers behind the new facade.
* The root README already links the Facade folder. Verify the link rather than making a redundant edit.
* The baseline diff script may have PowerShell-specific behavior. Do not edit frozen baseline tooling; record any workaround in the pilot notes.

## Outcome

No plan change was required. The diff script did fail in both installed PowerShell editions because of scalar output handling; a process-local compatibility shim generated the required artifact without changing the tool.