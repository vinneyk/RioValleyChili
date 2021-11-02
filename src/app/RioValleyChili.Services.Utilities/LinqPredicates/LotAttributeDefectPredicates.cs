using System;
using System.Linq.Expressions;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Utilities.LinqPredicates
{
    public static class LotAttributeDefectPredicates
    {
        public static Expression<Func<LotAttributeDefect, bool>> FilterByLotKey(ILotKey lotKey)
        {
            return d => d.LotDateCreated == lotKey.LotKey_DateCreated && d.LotDateSequence == lotKey.LotKey_DateSequence && d.LotTypeId == lotKey.LotKey_LotTypeId;
        }
    }
}