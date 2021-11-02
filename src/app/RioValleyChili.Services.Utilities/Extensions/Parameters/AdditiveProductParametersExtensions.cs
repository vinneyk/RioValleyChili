using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.ProductService;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class AdditiveProductParametersExtensions
    {
        internal static IResult<AdditiveProductCommandParameters> ToParsedParameters(this IAdditiveProductParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var additiveTypeKeyResult = KeyParserHelper.ParseResult<IAdditiveTypeKey>(parameters.AdditiveTypeKey);
            if(!additiveTypeKeyResult.Success)
            {
                return additiveTypeKeyResult.ConvertTo<AdditiveProductCommandParameters>();
            }

            return new SuccessResult<AdditiveProductCommandParameters>(new AdditiveProductCommandParameters
                {
                    Parameters = parameters,
                    AdditiveTypeKey = additiveTypeKeyResult.ResultingObject.ToAdditiveTypeKey()
                });
        }
    }
}