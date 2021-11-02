using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.InventoryAdjustmentsService;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Extensions.UtilityModels;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class InventoryAdjustmentParametersExtensions
    {
        internal static IResult<CreateInventoryAdjustmentItemCommandParameters> ToParsedParameters(this IInventoryAdjustmentParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var lotKeyResult = KeyParserHelper.ParseResult<ILotKey>(parameters.LotKey);
            if(!lotKeyResult.Success)
            {
                return lotKeyResult.ConvertTo((CreateInventoryAdjustmentItemCommandParameters)null);
            }

            var warehouseLocationKeyResult = KeyParserHelper.ParseResult<ILocationKey>(parameters.WarehouseLocationKey);
            if(!warehouseLocationKeyResult.Success)
            {
                return warehouseLocationKeyResult.ConvertTo((CreateInventoryAdjustmentItemCommandParameters)null);
            }

            var packagingProductKeyResult = KeyParserHelper.ParseResult<IPackagingProductKey>(parameters.PackagingProductKey);
            if(!packagingProductKeyResult.Success)
            {
                return packagingProductKeyResult.ConvertTo((CreateInventoryAdjustmentItemCommandParameters)null);
            }

            var treatmentKeyResult = KeyParserHelper.ParseResult<IInventoryTreatmentKey>(parameters.TreatmentKey);
            if(!treatmentKeyResult.Success)
            {
                return treatmentKeyResult.ConvertTo((CreateInventoryAdjustmentItemCommandParameters)null);
            }

            return new SuccessResult<CreateInventoryAdjustmentItemCommandParameters>(new CreateInventoryAdjustmentItemCommandParameters
                {
                    InventoryAdjustmentParameters = parameters,

                    LotKey = new LotKey(lotKeyResult.ResultingObject),
                    LocationKey = new LocationKey(warehouseLocationKeyResult.ResultingObject),
                    PackagingProductKey = new PackagingProductKey(packagingProductKeyResult.ResultingObject),
                    InventoryTreatmentKey = new InventoryTreatmentKey(treatmentKeyResult.ResultingObject),
                    ToteKey = parameters.ToteKey ?? ""
                });
        }
    }
}