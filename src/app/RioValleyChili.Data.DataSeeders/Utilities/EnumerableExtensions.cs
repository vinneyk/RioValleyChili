using System;
using System.Collections.Generic;
using System.Linq;

namespace RioValleyChili.Data.DataSeeders.Utilities
{
    internal static class EnumerableExtensions
    {
        internal static bool SelectSingleDistinctNotNull<TProp, T>(this IEnumerable<T> enumerable, Func<T, TProp?> propSelect, out TProp output) where TProp : struct
        {
            output = default(TProp);

            var values = enumerable.Select(propSelect).Where(p => p != null).Distinct().ToList();
            if(values.Count == 1)
            {
                output = values.Single().Value;
                return true;
            }

            return false;
        }

        internal static bool SelectSingleDistinct<TProp, T>(this IEnumerable<T> enumerable, Func<T, TProp> propSelect, out TProp output)
        {
            output = default(TProp);

            var values = enumerable.Select(propSelect).Distinct().ToList();
            if(values.Count == 1)
            {
                output = values.First();
                return true;
            }

            return false;
        }

        internal static bool SelectSingleDistinct<TProp, T>(this IEnumerable<T> enumerable, Func<T, TProp> propSelect, IEqualityComparer<TProp> equalityComparer, out TProp output)
        {
            output = default(TProp);

            var values = enumerable.Select(propSelect).Distinct(equalityComparer).ToList();
            if(values.Count == 1)
            {
                output = values.First();
                return true;
            }

            return false;
        }
    }
}