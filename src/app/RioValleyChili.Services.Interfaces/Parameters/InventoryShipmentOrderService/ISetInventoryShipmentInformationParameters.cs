using RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent;

namespace RioValleyChili.Services.Interfaces.Parameters.InventoryShipmentOrderService
{
    public interface ISetInventoryShipmentInformationParameters : ISetShipmentInformation
    {
        string InventoryShipmentOrderKey { get; }
    }
}