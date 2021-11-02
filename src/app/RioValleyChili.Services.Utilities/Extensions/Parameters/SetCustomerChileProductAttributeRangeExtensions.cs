using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.SalesService;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class SetCustomerChileProductAttributeRangeExtensions
    {
        internal static IResult<SetCustomerProductAttributeRangesParameters> ToParsedParameters(this ISetCustomerProductAttributeRangesParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var customerKeyResult = KeyParserHelper.ParseResult<ICustomerKey>(parameters.CustomerKey);
            if(!customerKeyResult.Success)
            {
                return customerKeyResult.ConvertTo<SetCustomerProductAttributeRangesParameters>();
            }

            var chileProductKeyResult = KeyParserHelper.ParseResult<IChileProductKey>(parameters.ChileProductKey);
            if(!chileProductKeyResult.Success)
            {
                return chileProductKeyResult.ConvertTo<SetCustomerProductAttributeRangesParameters>();
            }

            var attributeRanges = parameters.AttributeRanges.ToParsedParameters(i => i.ToParsedParameters());
            if(!attributeRanges.Success)
            {
                return attributeRanges.ConvertTo<SetCustomerProductAttributeRangesParameters>();
            }

            return new SuccessResult().ConvertTo(new SetCustomerProductAttributeRangesParameters
                {
                    Parameters = parameters,
                    CustomerKey = customerKeyResult.ResultingObject.ToCustomerKey(),
                    ChileProductKey = chileProductKeyResult.ResultingObject.ToChileProductKey(),
                    AttributeRanges = attributeRanges.ResultingObject
                });
        }

        internal static IResult<SetCustomerProductAttributeRangeParameters> ToParsedParameters(this ISetCustomerProductAttributeRangeParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var attributeNameKey = KeyParserHelper.ParseResult<IAttributeNameKey>(parameters.AttributeNameKey);
            if(!attributeNameKey.Success)
            {
                return attributeNameKey.ConvertTo<SetCustomerProductAttributeRangeParameters>();
            }

            return new SuccessResult().ConvertTo(new SetCustomerProductAttributeRangeParameters
                {
                    Parameters = parameters,
                    AttributeNameKey = attributeNameKey.ResultingObject.ToAttributeNameKey()
                });
        }
    }
}