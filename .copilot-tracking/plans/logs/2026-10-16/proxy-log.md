<!-- markdownlint-disable-file -->
# Planning Log: proxy

## Decisions

* Put the protection proxy outside the cache so every back-office request is
  authorized before reaching cached operations.
* The cache updates after adjustments and invalidates after successful
  reservations.
* Keep system stock reads, reservations, and order rollback behind the same
  interface without requiring a customer actor.
* The protection proxy gates only the staff-oriented methods used by BackOffice.
