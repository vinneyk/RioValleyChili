using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Services.Interfaces.Parameters.SalesService;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class SetSalesOrderItemParameters
    {
        internal ISalesOrderItem SalesOrderItem { get; set; }

        internal ContractItemKey ContractItemKey { get; set; }
        internal ProductKey ProductKey { get; set; }
        internal PackagingProductKey PackagingProductKey { get; set; }
        internal InventoryTreatmentKey InventoryTreatmentKey { get; set; }
    }
}