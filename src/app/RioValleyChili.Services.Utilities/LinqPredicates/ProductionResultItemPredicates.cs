using System;
using System.Linq.Expressions;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Utilities.LinqPredicates
{
    public static class ProductionResultItemPredicates
    {
        public static Expression<Func<LotProductionResultItem, bool>> FilterByInventoryKey(IInventoryKey inventoryKey)
        {
            return i => i.LotDateCreated == inventoryKey.LotKey_DateCreated &&
                        i.LotDateSequence == inventoryKey.LotKey_DateSequence &&
                        i.LotTypeId == inventoryKey.LotKey_LotTypeId &&
                        i.LocationId == inventoryKey.LocationKey_Id &&
                        i.PackagingProductId == inventoryKey.PackagingProductKey_ProductId &&
                        i.TreatmentId == inventoryKey.InventoryTreatmentKey_Id;
        }

        public static Expression<Func<LotProductionResultItem, bool>> FilterByLotKey(ILotKey lotKey)
        {
            return i => i.LotDateCreated == lotKey.LotKey_DateCreated &&
                        i.LotDateSequence == lotKey.LotKey_DateSequence &&
                        i.LotTypeId == lotKey.LotKey_LotTypeId;
        }
    }
}