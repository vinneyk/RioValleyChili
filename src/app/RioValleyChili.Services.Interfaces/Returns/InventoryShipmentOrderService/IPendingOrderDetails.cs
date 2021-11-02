using System;
using System.Collections.Generic;
using RioValleyChili.Core;

namespace RioValleyChili.Services.Interfaces.Returns.InventoryShipmentOrderService
{
    public interface IPendingOrderDetails
    {
        DateTime Now { get; }
        DateTime StartDate { get; }
        DateTime EndDate { get; }
        DateTime NewStartDate { get; }
        DateTime NewEndDate { get; }

        int ScheduledAmount { get; }
        int ShippedAmount { get; }
        int RemainingAmount { get; }
        int NewAmount { get; }

        IEnumerable<IPendingCustomerOrderDetail> PendingCustomerOrders { get; }
        IEnumerable<IPendingWarehouseOrderDetail> PendingWarehouseOrders { get; }
    }

    public interface IPendingCustomerOrderDetail
    {
        string Name { get; }
        DateTime? DateRecd { get; }
        DateTime? ShipmentDate { get; }
        string OrderNum { get; }
        string Origin { get; }
        OrderStatus Status { get; }
        bool Sample { get; }

        IEnumerable<IPendingOrderItem> Items { get; }
    }

    public interface IPendingOrderItem
    {
        int QuantityPicked { get; }
        int QuantityOrdered { get; }
        string Packaging { get; }
        string Product { get; }
        string Treatment { get; }
        int LbsToShip { get; }
    }

    public interface IPendingWarehouseOrderDetail
    {
        string From { get; }
        string To { get; }
        DateTime? DateRecd { get; }
        DateTime? ShipmentDate { get; }
        string OrderNum { get; }
        OrderStatus Status { get; }

        IEnumerable<IPendingOrderItem> Items { get; }
    }
}