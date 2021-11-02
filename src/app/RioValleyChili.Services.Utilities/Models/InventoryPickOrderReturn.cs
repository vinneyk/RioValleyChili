using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Returns;
using RioValleyChili.Services.Interfaces.Returns.SalesService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class CustomerPickOrderReturn : InventoryPickOrderBaseReturn<ISalesOrderItemReturn> { }

    internal class InventoryPickOrderReturn : InventoryPickOrderBaseReturn<IPickOrderItemReturn>, IInventoryPickOrderSummaryReturn
    {
        public int TotalQuantity { get; internal set; }
        public double TotalWeight { get; internal set; }
    }

    internal class InventoryPickOrderBaseReturn<TPickOrderItem> : IPickOrderDetailReturn<TPickOrderItem>
        where TPickOrderItem : IPickOrderItemReturn
    {
        public string InventoryPickKey { get { return InventoryPickOrderKeyReturn.InventoryPickOrderKey; } }
        public IEnumerable<TPickOrderItem> PickOrderItems { get; set; }

        internal InventoryPickOrderKeyReturn InventoryPickOrderKeyReturn { get; set; }
    }
}