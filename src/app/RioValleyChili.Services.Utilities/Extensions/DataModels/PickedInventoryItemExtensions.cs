using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Commands.Parameters;

namespace RioValleyChili.Services.Utilities.Extensions.DataModels
{
    internal static class PickedInventoryItemExtensions
    {
        internal static ModifyInventoryParameters ToModifyInventoryDestinationParameters(this PickedInventoryItem item, ILocationKey destinationLocation, IInventoryTreatmentKey newTreamentKey = null)
        {
            return new ModifyInventoryParameters(item, item, destinationLocation, newTreamentKey ?? item, item.ToteKey, item.Quantity);
        }
    }
}