using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.ProductService;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class UpdateProductParametersExtensions
    {
        internal static IResult<UpdateAdditiveProductParameters> ToParsedParameters(this IUpdateAdditiveProductParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var baseResult = ToParsedParameters<UpdateAdditiveProductParameters, IUpdateAdditiveProductParameters>(parameters);
            if(!baseResult.Success)
            {
                return baseResult;
            }

            var additiveTypeKeyResult = KeyParserHelper.ParseResult<IAdditiveTypeKey>(parameters.AdditiveTypeKey);
            if (!additiveTypeKeyResult.Success)
            {
                return additiveTypeKeyResult.ConvertTo<UpdateAdditiveProductParameters>();
            }

            baseResult.ResultingObject.AdditiveTypeKey = additiveTypeKeyResult.ResultingObject.ToAdditiveTypeKey();
            return baseResult;
        }

        internal static IResult<UpdateChileProductParameters> ToParsedParameters(this IUpdateChileProductParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var baseResult = ToParsedParameters<UpdateChileProductParameters, IUpdateChileProductParameters>(parameters);
            if(!baseResult.Success)
            {
                return baseResult;
            }

            var chileTypeKeyResult = KeyParserHelper.ParseResult<IChileTypeKey>(parameters.ChileTypeKey);
            if(!chileTypeKeyResult.Success)
            {
                return chileTypeKeyResult.ConvertTo<UpdateChileProductParameters>();
            }

            var rangesResult = parameters.AttributeRanges.ToParsedParameters(r => r.ToParsedParameters());
            if(!rangesResult.Success)
            {
                return rangesResult.ConvertTo<UpdateChileProductParameters>();
            }

            var ingredientsResult = parameters.Ingredients.ToParsedParameters(i => i.ToParsedParameters());
            if(!ingredientsResult.Success)
            {
                return ingredientsResult.ConvertTo<UpdateChileProductParameters>();
            }

            baseResult.ResultingObject.ChileTypeKey = chileTypeKeyResult.ResultingObject.ToChileTypeKey();
            baseResult.ResultingObject.AttributeRanges = rangesResult.ResultingObject;
            baseResult.ResultingObject.Ingredients = ingredientsResult.ResultingObject;
            return baseResult;
        }

        internal static IResult<UpdatePackagingProductParameters> ToParsedParameters(this IUpdatePackagingProductParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }
            return ToParsedParameters<UpdatePackagingProductParameters, IUpdatePackagingProductParameters>(parameters);
        }

        private static IResult<T> ToParsedParameters<T, T2>(this T2 parameters)
            where T : UpdateProductParametersBase<T2>, new()
            where T2 : IUpdateProductParameters
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var productKeyResult = KeyParserHelper.ParseResult<IProductKey>(parameters.ProductKey);
            if(!productKeyResult.Success)
            {
                return productKeyResult.ConvertTo<T>();
            }

            return new SuccessResult<T>(new T
                {
                    Parameters = parameters,
                    ProductKey = productKeyResult.ResultingObject.ToProductKey()
                });
        }
    }
}