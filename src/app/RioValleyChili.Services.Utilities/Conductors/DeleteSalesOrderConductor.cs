using System;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Conductors
{
    internal class DeleteSalesOrderConductor : PickedInventoryConductorBase<IInventoryShipmentOrderUnitOfWork>
    {
        internal DeleteSalesOrderConductor(IInventoryShipmentOrderUnitOfWork inventoryShipmentOrderUnitOfWork)
            : base(inventoryShipmentOrderUnitOfWork) { }

        internal IResult Execute(ISalesOrderKey key, out int? orderNum)
        {
            orderNum = null;
            var orderKey = key.ToSalesOrderKey();
            var order = UnitOfWork.SalesOrderRepository.FindByKey(orderKey,
                c => c.InventoryShipmentOrder.ShipmentInformation,
                c => c.LotAllowances,
                c => c.SalesOrderItems,
                c => c.SalesOrderPickedItems,
                c => c.InventoryShipmentOrder.PickedInventory.Items.Select(i => i.CurrentLocation));
            if(order == null)
            {
                return new InvalidResult(string.Format(UserMessages.SalesOrderNotFound, orderKey));
            }

            if(order.InventoryShipmentOrder.ShipmentInformation.Status == ShipmentStatus.Shipped)
            {
                return new InvalidResult(string.Format(UserMessages.CannotDeleteShippedShipmentOrder));
            }

            var removePickedItemsResult = UpdatePickedInventory(null, null, DateTime.UtcNow, order.InventoryShipmentOrder.PickedInventory, null);
            if(!removePickedItemsResult.Success)
            {
                return removePickedItemsResult;
            }

            orderNum = order.InventoryShipmentOrder.MoveNum;

            UnitOfWork.SalesOrderRepository.Remove(order);
            return new SuccessResult();
        }
    }
}