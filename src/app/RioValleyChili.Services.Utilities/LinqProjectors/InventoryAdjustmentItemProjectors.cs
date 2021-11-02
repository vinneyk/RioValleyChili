// ReSharper disable ConvertClosureToMethodGroup

using System;
using System.Linq.Expressions;
using LinqKit;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class InventoryAdjustmentItemProjectors
    {
        internal static Expression<Func<InventoryAdjustmentItem, InventoryAdjustmentItemKeyReturn>> SelectKey()
        {
            return i => new InventoryAdjustmentItemKeyReturn
                {
                    InventoryAdjustmentKey_AdjustmentDate = i.AdjustmentDate,
                    InventoryAdjustmentKey_Sequence = i.Sequence,
                    InventoryAdjustmetItemKey_Sequence = i.ItemSequence
                };
        }

        internal static Expression<Func<InventoryAdjustmentItem, InventoryAdjustmentItemReturn>> Select(ILotUnitOfWork lotUnitOfWork)
        {
            var key = SelectKey();

            var inventoryProduct = LotProjectors.SelectDerivedProduct();
            var lotKey = LotProjectors.SelectLotKey<Lot>();
            var warehouseLocation = LocationProjectors.SelectLocation();
            var packagingProduct = ProductProjectors.SelectPackagingProduct();
            var treatment = InventoryTreatmentProjectors.SelectInventoryTreatment();

            return i => new InventoryAdjustmentItemReturn
                {
                    InventoryAdjustmentItemKeyReturn = key.Invoke(i),

                    AdjustmentQuantity = i.QuantityAdjustment,
                    InventoryProduct = inventoryProduct.Invoke(i.Lot),

                    LotKeyReturn = lotKey.Invoke(i.Lot),
                    Location = warehouseLocation.Invoke(i.Location),
                    PackagingProduct = packagingProduct.Invoke(i.PackagingProduct),
                    InventoryTreatment = treatment.Invoke(i.Treatment),
                    ToteKey = i.ToteKey
                };
        }
    }
}

// ReSharper restore ConvertClosureToMethodGroup