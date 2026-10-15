namespace CornerStore.Tests;

public class IteratorTests
{
    [Fact]
    public void DepthFirstIterator_WhenTraversingTree_VisitsParentsBeforeChildren()
    {
        var (root, _) = CreateTree();

        var visits = Read(new DepthFirstCategoryIterator(root));

        Assert.Equal(
            [
                ("Root", 0),
                ("Left", 1),
                ("Leaf", 2),
                ("Right", 1),
            ],
            visits);
    }

    [Fact]
    public void BreadthFirstIterator_WhenTraversingTree_VisitsEachLevelInOrder()
    {
        var (root, _) = CreateTree();

        var visits = Read(new BreadthFirstCategoryIterator(root));

        Assert.Equal(
            [
                ("Root", 0),
                ("Left", 1),
                ("Right", 1),
                ("Leaf", 2),
            ],
            visits);
    }

    [Fact]
    public void DepthFirstIterator_WhenStartingBelowRoot_PreservesInitialDepth()
    {
        var (_, left) = CreateTree();

        var visits = Read(new DepthFirstCategoryIterator(left, 3));

        Assert.Equal([("Left", 3), ("Leaf", 4)], visits);
    }

    private static List<(string Name, int Depth)> Read(ICategoryIterator iterator)
    {
        using (iterator)
        {
            var visits = new List<(string Name, int Depth)>();
            while (iterator.MoveNext())
            {
                visits.Add((iterator.Current.Name, iterator.CurrentDepth));
            }

            return visits;
        }
    }

    private static (Category Root, Category Left) CreateTree()
    {
        var root = new Category("Root");
        var left = new Category("Left");
        var right = new Category("Right");
        var leaf = new Category("Leaf");
        root.Children.Add(left);
        root.Children.Add(right);
        left.Children.Add(leaf);

        return (root, left);
    }
}
