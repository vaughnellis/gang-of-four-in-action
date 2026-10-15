namespace CornerStore;

internal interface ILineItemVisitor
{
    void Visit(PhysicalProduct product);

    void Visit(DigitalProduct product);

    void Visit(GiftCardProduct product);
}
