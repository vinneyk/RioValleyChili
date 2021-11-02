using System;
using System.Linq.Expressions;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Utilities.LinqPredicates
{
    internal static class LotCustomerOrderAllowancePredicates
    {
        internal static Expression<Func<LotSalesOrderAllowance, bool>> ByCustomerOrderKey(ISalesOrderKey key)
        {
            return a => a.SalesOrderDateCreated == key.SalesOrderKey_DateCreated && a.SalesOrderSequence == key.SalesOrderKey_Sequence;
        }
    }
}