<!-- markdownlint-disable-file -->
# Plan critique: bridge

The proposed abstraction separates message content from delivery channel while keeping channel selection tied to the customer’s existing preference. Preserve the `email` versus fallback-to-SMS rule and the exact business text. Test both message types with both channel implementations, and leave notification subscribers and order lifecycle behavior outside S07 unchanged.
