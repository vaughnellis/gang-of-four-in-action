namespace CornerStore.Tests;

public class BuilderTests
{
    [Fact]
    public void GivenRequiredOrderValues_Build_CreatesOrderWithOptionalDefaults()
    {
        var customer = CreateCustomer();
        var quote = new Quote { Subtotal = 10m, Total = 12m };
        var lines = new List<CartLine>();

        var order = new OrderBuilder()
            .WithId("o-1001")
            .WithCustomer(customer)
            .WithLines(lines)
            .WithQuote(quote)
            .WithPaymentType("card")
            .Build();

        Assert.Equal("o-1001", order.Id);
        Assert.Equal("Pending", order.Status);
        Assert.Same(customer, order.Customer);
        Assert.Same(lines, order.Lines);
        Assert.Same(quote, order.Quote);
        Assert.Null(order.PromoCode);
        Assert.Null(order.GiftMessage);
        Assert.Null(order.Note);
        Assert.Null(order.TrackingNumber);
        Assert.Null(order.CancellationReason);
    }

    [Fact]
    public void GivenCompleteOrder_BuildInvoice_CopiesQuoteValues()
    {
        var order = new OrderBuilder()
            .WithId("o-1001")
            .WithCustomer(CreateCustomer())
            .WithLines([])
            .WithQuote(new Quote
            {
                Subtotal = 20m,
                Discount = 2m,
                AddOns = 4.50m,
                Shipping = 5m,
                Tax = 2.25m,
                Total = 29.75m,
            })
            .WithPaymentType("paypal")
            .Build();

        var invoice = new InvoiceBuilder().ForOrder(order).Build();

        Assert.Equal("o-1001-I", invoice.Id);
        Assert.Equal(order.Id, invoice.OrderId);
        Assert.Equal(20m, invoice.Subtotal);
        Assert.Equal(2m, invoice.Discount);
        Assert.Equal(4.50m, invoice.AddOns);
        Assert.Equal(5m, invoice.Shipping);
        Assert.Equal(2.25m, invoice.Tax);
        Assert.Equal(29.75m, invoice.Total);
        Assert.Null(invoice.Note);
    }

    [Fact]
    public void GivenMissingOrderId_Build_ThrowsClearValidationError()
    {
        var exception = Assert.Throws<InvalidOperationException>(() => new OrderBuilder().Build());

        Assert.Equal("order id is required", exception.Message);
    }

    [Fact]
    public void GivenMissingInvoiceOrder_Build_ThrowsClearValidationError()
    {
        var exception = Assert.Throws<InvalidOperationException>(() => new InvoiceBuilder().Build());

        Assert.Equal("invoice order is required", exception.Message);
    }

    private static Customer CreateCustomer()
    {
        return new Customer
        {
            Id = "cu-1",
            Number = 1,
            Name = "Ava",
            Region = "AU",
            Channel = "email",
            Address = "1 Harbour St",
            CreditLimit = 500m,
        };
    }
}