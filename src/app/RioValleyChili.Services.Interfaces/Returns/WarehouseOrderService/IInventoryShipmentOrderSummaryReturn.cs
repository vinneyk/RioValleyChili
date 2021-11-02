using System;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent;
using RioValleyChili.Services.Interfaces.Returns.WarehouseService;

namespace RioValleyChili.Services.Interfaces.Returns.WarehouseOrderService
{
    public interface IInventoryShipmentOrderSummaryReturn : IInventoryOrderSummaryReturn
    {
        OrderStatus OrderStatus { get; }
        DateTime? ShipmentDate { get; }
        int? MoveNum { get; }
        IFacilitySummaryReturn OriginFacility { get; }
        IFacilitySummaryReturn DestinationFacility { get; }
        IShipmentSummaryReturn Shipment { get; }
    }
}