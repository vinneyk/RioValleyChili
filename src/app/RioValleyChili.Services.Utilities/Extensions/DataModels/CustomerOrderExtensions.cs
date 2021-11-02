using System;
using RioValleyChili.Core.Models;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Utilities.Extensions.DataModels
{
    internal static class CustomerOrderExtensions
    {
        internal static void SetSoldTo(this SalesOrder salesOrder, ShippingLabel shippingLabel)
        {
            if(salesOrder == null) { throw new ArgumentNullException("salesOrder"); }

            if(shippingLabel == null)
            {
                salesOrder.SoldTo = new ShippingLabel();
            }
            else
            {
                salesOrder.SoldTo.SetShippingLabel(shippingLabel);
            }
        }
    }
}