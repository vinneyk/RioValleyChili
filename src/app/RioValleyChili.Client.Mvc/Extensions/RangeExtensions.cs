using System.Collections.Generic;
using RioValleyChili.Client.Core;

namespace RioValleyChili.Client.Mvc.Extensions
{
    public static class RangeExtensions
    {
        public static void Add<TRangeObject>(this IList<Range<TRangeObject>> list, TRangeObject minValue, TRangeObject maxValue)
        {
            list.Add(new Range<TRangeObject>(minValue, maxValue));
        }
    }
}