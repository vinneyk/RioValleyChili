using System;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using RioValleyChili.Services.Interfaces.Returns.WarehouseOrderService;

namespace RioValleyChili.Services.Interfaces.Returns.TreatmentOrderService
{
    public interface ITreatmentOrderSummaryReturn : IInventoryShipmentOrderSummaryReturn
    {
        DateTime? Returned { get; }
        IInventoryTreatmentReturn InventoryTreatment { get; }
    }
}