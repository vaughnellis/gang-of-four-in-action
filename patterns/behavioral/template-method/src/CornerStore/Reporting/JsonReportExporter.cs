using System.Text;

namespace CornerStore;

internal sealed class JsonReportExporter : ReportExporter
{
    protected override void Accumulate(ReportTotals totals, CartLine line)
    {
        var amount = line.UnitPrice * line.Quantity;
        // SMELL S23: JSON rows switch on each concrete line item type
        switch (line.Product)
        {
            case PhysicalProduct:
                totals.Physical += amount;
                break;
            case DigitalProduct:
                totals.Digital += amount;
                break;
            case GiftCardProduct:
                totals.GiftCard += amount;
                break;
        }
    }

    protected override string WriteReport(ReportTotals totals)
    {
        var builder = new StringBuilder();
        builder.Append("{\n  \"totals\": [\n");
        builder.Append("    { \"type\": \"Physical\", \"total\": ").Append(totals.Physical.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)).Append(" },\n");
        builder.Append("    { \"type\": \"Digital\", \"total\": ").Append(totals.Digital.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)).Append(" },\n");
        builder.Append("    { \"type\": \"GiftCard\", \"total\": ").Append(totals.GiftCard.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)).Append(" }\n");
        builder.Append("  ]\n}\n");
        return builder.ToString();
    }
}