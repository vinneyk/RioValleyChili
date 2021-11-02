using System;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.MillAndWetdownService;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class MillAndWetdownPickedItemParametersExtensions
    {
        internal static IResult<PickedInventoryParameters> ToPickedInventoryParameters(this IMillAndWetdownPickedItemParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var inventoryKeyResult = KeyParserHelper.ParseResult<IInventoryKey>(parameters.InventoryKey);
            if(!inventoryKeyResult.Success)
            {
                return inventoryKeyResult.ConvertTo((PickedInventoryParameters) null);
            }

            return new SuccessResult<PickedInventoryParameters>(new PickedInventoryParameters(inventoryKeyResult.ResultingObject, parameters.Quantity, null, null));
        }
    }
}