using RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent;

namespace RioValleyChili.Client.Mvc.Areas.API.Models
{
    public class TransitInformationDto : ITransitInformation
    {
        public string ShipmentMethod { get; set; }
        public string FreightType { get; set; }
        public string DriverName { get; set; }
        public string CarrierName { get; set; }
        public string TrailerLicenseNumber { get; set; }
        public string ContainerSeal { get; set; }
    }
}