namespace CornerStore;

internal class DigitalProduct : Product
{
    public int FileSizeMb { get; init; }

    internal override void Accept(ILineItemVisitor visitor)
    {
        visitor.Visit(this);
    }
}
