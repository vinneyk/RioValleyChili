using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class SetShipmentInformationWithStatus : ISetShipmentInformationWithStatus
    {
        public ShipmentStatus ShipmentStatus { get; set; }
        public double? PalletWeight { get; set; }
        public int PalletQuantity { get; set; }
        public SetShippingInstructions ShippingInstructions { get; set; }
        public SetTransitInformation TransitInformation { get; set; }

        ISetShippingInstructions ISetShipmentInformation.ShippingInstructions { get { return ShippingInstructions; } }
        ISetTransitInformation ISetShipmentInformation.TransitInformation { get { return TransitInformation; } }
    }
}