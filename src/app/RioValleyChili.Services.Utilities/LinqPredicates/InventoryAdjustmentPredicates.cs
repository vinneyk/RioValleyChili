using System;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Data;

namespace RioValleyChili.Services.Utilities.LinqPredicates
{
    internal static class InventoryAdjustmentPredicates
    {
        internal static Expression<Func<InventoryAdjustment, bool>> ByAdjustmentDate(DateTime adjustmentDate)
        {
            var date = adjustmentDate.Date;
            return a => a.AdjustmentDate == date;
        }

        internal static Expression<Func<InventoryAdjustment, bool>> ByLotKey(IKey<Lot> lotKey)
        {
            var lotKeyPredicate = lotKey.FindByPredicate;

            return a => a.Items.Any(i => lotKeyPredicate.Invoke(i.Lot));
        }

        internal static Expression<Func<InventoryAdjustment, bool>> ByTimeStampRange(DateTime? rangeStart, DateTime? rangeEnd)
        {
            var dateInRange = PredicateHelper.DateTimeInRange(rangeStart, rangeEnd);

            return a => dateInRange.Invoke(a.AdjustmentDate);
        }
    }
}