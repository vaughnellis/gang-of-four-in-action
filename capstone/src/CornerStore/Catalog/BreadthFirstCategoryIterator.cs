using System.Collections;

namespace CornerStore;

internal sealed class BreadthFirstCategoryIterator : ICategoryIterator
{
    private readonly Queue<(Category Category, int Depth)> pending = [];
    private Category? current;

    public BreadthFirstCategoryIterator(Category root, int depth = 0)
    {
        pending.Enqueue((root, depth));
    }

    public Category Current => current ?? throw new InvalidOperationException("The iterator is not positioned on a category.");

    object IEnumerator.Current => Current;

    public int CurrentDepth { get; private set; }

    public bool MoveNext()
    {
        if (!pending.TryDequeue(out var next))
        {
            current = null;
            return false;
        }

        current = next.Category;
        CurrentDepth = next.Depth;
        foreach (var child in next.Category.Children)
        {
            pending.Enqueue((child, next.Depth + 1));
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
