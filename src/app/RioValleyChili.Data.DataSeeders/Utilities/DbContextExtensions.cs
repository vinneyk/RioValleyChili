using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace RioValleyChili.Data.DataSeeders.Utilities
{
    public static class DbContextExtensions
    {
        public static int NextIdentity<TEntity>(this DbContext context, Func<TEntity, int> idSelector) where TEntity : class
        {
            if(context == null) { throw new ArgumentNullException("context"); }
            if(idSelector == null) { throw new ArgumentNullException("idSelector"); }

            var existingEntities = context.Set<TEntity>();
            return existingEntities.Any() ? existingEntities.Max(idSelector) + 1 : 1;
        }

        public static void ClearContext(this DbContext context)
        {
            var objectContextAdapter = context as IObjectContextAdapter;
            foreach(var entry in objectContextAdapter.ObjectContext.ObjectStateManager.GetObjectStateEntries(EntityState.Added | EntityState.Deleted | EntityState.Modified | EntityState.Unchanged))
            {
                objectContextAdapter.ObjectContext.Detach(entry.Entity);
            }
        }
    }
}

