namespace CornerStore;

internal class Category(string name)
{
    public string Name { get; } = name;

    public List<Category> Children { get; } = [];

    public List<Product> Products { get; } = [];
}
