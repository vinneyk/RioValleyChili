using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Services.Interfaces.Parameters.InventoryAdjustmentsService;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class CreateInventoryAdjustmentItemCommandParameters
    {
        internal IInventoryAdjustmentParameters InventoryAdjustmentParameters { get; set; }

        internal LotKey LotKey { get; set; }

        internal LocationKey LocationKey { get; set; }

        internal PackagingProductKey PackagingProductKey { get; set; }

        internal InventoryTreatmentKey InventoryTreatmentKey { get; set; }

        internal string ToteKey { get; set; }
    }
}