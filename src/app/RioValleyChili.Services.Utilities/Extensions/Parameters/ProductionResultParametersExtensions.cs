using System;
using System.Collections.Generic;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.ProductionResultsService;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class ProductionResultParametersExtensions
    {
        internal static IResult<ProductionResultParameters> ToParsedParameters(this ICreateProductionBatchResultsParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var parametersResult = ToBaseParameters(parameters);
            if(!parametersResult.Success)
            {
                return parametersResult;
            }

            var lotKeyResult = KeyParserHelper.ParseResult<ILotKey>(parameters.ProductionBatchKey);
            if(!lotKeyResult.Success)
            {
                return lotKeyResult.ConvertTo<ProductionResultParameters>();
            }

            parametersResult.ResultingObject.LotKey = new LotKey(lotKeyResult.ResultingObject);
            return parametersResult;
        }

        internal static IResult<ProductionResultParameters> ToParsedParameters(this IUpdateProductionBatchResultsParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var parametersResult = ToBaseParameters(parameters);
            if(!parametersResult.Success)
            {
                return parametersResult;
            }

            var lotKeyResult = KeyParserHelper.ParseResult<ILotKey>(parameters.ProductionResultKey);
            if(!lotKeyResult.Success)
            {
                return lotKeyResult.ConvertTo<ProductionResultParameters>();
            }

            parametersResult.ResultingObject.LotKey = new LotKey(lotKeyResult.ResultingObject);
            return parametersResult;
        }

        private static IResult<ProductionResultParameters> ToBaseParameters(IProductionBatchResultsParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var productionLineLocationKeyResult = KeyParserHelper.ParseResult<ILocationKey>(parameters.ProductionLineKey);
            if(!productionLineLocationKeyResult.Success)
            {
                return productionLineLocationKeyResult.ConvertTo<ProductionResultParameters>();
            }

            var resultItems = new List<CreateProductionResultItemCommandParameters>();
            foreach(var item in parameters.InventoryItems)
            {
                var itemResult = item.ToParsedParameters();
                if(!itemResult.Success)
                {
                    return itemResult.ConvertTo<ProductionResultParameters>();
                }
                resultItems.Add(itemResult.ResultingObject);
            }

            var pickedItemChanges = parameters.PickedInventoryItemChanges.ToParsedParameters();
            if(!pickedItemChanges.Success)
            {
                return pickedItemChanges.ConvertTo<ProductionResultParameters>();
            }

            return new SuccessResult<ProductionResultParameters>(new ProductionResultParameters
                {
                    Parameters = parameters,
                    ProductionLineLocationKey = new LocationKey(productionLineLocationKeyResult.ResultingObject),
                    InventoryItems = resultItems,
                    PickedInventoryItemChanges = pickedItemChanges.ResultingObject
                });
        }
    }
}