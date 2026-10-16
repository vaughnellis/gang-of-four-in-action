namespace CornerStore;

internal sealed record RegionalPricing(decimal TaxRate, decimal ShippingSurcharge) : IRegionalPricing;
