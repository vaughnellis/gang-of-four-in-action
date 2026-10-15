<!-- markdownlint-disable-file -->
# Plan critique: abstract-factory

The planned seam matches the S01 pattern map and limits behavior changes to regional policy selection. Tests must assert the complete AU/NZ product families and the unknown-region defaults. Keep `TaxCalculator.LineTax` and its S23 marker unchanged. Reuse the existing static call surfaces as thin delegates if that keeps unrelated callers untouched, and list all changed source and test files in the pattern README.
