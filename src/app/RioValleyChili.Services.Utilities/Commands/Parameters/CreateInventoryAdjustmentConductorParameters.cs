using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Parameters.InventoryAdjustmentsService;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class CreateInventoryAdjustmentConductorParameters
    {
        internal ICreateInventoryAdjustmentParameters CreateInventoryAdjustmentParameters { get; set; }

        internal List<CreateInventoryAdjustmentItemCommandParameters> Items { get; set; }
    }
}