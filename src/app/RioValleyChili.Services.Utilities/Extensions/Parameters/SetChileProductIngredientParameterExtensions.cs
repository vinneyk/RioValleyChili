using System;
using System.Collections.Generic;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.ProductService;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class SetChileProductIngredientParameterExtensions
    {
        internal static IResult<SetChileProductIngredientsCommandParameters> ToParsedParameters(this ISetChileProductIngredientsParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var chileProductKeyResult = KeyParserHelper.ParseResult<IChileProductKey>(parameters.ChileProductKey);
            if(!chileProductKeyResult.Success)
            {
                return chileProductKeyResult.ConvertTo<SetChileProductIngredientsCommandParameters>();
            }

            var ingredients = new List<SetChileProductIngredientCommandParameters>();
            foreach(var item in parameters.Ingredients ?? new List<ISetChileProductIngredientParameters>())
            {
                var itemResult = item.ToParsedParameters();
                if(!itemResult.Success)
                {
                    return itemResult.ConvertTo<SetChileProductIngredientsCommandParameters>();
                }

                ingredients.Add(itemResult.ResultingObject);
            }

            return new SuccessResult<SetChileProductIngredientsCommandParameters>(new SetChileProductIngredientsCommandParameters
                {
                    Parameters = parameters,
                    ChileProductKey = chileProductKeyResult.ResultingObject.ToChileProductKey(),
                    Ingredients = ingredients
                });
        }

        internal static IResult<SetChileProductIngredientCommandParameters> ToParsedParameters(this ISetChileProductIngredientParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var additiveTypeKeyResult = KeyParserHelper.ParseResult<IAdditiveTypeKey>(parameters.AdditiveTypeKey);
            if(!additiveTypeKeyResult.Success)
            {
                return additiveTypeKeyResult.ConvertTo<SetChileProductIngredientCommandParameters>();
            }

            if(parameters.Percentage < 0.0 || parameters.Percentage > 100.0)
            {
                return new InvalidResult<SetChileProductIngredientCommandParameters>(null, UserMessages.IngredientPercentageCanIngredientPercentageOutOfRange);
            }

            return new SuccessResult<SetChileProductIngredientCommandParameters>(new SetChileProductIngredientCommandParameters
                {
                    Parameters = parameters,
                    AdditiveTypeKey = additiveTypeKeyResult.ResultingObject.ToAdditiveTypeKey()
                });
        }
    }
}