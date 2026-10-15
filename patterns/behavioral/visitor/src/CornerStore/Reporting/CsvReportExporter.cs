using System.Globalization;
using System.Text;

namespace CornerStore;

internal class CsvReportExporter
{
    public string Export(List<Order> source, string status)
    {
        // SMELL S22: CSV export duplicates the load, filter, aggregate and write skeleton
        var orders = source.Where(order => status == "all" || order.Status == status).ToList();
        var physical = 0m;
        var digital = 0m;
        var giftCard = 0m;
        foreach (var order in orders)
        {
            foreach (var line in order.Lines)
            {
                var amount = line.UnitPrice * line.Quantity;
                // SMELL S23: CSV rows switch on each concrete line item type
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
        builder.Append("type,total\n");
        builder.Append("Physical,").Append(physical.ToString("0.00", CultureInfo.InvariantCulture)).Append('\n');
        builder.Append("Digital,").Append(digital.ToString("0.00", CultureInfo.InvariantCulture)).Append('\n');
        builder.Append("GiftCard,").Append(giftCard.ToString("0.00", CultureInfo.InvariantCulture)).Append('\n');
        return builder.ToString();
    }
}