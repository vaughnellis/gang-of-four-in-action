using System.Globalization;

namespace CornerStore;

internal class IdSequence
{
    private int _customer;
    private int _order = 1000;
    private int _shipment;

    public string NextCustomerId() => "cu-" + (++_customer).ToString(CultureInfo.InvariantCulture);

    public string NextOrderId() => "o-" + (++_order).ToString(CultureInfo.InvariantCulture);

    public string NextShipmentId() => "SF-" + (++_shipment).ToString("D4", CultureInfo.InvariantCulture);
}
