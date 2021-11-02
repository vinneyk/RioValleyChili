using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Services.Interfaces.Parameters.WarehouseOrderService
{
    public interface IUpdateInterWarehouseOrderParameters : ISetOrderParameters
    {
        string InventoryShipmentOrderKey { get; }
        IEnumerable<ISetPickedInventoryItemCodesParameters> PickedInventoryItemCodes { get; }
    }
}