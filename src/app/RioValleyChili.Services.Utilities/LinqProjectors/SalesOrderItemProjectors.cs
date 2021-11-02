// ReSharper disable ConstantNullCoalescingCondition
// ReSharper disable RedundantCast

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EF_Projectors;
using EF_Projectors.Extensions;
using LinqKit;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class SalesOrderItemProjectors
    {
        internal static Expression<Func<SalesOrderItem, SalesOrderItemKeyReturn>> SelectKey()
        {
            return i => new SalesOrderItemKeyReturn
                {
                    SalesOrderKey_DateCreated = i.DateCreated,
                    SalesOrderKey_Sequence = i.Sequence,
                    SalesOrderItemKey_ItemSequence = i.ItemSequence
                };
        }

        internal static Expression<Func<SalesOrderItem, SalesOrderItemInternalAcknowledgement>> SelectInternalAcknoweldgement()
        {
            var key = SelectKey();
            var contractKey = ContractProjectors.SelectKey();

            return Projector<SalesOrderItem>.To(i => new SalesOrderItemInternalAcknowledgement
                {
                    OrderItemKeyReturn = key.Invoke(i),
                    ContractKeyReturn = new[] { i.ContractItem }.Where(n => n != null).Select(n => contractKey.Invoke(n.Contract)).FirstOrDefault(),
                    ContractId = new[] { i.ContractItem }.Where(n => n != null).Select(n => n.Contract.ContractId).FirstOrDefault(),
                    TotalPrice = i.PriceBase + i.PriceFreight + i.PriceTreatment + i.PriceWarehouse - i.PriceRebate
                });
        }

        internal static IEnumerable<Expression<Func<SalesOrderItem, SalesOrderItemReturn>>> SplitSelect()
        {
            var key = SelectKey();
            var contractItemKey = SelectContractItemKey();

            return new Projectors<SalesOrderItem, SalesOrderItemReturn>
                {
                    { InventoryPickOrderItemProjectors.SplitSelect(), p => p.Translate().To<SalesOrderItem, SalesOrderItemReturn>(i => i.InventoryPickOrderItem) },
                   i => new SalesOrderItemReturn
                       {
                           SalesOrderItemKeyReturn = key.Invoke(i),
                           ContractItemKeyReturn = contractItemKey.Invoke(i),
                           
                           PriceBase = i.PriceBase,
                           PriceFreight = i.PriceFreight,
                           PriceTreatment = i.PriceTreatment,
                           PriceWarehouse = i.PriceWarehouse,
                           PriceRebate = i.PriceRebate
                       }
                };
        }

        internal static Expression<Func<SalesOrderItem, CustomerContractOrderItemReturn>> SelectContractOrderItem()
        {
            var contractOrderItem = SelectContractOrderItemFromPicked();

            return i => contractOrderItem.Invoke(i, i.Order.SalesOrderPickedItems.Where(p => p.OrderItemSequence == i.ItemSequence));
        }

        internal static Expression<Func<SalesOrderItem, PendingOrderItem>> SelectPending()
        {
            return InventoryPickOrderItemProjectors.SelectPending()
                .Merge(Projector<SalesOrderItem>.To(i => new PendingOrderItem
                    {
                        QuantityPicked = i.PickedItems.Select(p => p.PickedInventoryItem.Quantity).DefaultIfEmpty(0).Sum()
                    }), i => i.InventoryPickOrderItem);
        }

        private static Expression<Func<SalesOrderItem, NullableContractItemKeyReturn>> SelectContractItemKey()
        {
            return i => new NullableContractItemKeyReturn
                {
                    ContractKey_Year = i.ContractYear,
                    ContractKey_Sequence = i.ContractSequence,
                    ContractItemKey_Sequence = i.ContractItemSequence
                };
        }

        private static Expression<Func<SalesOrderItem, IEnumerable<SalesOrderPickedItem>, CustomerContractOrderItemReturn>> SelectContractOrderItemFromPicked()
        {
            var key = SelectKey();
            var contractItemKey = SelectContractItemKey();

            var product = ProductProjectors.SelectProduct();
            var packaging = ProductProjectors.SelectPackagingProduct();
            var treatment = InventoryTreatmentProjectors.SelectInventoryTreatment();

            return (orderItem, pickedItems) => new CustomerContractOrderItemReturn
                {
                    SalesOrderItemKeyReturn = key.Invoke(orderItem),
                    ContractItemKeyReturn = contractItemKey.Invoke(orderItem),

                    Product = product.Invoke(orderItem.InventoryPickOrderItem.Product),
                    Packaging = packaging.Invoke(orderItem.InventoryPickOrderItem.PackagingProduct),
                    Treatment = treatment.Invoke(orderItem.InventoryPickOrderItem.InventoryTreatment),

                    TotalQuantityPicked = (int) (((int?) pickedItems.Sum(p => p.PickedInventoryItem.Quantity)) ?? 0),
                    TotalWeightPicked = (int) (((int?) pickedItems.Sum(p => p.PickedInventoryItem.Quantity * p.PickedInventoryItem.PackagingProduct.Weight)) ?? 0),
                    TotalPrice = orderItem.PriceBase + orderItem.PriceFreight + orderItem.PriceTreatment + orderItem.PriceWarehouse - orderItem.PriceRebate
                };
        }
    }
}

// ReSharper restore RedundantCast
// ReSharper restore ConstantNullCoalescingCondition