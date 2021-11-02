using System.Collections.Generic;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Utilities.Extensions.UtilityModels;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class LocationKeysExtensions
    {
        internal static IResult<List<LocationKey>> ToParsedParameters(this IEnumerable<string> locationKeys)
        {
            var keys = new List<LocationKey>();

            foreach(var key in locationKeys)
            {
                var parsedKey = KeyParserHelper.ParseResult<ILocationKey>(key);
                if(!parsedKey.Success)
                {
                    return parsedKey.ConvertTo<List<LocationKey>>();
                }
                keys.Add(new LocationKey(parsedKey.ResultingObject));
            }

            return new SuccessResult<List<LocationKey>>(keys);
        }
    }
}