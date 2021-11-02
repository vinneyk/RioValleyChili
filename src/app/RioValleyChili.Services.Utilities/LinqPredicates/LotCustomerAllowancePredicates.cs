using System;
using System.Linq.Expressions;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Utilities.LinqPredicates
{
    internal static class LotCustomerAllowancePredicates
    {
        internal static Expression<Func<LotCustomerAllowance, bool>> ByCustomerKey(ICustomerKey key)
        {
            return a => a.CustomerId == key.CustomerKey_Id;
        }
    }
}