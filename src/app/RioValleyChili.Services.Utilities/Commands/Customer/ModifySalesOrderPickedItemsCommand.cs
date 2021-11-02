using System;
using System.Collections.Generic;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using RioValleyChili.Services.Utilities.LinqPredicates;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.Customer
{
    //todo: Consider basing off of ModifyPickedInvenoryItemsCommand. - RI 2014/4/4
    internal class ModifySalesOrderPickedItemsCommand
    {
        private readonly ISalesUnitOfWork _salesUnitOfWork;

        internal ModifySalesOrderPickedItemsCommand(ISalesUnitOfWork salesUnitOfWork)
        {
            if(salesUnitOfWork == null) { throw new ArgumentNullException("salesUnitOfWork"); }
            _salesUnitOfWork = salesUnitOfWork;
        }

        internal IResult Execute(PickedInventoryKey pickedInventoryKey, List<ModifySalesOrderPickedInventoryItemParameters> items)
        {
            if(items == null) { throw new ArgumentNullException("items"); }

            foreach(var item in items)
            {
                if(item.SalesOrderPickedItemKey != null)
                {
                    var notPendingResult = new EFUnitOfWorkHelper(_salesUnitOfWork).EntityHasNoPendingChanges(item.SalesOrderPickedItemKey, item.SalesOrderPickedItemKey);
                    if(!notPendingResult.Success)
                    {
                        return notPendingResult;
                    }

                    var pickedItem = _salesUnitOfWork.SalesOrderPickedItemRepository.FindByKey(item.SalesOrderPickedItemKey);
                    if(pickedItem == null)
                    {
                        return new InvalidResult(string.Format(UserMessages.SalesOrderPickedItemNotFound, item.SalesOrderPickedItemKey.KeyValue));
                    }

                    if(pickedItem.PickedInventoryItem.CurrentLocationId != pickedItem.PickedInventoryItem.FromLocationId)
                    {
                        return new InvalidResult(string.Format(UserMessages.PickedInventoryItemNotInOriginalLocation, new PickedInventoryItemKey(pickedItem).KeyValue));
                    }

                    pickedItem.PickedInventoryItem.Quantity = item.NewQuantity;
                    pickedItem.OrderItemSequence = item.SalesOrderItemKey.SalesOrderItemKey_ItemSequence;

                    if(pickedItem.PickedInventoryItem.Quantity < 0)
                    {
                        return new InvalidResult(string.Format(UserMessages.QuantityForPickedCannotBeNegative, item.PickedInventoryItemKey.KeyValue));
                    }

                    if(pickedItem.PickedInventoryItem.Quantity == 0)
                    {
                        _salesUnitOfWork.PickedInventoryItemRepository.Remove(pickedItem.PickedInventoryItem);
                        _salesUnitOfWork.SalesOrderPickedItemRepository.Remove(pickedItem);
                    }
                    else
                    {
                        pickedItem.PickedInventoryItem.CustomerLotCode = item.CustomerLotCode;
                        pickedItem.PickedInventoryItem.CustomerProductCode = item.CustomerProductCode;
                    }
                }
                else
                {
                    if(item.NewQuantity <= 0)
                    {
                        return new InvalidResult(string.Format(UserMessages.QuantityForPickingFromInventoryMustBeGreaterThanZero, item.InventoryKey.KeyValue));
                    }

                    var newSequence = new EFUnitOfWorkHelper(_salesUnitOfWork).GetNextSequence(PickedInventoryItemPredicates.FilterByPickedInventoryKey(pickedInventoryKey), i => i.ItemSequence);
                    var newPickedInventoryItem = new PickedInventoryItem
                        {
                            DateCreated = pickedInventoryKey.PickedInventoryKey_DateCreated,
                            Sequence = pickedInventoryKey.PickedInventoryKey_Sequence,
                            ItemSequence = newSequence,

                            LotDateCreated = item.InventoryKey.LotKey_DateCreated,
                            LotDateSequence = item.InventoryKey.LotKey_DateSequence,
                            LotTypeId = item.InventoryKey.LotKey_LotTypeId,

                            PackagingProductId = item.InventoryKey.PackagingProductKey_ProductId,
                            FromLocationId = item.InventoryKey.LocationKey_Id,
                            TreatmentId = item.InventoryKey.InventoryTreatmentKey_Id,
                            CurrentLocationId = item.InventoryKey.LocationKey_Id,
                            ToteKey = item.InventoryKey.InventoryKey_ToteKey,

                            Quantity = item.NewQuantity,

                            CustomerLotCode = item.CustomerLotCode,
                            CustomerProductCode = item.CustomerProductCode
                        };
                    _salesUnitOfWork.PickedInventoryItemRepository.Add(newPickedInventoryItem);

                    var customerOrderPickedItem = new SalesOrderPickedItem
                        {
                            DateCreated = pickedInventoryKey.PickedInventoryKey_DateCreated,
                            Sequence = pickedInventoryKey.PickedInventoryKey_Sequence,
                            ItemSequence = newSequence,
                            OrderItemSequence = item.SalesOrderItemKey.SalesOrderItemKey_ItemSequence
                        };
                    _salesUnitOfWork.SalesOrderPickedItemRepository.Add(customerOrderPickedItem);
                }
            }

            return new SuccessResult();
        }
    }
}