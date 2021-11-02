using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.TreatmentOrderService;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Extensions.UtilityModels;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class ReceiveTreatmentOrderParametersExtensions
    {
        internal static IResult<ReceiveTreatmentOrderParameters> ToParsedParameters(this IReceiveTreatmentOrderParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var orderKey = KeyParserHelper.ParseResult<ITreatmentOrderKey>(parameters.TreatmentOrderKey);
            if(!orderKey.Success)
            {
                return orderKey.ConvertTo<ReceiveTreatmentOrderParameters>();
            }

            var locationKey = KeyParserHelper.ParseResult<ILocationKey>(parameters.DestinationLocationKey);
            if(!locationKey.Success)
            {
                return locationKey.ConvertTo<ReceiveTreatmentOrderParameters>();
            }

            return new SuccessResult<ReceiveTreatmentOrderParameters>(new ReceiveTreatmentOrderParameters
                {
                    Parameters = parameters,
                    TreatmentOrderKey = new TreatmentOrderKey(orderKey.ResultingObject),
                    DestinationLocationKey = new LocationKey(locationKey.ResultingObject)
                });
        }
    }
}