using System.Globalization;

namespace CornerStore.Tests;

public class TemplateMethodTests
{
    [Fact]
    public void GivenOrdersWithDifferentStatuses_Export_FiltersBeforeAccumulating()
    {
        var paidOrder = CreateOrder("Paid", new PhysicalProduct(), 10m);
        var shippedOrder = CreateOrder("Shipped", new DigitalProduct(), 20m);

        var csv = new CsvReportExporter().Export([paidOrder, shippedOrder], "Paid");

        Assert.Contains("Physical,10.00\n", csv);
        Assert.Contains("Digital,0.00\n", csv);
    }

    [Fact]
    public void GivenCsvExporter_Export_WritesCsvStructure()
    {
        var order = CreateOrder("Paid", new GiftCardProduct(), 25m);

        var report = new CsvReportExporter().Export([order], "all");

        Assert.StartsWith("type,total\n", report);
        Assert.Contains("GiftCard,25.00\n", report);
    }

    [Fact]
    public void GivenJsonExporter_Export_WritesJsonStructure()
    {
        var order = CreateOrder("Paid", new DigitalProduct(), 15m);

        var report = new JsonReportExporter().Export([order], "all");

        Assert.StartsWith("{\n  \"totals\": [\n", report);
        Assert.Contains("{ \"type\": \"Digital\", \"total\": 15.00 }", report);
        Assert.EndsWith("  ]\n}\n", report);
    }

    [Fact]
    public void GivenCustomExporter_Export_UsesSubclassHooks()
    {
        var order = CreateOrder("Paid", new PhysicalProduct(), 8.25m);

        var report = new TestReportExporter().Export([order], "all");

        Assert.Equal("custom:8.25", report);
    }

    private static Order CreateOrder(string status, Product product, decimal price)
    {
        var customer = new Customer
        {
            Id = "cu-1",
            Number = 1,
            Name = "Ava",
            Region = "AU",
            Channel = "email",
            Address = "1 Harbour St",
            CreditLimit = 500m,
        };
        var line = new CartLine
        {
            Product = product,
            Quantity = 1,
            UnitPrice = price,
        };
        return new Order(
            "o-1001",
            customer,
            CreateState(status),
            [line],
            new Quote(),
            "card",
            null,
            null,
            null,
            null,
            null);
    }

    private static OrderState CreateState(string status)
    {
        return status switch
        {
            "Pending" => new PendingOrderState(),
            "Paid" => new PaidOrderState(),
            "Shipped" => new ShippedOrderState(),
            "Delivered" => new DeliveredOrderState(),
            "Cancelled" => new CancelledOrderState(),
            "Refunded" => new RefundedOrderState(),
            _ => throw new ArgumentOutOfRangeException(nameof(status)),
        };
    }

    private sealed class TestReportExporter : ReportExporter
    {
        private decimal total;

        protected override void Accumulate(ReportTotals totals, CartLine line)
        {
            total += line.UnitPrice * line.Quantity;
        }

        protected override string WriteReport(ReportTotals totals)
        {
            return "custom:" + total.ToString("0.00", CultureInfo.InvariantCulture);
        }
    }
}
