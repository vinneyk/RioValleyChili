using System;
using System.Collections.Generic;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.PackScheduleService;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class ProductionScheduleParametersExtensions
    {
        internal static IResult<CreateProductionScheduleParameters> ToParsedParameters(this ICreateProductionScheduleParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var locationKey = KeyParserHelper.ParseResult<ILocationKey>(parameters.ProductionLineLocationKey);
            if(!locationKey.Success)
            {
                return locationKey.ConvertTo<CreateProductionScheduleParameters>();
            }

            return new SuccessResult<CreateProductionScheduleParameters>(new CreateProductionScheduleParameters
                {
                    Parameters = parameters,
                    ProductionLocationKey = locationKey.ResultingObject.ToLocationKey()
                });
        }

        internal static IResult<UpdateProductionScheduleParameters> ToParsedParameters(this IUpdateProductionScheduleParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var productionScheduleKey = KeyParserHelper.ParseResult<IProductionScheduleKey>(parameters.ProductionScheduleKey);
            if(!productionScheduleKey.Success)
            {
                return productionScheduleKey.ConvertTo<UpdateProductionScheduleParameters>();
            }

            var scheduledItems = new List<UpdateProductionScheduleItemParameters>();
            foreach(var item in parameters.ScheduledItems)
            {
                var parsedItem = item.ToParsedParameters();
                if(!parsedItem.Success)
                {
                    return parsedItem.ConvertTo<UpdateProductionScheduleParameters>();
                }

                scheduledItems.Add(parsedItem.ResultingObject);
            }

            return new SuccessResult<UpdateProductionScheduleParameters>(new UpdateProductionScheduleParameters
                {
                    Parameters = parameters,
                    ProductionScheduleKey = productionScheduleKey.ResultingObject.ToProductionScheduleKey(),
                    ScheduledItems = scheduledItems
                });
        }

        private static IResult<UpdateProductionScheduleItemParameters> ToParsedParameters(this ISetProductionScheduleItemParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var packScheduleKey = KeyParserHelper.ParseResult<IPackScheduleKey>(parameters.PackScheduleKey);
            if(!packScheduleKey.Success)
            {
                return packScheduleKey.ConvertTo<UpdateProductionScheduleItemParameters>();
            }

            return new SuccessResult<UpdateProductionScheduleItemParameters>(new UpdateProductionScheduleItemParameters
                {
                    Parameters = parameters,
                    PackScheduleKey = packScheduleKey.ResultingObject.ToPackScheduleKey()
                });
        }
    }
}