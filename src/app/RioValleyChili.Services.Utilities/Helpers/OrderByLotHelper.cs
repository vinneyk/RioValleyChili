using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Utilities.Helpers
{
    internal static class OrderByLotHelper
    {
        internal static IOrderedQueryable<Lot> OrderByLot(this IQueryable<Lot> lots)
        {
            return lots.OrderBy(l => l.LotTypeId).ThenBy(l => l.LotDateCreated).ThenBy(l => l.LotDateSequence);
        }

        internal static IOrderedEnumerable<Lot> OrderByLot(this IEnumerable<Lot> lots)
        {
            return lots.OrderBy(l => l.LotTypeId).ThenBy(l => l.LotDateCreated).ThenBy(l => l.LotDateSequence);
        }
    }
}