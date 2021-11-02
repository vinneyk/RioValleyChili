using System;
using System.Linq.Expressions;
using LinqKit;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Utilities.LinqPredicates
{
    internal static class TreatmentOrderPredicates
    {
        internal static Expression<Func<TreatmentOrder, bool>> ByInventoryShipmentOrder(IInventoryShipmentOrderKey inventoryShipmentOrderKey)
        {
            var predicate = new InventoryShipmentOrderKey(inventoryShipmentOrderKey).FindByPredicate;
            Expression<Func<TreatmentOrder, bool>> expression = i => predicate.Invoke(i.InventoryShipmentOrder);
            return expression.Expand();
        }
    }
}