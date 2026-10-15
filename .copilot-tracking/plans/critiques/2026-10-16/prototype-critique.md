<!-- markdownlint-disable-file -->
# Prototype plan critique

The chosen seam is narrow: `Catalog.CreateVariant` delegates field preservation and variant overrides to `PhysicalProduct.Clone`. Keep existing SKU, parent-type and duplicate checks in the catalog, and preserve registration in the parent's category. Tests should verify the clone is a separate object, all unchanged attributes match the prototype, and SKU, name, size and colour reflect the requested variant. Avoid changing equality or mutability semantics of the product hierarchy.
