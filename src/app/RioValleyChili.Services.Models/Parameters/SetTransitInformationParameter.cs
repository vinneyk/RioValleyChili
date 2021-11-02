using RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent;

namespace RioValleyChili.Services.Models.Parameters
{
    public class SetTransitInformationParameter : ISetTransitInformation
    {
        public SetTransitInformationParameter() { }

        public SetTransitInformationParameter(ITransitInformation transitInformation)
        {
            if(transitInformation != null)
            {
                FreightBillType = transitInformation.FreightType;
                DriverName = transitInformation.DriverName;
                CarrierName = transitInformation.CarrierName;
                TrailerLicenseNumber = transitInformation.TrailerLicenseNumber;
                ContainerSeal = transitInformation.ContainerSeal;
            }
        }

        public string FreightBillType { get; set; }
        public string ShipmentMethod { get; set; }
        public string DriverName { get; set; }
        public string CarrierName { get; set; }
        public string TrailerLicenseNumber { get; set; }
        public string ContainerSeal { get; set; }
    }
}