using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.ProductionResultsService;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class BatchResultItemParametersExtensions
    {
        internal static IResult<CreateProductionResultItemCommandParameters> ToParsedParameters(this IBatchResultItemParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            if(parameters.Quantity <= 0)
            {
                return new InvalidResult<CreateProductionResultItemCommandParameters>(null, UserMessages.QuantityNotGreaterThanZero);
            }

            var packagingProductKeyResult = KeyParserHelper.ParseResult<IPackagingProductKey>(parameters.PackagingKey);
            if(!packagingProductKeyResult.Success)
            {
                return packagingProductKeyResult.ConvertTo<CreateProductionResultItemCommandParameters>();
            }

            var warehouseLocationKeyResult = KeyParserHelper.ParseResult<ILocationKey>(parameters.LocationKey);
            if(!warehouseLocationKeyResult.Success)
            {
                return warehouseLocationKeyResult.ConvertTo<CreateProductionResultItemCommandParameters>();
            }

            var inventoryTreatmentKeyResult = KeyParserHelper.ParseResult<IInventoryTreatmentKey>(parameters.InventoryTreatmentKey);
            if(!inventoryTreatmentKeyResult.Success)
            {
                return inventoryTreatmentKeyResult.ConvertTo<CreateProductionResultItemCommandParameters>();
            }

            return new SuccessResult<CreateProductionResultItemCommandParameters>(new CreateProductionResultItemCommandParameters
                {
                    PackagingProductKey = new PackagingProductKey(packagingProductKeyResult.ResultingObject),
                    LocationKey = new LocationKey(warehouseLocationKeyResult.ResultingObject),
                    InventoryTreatmentKey = new InventoryTreatmentKey(inventoryTreatmentKeyResult.ResultingObject),
                    Quantity = parameters.Quantity
                });
        }
    }
}