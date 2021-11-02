using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Enumerations;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.Inventory
{
    internal class InventoryPickOrderCommand
    {
        private readonly IInventoryPickOrderUnitOfWork _inventoryPickOrderUnitOfWork;

        public InventoryPickOrderCommand(IInventoryPickOrderUnitOfWork inventoryPickOrderUnitOfWork)
        {
            if(inventoryPickOrderUnitOfWork == null) { throw new ArgumentNullException("inventoryPickOrderUnitOfWork"); }
            _inventoryPickOrderUnitOfWork = inventoryPickOrderUnitOfWork;
        }

        public IResult Execute(InventoryPickOrder currentPickOrder, List<ISchedulePickOrderItemParameter> newItems)
        {
            if(currentPickOrder == null) { throw new ArgumentNullException("currentPickOrder"); }
            if(newItems == null) { throw new ArgumentNullException("newItems"); }

            var currentItems = currentPickOrder.Items != null ? currentPickOrder.Items.ToList() : new List<InventoryPickOrderItem>();
            var scheduledItems = ScheduleInventoryPickOrderItemsHelper.ScheduleStatus(new InventoryPickOrderKey(currentPickOrder), currentItems, newItems);

            var removeCommand = new RemoveInventoryPickOrderItemCommand(_inventoryPickOrderUnitOfWork);
            var updateCommand = new UpdateInventoryPickOrderItemCommand(_inventoryPickOrderUnitOfWork);
            var createCommand = new CreateInventoryPickOrderItemCommand(_inventoryPickOrderUnitOfWork);

            foreach(var item in scheduledItems.Where(i => i.Status == ScheduledStatus.Remove))
            {
                var removeResult = removeCommand.Execute(item);
                if(!removeResult.Success)
                {
                    return removeResult;
                }
            }

            foreach(var item in scheduledItems.Where(i => i.Status == ScheduledStatus.Update))
            {
                var updateResult = updateCommand.Execute(item);
                if(!updateResult.Success)
                {
                    return updateResult;
                }
            }

            foreach(var item in scheduledItems.Where(i => i.Status == ScheduledStatus.Create))
            {
                var createResult = createCommand.Execute(item);
                if(!createResult.Success)
                {
                    return createResult;
                }
            }

            return new SuccessResult();
        }
    }
}