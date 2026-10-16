namespace CornerStore.Tests;

public class DecoratorTests
{
    [Fact]
    public void GivenBaseComponent_Cost_IsZero()
    {
        var component = new BaseAddOnPriceComponent();

        Assert.Equal(0m, component.Cost);
    }

    [Fact]
    public void GivenGiftWrapDecorator_Cost_AddsFixedCharge()
    {
        IAddOnPriceComponent component = new BaseAddOnPriceComponent();

        component = new GiftWrapPriceDecorator(component, true);

        Assert.Equal(4.50m, component.Cost);
    }

    [Fact]
    public void GivenInsuranceDecorator_Cost_AddsRoundedDiscountedSubtotalCharge()
    {
        IAddOnPriceComponent component = new BaseAddOnPriceComponent();

        component = new InsurancePriceDecorator(component, true, 179.90m);

        Assert.Equal(3.60m, component.Cost);
    }

    [Fact]
    public void GivenBothDecorators_Cost_ExcludesGiftWrapFromInsuranceBasis()
    {
        IAddOnPriceComponent component = new BaseAddOnPriceComponent();
        component = new GiftWrapPriceDecorator(component, true);
        component = new InsurancePriceDecorator(component, true, 100m);

        Assert.Equal(6.50m, component.Cost);
    }
}