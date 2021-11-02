using System;
using System.Collections.Generic;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.MillAndWetdownService;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class MillAndWetdownParametersExtensions
    {
        internal static IResult<CreateMillAndWetdownParameters> ToParsedParameters(this ICreateMillAndWetdownParameters parameters)
        {
            return Parse<CreateMillAndWetdownParameters, ICreateMillAndWetdownParameters>(parameters);
        }

        internal static IResult<UpdateMillAndWetdownParameters> ToParsedParameters(this IUpdateMillAndWetdownParameters parameters)
        {
            var baseResult = Parse<UpdateMillAndWetdownParameters, IUpdateMillAndWetdownParameters>(parameters);
            if(!baseResult.Success)
            {
                return baseResult.ConvertTo<UpdateMillAndWetdownParameters>();
            }

            var lotKeyResult = KeyParserHelper.ParseResult<ILotKey>(parameters.LotKey);
            if(!lotKeyResult.Success)
            {
                return lotKeyResult.ConvertTo<UpdateMillAndWetdownParameters>();
            }

            baseResult.ResultingObject.LotKey = new LotKey(lotKeyResult.ResultingObject);
            return baseResult;
        }

        private static IResult<TParsed> Parse<TParsed, TParams>(TParams parameters)
            where TParsed : SetMillAndWetdownParameters<TParams>, new()
            where TParams : ISetMillAndWetdownParameters
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var chileProductKeyResult = KeyParserHelper.ParseResult<IChileProductKey>(parameters.ChileProductKey);
            if(!chileProductKeyResult.Success)
            {
                return chileProductKeyResult.ConvertTo<TParsed>();
            }

            var productionLineKeyResult = KeyParserHelper.ParseResult<ILocationKey>(parameters.ProductionLineKey);
            if(!productionLineKeyResult.Success)
            {
                return productionLineKeyResult.ConvertTo<TParsed>();
            }

            var resultItems = new List<CreateMillAndWetdownResultItemCommandParameters>();
            foreach(var item in parameters.ResultItems ?? new List<IMillAndWetdownResultItemParameters>())
            {
                var itemResult = item.ToCreateMillAndWetdownResultItemCommandParameters();
                if(!itemResult.Success)
                {
                    return itemResult.ConvertTo<TParsed>();
                }
                resultItems.Add(itemResult.ResultingObject);
            }

            var pickedItems = new List<PickedInventoryParameters>();
            foreach(var item in parameters.PickedItems ?? new List<IMillAndWetdownPickedItemParameters>())
            {
                var itemResult = item.ToPickedInventoryParameters();
                if(!itemResult.Success)
                {
                    return itemResult.ConvertTo<TParsed>();
                }
                pickedItems.Add(itemResult.ResultingObject);
            }

            return new SuccessResult<TParsed>(new TParsed
            {
                Params = parameters,
                ChileProductKey = chileProductKeyResult.ResultingObject.ToChileProductKey(),
                ProductionLineKey = productionLineKeyResult.ResultingObject.ToLocationKey(),
                ResultItems = resultItems,
                PickedItems = pickedItems
            });
        }
    }
}