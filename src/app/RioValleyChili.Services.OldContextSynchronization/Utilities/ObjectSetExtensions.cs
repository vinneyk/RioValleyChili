using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;
using RioValleyChili.Core.Extensions;

namespace RioValleyChili.Services.OldContextSynchronization.Utilities
{
    public static class ObjectSetExtensions
    {
        public static IEnumerable<T> Local<T>(this ObjectSet<T> set)
            where T : class
        {
            return from stateEntry in set.Context.ObjectStateManager.GetObjectStateEntries(EntityState.Added | EntityState.Modified | EntityState.Unchanged)
                   where stateEntry.Entity != null && stateEntry.EntitySet == set.EntitySet
                   select stateEntry.Entity as T;
        }

        public static DateTime GetNextUnique<T>(this ObjectSet<T> set, DateTime requested, Expression<Func<T, DateTime>> select)
            where T : class
        {
            requested = requested.RoundMillisecondsForSQL();
            var predicate = ToPredicate(select, requested);
            if(set.Local().Any(predicate.Compile()) || set.Any(predicate))
            {
                return set.GetNextUnique(requested.AddMilliseconds(10), select);
            }
            return requested;
        }

        public static int GetNextUnique<T>(this ObjectSet<T> set, Expression<Func<T, int>> select)
            where T : class
        {
            return Math.Max(set.Local().Any() ? set.Local().Max(select.Compile()) + 1 : 1, set.Any() ? set.Max(select) + 1 : 1);
        }

        private static Expression<Func<T, bool>> ToPredicate<T>(Expression<Func<T, DateTime>> select, DateTime value)
        {
            Expression<Func<T, bool>> predicate = t => @select.Invoke(t) == value;
            return predicate.Expand();
        }
    }
}