<!-- markdownlint-disable-file -->
# Details: proxy

## P01: Copy

* Generate the Structural Proxy solution from the approved baseline.
* Run unchanged Release tests and commit before modifying the copy.

## P02: Refactor and test

* Define `IInventoryService` for application services.
* Add a caching proxy and an outer protection proxy.
* Move back-office staff checks into the protection proxy.
* Route checkout and order inventory calls through the same interface.
* Test cache hits, mutation coherence, and rejected non-staff access.

## P03: Document and review

* Complete the pattern README and regenerate `DIFF.md`.
* Update the map, run Release validation, review, commit, tag, and record
  completion in run-all progress.
