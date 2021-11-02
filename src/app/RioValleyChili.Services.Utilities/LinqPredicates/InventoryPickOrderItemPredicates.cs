using System;
using System.Linq.Expressions;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Utilities.LinqPredicates
{
    public static class InventoryPickOrderItemPredicates
    {
        public static Expression<Func<InventoryPickOrderItem, bool>> FilterByInventoryPickOrderKey(IInventoryPickOrderKey inventoryPickOrderKey)
        {
            return i => i.DateCreated == inventoryPickOrderKey.InventoryPickOrderKey_DateCreated &&
                        i.OrderSequence == inventoryPickOrderKey.InventoryPickOrderKey_Sequence;
        }
        
    }
}