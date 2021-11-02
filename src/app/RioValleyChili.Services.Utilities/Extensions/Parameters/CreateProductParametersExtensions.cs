using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.ProductService;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class CreateProductParametersExtensions
    {
        internal static IResult<CreateAdditiveProductParameters> ToParsedParameters(this ICreateAdditiveProductParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var additiveTypeKeyResult = KeyParserHelper.ParseResult<IAdditiveTypeKey>(parameters.AdditiveTypeKey);
            if(!additiveTypeKeyResult.Success)
            {
                return additiveTypeKeyResult.ConvertTo<CreateAdditiveProductParameters>();
            }

            return new SuccessResult().ConvertTo(new CreateAdditiveProductParameters
                {
                    Parameters = parameters,
                    AdditiveTypeKey = additiveTypeKeyResult.ResultingObject.ToAdditiveTypeKey()
                });
        }

        internal static IResult<CreateChileProductParameters> ToParsedParameters(this ICreateChileProductParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var chileTypeKeyResult = KeyParserHelper.ParseResult<IChileTypeKey>(parameters.ChileTypeKey);
            if(!chileTypeKeyResult.Success)
            {
                return chileTypeKeyResult.ConvertTo<CreateChileProductParameters>();
            }

            var rangesResult = parameters.AttributeRanges.ToParsedParameters(r => r.ToParsedParameters());
            if(!rangesResult.Success)
            {
                return rangesResult.ConvertTo<CreateChileProductParameters>();
            }

            var ingredientsResult = parameters.Ingredients.ToParsedParameters(i => i.ToParsedParameters());
            if(!ingredientsResult.Success)
            {
                return ingredientsResult.ConvertTo<CreateChileProductParameters>();
            }

            return new SuccessResult().ConvertTo(new CreateChileProductParameters
                {
                    Parameters = parameters,
                    ChileTypeKey = chileTypeKeyResult.ResultingObject.ToChileTypeKey(),
                    AttributeRanges = rangesResult.ResultingObject,
                    Ingredients = ingredientsResult.ResultingObject
                });
        }

        internal static IResult<CreatePackagingProductParameters> ToParsedParameters(this ICreatePackagingProductParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            return new SuccessResult().ConvertTo(new CreatePackagingProductParameters
                {
                    Parameters = parameters
                });
        }
    }
}