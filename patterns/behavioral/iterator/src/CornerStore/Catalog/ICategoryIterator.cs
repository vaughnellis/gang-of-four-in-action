using System.Collections;

namespace CornerStore;

internal interface ICategoryIterator : IEnumerator<Category>
{
    int CurrentDepth { get; }
}
