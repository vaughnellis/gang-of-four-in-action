using System.Text;

namespace CornerStore;

internal sealed class JsonReportExporter : ReportExporter
{
    protected override string WriteReport(ReportTotalsVisitor totals)
    {
        var builder = new StringBuilder();
        builder.Append("{\n  \"totals\": [\n");
        builder.Append("    { \"type\": \"Physical\", \"total\": ").Append(totals.PhysicalTotal.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)).Append(" },\n");
        builder.Append("    { \"type\": \"Digital\", \"total\": ").Append(totals.DigitalTotal.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)).Append(" },\n");
        builder.Append("    { \"type\": \"GiftCard\", \"total\": ").Append(totals.GiftCardTotal.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)).Append(" }\n");
        builder.Append("  ]\n}\n");
        return builder.ToString();
    }
}
