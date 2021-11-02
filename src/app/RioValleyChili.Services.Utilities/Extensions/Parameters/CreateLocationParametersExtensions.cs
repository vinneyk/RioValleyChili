using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.WarehouseService;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Extensions.UtilityModels;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class CreateLocationParametersExtensions
    {
        internal static IResult<CreateLocationParameters> ToParsedParameters(this ICreateLocationParameters parameters)
        {
            var facilityKey = KeyParserHelper.ParseResult<IFacilityKey>(parameters.FacilityKey);
            if(!facilityKey.Success)
            {
                return facilityKey.ConvertTo<CreateLocationParameters>(null);
            }

            return new SuccessResult<CreateLocationParameters>(new CreateLocationParameters
                {
                    Params = parameters,
                    FacilityKey = new FacilityKey(facilityKey.ResultingObject)
                });
        }
    }
}