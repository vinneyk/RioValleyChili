using System.Data.Entity;
using System.Data.Entity.Core.Objects;

namespace RioValleyChili.Data.DataSeeders.Utilities
{
    public static class ObjectContextExtensions
    {
        public static void ClearContext(this ObjectContext context)
        {
            foreach(var entry in context.ObjectStateManager.GetObjectStateEntries(EntityState.Added | EntityState.Deleted | EntityState.Modified | EntityState.Unchanged))
            {
                context.Detach(entry.Entity);
            }
        }
    }
}