namespace CornerStore;

internal interface IPaymentProcessor
{
    void Charge(Customer customer, decimal amount);
}