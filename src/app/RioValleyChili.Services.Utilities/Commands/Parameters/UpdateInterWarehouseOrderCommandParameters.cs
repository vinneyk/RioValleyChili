using RioValleyChili.Business.Core.Keys;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    public class UpdateInterWarehouseOrderCommandParameters : InterWarehouseOrderCommandParameters
    {
        public InventoryShipmentOrderKey OrderKey { get; set; }
    }
}