using System.Globalization;
using System.Text;

namespace CornerStore;

internal class CsvReportExporter
{
    public string Export(List<Order> source, string status)
    {
        // SMELL S22: CSV export duplicates the load, filter, aggregate and write skeleton
        var orders = source.Where(order => status == "all" || order.Status == status).ToList();
        var visitor = new ReportTotalsVisitor();
        foreach (var order in orders)
        {
            foreach (var line in order.Lines)
            {
                var amount = line.UnitPrice * line.Quantity;
                visitor.Add(line.Product, amount);
            }
        }

        var builder = new StringBuilder();
        builder.Append("type,total\n");
        builder.Append("Physical,").Append(visitor.PhysicalTotal.ToString("0.00", CultureInfo.InvariantCulture)).Append('\n');
        builder.Append("Digital,").Append(visitor.DigitalTotal.ToString("0.00", CultureInfo.InvariantCulture)).Append('\n');
        builder.Append("GiftCard,").Append(visitor.GiftCardTotal.ToString("0.00", CultureInfo.InvariantCulture)).Append('\n');
        return builder.ToString();
    }
}