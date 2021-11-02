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
    internal static class SetLotStatusParametersExtensions
    {
        internal static IResult<SetLotStatusParameters> ToParsedParameters(this ISetLotStatusParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var lotKeyResult = KeyParserHelper.ParseResult<ILotKey>(parameters.LotKey);
            if(!lotKeyResult.Success)
            {
                return lotKeyResult.ConvertTo<SetLotStatusParameters>(null);
            }

            return new SuccessResult<SetLotStatusParameters>(new SetLotStatusParameters
                {
                    Parameters = parameters,
                    LotKey = new LotKey(lotKeyResult.ResultingObject)
                });
        }
    }
}