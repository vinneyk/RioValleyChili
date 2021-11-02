using System;
using System.Linq;
using System.Linq.Expressions;
using RioValleyChili.Core;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Returns.InventoryShipmentOrderService;
using RioValleyChili.Services.Utilities.Helpers;
using RioValleyChili.Services.Utilities.LinqPredicates;
using RioValleyChili.Services.Utilities.LinqProjectors;
using RioValleyChili.Services.Utilities.Models;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Conductors
{
    internal class PendingOrderDetailsConductor
    {
        private readonly IInventoryShipmentOrderUnitOfWork _inventoryShipmentOrderUnitOfWork;

        internal PendingOrderDetailsConductor(IInventoryShipmentOrderUnitOfWork inventoryShipmentOrderUnitOfWork)
        {
            if(inventoryShipmentOrderUnitOfWork == null) { throw new ArgumentNullException("inventoryShipmentOrderUnitOfWork"); }
            _inventoryShipmentOrderUnitOfWork = inventoryShipmentOrderUnitOfWork;
        }

        internal IResult<IPendingOrderDetails> Get(DateTime startDate, DateTime endDate)
        {
            startDate = startDate.ToSimpleDate();
            endDate = endDate.ToSimpleDate();

            if(startDate > endDate)
            {
                var tempDate = endDate;
                endDate = startDate;
                startDate = tempDate;
            }

            var newStartDate = endDate.AddDays(1);
            var newEndDate = endDate.AddDays(30);

            var byDateRange = InventoryShipmentOrderPredicates.ByShipmentDateRange(startDate, endDate);
            Expression<Func<InventoryShipmentOrder, bool>> customerOrder = o => o.OrderType == InventoryShipmentOrderTypeEnum.SalesOrder && o.OrderStatus != OrderStatus.Void;
            
            var scheduledAmount = GetOrderedWeight(_inventoryShipmentOrderUnitOfWork
                .InventoryShipmentOrderRepository
                .Filter(byDateRange.AndExpanded(customerOrder)));

            var shippedAmount = GetOrderedWeight(_inventoryShipmentOrderUnitOfWork
                .InventoryShipmentOrderRepository
                .Filter(byDateRange.AndExpanded(customerOrder).AndExpanded(o => o.ShipmentInformation.Status == ShipmentStatus.Shipped)));

            var remainingAmount = GetOrderedWeight(_inventoryShipmentOrderUnitOfWork
                .InventoryShipmentOrderRepository
                .Filter(byDateRange.AndExpanded(customerOrder).AndExpanded(o => o.ShipmentInformation.Status != ShipmentStatus.Shipped)));

            var newAmount = GetOrderedWeight(_inventoryShipmentOrderUnitOfWork
                .InventoryShipmentOrderRepository
                .Filter(InventoryShipmentOrderPredicates.ByShipmentDateRange(newStartDate, newEndDate).AndExpanded(customerOrder)));

            var pendingCustomerOrders = _inventoryShipmentOrderUnitOfWork
                .SalesOrderRepository
                .Filter(o => o.InventoryShipmentOrder.ShipmentInformation.Status != ShipmentStatus.Shipped && o.InventoryShipmentOrder.OrderStatus != OrderStatus.Void)
                .Select(SalesOrderProjectors.SelectPendingOrderDetails())
                .ToList();

            var pendingWarehouseOrders = _inventoryShipmentOrderUnitOfWork
                .InventoryShipmentOrderRepository
                .Filter(o => (o.OrderType == InventoryShipmentOrderTypeEnum.InterWarehouseOrder || o.OrderType == InventoryShipmentOrderTypeEnum.ConsignmentOrder) && o.ShipmentInformation.Status != ShipmentStatus.Shipped && o.OrderStatus != OrderStatus.Void)
                .Select(InventoryShipmentOrderProjectors.SelectPendingWarehouseOrder())
                .ToList();
            pendingWarehouseOrders.ForEach(o => o.Initialize());

            return new SuccessResult<IPendingOrderDetails>(new PendingOrderDetails
                {
                    Now = DateTime.Now.ToSimpleDate(),
                    StartDate = startDate,
                    EndDate = endDate,
                    NewStartDate = newStartDate,
                    NewEndDate = newEndDate,

                    ScheduledAmount = (int)scheduledAmount,
                    ShippedAmount = (int)shippedAmount,
                    RemainingAmount = (int)remainingAmount,
                    NewAmount = (int)newAmount,

                    PendingCustomerOrders = pendingCustomerOrders,
                    PendingWarehouseOrders = pendingWarehouseOrders
                });
        }

        private static double GetOrderedWeight(IQueryable<InventoryShipmentOrder> orders)
        {
            return orders.SelectMany(i => i.InventoryPickOrder.Items.Select(m => m.Quantity * m.PackagingProduct.Weight))
                         .DefaultIfEmpty(0)
                         .Sum();
        }
    }
}