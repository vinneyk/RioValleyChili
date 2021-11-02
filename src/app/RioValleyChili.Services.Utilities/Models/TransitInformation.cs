using RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent;

namespace RioValleyChili.Services.Utilities.Models
{
    public class TransitInformation : ITransitInformation
    {
        public string ShipmentMethod { get; set; }
        public string FreightType { get; set; }
        public string DriverName { get; set; }
        public string CarrierName { get; set; }
        public string TrailerLicenseNumber { get; set; }
        public string ContainerSeal { get; set; }
    }
}