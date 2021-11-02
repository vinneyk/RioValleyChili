using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.ProductService;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Extensions.UtilityModels;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class ChileProductParametersExtensions
    {
        internal static IResult<ChileProductCommandParameters> ToParsedParameters(this IChileProductParameters parameters, LotTypeEnum lotType)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var chileTypeKeyResult = KeyParserHelper.ParseResult<IChileTypeKey>(parameters.ChileTypeKey);
            if(!chileTypeKeyResult.Success)
            {
                return chileTypeKeyResult.ConvertTo((ChileProductCommandParameters) null);
            }

            return new SuccessResult<ChileProductCommandParameters>(new ChileProductCommandParameters
                {
                    Parameters = parameters,
                    ChileTypeKey = new ChileTypeKey(chileTypeKeyResult.ResultingObject),
                    ChileState = lotType.ToChileState()
                });
        }
    }
}