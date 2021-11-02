using System;
using System.Linq.Expressions;
using LinqKit;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Helpers;

namespace RioValleyChili.Services.Utilities.LinqPredicates
{
    internal static class ProductionResultPredicates
    {
        internal static Expression<Func<LotProductionResults, bool?>> FilterByProductionBegin(DateTime? rangeStart, DateTime? rangeEnd)
        {
            var dateInRange = PredicateHelper.DateInRange(rangeStart, rangeEnd);
            return r => dateInRange.Invoke(r.ProductionBegin);
        }
    }
}