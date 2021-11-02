using RioValleyChili.Services.Interfaces.Returns.InventoryShipmentOrderService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class SalesOrderItemInternalAcknowledgement : ISalesOrderItemInternalAcknowledgement
    {
        public string CustomerOrderItemKey { get { return OrderItemKeyReturn.SalesOrderItemKey; } }
        public string ContractKey { get { return ContractKeyReturn == null ? null : ContractKeyReturn.ContractKey; } }
        public int? ContractId { get; set; }
        public double TotalPrice { get; set; }

        internal SalesOrderItemKeyReturn OrderItemKeyReturn { get; set; }
        internal ContractKeyReturn ContractKeyReturn { get; set; }
    }
}