using RioValleyChili.Services.Interfaces.Parameters.PickInventoryServiceComponent;

namespace RioValleyChili.Services.Models.Parameters
{
    public class IntraWarehouseOrderPickedItemParameters : SetPickedInventoryItemParameters, IIntraWarehouseOrderPickedItemParameters
    {
        public string DestinationLocationKey { get; set; }
    }
}