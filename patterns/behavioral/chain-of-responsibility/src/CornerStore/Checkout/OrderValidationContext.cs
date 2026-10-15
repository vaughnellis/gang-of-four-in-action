namespace CornerStore;

internal sealed record OrderValidationContext(Customer Customer, Cart Cart, Quote Quote);