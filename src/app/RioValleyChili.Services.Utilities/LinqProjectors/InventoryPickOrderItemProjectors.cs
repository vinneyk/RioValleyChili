// ReSharper disable ConvertClosureToMethodGroup

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
    internal static class InventoryPickOrderItemProjectors
    {
        internal static Expression<Func<InventoryPickOrderItem, InventoryPickOrderItemKeyReturn>> SelectKey()
        {
            return i => new InventoryPickOrderItemKeyReturn
                {
                    InventoryPickOrderKey_DateCreated = i.DateCreated,
                    InventoryPickOrderKey_Sequence = i.OrderSequence,
                    InventoryPickOrderItemKey_Sequence = i.ItemSequence
                };
        }

        internal static IEnumerable<Expression<Func<InventoryPickOrderItem, InventoryPickOrderItemReturn>>> SplitSelect()
        {
            var key = SelectKey();
            var productKey = ProductProjectors.SelectProductKey();
            var inventoryTreatmentKey = InventoryTreatmentProjectors.SelectInventoryTreatmentKey();
            var company = CompanyProjectors.SelectHeader();

            return new Projectors<InventoryPickOrderItem, InventoryPickOrderItemReturn>
                {
                    i => new InventoryPickOrderItemReturn
                        {
                            InventoryPickOrderItemKeyReturn = key.Invoke(i),
                            ProductKeyReturn = productKey.Invoke(i.Product),
                            ProductName = i.Product.Name,
                            ProductCode = i.Product.ProductCode,
                            TreatmentNameShort = i.InventoryTreatment.ShortName,
                            InventoryTreatmentKeyReturn = inventoryTreatmentKey.Invoke(i.InventoryTreatment),
                            Quantity = i.Quantity,
                            CustomerLotCode = i.CustomerLotCode,
                            CustomerProductCode = i.CustomerProductCode
                        },
                    i => new InventoryPickOrderItemReturn
                        {
                            PackagingProductKeyReturn = productKey.Invoke(i.PackagingProduct.Product),
                            PackagingName = i.PackagingProduct.Product.Name,
                            PackagingWeight = i.PackagingProduct.Weight,
                            TotalWeight = i.PackagingProduct.Weight * i.Quantity
                        },
                    i => new InventoryPickOrderItemReturn
                        {
                            Customer = new [] { i.Customer }.Where(c => c != null).Select(c => company.Invoke(c.Company)).FirstOrDefault()
                        }
                };
        }

        internal static Expression<Func<InventoryPickOrderItem, PendingOrderItem>> SelectPending()
        {
            var packaging = ProductProjectors.SelectPackagingProduct();

            return Projector<InventoryPickOrderItem>.To(i => new PendingOrderItem
            {
                QuantityOrdered = i.Quantity,
                PackagingProduct = packaging.Invoke(i.PackagingProduct),
                Product = i.Product.Name,
                Treatment = i.InventoryTreatment.ShortName
            });
        }

        internal static Expression<Func<InventoryPickOrderItem, PendingPickOrderItem>> SelectPickPending()
        {
            return SelectPending()
                .Merge(Projector<InventoryPickOrderItem>.To(i => new PendingPickOrderItem
                    {
                        DataModel = i
                    }));
        }
    }
}

// ReSharper restore ConvertClosureToMethodGroup