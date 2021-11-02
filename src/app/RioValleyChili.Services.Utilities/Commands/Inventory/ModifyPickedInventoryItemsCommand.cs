using System;
using System.Collections.Generic;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using RioValleyChili.Services.Utilities.LinqPredicates;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.Inventory
{
    /// todo: Would be really sweet to update so that inventory modifications would not have to be sent in one batch. From what I can tell we would need to:
    /// - Search inventory pending deletion (should be inventory going negative) for possibility of quantity going back to positive.
    /// - Validate no negative quantities are being commited when saving the context, which is the main reason it is currently architected to work in one batch call (to catch those negative quantities).
    ///     - Could wrap a context save call around a process to check for negative quantities.
    ///     - *Maybe* the existing context save changes can have a check hooked into it? Will have to research.
    /// - If implemented, then it wouldn't be necessary to pass along inventory modification lists between providers/commands (look at changing picked items for production batches currently),
    ///     they would be able to call this command whenever/wherever.
    /// RI 2015-12-21

    internal class ModifyPickedInventoryItemsCommand
    {
        private readonly IPickedInventoryUnitOfWork _pickedInventoryUnitOfWork;
        private readonly EFUnitOfWorkHelper _unitOfWorkHelper;

        internal ModifyPickedInventoryItemsCommand(IPickedInventoryUnitOfWork pickedInventoryUnitOfWork)
        {
            if(pickedInventoryUnitOfWork == null) { throw new ArgumentNullException("pickedInventoryUnitOfWork"); }
            _pickedInventoryUnitOfWork = pickedInventoryUnitOfWork;
            _unitOfWorkHelper = new EFUnitOfWorkHelper(_pickedInventoryUnitOfWork);
        }

        internal IResult<List<PickedInventoryItem>> Execute(IPickedInventoryKey pickedInventoryKey, IEnumerable<ModifyPickedInventoryItemParameters> items)
        {
            if(pickedInventoryKey == null) { throw new ArgumentNullException("pickedInventoryKey"); }
            if(items == null) { throw new ArgumentNullException("items"); }

            var newItems = new List<PickedInventoryItem>();
            foreach(var item in items)
            {
                if(item.PickedInventoryItemKey != null)
                {
                    var notPendingResult = _unitOfWorkHelper.EntityHasNoPendingChanges(item.PickedInventoryItemKey, item.PickedInventoryItemKey);
                    if(!notPendingResult.Success)
                    {
                        return notPendingResult.ConvertTo<List<PickedInventoryItem>>();
                    }

                    var pickedItem = _pickedInventoryUnitOfWork.PickedInventoryItemRepository.FindByKey(item.PickedInventoryItemKey);
                    if(pickedItem == null)
                    {
                        return new InvalidResult<List<PickedInventoryItem>>(null, string.Format(UserMessages.PickedInventoryItemNotFound, item.PickedInventoryItemKey.KeyValue));
                    }

                    if(pickedItem.CurrentLocationId != pickedItem.FromLocationId)
                    {
                        return new InvalidResult<List<PickedInventoryItem>>(null, string.Format(UserMessages.PickedInventoryItemNotInOriginalLocation, new PickedInventoryItemKey(pickedItem).KeyValue));
                    }

                    pickedItem.Quantity = item.NewQuantity;
                    pickedItem.CurrentLocationId = item.CurrentLocationKey.LocationKey_Id;
                    pickedItem.CustomerLotCode = item.CustomerLotCode;
                    pickedItem.CustomerProductCode = item.CustomerProductCode;

                    if(pickedItem.Quantity < 0)
                    {
                        return new InvalidResult<List<PickedInventoryItem>>(null, string.Format(UserMessages.QuantityForPickedCannotBeNegative, item.PickedInventoryItemKey.KeyValue));
                    }

                    if(pickedItem.Quantity == 0)
                    {
                        _pickedInventoryUnitOfWork.PickedInventoryItemRepository.Remove(pickedItem);
                    }
                    else
                    {
                        newItems.Add(pickedItem);
                    }
                }
                else
                {
                    if(item.NewQuantity <= 0)
                    {
                        return new InvalidResult<List<PickedInventoryItem>>(null, string.Format(UserMessages.QuantityForPickingFromInventoryMustBeGreaterThanZero, item.InventoryKey.KeyValue));
                    }

                    var newSequence = _unitOfWorkHelper.GetNextSequence(PickedInventoryItemPredicates.FilterByPickedInventoryKey(pickedInventoryKey), i => i.ItemSequence);
                    var newPickedInventoryItem = new PickedInventoryItem
                        {
                            DateCreated = pickedInventoryKey.PickedInventoryKey_DateCreated,
                            Sequence = pickedInventoryKey.PickedInventoryKey_Sequence,
                            ItemSequence = newSequence,

                            LotDateCreated = item.InventoryKey.LotKey_DateCreated,
                            LotDateSequence = item.InventoryKey.LotKey_DateSequence,
                            LotTypeId = item.InventoryKey.LotKey_LotTypeId,
                            PackagingProductId = item.InventoryKey.PackagingProductKey_ProductId,
                            TreatmentId = item.InventoryKey.InventoryTreatmentKey_Id,
                            ToteKey = item.InventoryKey.InventoryKey_ToteKey,

                            FromLocationId = item.InventoryKey.LocationKey_Id,
                            CurrentLocationId = item.CurrentLocationKey.LocationKey_Id,
                            CustomerLotCode = item.CustomerLotCode,
                            CustomerProductCode = item.CustomerProductCode,

                            Quantity = item.NewQuantity
                        };
                    newItems.Add(_pickedInventoryUnitOfWork.PickedInventoryItemRepository.Add(newPickedInventoryItem));
                }
            }

            return new SuccessResult<List<PickedInventoryItem>>(newItems);
        }
    }
}