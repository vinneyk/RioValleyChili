using System;
using System.Linq.Expressions;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Utilities.LinqPredicates
{
    internal static class LotAttributePredicates
    {
        internal static Expression<Func<LotAttribute, bool>> ByAttributeNameKey(IAttributeNameKey key)
        {
            return l => l.AttributeShortName == key.AttributeNameKey_ShortName;
        }
    }
}