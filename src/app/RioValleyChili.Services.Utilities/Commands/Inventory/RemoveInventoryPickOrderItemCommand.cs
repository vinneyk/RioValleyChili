using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.Inventory
{
    internal class RemoveInventoryPickOrderItemCommand
    {
        private readonly IInventoryPickOrderUnitOfWork _inventoryPickOrderUnitOfWork;

        public RemoveInventoryPickOrderItemCommand(IInventoryPickOrderUnitOfWork inventoryPickOrderUnitOfWork)
        {
            if (inventoryPickOrderUnitOfWork == null) { throw new ArgumentNullException("inventoryPickOrderUnitOfWork"); }
            _inventoryPickOrderUnitOfWork = inventoryPickOrderUnitOfWork;
        }

        public IResult Execute(ISchedulePickOrderItemParameter item)
        {
            var key = new InventoryPickOrderItemKey(item);
            var orderItem = _inventoryPickOrderUnitOfWork.InventoryPickOrderItemRepository.FindByKey(key);

            if(orderItem == null)
            {
                return new InvalidResult(string.Format("Could not find Inventory Pick Order Item with key '{0}'.", key.KeyValue));
            }

            _inventoryPickOrderUnitOfWork.InventoryPickOrderItemRepository.Remove(orderItem);
            return new SuccessResult();
        }
    }
}