using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Helpers
{
    //todo: Go over service components and implement usage. RI - 11/26/2013
    internal class ToteKeyParser
    {
        internal static IResult<string> Parse(string toteKey)
        {
            var parsedToteKey = (toteKey ?? "").Trim();
            if(parsedToteKey.Contains(InventoryKey.SEPARATOR))
            {
                return new InvalidResult<string>(null, string.Format(UserMessages.ToteKeyInvalidCharacter, parsedToteKey, InventoryKey.SEPARATOR));
            }

            return new SuccessResult<string>(parsedToteKey);
        }
    }
}