using System;
using System.Collections.Generic;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.LotService;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class SetLotAttributeParametersExtensions
    {
        internal static IResult<SetLotAttributeParameters> ToParsedParameters(this ISetLotAttributeParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var lotKey = KeyParserHelper.ParseResult<ILotKey>(parameters.LotKey);
            if(!lotKey.Success)
            {
                return lotKey.ConvertTo<SetLotAttributeParameters>();
            }

            var attributes = new Dictionary<AttributeNameKey, IAttributeValueParameters>();
            foreach(var attribute in parameters.Attributes)
            {
                var attributeNameResult = KeyParserHelper.ParseResult<IAttributeNameKey>(attribute.Key);
                if(!attributeNameResult.Success)
                {
                    return attributeNameResult.ConvertTo<SetLotAttributeParameters>();
                }
                attributes.Add(new AttributeNameKey(attributeNameResult.ResultingObject), attribute.Value);
            }

            return new SuccessResult<SetLotAttributeParameters>(new SetLotAttributeParameters
                {
                    Parameters = parameters,
                    LotKey = lotKey.ResultingObject.ToLotKey(),
                    Attributes = attributes
                });
        }
    }
}