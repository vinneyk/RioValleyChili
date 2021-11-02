using System;
using System.Linq.Expressions;
using EF_Projectors.Extensions;
using LinqKit;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class LotProductionResultItemProjectors
    {
        internal static Expression<Func<LotProductionResultItem, LotProductionResultItemKeyReturn>> SelectProductionResultItemKey()
        {
            return LotProjectors.SelectLotKey<LotProductionResultItem>().Merge(i => new LotProductionResultItemKeyReturn
                {
                    ProductionResultItemKey_Sequence = i.ResultItemSequence
                });
        }

        internal static Expression<Func<LotProductionResultItem, ProductionResultItemReturn>> Select()
        {
            var key = SelectProductionResultItemKey();
            var packagingProduct = ProductProjectors.SelectPackagingProduct();
            var warehouseLocation = LocationProjectors.SelectLocation();
            var treatment = InventoryTreatmentProjectors.SelectInventoryTreatment();

            return i => new ProductionResultItemReturn
                {
                    LotProductionResultItemKeyReturn = key.Invoke(i),
                    PackagingProduct = packagingProduct.Invoke(i.PackagingProduct),
                    Location = warehouseLocation.Invoke(i.Location),
                    Treatment = treatment.Invoke(i.Treatment),
                    Quantity = i.Quantity
                };
        }

        internal static Expression<Func<LotProductionResultItem, MillAndWetdownResultItemReturn>> SelectMillAndWetdownResultItem()
        {
            var key = SelectProductionResultItemKey();
            var warehouseLocation = LocationProjectors.SelectLocation();
            var packagingProduct = ProductProjectors.SelectPackagingProduct();

            return r => new MillAndWetdownResultItemReturn
                {
                    ItemKeyReturn = key.Invoke(r),
                    PackagingProduct = packagingProduct.Invoke(r.PackagingProduct),
                    Location = warehouseLocation.Invoke(r.Location),

                    QuantityProduced = r.Quantity,
                    TotalWeightProduced = (int)(r.Quantity * r.PackagingProduct.Weight)
                };
        }
    }
}