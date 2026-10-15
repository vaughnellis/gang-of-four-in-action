using System.Collections;

namespace CornerStore;

internal sealed class DepthFirstCategoryIterator : ICategoryIterator
{
    private readonly Stack<(Category Category, int Depth)> pending = [];
    private Category? current;

    public DepthFirstCategoryIterator(Category root, int depth = 0)
    {
        pending.Push((root, depth));
    }

    public Category Current => current ?? throw new InvalidOperationException("The iterator is not positioned on a category.");

    object IEnumerator.Current => Current;

    public int CurrentDepth { get; private set; }

    public bool MoveNext()
    {
        if (!pending.TryPop(out var next))
        {
            current = null;
            return false;
        }

        current = next.Category;
        CurrentDepth = next.Depth;
        for (var i = next.Category.Children.Count - 1; i >= 0; i--)
        {
            pending.Push((next.Category.Children[i], next.Depth + 1));
        }

        return true;
    }

    public void Reset()
    {
        throw new NotSupportedException();
    }

    public void Dispose()
    {
    }
}
