using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Parameters.PickInventoryServiceComponent;

namespace RioValleyChili.Services.Interfaces.Parameters.IntraWarehouseOrderService
{
    public interface ICreateIntraWarehouseOrderParameters : IIntraWarehouseOrderParameters
    {
        decimal TrackingSheetNumber { get; }
        IEnumerable<IIntraWarehouseOrderPickedItemParameters> PickedItems { get; }
    }
}