using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.WarehouseService;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Extensions.UtilityModels;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class UpdateLocationParametersExtensions
    {
        internal static IResult<UpdateLocationParameters> ToParsedParameters(this IUpdateLocationParameters parameters)
        {
            var locationKey = KeyParserHelper.ParseResult<ILocationKey>(parameters.LocationKey);
            if(!locationKey.Success)
            {
                return locationKey.ConvertTo<UpdateLocationParameters>(null);
            }
            
            return new SuccessResult<UpdateLocationParameters>(new UpdateLocationParameters
                {
                    Params = parameters,
                    LocationKey = new LocationKey(locationKey.ResultingObject)
                });
        }
    }
}