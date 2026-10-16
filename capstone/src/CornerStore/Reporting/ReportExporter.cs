namespace CornerStore;

internal abstract class ReportExporter
{
    public string Export(List<Order> source, string status)
    {
        var orders = source.Where(order => status == "all" || order.Status == status).ToList();
        var totals = new ReportTotalsVisitor();
        foreach (var order in orders)
        {
            foreach (var line in order.Lines)
            {
                totals.Add(line.Product, line.UnitPrice * line.Quantity);
            }
        }

        return WriteReport(totals);
    }

    protected abstract string WriteReport(ReportTotalsVisitor totals);
}
