<!-- markdownlint-disable-file -->
# Interpreter plan critique

Keep the grammar limited to the four operators and conjunctions used by the configured promo rules. Parse definitions once, preserve their original strings for display, and fail explicitly on malformed syntax instead of treating unknown terms as true. Ensure numeric parsing is culture-invariant, category matching checks all lines, quantity sums every line, and `AND` short-circuits. Avoid changing pricing, promo definitions, or transcript behavior outside S15.
