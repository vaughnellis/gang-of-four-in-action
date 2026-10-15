using System.Globalization;
using System.Text;

namespace CornerStore;

internal class JsonReportExporter
{
    public string Export(List<Order> source, string status)
    {
        // SMELL S22: JSON export duplicates the load, filter, aggregate and write skeleton
        var orders = source.Where(order => status == "all" || order.Status == status).ToList();
        var physical = 0m;
        var digital = 0m;
        var giftCard = 0m;
        foreach (var order in orders)
        {
            foreach (var line in order.Lines)
            {
                var amount = line.UnitPrice * line.Quantity;
                // SMELL S23: JSON rows switch on each concrete line item type
                switch (line.Product)
                {
                    case PhysicalProduct:
                        physical += amount;
                        break;
                    case DigitalProduct:
                        digital += amount;
                        break;
                    case GiftCardProduct:
                        giftCard += amount;
                        break;
                }
            }
        }

        var builder = new StringBuilder();
        builder.Append("{\n  \"totals\": [\n");
        builder.Append("    { \"type\": \"Physical\", \"total\": ").Append(physical.ToString("0.00", CultureInfo.InvariantCulture)).Append(" },\n");
        builder.Append("    { \"type\": \"Digital\", \"total\": ").Append(digital.ToString("0.00", CultureInfo.InvariantCulture)).Append(" },\n");
        builder.Append("    { \"type\": \"GiftCard\", \"total\": ").Append(giftCard.ToString("0.00", CultureInfo.InvariantCulture)).Append(" }\n");
        builder.Append("  ]\n}\n");
        return builder.ToString();
    }
}