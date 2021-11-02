using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace RioValleyChili.Data.Helpers
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> Include<T>(this IQueryable<T> query, params Expression<Func<T, object>>[] paths)
        {
            return paths == null || !paths.Any() ? query : paths.Aggregate(query, QueryableExtensions.Include);
        }
    }
}