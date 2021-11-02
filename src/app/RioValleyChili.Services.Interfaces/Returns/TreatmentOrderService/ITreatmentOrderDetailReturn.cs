using System;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using RioValleyChili.Services.Interfaces.Returns.WarehouseOrderService;

namespace RioValleyChili.Services.Interfaces.Returns.TreatmentOrderService
{
    public interface ITreatmentOrderDetailReturn : IInventoryShipmentOrderDetailReturn<IPickOrderDetailReturn<IPickOrderItemReturn>, IPickOrderItemReturn>
    {
        DateTime? Returned { get; }
        IInventoryTreatmentReturn InventoryTreatment { get; }
    }
}