using System;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.PackScheduleService;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Extensions.UtilityModels;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class CreateProductionBatchParametersExtensions
    {
        internal static IResult<CreateProductionBatchCommandParameters> ToParsedParameters(this ICreateProductionBatchParameters parameters)
        {
            if(parameters == null) {  throw new ArgumentNullException("parameters"); }

            var packScheduleKey = KeyParserHelper.ParseResult<IPackScheduleKey>(parameters.PackScheduleKey);
            if(!packScheduleKey.Success)
            {
                return packScheduleKey.ConvertTo((CreateProductionBatchCommandParameters) null);
            }

            return new SuccessResult<CreateProductionBatchCommandParameters>(new CreateProductionBatchCommandParameters
                {
                    Parameters = parameters,
                    PackScheduleKey = new PackScheduleKey(packScheduleKey.ResultingObject)
                });
        }
    }
}