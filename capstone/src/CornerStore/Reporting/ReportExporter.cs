namespace CornerStore;

internal abstract class ReportExporter
{
    public string Export(List<Order> source, string status)
    {
        var orders = source.Where(order => status == "all" || order.Status == status).ToList();
        var totals = new ReportTotals();
        foreach (var order in orders)
        {
            foreach (var line in order.Lines)
            {
                Accumulate(totals, line);
            }
        }

        return WriteReport(totals);
    }

    protected abstract void Accumulate(ReportTotals totals, CartLine line);

    protected abstract string WriteReport(ReportTotals totals);
}
