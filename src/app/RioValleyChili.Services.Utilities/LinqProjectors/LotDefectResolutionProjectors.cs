using System;
using System.Linq.Expressions;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class LotDefectResolutionProjectors
    {
        internal static Expression<Func<LotDefectResolution, LotDefectResolutionReturn>> Select()
        {
            return r => new LotDefectResolutionReturn
                {
                    ResolutionType = r.ResolutionType,
                    Description = r.Description
                };
        }
    }
}