using System;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent;
using RioValleyChili.Services.Interfaces.Returns;
using RioValleyChili.Services.Interfaces.Returns.WarehouseOrderService;
using RioValleyChili.Services.Interfaces.Returns.WarehouseService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class InventoryShipmentOrderDetailReturn : InventoryShipmentOrderDetailBaseReturn,
        IInventoryShipmentOrderDetailReturn<IPickOrderDetailReturn<IPickOrderItemReturn>, IPickOrderItemReturn>
    {
        public IPickOrderDetailReturn<IPickOrderItemReturn> PickOrder { get; internal set; }
    }

    internal class InventoryShipmentOrderDetailBaseReturn : InventoryShipmentOrderBaseReturn
    {
        public string PurchaseOrderNumber { get; internal set; }
        public DateTime? DateOrderReceived { get; internal set; }
        public string OrderRequestedBy { get; internal set; }
        public string OrderTakenBy { get; internal set; }
        
        public IPickedInventoryDetailReturn PickedInventory { get; internal set; }
        public IShipmentDetailReturn Shipment { get; internal set; }
        public virtual InventoryOrderEnum InventoryOrderEnum { get { return InventoryOrderEnum.TransWarehouseMovements; } }
    }

    internal class InventoryShipmenOrderSummaryReturn : InventoryShipmentOrderBaseReturn, IInventoryShipmentOrderSummaryReturn
    {
        public IShipmentSummaryReturn Shipment { get; internal set; }
        public IInventoryPickOrderSummaryReturn PickOrder { get; internal set; }
        public IPickedInventorySummaryReturn PickedInventory { get; internal set; }
    }

    internal class InventoryShipmentOrderBaseReturn
    {
        public string MovementKey { get { return InventoryShipmentOrderKeyReturn.InventoryShipmentOrderKey; } }
        public OrderStatus OrderStatus { get; internal set; }
        public DateTime DateCreated { get; internal set; }
        public DateTime? ShipmentDate { get; internal set; }
        public int? MoveNum { get; internal set; }

        public IFacilitySummaryReturn OriginFacility { get; internal set; }
        public IFacilitySummaryReturn DestinationFacility { get; internal set; }
        internal InventoryShipmentOrderKeyReturn InventoryShipmentOrderKeyReturn { get; set; }
    }
}