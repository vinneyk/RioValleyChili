using System;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Commands.Parameters;

namespace RioValleyChili.Services.Utilities.Extensions.DataModels
{
    internal static class ChileMaterialsReceivedItemExtensions
    {
        internal static ModifyInventoryParameters ToAddInventoryParameters(this ChileMaterialsReceivedItem item, IInventoryTreatmentKey treatment)
        {
            if(item == null) { throw new ArgumentNullException("item"); }

            return new ModifyInventoryParameters(item, item, item, treatment, item.ToteKey, item.Quantity);
        }

        internal static ModifyInventoryParameters ToRemoveInventoryParameters(this ChileMaterialsReceivedItem item, IInventoryTreatmentKey treatment)
        {
            if(item == null) { throw new ArgumentNullException("item"); }

            return new ModifyInventoryParameters(item, item, item, treatment, item.ToteKey, -item.Quantity);
        }
    }
}