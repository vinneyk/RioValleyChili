// ReSharper disable ConvertClosureToMethodGroup

using System;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class LotDefectProjectors
    {
        internal static Expression<Func<LotDefect, LotDefectKeyReturn>> SelectKey()
        {
            return d => new LotDefectKeyReturn
                {
                    LotKey_DateCreated = d.LotDateCreated,
                    LotKey_DateSequence = d.LotDateSequence,
                    LotKey_LotTypeId = d.LotTypeId,
                    LotDefectKey_Id = d.DefectId
                };
        }

        internal static Expression<Func<LotDefect, LotDefectReturn>> SelectLotDefect()
        {
            var lotDefectKey = SelectKey();
            var resolution = LotDefectResolutionProjectors.Select();
            var lotAttributeDefect = LotAttributeDefectProjectors.Select();

            return d => new LotDefectReturn
                {
                    LotDefectKeyReturn = lotDefectKey.Invoke(d),

                    DefectType = d.DefectType,
                    Description = d.Description,

                    AttributeDefect = d.Lot.AttributeDefects.Where(a => a.DefectId == d.DefectId)
                        .Select(a => lotAttributeDefect.Invoke(a))
                        .FirstOrDefault(),
                    Resolution = new [] { d.Resolution }.Where(r => r != null).Select(r => resolution.Invoke(r)).FirstOrDefault(),
                };
        }
    }
}

// ReSharper restore ConvertClosureToMethodGroup