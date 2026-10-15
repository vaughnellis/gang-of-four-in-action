<!-- markdownlint-disable-file -->
# Plan critique: visitor

Keep the visitor seam limited to S23 dispatch. All three product types should dispatch to each operation visitor, and tax rounding, quantity-based weight, and both report formats must preserve their current results. Do not fold the exporters' S22 workflow into a shared abstraction.
