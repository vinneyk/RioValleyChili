using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.LotService;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Extensions.UtilityModels;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class CreateLotDefectParametersExtensions
    {
        internal static IResult<CreateLotDefectParameters> ToParsedParameters(this ICreateLotDefectParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            if(parameters.DefectType != DefectTypeEnum.InHouseContamination)
            {
                return new InvalidResult<CreateLotDefectParameters>(null, UserMessages.OnlyInHouseContaminationValid);
            }

            var lotKey = KeyParserHelper.ParseResult<ILotKey>(parameters.LotKey);
            if(!lotKey.Success)
            {
                return lotKey.ConvertTo<CreateLotDefectParameters>(null);
            }

            return new SuccessResult<CreateLotDefectParameters>(new CreateLotDefectParameters
                {
                    Parameters = parameters,
                    LotKey = new LotKey(lotKey.ResultingObject)
                });
        }
    }
}