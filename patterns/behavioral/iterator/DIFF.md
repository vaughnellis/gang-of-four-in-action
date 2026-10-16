# DIFF: iterator

## File summary

| Status | File | Added | Removed |
| --- | --- | ---: | ---: |
| Added | CornerStore/Catalog/BreadthFirstCategoryIterator.cs | 47 | 0 |
| Modified | CornerStore/Catalog/Catalog.cs | 19 | 22 |
| Added | CornerStore/Catalog/DepthFirstCategoryIterator.cs | 47 | 0 |
| Added | CornerStore/Catalog/ICategoryIterator.cs | 8 | 0 |
| Modified | CornerStore/CornerStore.csproj | 4 | 0 |
| Added | CornerStore.Tests/IteratorTests.cs | 75 | 0 |

## Full diff

```diff
diff --git a/pattern/src/CornerStore/Catalog/BreadthFirstCategoryIterator.cs b/pattern/src/CornerStore/Catalog/BreadthFirstCategoryIterator.cs
new file mode 100644
index 0000000..12f036e
--- /dev/null
+++ b/pattern/src/CornerStore/Catalog/BreadthFirstCategoryIterator.cs
@@ -0,0 +1,47 @@
+using System.Collections;
+
+namespace CornerStore;
+
+internal sealed class BreadthFirstCategoryIterator : ICategoryIterator
+{
+    private readonly Queue<(Category Category, int Depth)> pending = [];
+    private Category? current;
+
+    public BreadthFirstCategoryIterator(Category root, int depth = 0)
+    {
+        pending.Enqueue((root, depth));
+    }
+
+    public Category Current => current ?? throw new InvalidOperationException("The iterator is not positioned on a category.");
+
+    object IEnumerator.Current => Current;
+
+    public int CurrentDepth { get; private set; }
+
+    public bool MoveNext()
+    {
+        if (!pending.TryDequeue(out var next))
+        {
+            current = null;
+            return false;
+        }
+
+        current = next.Category;
+        CurrentDepth = next.Depth;
+        foreach (var child in next.Category.Children)
+        {
+            pending.Enqueue((child, next.Depth + 1));
+        }
+
+        return true;
+    }
+
+    public void Reset()
+    {
+        throw new NotSupportedException();
+    }
+
+    public void Dispose()
+    {
+    }
+}
diff --git a/baseline/src/CornerStore/Catalog/Catalog.cs b/pattern/src/CornerStore/Catalog/Catalog.cs
index 2131b22..78e6901 100644
--- a/baseline/src/CornerStore/Catalog/Catalog.cs
+++ b/pattern/src/CornerStore/Catalog/Catalog.cs
@@ -160,27 +160,26 @@ internal class Catalog
     public void PrintTree(Category category, int depth)
     {
-        // SMELL S16: manual recursion over the category tree, first copy
-        var pad = new string(' ', depth * 2);
-        Log.Info(pad + category.Name);
-        foreach (var product in category.Products)
+        using var iterator = new DepthFirstCategoryIterator(category, depth);
+        while (iterator.MoveNext())
         {
-            Log.Info(pad + "  - " + product.Sku + " " + product.Name + " " + FormatPrice(PriceOf(product.Sku)));
-        }
-
-        foreach (var child in category.Children)
-        {
-            PrintTree(child, depth + 1);
+            var current = iterator.Current;
+            var pad = new string(' ', iterator.CurrentDepth * 2);
+            Log.Info(pad + current.Name);
+            foreach (var product in current.Products)
+            {
+                Log.Info(pad + "  - " + product.Sku + " " + product.Name + " " + FormatPrice(PriceOf(product.Sku)));
+            }
         }
     }
     public int CountProducts(Category category)
     {
-        // SMELL S16: manual recursion over the category tree, second copy
-        var count = category.Products.Count;
-        foreach (var child in category.Children)
+        var count = 0;
+        using var iterator = new DepthFirstCategoryIterator(category);
+        while (iterator.MoveNext())
         {
-            count += CountProducts(child);
+            count += iterator.Current.Products.Count;
         }
         return count;
@@ -188,15 +187,13 @@ internal class Catalog
     public void CollectSkus(Category category, List<string> skus)
     {
-        // SMELL S16: manual recursion over the category tree, third copy
-        foreach (var product in category.Products)
+        using var iterator = new DepthFirstCategoryIterator(category);
+        while (iterator.MoveNext())
         {
-            skus.Add(product.Sku);
-        }
-
-        foreach (var child in category.Children)
-        {
-            CollectSkus(child, skus);
+            foreach (var product in iterator.Current.Products)
+            {
+                skus.Add(product.Sku);
+            }
         }
     }
diff --git a/pattern/src/CornerStore/Catalog/DepthFirstCategoryIterator.cs b/pattern/src/CornerStore/Catalog/DepthFirstCategoryIterator.cs
new file mode 100644
index 0000000..09eda86
--- /dev/null
+++ b/pattern/src/CornerStore/Catalog/DepthFirstCategoryIterator.cs
@@ -0,0 +1,47 @@
+using System.Collections;
+
+namespace CornerStore;
+
+internal sealed class DepthFirstCategoryIterator : ICategoryIterator
+{
+    private readonly Stack<(Category Category, int Depth)> pending = [];
+    private Category? current;
+
+    public DepthFirstCategoryIterator(Category root, int depth = 0)
+    {
+        pending.Push((root, depth));
+    }
+
+    public Category Current => current ?? throw new InvalidOperationException("The iterator is not positioned on a category.");
+
+    object IEnumerator.Current => Current;
+
+    public int CurrentDepth { get; private set; }
+
+    public bool MoveNext()
+    {
+        if (!pending.TryPop(out var next))
+        {
+            current = null;
+            return false;
+        }
+
+        current = next.Category;
+        CurrentDepth = next.Depth;
+        for (var i = next.Category.Children.Count - 1; i >= 0; i--)
+        {
+            pending.Push((next.Category.Children[i], next.Depth + 1));
+        }
+
+        return true;
+    }
+
+    public void Reset()
+    {
+        throw new NotSupportedException();
+    }
+
+    public void Dispose()
+    {
+    }
+}
diff --git a/pattern/src/CornerStore/Catalog/ICategoryIterator.cs b/pattern/src/CornerStore/Catalog/ICategoryIterator.cs
new file mode 100644
index 0000000..4bab1d9
--- /dev/null
+++ b/pattern/src/CornerStore/Catalog/ICategoryIterator.cs
@@ -0,0 +1,8 @@
+using System.Collections;
+
+namespace CornerStore;
+
+internal interface ICategoryIterator : IEnumerator<Category>
+{
+    int CurrentDepth { get; }
+}
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
diff --git a/pattern/tests/CornerStore.Tests/IteratorTests.cs b/pattern/tests/CornerStore.Tests/IteratorTests.cs
new file mode 100644
index 0000000..1c97164
--- /dev/null
+++ b/pattern/tests/CornerStore.Tests/IteratorTests.cs
@@ -0,0 +1,75 @@
+namespace CornerStore.Tests;
+
+public class IteratorTests
+{
+    [Fact]
+    public void DepthFirstIterator_WhenTraversingTree_VisitsParentsBeforeChildren()
+    {
+        var (root, _) = CreateTree();
+
+        var visits = Read(new DepthFirstCategoryIterator(root));
+
+        Assert.Equal(
+            [
+                ("Root", 0),
+                ("Left", 1),
+                ("Leaf", 2),
+                ("Right", 1),
+            ],
+            visits);
+    }
+
+    [Fact]
+    public void BreadthFirstIterator_WhenTraversingTree_VisitsEachLevelInOrder()
+    {
+        var (root, _) = CreateTree();
+
+        var visits = Read(new BreadthFirstCategoryIterator(root));
+
+        Assert.Equal(
+            [
+                ("Root", 0),
+                ("Left", 1),
+                ("Right", 1),
+                ("Leaf", 2),
+            ],
+            visits);
+    }
+
+    [Fact]
+    public void DepthFirstIterator_WhenStartingBelowRoot_PreservesInitialDepth()
+    {
+        var (_, left) = CreateTree();
+
+        var visits = Read(new DepthFirstCategoryIterator(left, 3));
+
+        Assert.Equal([("Left", 3), ("Leaf", 4)], visits);
+    }
+
+    private static List<(string Name, int Depth)> Read(ICategoryIterator iterator)
+    {
+        using (iterator)
+        {
+            var visits = new List<(string Name, int Depth)>();
+            while (iterator.MoveNext())
+            {
+                visits.Add((iterator.Current.Name, iterator.CurrentDepth));
+            }
+
+            return visits;
+        }
+    }
+
+    private static (Category Root, Category Left) CreateTree()
+    {
+        var root = new Category("Root");
+        var left = new Category("Left");
+        var right = new Category("Right");
+        var leaf = new Category("Leaf");
+        root.Children.Add(left);
+        root.Children.Add(right);
+        left.Children.Add(leaf);
+
+        return (root, left);
+    }
+}
```
