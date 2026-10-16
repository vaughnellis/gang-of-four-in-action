namespace CornerStore.Tests;

public class AdapterTests
{
    [Fact]
    public void GivenOrder_BookShipment_ConvertsAmountToCentsAndReturnsTracking()
    {
        var log = Log.Instance;
        var output = new StringWriter();
        log.SetOutput(output);

        try
        {
            var adapter = new ShipFastAdapter(new ShipFastClient(new IdSequence()));
            var tracking = adapter.BookShipment(CreateOrder("Ava", "1 Harbour St"));

            Assert.Equal("SF-0001", tracking);
            Assert.Contains("ShipFast booked SF-0001 for o-1001 (1234 cents)", output.ToString());
        }
        finally
        {
            log.ResetOutput();
        }
    }

    [Fact]
    public void GivenSpecialXmlCharacters_BookShipment_SendsValidRequest()
    {
        var log = Log.Instance;
        var output = new StringWriter();
        log.SetOutput(output);

        try
        {
            var adapter = new ShipFastAdapter(new ShipFastClient(new IdSequence()));

            var tracking = adapter.BookShipment(CreateOrder("Ava & Co", "1 <Harbour> & 2"));

            Assert.Equal("SF-0001", tracking);
            Assert.Contains("ShipFast booked SF-0001 for o-1001 (1234 cents)", output.ToString());
        }
        finally
        {
            log.ResetOutput();
        }
    }

    private static Order CreateOrder(string name, string address)
    {
        var customer = new Customer
        {
            Id = "cu-1",
            Number = 1,
            Name = name,
            Region = "AU",
            Channel = "email",
            Address = address,
            CreditLimit = 500m,
        };
        return new Order("o-1001", customer, "Pending", [], new Quote { Total = 12.34m }, "card", null, null, null, null, null);
    }
}