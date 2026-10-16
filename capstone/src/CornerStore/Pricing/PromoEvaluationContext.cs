namespace CornerStore;

internal sealed record PromoEvaluationContext(Cart Cart, Customer Customer, decimal Subtotal);
