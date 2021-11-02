using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.ProductService;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Extensions.UtilityModels;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class RemoveChileProductIngredientParametersExtensions
    {
        internal static IResult<RemoveChileProductIngredientParameters> ToParsedParameters(this IRemoveChileProductIngredientParameters parameters)
        {
            if(parameters == null) {  throw new ArgumentNullException("parameters"); }

            var productKeyResult = KeyParserHelper.ParseResult<IChileProductKey>(parameters.ChileProductKey);
            if(!productKeyResult.Success)
            {
                return productKeyResult.ConvertTo<RemoveChileProductIngredientParameters>(null);
            }

            var additiveTypeKeyResult = KeyParserHelper.ParseResult<IAdditiveTypeKey>(parameters.AdditiveTypeKey);
            if(!additiveTypeKeyResult.Success)
            {
                return additiveTypeKeyResult.ConvertTo<RemoveChileProductIngredientParameters>(null); ;
            }

            return new SuccessResult<RemoveChileProductIngredientParameters>(new RemoveChileProductIngredientParameters
                {
                    Parameters = parameters,
                    ChileProductIngredientKey = new ChileProductIngredientKey(productKeyResult.ResultingObject, additiveTypeKeyResult.ResultingObject)
                });
        }
    }
}