namespace RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent
{
    public interface ISetTransitInformation
    {
        string FreightBillType { get; }
        string ShipmentMethod { get; }
        string DriverName { get; }
        string CarrierName { get; }
        string TrailerLicenseNumber { get; }
        string ContainerSeal { get; }
    }
}