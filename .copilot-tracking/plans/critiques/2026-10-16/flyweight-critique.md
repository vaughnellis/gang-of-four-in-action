<!-- markdownlint-disable-file -->
# Flyweight plan critique

The design separates intrinsic product specification from extrinsic listing identity and price. Use immutable shared specifications and a factory keyed by the exact source dimensions, brand, material and origin. Tests must prove reference reuse, not only equal values. Keep the original algorithm in benchmark-only code so the comparison does not add a baseline project to the production solution. The benchmark should retain its returned listings and use MemoryDiagnoser so it measures allocations, not a proxy such as object count.
