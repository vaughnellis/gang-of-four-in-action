namespace CornerStore;

internal interface IShippingProvider
{
    string BookShipment(Order order);
}