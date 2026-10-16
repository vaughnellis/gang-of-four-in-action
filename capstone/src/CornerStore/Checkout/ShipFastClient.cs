using System.Globalization;
using System.Xml.Linq;

namespace CornerStore;

internal class ShipFastClient(IdSequence ids)
{
    public string CreateConsignmentFromXml(string requestXml)
    {
        var request = XDocument.Parse(requestXml).Root ?? throw new InvalidOperationException("shipment request is empty");
        var orderId = request.Element("orderId")?.Value ?? "";
        var cents = request.Element("amountCents")?.Value ?? "0";
        var tracking = ids.NextShipmentId();
        Log.Instance.Info("ShipFast booked " + tracking + " for " + orderId + " (" + cents + " cents)");
        return "<shipment><tracking>" + tracking + "</tracking></shipment>";
    }
}