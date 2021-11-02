using System;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class InventoryPickOrderExtensions
    {
        internal static InventoryPickOrder EmptyItems(this InventoryPickOrder pickOrder)
        {
            if(pickOrder == null) { throw new ArgumentNullException("pickOrder"); }

            pickOrder.Items = null;

            return pickOrder;
        }
    }
}