using System;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Returns;
using RioValleyChili.Services.Interfaces.Returns.IntraWarehouseOrderService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class IntraWarehouseOrderReturn : IIntraWarehouseOrderDetailReturn, IIntraWarehouseOrderSummaryReturn
    {
        public string MovementKey { get { return IntraWarehouseOrderKeyReturn.IntraWarehouserOrderKey; } }
        public decimal TrackingSheetNumber { get; set; }
        public string OperatorName { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime MovementDate { get; internal set; }
        public InventoryOrderEnum InventoryOrderEnum { get { return InventoryOrderEnum.WarehouseMovements; } }

        IInventoryPickOrderSummaryReturn IInventoryOrderSummaryReturn.PickOrder { get { return PickOrderSummary; } }
        public IInventoryPickOrderSummaryReturn PickOrderSummary { get; internal set; }

        IPickedInventorySummaryReturn IInventoryOrderSummaryReturn.PickedInventory { get { return PickedInventorySummary; } }
        public IPickedInventorySummaryReturn PickedInventorySummary { get; internal set; }

        IPickOrderDetailReturn<IPickOrderItemReturn> IInventoryOrderDetailReturn<IPickOrderDetailReturn<IPickOrderItemReturn>, IPickOrderItemReturn>.PickOrder { get { return PickOrderDetail; } }
        public IPickOrderDetailReturn<IPickOrderItemReturn> PickOrderDetail { get { return null; } }

        IPickedInventoryDetailReturn IInventoryOrderDetailReturn<IPickOrderDetailReturn<IPickOrderItemReturn>, IPickOrderItemReturn>.PickedInventory { get { return PickedInventoryDetail; } }
        public IPickedInventoryDetailReturn PickedInventoryDetail { get; internal set; }

        internal IntraWarehouseOrderKeyReturn IntraWarehouseOrderKeyReturn { get; set; }
    }
}