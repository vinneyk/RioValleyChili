using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent;

namespace RioValleyChili.Services.Interfaces.Returns.InventoryShipmentOrderService
{
    public interface IShipmentOrderSummaryReturn
    {
        string InventoryShipmentOrderKey { get; }
        InventoryShipmentOrderTypeEnum OrderType { get; }
        ShipmentStatus Status { get; }
        double? PalletWeight { get; }
        int PalletQuantity { get; }
        double TotalWeightOrdered { get; }
        double TotalWeightPicked { get; }

        ITransitInformation TransitInformation { get; }
        IShippingInstructions ShippingInstructions { get; }
    }
}