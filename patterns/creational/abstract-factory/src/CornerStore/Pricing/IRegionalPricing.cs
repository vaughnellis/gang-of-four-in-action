namespace CornerStore;

internal interface IRegionalPricing
{
    decimal TaxRate { get; }

    decimal ShippingSurcharge { get; }
}
