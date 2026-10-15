# DIFF: prototype

## File summary

| Status | File | Added | Removed |
| --- | --- | ---: | ---: |
| Modified | CornerStore/Catalog/Catalog.cs | 1 | 16 |
| Modified | CornerStore/Catalog/PhysicalProduct.cs | 18 | 0 |
| Modified | CornerStore/CornerStore.csproj | 4 | 0 |
| Added | CornerStore.Tests/PrototypeTests.cs | 69 | 0 |

## Full diff

```diff
diff --git a/baseline/src/CornerStore/Catalog/Catalog.cs b/pattern/src/CornerStore/Catalog/Catalog.cs
index 2131b22..99db0dd 100644
--- a/baseline/src/CornerStore/Catalog/Catalog.cs
+++ b/pattern/src/CornerStore/Catalog/Catalog.cs
@@ -138,22 +138,7 @@ internal class Catalog
             return "sku " + sku + " already exists";
         }
-        // SMELL S04: the variant is built by copying the parent's fields one by one
-        var variant = new PhysicalProduct
-        {
-            Sku = sku,
-            Name = parent.Name + " (" + size + ", " + colour + ")",
-            Price = parent.Price,
-            CategoryName = parent.CategoryName,
-            Brand = parent.Brand,
-            Manufacturer = parent.Manufacturer,
-            WeightKg = parent.WeightKg,
-            Size = size,
-            Colour = colour,
-            Material = parent.Material,
-            Description = parent.Description,
-        };
-
+        var variant = parent.Clone(sku, size, colour);
         Register(categories[parent.CategoryName], variant);
         return null;
     }
diff --git a/baseline/src/CornerStore/Catalog/PhysicalProduct.cs b/pattern/src/CornerStore/Catalog/PhysicalProduct.cs
index 8cc62f4..014c182 100644
--- a/baseline/src/CornerStore/Catalog/PhysicalProduct.cs
+++ b/pattern/src/CornerStore/Catalog/PhysicalProduct.cs
@@ -15,4 +15,22 @@ internal class PhysicalProduct : Product
     public string Material { get; init; } = "";
     public string Description { get; init; } = "";
+
+    public PhysicalProduct Clone(string sku, string size, string colour)
+    {
+        return new PhysicalProduct
+        {
+            Sku = sku,
+            Name = Name + " (" + size + ", " + colour + ")",
+            Price = Price,
+            CategoryName = CategoryName,
+            Brand = Brand,
+            Manufacturer = Manufacturer,
+            WeightKg = WeightKg,
+            Size = size,
+            Colour = colour,
+            Material = Material,
+            Description = Description,
+        };
+    }
 }
diff --git a/baseline/src/CornerStore/CornerStore.csproj b/pattern/src/CornerStore/CornerStore.csproj
index a15a29b..f8dc6c1 100644
--- a/baseline/src/CornerStore/CornerStore.csproj
+++ b/pattern/src/CornerStore/CornerStore.csproj
@@ -5,4 +5,8 @@
     <TargetFramework>net10.0</TargetFramework>
   </PropertyGroup>
+  <ItemGroup>
+    <InternalsVisibleTo Include="CornerStore.Tests" />
+  </ItemGroup>
+
 </Project>
diff --git a/pattern/tests/CornerStore.Tests/PrototypeTests.cs b/pattern/tests/CornerStore.Tests/PrototypeTests.cs
new file mode 100644
index 0000000..8fc7be8
--- /dev/null
+++ b/pattern/tests/CornerStore.Tests/PrototypeTests.cs
@@ -0,0 +1,69 @@
+namespace CornerStore.Tests;
+
+public class PrototypeTests
+{
+    [Fact]
+    public void CreateVariant_ClonesProductAndAppliesRequestedVariantDetails()
+    {
+        var catalog = new Catalog();
+        var parent = Assert.IsType<PhysicalProduct>(catalog.Lookup("SHOE-RUN-42"));
+
+        var error = catalog.CreateVariant("SHOE-RUN-42", "SHOE-RUN-44", "44", "Blue");
+
+        var variant = Assert.IsType<PhysicalProduct>(catalog.Lookup("SHOE-RUN-44"));
+
+        Assert.Null(error);
+        Assert.NotSame(parent, variant);
+        Assert.Equal("SHOE-RUN-44", variant.Sku);
+        Assert.Equal("Runner Pro (44, Blue)", variant.Name);
+        Assert.Equal("44", variant.Size);
+        Assert.Equal("Blue", variant.Colour);
+    }
+
+    [Fact]
+    public void CreateVariant_PreservesPrototypeAttributesAndLeavesItUnchanged()
+    {
+        var catalog = new Catalog();
+        var parent = Assert.IsType<PhysicalProduct>(catalog.Lookup("SHOE-RUN-42"));
+        var original = new
+        {
+            parent.Sku,
+            parent.Name,
+            parent.Price,
+            parent.CategoryName,
+            parent.Brand,
+            parent.Manufacturer,
+            parent.WeightKg,
+            parent.Size,
+            parent.Colour,
+            parent.Material,
+            parent.Description,
+        };
+
+        Assert.Null(catalog.CreateVariant("SHOE-RUN-42", "SHOE-RUN-44", "44", "Blue"));
+
+        var variant = Assert.IsType<PhysicalProduct>(catalog.Lookup("SHOE-RUN-44"));
+
+        Assert.Equal(original.Price, variant.Price);
+        Assert.Equal(original.CategoryName, variant.CategoryName);
+        Assert.Equal(original.Brand, variant.Brand);
+        Assert.Equal(original.Manufacturer, variant.Manufacturer);
+        Assert.Equal(original.WeightKg, variant.WeightKg);
+        Assert.Equal(original.Material, variant.Material);
+        Assert.Equal(original.Description, variant.Description);
+        Assert.Equal(original, new
+        {
+            parent.Sku,
+            parent.Name,
+            parent.Price,
+            parent.CategoryName,
+            parent.Brand,
+            parent.Manufacturer,
+            parent.WeightKg,
+            parent.Size,
+            parent.Colour,
+            parent.Material,
+            parent.Description,
+        });
+    }
+}
```
