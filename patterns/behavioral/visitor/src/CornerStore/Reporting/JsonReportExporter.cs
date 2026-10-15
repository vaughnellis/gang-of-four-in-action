using System.Globalization;
using System.Text;

namespace CornerStore;

internal class JsonReportExporter
{
    public string Export(List<Order> source, string status)
    {
        // SMELL S22: JSON export duplicates the load, filter, aggregate and write skeleton
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
        builder.Append("{\n  \"totals\": [\n");
        builder.Append("    { \"type\": \"Physical\", \"total\": ").Append(visitor.PhysicalTotal.ToString("0.00", CultureInfo.InvariantCulture)).Append(" },\n");
        builder.Append("    { \"type\": \"Digital\", \"total\": ").Append(visitor.DigitalTotal.ToString("0.00", CultureInfo.InvariantCulture)).Append(" },\n");
        builder.Append("    { \"type\": \"GiftCard\", \"total\": ").Append(visitor.GiftCardTotal.ToString("0.00", CultureInfo.InvariantCulture)).Append(" }\n");
        builder.Append("  ]\n}\n");
        return builder.ToString();
    }
}