using System;
using System.Linq.Expressions;
using LinqKit;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class ChileTypeProjectors
    {
        internal static Expression<Func<ChileType, ChileTypeKeyReturn>> SelectKey()
        {
            return c => new ChileTypeKeyReturn
                {
                    ChileTypeKey_ChileTypeId = c.Id
                };
        }

        internal static Expression<Func<ChileType, ChileTypeSummaryReturn>> SelectSummary()
        {
            var key = SelectKey();

            return c => new ChileTypeSummaryReturn
                {
                    ChileTypeKeyReturn = key.Invoke(c),
                    ChileTypeDescription = c.Description
                };
        }
    }
}