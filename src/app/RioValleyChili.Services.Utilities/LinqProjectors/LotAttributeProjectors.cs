using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EF_Projectors;
using EF_Projectors.Extensions;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class LotAttributeProjectors
    {
        internal static Expression<Func<LotAttribute, LotAttributeReturn>> Select()
        {
            return SelectBase().Merge(a => new LotAttributeReturn
                {
                    AttributeDate = a.AttributeDate,
                    Computed = a.Computed
                });
        }

        internal static Expression<Func<LotAttribute, TAttributeReturn>> Select<TAttributeReturn>()
            where TAttributeReturn : LotAttributeReturn, new()
        {
            return Select().Merge(a => new TAttributeReturn { });
        }

        internal static Expression<Func<LotAttribute, LotAttributeParameterReturn>> SelectParameter()
        {
            return SelectBase().Merge(a => new LotAttributeParameterReturn
                {
                    NameActive = a.AttributeName.Active
                });
        }

        internal static Expression<Func<IEnumerable<LotAttribute>, double>> SelectAverage(IAttributeNameKey attribute)
        {
            return attributes => attributes.Where(a => a.AttributeShortName == attribute.AttributeNameKey_ShortName)
                                           .Select(a => a.AttributeValue)
                                           .DefaultIfEmpty(0.0)
                                           .Average();
        }

        internal static Expression<Func<LotAttribute, LotHistoryAttributeReturn>> SelectHistory()
        {
            return Projector<LotAttribute>.To(a => new LotHistoryAttributeReturn
                {
                    AttributeShortName = a.AttributeShortName,
                    Value = a.AttributeValue,
                    AttributeDate = a.AttributeDate,
                    Computed = a.Computed
                });
        }

        private static Expression<Func<LotAttribute, LotAttributeBaseReturn>> SelectBase()
        {
            return a => new LotAttributeBaseReturn
                {
                    Key = a.AttributeShortName,
                    Name = a.AttributeName.Name,
                    Value = a.AttributeValue
                };
        }
    }
}