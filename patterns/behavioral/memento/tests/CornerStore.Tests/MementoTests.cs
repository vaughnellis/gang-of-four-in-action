namespace CornerStore.Tests;

public class MementoTests
{
    [Fact]
    public void Cart_Restore_ReturnsAllMutableStateToTheCapturedValues()
    {
        var cart = CreateCart();
        var memento = cart.CreateMemento();
        cart.Lines[0].Quantity = 5;
        cart.Lines[0].UnitPrice = 20m;
        cart.PromoCode = "OTHER";
        cart.ShippingMethod = "express";
        cart.GiftWrap = false;
        cart.Insurance = false;

        cart.Restore(memento);

        Assert.Equal(2, cart.Lines[0].Quantity);
        Assert.Equal(12.50m, cart.Lines[0].UnitPrice);
        Assert.Equal("SKU-1", cart.Lines[0].Product.Sku);
        Assert.Equal("BUN-1", cart.Lines[0].FromBundle);
        Assert.Equal("SAVE", cart.PromoCode);
        Assert.Equal("pickup", cart.ShippingMethod);
        Assert.True(cart.GiftWrap);
        Assert.True(cart.Insurance);
    }

    [Fact]
    public void CartMemento_DoesNotShareMutableLineStateWithTheCart()
    {
        var cart = CreateCart();
        var memento = cart.CreateMemento();
        cart.Lines[0].Quantity = 5;
        cart.Restore(memento);
        cart.Lines[0].Quantity = 9;

        cart.Restore(memento);

        Assert.Equal(2, cart.Lines[0].Quantity);
    }

    private static Cart CreateCart()
    {
        var cart = new Cart("cu-1")
        {
            PromoCode = "SAVE",
            ShippingMethod = "pickup",
            GiftWrap = true,
            Insurance = true,
        };
        cart.Lines.Add(new CartLine
        {
            Product = new PhysicalProduct { Sku = "SKU-1", Name = "Sample" },
            Quantity = 2,
            UnitPrice = 12.50m,
            FromBundle = "BUN-1",
        });

        return cart;
    }
}
