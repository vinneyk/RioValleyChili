using System;
using System.Linq.Expressions;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Utilities.LinqPredicates
{
    internal static class CustomerProductAttributeRangePredicates
    {
        internal static Expression<Func<CustomerProductAttributeRange, bool>> ByCustomerProduct(ICustomerKey customerKey, IChileProductKey chileProductKey)
        {
            return r => r.CustomerId == customerKey.CustomerKey_Id && r.ChileProductId == chileProductKey.ChileProductKey_ProductId;
        }
    }
}