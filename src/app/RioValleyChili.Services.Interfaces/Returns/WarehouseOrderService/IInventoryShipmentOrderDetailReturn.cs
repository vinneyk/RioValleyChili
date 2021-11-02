using System;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent;
using RioValleyChili.Services.Interfaces.Returns.WarehouseService;

namespace RioValleyChili.Services.Interfaces.Returns.WarehouseOrderService
{
    public interface IInventoryShipmentOrderDetailReturn<out TPickOrder, out TPickOrderItem> : IInventoryOrderDetailReturn<TPickOrder, TPickOrderItem>
        where TPickOrder : IPickOrderDetailReturn<TPickOrderItem>
        where TPickOrderItem : IPickOrderItemReturn
    {
        string PurchaseOrderNumber { get; }
        DateTime? DateOrderReceived { get; }
        string OrderRequestedBy { get; }
        string OrderTakenBy { get; }
        OrderStatus OrderStatus { get; }
        int? MoveNum { get; }

        IFacilitySummaryReturn OriginFacility { get; }
        IFacilitySummaryReturn DestinationFacility { get; }
        IShipmentDetailReturn Shipment { get; }
    }
}