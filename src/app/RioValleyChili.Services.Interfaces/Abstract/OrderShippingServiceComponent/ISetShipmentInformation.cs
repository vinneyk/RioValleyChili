using RioValleyChili.Core;

namespace RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent
{
    public interface ISetShipmentInformationWithStatus : ISetShipmentInformation
    {
        ShipmentStatus ShipmentStatus { get; }
    }

    public interface ISetShipmentInformation
    {
        double? PalletWeight { get; }
        int PalletQuantity { get; }

        ISetShippingInstructions ShippingInstructions { get; }
        ISetTransitInformation TransitInformation { get; }
    }
}