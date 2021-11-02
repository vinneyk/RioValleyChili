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
    internal static class SetChileProductAttributeRangeParameterExtensions
    {
        internal static IResult<SetChileProductAttributeRangesCommandParameters> ToParsedParameters(this ISetChileProductAttributeRangesParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var chileProductKey = KeyParserHelper.ParseResult<IChileProductKey>(parameters.ChileProductKey);
            if(!chileProductKey.Success)
            {
                return chileProductKey.ConvertTo((SetChileProductAttributeRangesCommandParameters) null);
            }

            var attributeRanges = new List<SetChileProductAttributeRangeParameters>();
            foreach(var range in parameters.AttributeRanges ?? new List<ISetAttributeRangeParameters>())
            {
                var rangeResult = range.ToParsedParameters();
                if(!rangeResult.Success)
                {
                    return rangeResult.ConvertTo<SetChileProductAttributeRangesCommandParameters>();
                }

                attributeRanges.Add(rangeResult.ResultingObject);
            }

            return new SuccessResult().ConvertTo(new SetChileProductAttributeRangesCommandParameters
                {
                    Parameters = parameters,
                    ChileProductKey = chileProductKey.ResultingObject.ToChileProductKey(),
                    AttributeRanges = attributeRanges
                });
        }

        internal static IResult<SetChileProductAttributeRangeParameters> ToParsedParameters(this ISetAttributeRangeParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            if(parameters.RangeMin > parameters.RangeMax)
            {
                return new InvalidResult<SetChileProductAttributeRangeParameters>(null, UserMessages.ChileProductAttributeRangeMinGreaterThanMax);
            }

            var attributeNameKey = KeyParserHelper.ParseResult<IAttributeNameKey>(parameters.AttributeNameKey);
            if(!attributeNameKey.Success)
            {
                return attributeNameKey.ConvertTo<SetChileProductAttributeRangeParameters>();
            }

            return new SuccessResult().ConvertTo(new SetChileProductAttributeRangeParameters
                {
                    Parameters = parameters,
                    AttributeNameKey = attributeNameKey.ResultingObject.ToAttributeNameKey()
                });
        }
    }
}