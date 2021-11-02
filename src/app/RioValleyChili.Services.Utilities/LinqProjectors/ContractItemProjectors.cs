// ReSharper disable ConvertClosureToMethodGroup
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EF_Projectors;
using LinqKit;
using RioValleyChili.Core;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class ContractItemProjectors
    {
        internal static Expression<Func<ContractItem, ContractItemKeyReturn>> SelectKey()
        {
            return i => new ContractItemKeyReturn
                {
                    ContractKey_Year = i.ContractYear,
                    ContractKey_Sequence = i.ContractSequence,
                    ContractItemKey_Sequence = i.ContractItemSequence
                };
        }

        internal static IEnumerable<Expression<Func<ContractItem, ContractItemReturn>>> Select()
        {
            var key = SelectKey();
            var product = ProductProjectors.SelectChileProductSummary();
            var packaging = ProductProjectors.SelectPackagingProduct();
            var treament = InventoryTreatmentProjectors.SelectInventoryTreatment();

            return new[]
                {
                    Projector<ContractItem>.To(i => new ContractItemReturn
                        {
                            ContractItemKeyReturn = key.Invoke(i),
                            Treatment = treament.Invoke(i.Treatment),

                            UseCustomerSpec = i.UseCustomerSpec,
                            CustomerProductCode = i.CustomerProductCode,
                            Quantity = i.Quantity,
                            PriceBase = i.PriceBase,
                            PriceFreight = i.PriceFreight,
                            PriceTreatment = i.PriceTreatment,
                            PriceWarehouse = i.PriceWarehouse,
                            PriceRebate = i.PriceRebate
                        }),
                    Projector<ContractItem>.To(i => new ContractItemReturn
                        {
                            ChileProduct = product.Invoke(i.ChileProduct),
                            PackagingProduct = packaging.Invoke(i.PackagingProduct),
                        })
                };
        }

        internal static IEnumerable<Expression<Func<ContractItem, ContractItemShipmentSummaryReturn>>> SplitSelectShipmentSummary()
        {
            var key = SelectKey();
            var product = ProductProjectors.SelectProduct();
            var packaging = ProductProjectors.SelectPackagingProduct();
            var treatment = InventoryTreatmentProjectors.SelectInventoryTreatment();
            var total = SelectTotal();
            var totalShipped = SelectTotalOrdered(o => o.Order.InventoryShipmentOrder.OrderStatus != OrderStatus.Void && (o.Order.InventoryShipmentOrder.ShipmentInformation.Status == ShipmentStatus.Shipped));
            var totalPending = SelectTotalOrdered(o => o.Order.InventoryShipmentOrder.OrderStatus != OrderStatus.Void && (o.Order.InventoryShipmentOrder.ShipmentInformation.Status == ShipmentStatus.Unscheduled || o.Order.InventoryShipmentOrder.ShipmentInformation.Status == ShipmentStatus.Scheduled));

            return new List<Expression<Func<ContractItem, ContractItemShipmentSummaryReturn>>>
                {
                    c => new ContractItemShipmentSummaryReturn
                        {
                            ContractItemKeyReturn = key.Invoke(c),
                            BasePrice = c.PriceBase,
                            ChileProduct = product.Invoke(c.ChileProduct.Product),
                            PackagingProduct = packaging.Invoke(c.PackagingProduct),
                            Treatment = treatment.Invoke(c.Treatment),
                            CustomerProductCode = c.CustomerProductCode,
                        },
                    c => new ContractItemShipmentSummaryReturn
                        {
                            TotalValue = total.Invoke(c) * (c.PriceBase + c.PriceFreight + c.PriceTreatment + c.PriceWarehouse - c.PriceRebate),
                            TotalWeight = total.Invoke(c),
                            TotalWeightShipped = totalShipped.Invoke(c),
                            TotalWeightPending = totalPending.Invoke(c),
                            TotalWeightRemaining =  total.Invoke(c) - (totalShipped.Invoke(c) + totalPending.Invoke(c))
                        }
                };
        }

        private static Expression<Func<ContractItem, int>> SelectTotalOrdered(Expression<Func<SalesOrderItem, bool>> orderItemFilter)
        {
            return c => (int) (!c.OrderItems.Any(o => orderItemFilter.Invoke(o)) ? 0 : c.OrderItems.Where(o => orderItemFilter.Invoke(o))
                                .Sum(o => !o.PickedItems.Any() ? 0 : o.PickedItems.Sum(p => p.PickedInventoryItem.PackagingProduct.Weight * p.PickedInventoryItem.Quantity)));
        }

        private static Expression<Func<ContractItem, int>> SelectTotal()
        {
            return c => (int) (c.PackagingProduct.Weight * c.Quantity);
        }
    }
}
// ReSharper restore ConvertClosureToMethodGroup