using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.PackScheduleService;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class UpdateProductionBatchParametersExtensions
    {
        internal static IResult<UpdateProductionBatchCommandParameters> ToParsedParameters(this IUpdateProductionBatchParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var productionBatchKey = KeyParserHelper.ParseResult<ILotKey>(parameters.ProductionBatchKey);
            if(!productionBatchKey.Success)
            {
                return productionBatchKey.ConvertTo<UpdateProductionBatchCommandParameters>();
            }

            return new SuccessResult<UpdateProductionBatchCommandParameters>(new UpdateProductionBatchCommandParameters
                {
                    Parameters = parameters,
                    ProductionBatchKey = new LotKey(productionBatchKey.ResultingObject)
                });
        }
    }
}