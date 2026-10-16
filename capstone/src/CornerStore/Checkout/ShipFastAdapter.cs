using System.Globalization;
using System.Xml.Linq;

namespace CornerStore;

internal sealed class ShipFastAdapter(ShipFastClient client) : IShippingProvider
{
    public string BookShipment(Order order)
    {
        var cents = Math.Round(order.Quote.Total * 100m, 0, MidpointRounding.AwayFromZero)
            .ToString("0", CultureInfo.InvariantCulture);
        var name = System.Security.SecurityElement.Escape(order.Customer.Name) ?? "";
        var address = System.Security.SecurityElement.Escape(order.Customer.Address) ?? "";
        var request = "<shipment><orderId>" + order.Id + "</orderId><recipient>" + name
            + "</recipient><address>" + address + "</address><amountCents>" + cents + "</amountCents></shipment>";
        var response = client.CreateConsignmentFromXml(request);
        return XDocument.Parse(response).Root?.Element("tracking")?.Value ?? "";
    }
}