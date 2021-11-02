using System;
using System.Linq.Expressions;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class LotAttributeDefectProjectors
    {
        internal static Expression<Func<LotAttributeDefect, LotAttributeDefectReturn>> Select()
        {
            return a => new LotAttributeDefectReturn
            {
                AttributeShortName = a.AttributeShortName,
                OriginalValue = a.OriginalAttributeValue,
                OriginalMinLimit = a.OriginalAttributeMinLimit,
                OriginalMaxLimit = a.OriginalAttributeMaxLimit
            };
        }
    }
}