using System.Text;

namespace CornerStore;

internal sealed class CsvReportExporter : ReportExporter
{
    protected override string WriteReport(ReportTotalsVisitor totals)
    {
        var builder = new StringBuilder();
        builder.Append("type,total\n");
        builder.Append("Physical,").Append(totals.PhysicalTotal.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)).Append('\n');
        builder.Append("Digital,").Append(totals.DigitalTotal.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)).Append('\n');
        builder.Append("GiftCard,").Append(totals.GiftCardTotal.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)).Append('\n');
        return builder.ToString();
    }
}
