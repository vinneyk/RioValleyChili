using System;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Commands;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Conductors
{
    internal abstract class DeleteInventoryShipmentOrderConductorBase : PickedInventoryConductorBase<IInventoryShipmentOrderUnitOfWork>
    {
        protected DeleteInventoryShipmentOrderConductorBase(IInventoryShipmentOrderUnitOfWork unitOfWork) : base(unitOfWork) { }

        protected IResult DeleteInventoryShipmentOrder(InventoryShipmentOrder inventoryShipmentOrder)
        {
            if(inventoryShipmentOrder == null) { throw new ArgumentNullException("inventoryShipmentOrder"); }

            if(inventoryShipmentOrder.OrderStatus == OrderStatus.Fulfilled)
            {
                return new InvalidResult(UserMessages.CannotDeleteFulfilledShipmentOrder);
            }

            if(inventoryShipmentOrder.ShipmentInformation.Status == ShipmentStatus.Shipped)
            {
                return new InvalidResult(UserMessages.CannotDeleteShippedShipmentOrder);
            }

            var removePickedItems = UpdatePickedInventory(null, null, DateTime.UtcNow, inventoryShipmentOrder.PickedInventory, null);
            if(!removePickedItems.Success)
            {
                return removePickedItems;
            }

            var removePickOrderItems = new DeleteInventoryPickOrderItems(UnitOfWork).Execute(inventoryShipmentOrder.InventoryPickOrder);
            if(!removePickOrderItems.Success)
            {
                return removePickOrderItems;
            }

            UnitOfWork.PickedInventoryRepository.Remove(inventoryShipmentOrder.PickedInventory);
            UnitOfWork.InventoryShipmentOrderRepository.Remove(inventoryShipmentOrder);

            return new SuccessResult();
        }
    }
}