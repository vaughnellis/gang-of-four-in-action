namespace CornerStore;

internal class GiftCardProduct : Product
{
    internal override void Accept(ILineItemVisitor visitor)
    {
        visitor.Visit(this);
    }
}
