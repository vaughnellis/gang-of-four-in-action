namespace CornerStore;

internal class ReportService(OrderService orders)
{
    private readonly CsvReportExporter csv = new();
    private readonly JsonReportExporter json = new();

    public string ExportCsv(string status)
    {
        return csv.Export(orders.GetAll(), status);
    }

    public string ExportJson(string status)
    {
        return json.Export(orders.GetAll(), status);
    }
}