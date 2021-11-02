using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.Customer
{
    internal class SetSalesOrderItemsCommand
    {
        private readonly ISalesUnitOfWork _salesUnitOfWork;

        internal SetSalesOrderItemsCommand(ISalesUnitOfWork salesUnitOfWork)
        {
            if(salesUnitOfWork == null) { throw new ArgumentNullException("salesUnitOfWork"); }
            _salesUnitOfWork = salesUnitOfWork;
        }

        internal IResult Execute(SalesOrder order, List<SetSalesOrderItemParameters> newItems)
        {
            if(order == null) { throw new ArgumentNullException("order"); }
            if(newItems == null) { throw new ArgumentNullException("newItems"); }

            var unitOfWorkHelper = new EFUnitOfWorkHelper(_salesUnitOfWork);

            var matches = (order.SalesOrderItems ?? new SalesOrderItem[0]).BestMatches(newItems,
                (c, n) => n.ContractItemKey == null ? c.ContractItem == null : n.ContractItemKey.Equals(c.ContractItem),
                (c, n) => n.ProductKey.Equals(c.InventoryPickOrderItem),
                (c, n) => n.PackagingProductKey.Equals(c.InventoryPickOrderItem),
                (c, n) => n.InventoryTreatmentKey.Equals(c.InventoryPickOrderItem),
                (c, n) => n.SalesOrderItem.Quantity == c.InventoryPickOrderItem.Quantity,
                (c, n) => n.SalesOrderItem.CustomerLotCode == c.InventoryPickOrderItem.CustomerLotCode,
                (c, n) => n.SalesOrderItem.CustomerProductCode == c.InventoryPickOrderItem.CustomerProductCode,
                (c, n) => n.SalesOrderItem.PriceBase == c.PriceBase,
                (c, n) => n.SalesOrderItem.PriceFreight == c.PriceFreight,
                (c, n) => n.SalesOrderItem.PriceTreatment == c.PriceTreatment,
                (c, n) => n.SalesOrderItem.PriceWarehouse == c.PriceWarehouse,
                (c, n) => n.SalesOrderItem.PriceRebate == c.PriceRebate);

            foreach(var match in matches)
            {
                if(match.Item1 != null && match.Item2 != null)
                {
                    SetOrderItem(match.Item1, match.Item2);
                }
                else if(match.Item1 != null)
                {
                    if(match.Item1.PickedItems.Any())
                    {
                        return new InvalidResult(UserMessages.CannotRemovePickedCustomerOrderItem);
                    }
                    _salesUnitOfWork.InventoryPickOrderItemRepository.Remove(match.Item1.InventoryPickOrderItem);
                    _salesUnitOfWork.SalesOrderItemRepository.Remove(match.Item1);
                }
                else if(match.Item2 != null)
                {
                    var nextItemSequence = unitOfWorkHelper.GetNextSequence<SalesOrderItem>(i => i.DateCreated == order.DateCreated && i.Sequence == order.Sequence, i => i.ItemSequence);
                    SetOrderItem(_salesUnitOfWork.SalesOrderItemRepository.Add(new SalesOrderItem
                    {
                        DateCreated = order.DateCreated,
                        Sequence = order.Sequence,
                        ItemSequence = nextItemSequence,

                        InventoryPickOrderItem = new InventoryPickOrderItem
                            {
                                DateCreated = order.DateCreated,
                                OrderSequence = order.Sequence,
                                ItemSequence = nextItemSequence
                            }
                    }), match.Item2);
                }
            }

            return new SuccessResult();
        }

        private static void SetOrderItem(SalesOrderItem orderItem, SetSalesOrderItemParameters newItem)
        {
            var contractItemKey = newItem.ContractItemKey;
            orderItem.ContractYear = contractItemKey == null ? (int?) null : contractItemKey.ContractKey_Year;
            orderItem.ContractSequence = contractItemKey == null ? (int?) null : contractItemKey.ContractKey_Sequence;
            orderItem.ContractItemSequence = contractItemKey == null ? (int?) null : contractItemKey.ContractItemKey_Sequence;

            orderItem.InventoryPickOrderItem.ProductId = newItem.ProductKey.ProductKey_ProductId;
            orderItem.InventoryPickOrderItem.PackagingProductId = newItem.PackagingProductKey.PackagingProductKey_ProductId;
            orderItem.InventoryPickOrderItem.TreatmentId = newItem.InventoryTreatmentKey.InventoryTreatmentKey_Id;
            orderItem.InventoryPickOrderItem.Quantity = newItem.SalesOrderItem.Quantity;

            orderItem.InventoryPickOrderItem.CustomerLotCode = newItem.SalesOrderItem.CustomerLotCode;
            orderItem.InventoryPickOrderItem.CustomerProductCode = newItem.SalesOrderItem.CustomerProductCode;
            orderItem.PriceBase = newItem.SalesOrderItem.PriceBase;
            orderItem.PriceFreight = newItem.SalesOrderItem.PriceFreight;
            orderItem.PriceTreatment = newItem.SalesOrderItem.PriceTreatment;
            orderItem.PriceWarehouse = newItem.SalesOrderItem.PriceWarehouse;
            orderItem.PriceRebate = newItem.SalesOrderItem.PriceRebate;

            if(orderItem.PickedItems != null)
            {
                foreach(var pickedItem in orderItem.PickedItems)
                {
                    pickedItem.PickedInventoryItem.CustomerLotCode = orderItem.InventoryPickOrderItem.CustomerLotCode;
                    pickedItem.PickedInventoryItem.CustomerProductCode = orderItem.InventoryPickOrderItem.CustomerProductCode;
                }
            }
        }
    }
}