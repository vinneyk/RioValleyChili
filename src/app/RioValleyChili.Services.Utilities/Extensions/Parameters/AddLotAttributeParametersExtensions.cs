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
    internal static class AddLotAttributeParametersExtensions
    {
        internal static IResult<AddLotAttributeParameters> ToParsedParameters(this IAddLotAttributesParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var lotKeys = new List<LotKey>();
            foreach(var lotKey in parameters.LotKeys)
            {
                var lotKeyResult = KeyParserHelper.ParseResult<ILotKey>(lotKey);
                if(!lotKeyResult.Success)
                {
                    return lotKeyResult.ConvertTo<AddLotAttributeParameters>();
                }

                lotKeys.Add(lotKeyResult.ResultingObject.ToLotKey());
            }

            var attributes = new Dictionary<AttributeNameKey, IAttributeValueParameters>();
            foreach(var attribute in parameters.Attributes)
            {
                var attributeNameResult = KeyParserHelper.ParseResult<IAttributeNameKey>(attribute.Key);
                if(!attributeNameResult.Success)
                {
                    return attributeNameResult.ConvertTo<AddLotAttributeParameters>();
                }
                attributes.Add(new AttributeNameKey(attributeNameResult.ResultingObject), attribute.Value);
            }

            return new SuccessResult<AddLotAttributeParameters>(new AddLotAttributeParameters
                {
                    Parameters = parameters,
                    LotKeys = lotKeys,
                    Attributes = attributes
                });
        }
    }
}