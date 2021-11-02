using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.InventoryService;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class ReceiveInventoryItemParametersExtensions
    {
        internal static IResult<ReceiveInventoryItemParameters> ToParsedParameters(this IReceiveInventoryItemParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var packagingKeyResult = KeyParserHelper.ParseResult<IPackagingProductKey>(parameters.PackagingProductKey);
            if(!packagingKeyResult.Success)
            {
                return packagingKeyResult.ConvertTo<ReceiveInventoryItemParameters>();
            }

            var warehouseLocationKeyResult = KeyParserHelper.ParseResult<ILocationKey>(parameters.WarehouseLocationKey);
            if(!warehouseLocationKeyResult.Success)
            {
                return warehouseLocationKeyResult.ConvertTo<ReceiveInventoryItemParameters>();
            }

            var inventoryTreatmentKeyResult = KeyParserHelper.ParseResult<IInventoryTreatmentKey>(parameters.TreatmentKey);
            if(!inventoryTreatmentKeyResult.Success)
            {
                return inventoryTreatmentKeyResult.ConvertTo<ReceiveInventoryItemParameters>();
            }

            return new SuccessResult<ReceiveInventoryItemParameters>(new ReceiveInventoryItemParameters
                {
                    Parameters = parameters,
                    PackagingProductKey = new PackagingProductKey(packagingKeyResult.ResultingObject),
                    LocationKey = new LocationKey(warehouseLocationKeyResult.ResultingObject),
                    TreatmentKey = new InventoryTreatmentKey(inventoryTreatmentKeyResult.ResultingObject)
                });
        }

        internal static ModifyInventoryParameters ToModifyInventory(this ReceiveInventoryItemParameters item, ILotKey lotKey)
        {
            return new ModifyInventoryParameters(lotKey, item.PackagingProductKey, item.LocationKey, item.TreatmentKey, item.Parameters.ToteKey, item.Parameters.Quantity);
        }
    }
}