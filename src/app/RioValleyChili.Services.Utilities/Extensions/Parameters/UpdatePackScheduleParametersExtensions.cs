using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.PackScheduleService;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Extensions.UtilityModels;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class UpdatePackScheduleParametersExtensions
    {
        internal static IResult<UpdatePackScheduleCommandParameters> ToParsedParameters(this IUpdatePackScheduleParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var packScheduleKey = KeyParserHelper.ParseResult<IPackScheduleKey>(parameters.PackScheduleKey);
            if(!packScheduleKey.Success)
            {
                return packScheduleKey.ConvertTo((UpdatePackScheduleCommandParameters)null);
            }

            var workTypeKey = KeyParserHelper.ParseResult<IWorkTypeKey>(parameters.WorkTypeKey);
            if(!workTypeKey.Success)
            {
                return workTypeKey.ConvertTo((UpdatePackScheduleCommandParameters)null);
            }

            var chileProductKey = KeyParserHelper.ParseResult<IChileProductKey>(parameters.ChileProductKey);
            if(!chileProductKey.Success)
            {
                return chileProductKey.ConvertTo((UpdatePackScheduleCommandParameters)null);
            }

            var packagingProductKey = KeyParserHelper.ParseResult<IPackagingProductKey>(parameters.PackagingProductKey);
            if(!packagingProductKey.Success)
            {
                return packagingProductKey.ConvertTo((UpdatePackScheduleCommandParameters)null);
            }

            var productionLocationKey = KeyParserHelper.ParseResult<ILocationKey>(parameters.ProductionLineKey);
            if(!productionLocationKey.Success)
            {
                return productionLocationKey.ConvertTo((UpdatePackScheduleCommandParameters)null);
            }

            CustomerKey customerKey = null;
            if(!string.IsNullOrWhiteSpace(parameters.CustomerKey))
            {
                var customerKeyResult = KeyParserHelper.ParseResult<ICustomerKey>(parameters.CustomerKey);
                if(!customerKeyResult.Success)
                {
                    return productionLocationKey.ConvertTo((UpdatePackScheduleCommandParameters)null);
                }
                customerKey = new CustomerKey(customerKeyResult.ResultingObject);
            }

            return new SuccessResult<UpdatePackScheduleCommandParameters>(new UpdatePackScheduleCommandParameters
                {
                    Parameters = parameters,

                    PackScheduleKey = new PackScheduleKey(packScheduleKey.ResultingObject),
                    WorkTypeKey = new WorkTypeKey(workTypeKey.ResultingObject),
                    ChileProductKey = new ChileProductKey(chileProductKey.ResultingObject),
                    PackagingProductKey = new PackagingProductKey(packagingProductKey.ResultingObject),
                    ProductionLocationKey = new LocationKey(productionLocationKey.ResultingObject),
                    CustomerKey = customerKey
                }); 
        }
    }
}