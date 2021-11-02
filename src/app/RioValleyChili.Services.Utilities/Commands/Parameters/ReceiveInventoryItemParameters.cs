using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Services.Interfaces.Parameters.InventoryService;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class ReceiveInventoryItemParameters
    {
        internal IReceiveInventoryItemParameters Parameters { get; set; }

        internal PackagingProductKey PackagingProductKey { get; set; }
        internal LocationKey LocationKey { get; set; }
        internal InventoryTreatmentKey TreatmentKey { get; set; }
    }
}