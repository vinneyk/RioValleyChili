using System;
using System.Linq.Expressions;
using EF_Projectors;
using LinqKit;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class SalesQuoteItemProjectors
    {
        internal static Expression<Func<SalesQuoteItem, SalesQuoteItemKeyReturn>> SelectKey()
        {
            return i => new SalesQuoteItemKeyReturn
                {
                    SalesQuoteKey_DateCreated = i.DateCreated,
                    SalesQuoteKey_Sequence = i.Sequence,
                    SalesQuoteItemKey_ItemSequence = i.ItemSequence
                };
        }

        internal static Expression<Func<SalesQuoteItem, SalesQuoteItemReturn>> Select()
        {
            var key = SelectKey();
            var treatmentKey = InventoryTreatmentProjectors.SelectInventoryTreatmentKey();
            var product = ProductProjectors.SelectProduct();
            var packaging = ProductProjectors.SelectPackagingProduct();

            return Projector<SalesQuoteItem>.To(i => new SalesQuoteItemReturn
                {
                    SalesQuoteItemKeyReturn = key.Invoke(i),
                    Quantity = i.Quantity,
                    CustomerProductCode = i.CustomerProductCode,
                    PriceBase = i.PriceBase,
                    PriceFreight = i.PriceFreight,
                    PriceTreatment = i.PriceTreatment,
                    PriceWarehouse = i.PriceWarehouse,
                    PriceRebate = i.PriceRebate,
                    InventoryTreatmentKeyReturn = treatmentKey.Invoke(i.Treatment),
                    Product = product.Invoke(i.Product),
                    Packaging = packaging.Invoke(i.PackagingProduct)
                });
        }
    }
}