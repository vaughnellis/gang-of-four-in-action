# Copilot instructions: Gang of Four in Action

This repository showcases the 23 Gang of Four design patterns in C# on .NET 10. It contains one deliberately messy baseline app (CornerStore) and one solution per pattern, each a copy of the baseline with exactly one smell refactored. The full plan is in `prompts/00-project-brief.md`; read it before any non-trivial change.

## Rules that apply everywhere

- **The mess is intentional.** Code marked `// SMELL Sxx:` is a planted smell that a pattern solution will fix. Never refactor, rename, simplify or "improve" it unless the task is that pattern's refactor. Do not suggest fixes for it in reviews.
- **One pattern, one seam.** A pattern solution changes only the code for its own smell. Leave every other `SMELL` marker and its code untouched.
- **Names never change between copies.** Projects and namespaces are `CornerStore` and `CornerStore.Tests` in every folder. Only the `.slnx` file name differs.
- **Behaviour is fixed by the golden master.** Never edit `scenarios/trading-day.json` or `scenarios/expected/baseline.txt` from a pattern task. Never edit `GoldenMasterTests.cs` in a pattern folder.
- **Determinism.** No `DateTime.Now`, `DateTime.UtcNow`, `Guid.NewGuid()`, unseeded `Random` or `Thread.Sleep` in app code. Use the injected `TimeProvider`, sequential IDs, the seeded `Random` and invariant culture.
- **Transcript content.** The transcript records business outcomes only, never timings, memory, call counts or hash codes.
- **Hands off the process files.** Do not edit `prompts/`, this file or `.github/instructions/`. Propose changes in the RPI change record instead.
- **No pushing.** Commits, tags and releases are made by the maintainer, except during the automatic run in `prompts/05-run-all.md`, which allows local commits and local tags. Pushing is always the maintainer's job.

## C# conventions

- File-scoped namespaces, one public type per file, nullable reference types respected (no `!` to silence warnings).
- Pattern code reads like a teaching example: clear names that match the GoF participants where it helps (`CheckoutFacade`, `IShippingCostStrategy`), small classes, no clever tricks.
- Tests use xUnit. Test names describe behaviour: `PlaceOrder_WhenValidationFails_TakesNoPayment`.

## Writing conventions

- READMEs and docs: plain, direct prose. No em dashes.
- Never reproduce text or diagrams from the GoF book. Paraphrase intents and cite the chapter.
- Diagrams in Mermaid.
