using System;
using System.Linq.Expressions;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;

namespace RioValleyChili.Services.Utilities.LinqPredicates
{
    internal static class ChileMaterialsReceivedItemPredicates
    {
        internal static Expression<Func<ChileMaterialsReceivedItem, bool>> FilterByLotToteReturn(PickedLotReturn pickedLotReturn)
        {
            return i => i.LotDateCreated == pickedLotReturn.LotKey.LotKey_DateCreated &&
                        i.LotDateSequence == pickedLotReturn.LotKey.LotKey_DateSequence &&
                        i.LotTypeId == pickedLotReturn.LotKey.LotKey_LotTypeId &&
                        i.ToteKey == pickedLotReturn.ToteKey;
        }
    }
}