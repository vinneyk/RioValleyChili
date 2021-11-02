namespace RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent
{
    public interface ITransitInformation
    {
        string ShipmentMethod { get; }
        string FreightType { get; }
        string DriverName { get; }
        string CarrierName { get; }
        string TrailerLicenseNumber { get; }
        string ContainerSeal { get; }
    }
}