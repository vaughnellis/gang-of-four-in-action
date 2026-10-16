namespace CornerStore;

internal interface IOrderStatusObserver
{
    void OnStatusChanged(Order order);
}
