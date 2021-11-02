using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.LotService;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Extensions.UtilityModels;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class RemoveLotDefectResolutionParametersExtensions
    {
        internal static IResult<RemoveLotDefectResolutionParameters> ToParsedParameters(this IRemoveLotDefectResolutionParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var lotDefectKey = KeyParserHelper.ParseResult<ILotDefectKey>(parameters.LotDefectKey);
            if(!lotDefectKey.Success)
            {
                return lotDefectKey.ConvertTo<RemoveLotDefectResolutionParameters>(null);
            }

            return new SuccessResult<RemoveLotDefectResolutionParameters>(new RemoveLotDefectResolutionParameters
                {
                    Parameters = parameters,
                    LotDefectKey = new LotDefectKey(lotDefectKey.ResultingObject)
                });
        }
    }
}