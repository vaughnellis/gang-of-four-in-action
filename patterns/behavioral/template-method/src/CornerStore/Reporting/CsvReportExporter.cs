using System.Text;

namespace CornerStore;

internal sealed class CsvReportExporter : ReportExporter
{
    protected override void Accumulate(ReportTotals totals, CartLine line)
    {
        var amount = line.UnitPrice * line.Quantity;
        // SMELL S23: CSV rows switch on each concrete line item type
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
        builder.Append("type,total\n");
        builder.Append("Physical,").Append(totals.Physical.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)).Append('\n');
        builder.Append("Digital,").Append(totals.Digital.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)).Append('\n');
        builder.Append("GiftCard,").Append(totals.GiftCard.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)).Append('\n');
        return builder.ToString();
    }
}