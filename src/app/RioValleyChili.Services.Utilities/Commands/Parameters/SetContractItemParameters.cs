using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Services.Interfaces.Parameters.SalesService;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class SetContractItemParameters
    {
        internal IContractItem ContractItemParameters { get; set; }

        internal ChileProductKey ChileProductKey { get; set; }

        internal PackagingProductKey PackagingProductKey { get; set; }

        internal InventoryTreatmentKey TreatmentKey { get; set; }
    }
}