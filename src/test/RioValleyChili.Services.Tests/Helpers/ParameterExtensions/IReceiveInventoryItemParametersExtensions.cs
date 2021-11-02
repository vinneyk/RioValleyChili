using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.InventoryService;

namespace RioValleyChili.Services.Tests.Helpers.ParameterExtensions
{
    internal static class IReceiveInventoryItemParametersExtensions
    {
        internal static bool IsAsExpected(this IReceiveInventoryItemParameters parameters, Inventory inventory)
        {
            if(parameters.Quantity != inventory.Quantity)
            {
                return false;
            }

            if(parameters.PackagingProductKey != new PackagingProductKey(inventory).KeyValue)
            {
                return false;
            }

            if(parameters.WarehouseLocationKey != new LocationKey(inventory).KeyValue)
            {
                return false;
            }

            if(parameters.TreatmentKey != new InventoryTreatmentKey(inventory).KeyValue)
            {
                return false;
            }

            return parameters.ToteKey == inventory.ToteKey;
        }
    }
}