using System;

namespace RioValleyChili.Services.Interfaces.Returns.IntraWarehouseOrderService
{
    public interface IIntraWarehouseOrderDetailReturn : IInventoryOrderDetailReturn<IPickOrderDetailReturn<IPickOrderItemReturn>, IPickOrderItemReturn>
    {
        DateTime MovementDate { get; }
        decimal TrackingSheetNumber { get; }
        string OperatorName { get; }
    }
}