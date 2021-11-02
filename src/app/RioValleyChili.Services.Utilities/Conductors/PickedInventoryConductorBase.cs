using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Core.Attributes;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Commands.Inventory;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Conductors
{
    internal abstract class PickedInventoryConductorBase<TUnitOfWork>
        where TUnitOfWork : class, IPickedInventoryUnitOfWork
    {
        protected readonly TUnitOfWork UnitOfWork;

        protected PickedInventoryConductorBase(TUnitOfWork unitOfWork)
        {
            if(unitOfWork == null) { throw new ArgumentNullException("unitOfWork"); }
            UnitOfWork = unitOfWork;
        }

        [Issue("InventoryTransaction logs are actually seeded from tblIncoming/tblOutgoing in Access, and those only get created once results are entered." +
            "We're mirroring that process in the new context, so transactions are not getting logged here. Incidentally, this means that in order to get a proper" +
            "lot history, we need to return \"virtual\" transactions for batches that have not had results entered yet. -RI 2016-06-14",
            Todo = "Once free from Access, modify to log InventoryTransactions when picking for a batch, and ensure entering results does not duplicate transaction records (still need to log transaction if results modify picked items). - RI 2016-06-14",
            References = new[]{ "RVCADMIN-1153"},
            Flags = IssueFlags.TodoWhenAccessFreedom)]
        protected IResult UpdatePickedInventory(IInventoryValidator inventoryValidator, Employee employee, DateTime timeStamp, PickedInventory existingPickedInventory, List<PickedInventoryParameters> setPickedInventoryItems, bool moveToDestination = false)
        {
            List<ModifyInventoryParameters> inventoryModifications = null;
            var updateResult = UpdatePickedInventory(inventoryValidator, employee, timeStamp, existingPickedInventory, setPickedInventoryItems, ref inventoryModifications, moveToDestination);
            if(!updateResult.Success)
            {
                return updateResult.ConvertTo<List<PickedInventoryItem>>();
            }
            
            var modifyInventoryResult = new ModifyInventoryCommand(UnitOfWork).Execute(inventoryModifications, null);
            if(!modifyInventoryResult.Success)
            {
                return modifyInventoryResult.ConvertTo<List<PickedInventoryItem>>();
            }

            return updateResult;
        }

        protected IResult UpdatePickedInventory(IInventoryValidator inventoryValidator, Employee employee, DateTime timeStamp, PickedInventory existingPickedInventory, List<PickedInventoryParameters> setPickedInventoryItems, ref List<ModifyInventoryParameters> pendingInventoryModifications, bool moveToDestination = false)
        {
            if(employee != null)
            {
                existingPickedInventory.EmployeeId = employee.EmployeeId;
            }
            existingPickedInventory.TimeStamp = timeStamp;

            var pickedInventoryModifications = PickedInventoryHelper.CreateModifyPickedInventoryItemParameters(existingPickedInventory, setPickedInventoryItems);
            if(inventoryValidator != null)
            {
                var validatorResult = inventoryValidator.ValidateItems(UnitOfWork.InventoryRepository, pickedInventoryModifications.Where(i => i.DeltaQuantity > 0).Select(i => i.InventoryKey));
                if(!validatorResult.Success)
                {
                    return validatorResult.ConvertTo<List<PickedInventoryItem>>();
                }
            }

            var modifyPickedResult = new ModifyPickedInventoryItemsCommand(UnitOfWork).Execute(existingPickedInventory, pickedInventoryModifications);
            if(!modifyPickedResult.Success)
            {
                return modifyPickedResult;
            }

            pendingInventoryModifications = pendingInventoryModifications ?? new List<ModifyInventoryParameters>();
            pendingInventoryModifications.AddRange(pickedInventoryModifications.Select(p => p.ToModifySourceInventoryParameters()));
            if(moveToDestination)
            {
                pendingInventoryModifications.AddRange(pickedInventoryModifications.Select(p => p.ToModifyDestinationInventoryParameters()));
            }

            return modifyPickedResult;
        }
    }
}
