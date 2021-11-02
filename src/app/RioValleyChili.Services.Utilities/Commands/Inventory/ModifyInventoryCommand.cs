using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Attributes;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.Inventory
{
    internal class ModifyInventoryCommand
    {
        #region Fields and Constructors.

        private readonly IInventoryUnitOfWork _inventoryUnitOfWork;

        internal ModifyInventoryCommand(IInventoryUnitOfWork inventoryUnitOfWork)
        {
            if(inventoryUnitOfWork == null) { throw new ArgumentNullException("inventoryUnitOfWork"); }
            _inventoryUnitOfWork = inventoryUnitOfWork;
        }

        #endregion

        [Issue("Currently, logging InvenotryTransaction records is optional (transactionParameters may be null).",
            Todo = "Once free from Access, consider making InventoryTransaction logging *required* to increase the integrity of the system. - RI 2016-06-14",
            References = new [] { "RVCADMIN-1153"},
            Flags = IssueFlags.TodoWhenAccessFreedom)]
        internal IResult Execute(IEnumerable<ModifyInventoryParameters> inventoryModifications, InventoryTransactionParameters transactionParameters)
        {
            if(inventoryModifications == null) { throw new ArgumentNullException("inventoryModifications"); }

            var transactionCommand = new CreateInventoryTransactionCommand(_inventoryUnitOfWork);

            var groupedModifications = inventoryModifications.GroupBy(a => a.InventoryKey);
            var finalModifications = groupedModifications.Select(g => new ModifyInventoryParameters(g.Key, g.Sum(a => a.ModifyQuantity)));

            var resultingInventory = new List<Data.Models.Inventory>();
            foreach(var modification in finalModifications.Where(a => a.ModifyQuantity != 0))
            {
                var modifyResult = ModifyInventory(modification);
                if(!modifyResult.Success)
                {
                    return modifyResult;
                }

                resultingInventory.Add(modifyResult.ResultingObject);

                if(transactionParameters != null)
                {
                    var transactionResult = transactionCommand.Create(transactionParameters, modification.InventoryKey, modification.ModifyQuantity);
                    if(!transactionResult.Success)
                    {
                        return transactionResult;
                    }
                }
            }

            var negativeLots = resultingInventory
                .Where(i => i != null && i.Quantity < 0)
                .Select(i => new LotKey(i).KeyValue)
                .Distinct().ToList();
            if(negativeLots.Any())
            {
                return new InvalidResult(string.Format(UserMessages.NegativeInventoryLots, negativeLots.Aggregate((string)null, (c, n) => string.Format("{0}{1}", c == null ? "" : ", ", n))));
            }

            return new SuccessResult();
        }

        private IResult<Data.Models.Inventory> ModifyInventory(ModifyInventoryParameters input)
        {
            if(input == null) { throw new ArgumentNullException("input"); }

            Data.Models.Inventory inventory;
            EntityState? state;
            var notPendingResult = new EFUnitOfWorkHelper(_inventoryUnitOfWork).EntityHasNoPendingChanges(input.InventoryKey, input.InventoryKey, out inventory, out state);
            if(!notPendingResult.Success)
            {
                if(state != EntityState.Added && state != EntityState.Modified)
                {
                    return notPendingResult.ConvertTo<Data.Models.Inventory>();
                }
            }

            var addedOrModified = !notPendingResult.Success;
            if(inventory == null)
            {
                inventory = _inventoryUnitOfWork.InventoryRepository.FindByKey(input.InventoryKey);
                if(inventory == null)
                {
                    inventory = _inventoryUnitOfWork.InventoryRepository.Add(new Data.Models.Inventory
                        {
                            LotDateCreated = input.InventoryKey.LotKey_DateCreated,
                            LotDateSequence = input.InventoryKey.LotKey_DateSequence,
                            LotTypeId = input.InventoryKey.LotKey_LotTypeId,
                            PackagingProductId = input.InventoryKey.PackagingProductKey_ProductId,
                            LocationId = input.InventoryKey.LocationKey_Id,
                            TreatmentId = input.InventoryKey.InventoryTreatmentKey_Id,
                            ToteKey = input.InventoryKey.InventoryKey_ToteKey,
                            Quantity = 0
                        });
                    addedOrModified = true;
                }
            }
            
            inventory.Quantity += input.ModifyQuantity;
            if(inventory.Quantity == 0)
            {
                if(addedOrModified)
                {
                    return new InvalidResult<Data.Models.Inventory>(null, string.Format(UserMessages.QuantityForInventoryMustBeGreaterThanZero, input.InventoryKey));
                }

                _inventoryUnitOfWork.InventoryRepository.Remove(inventory);
                inventory = null;
            }

            return new SuccessResult<Data.Models.Inventory>(inventory);
        }
    }
}