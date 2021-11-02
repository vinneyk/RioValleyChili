using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Solutionhead.Data;

namespace RioValleyChili.Data
{
    public abstract class EFUnitOfWorkBase : IUnitOfWork
    {
        public readonly DbContext Context;

        protected EFUnitOfWorkBase(DbContext context = null)
        {
            Context = context ?? new RioValleyChiliDataContext();
            Context.Database.CommandTimeout = 600;
        }

        public void Commit()
        {
            Context.SaveChanges();
        }

        public List<TObject> GetLocalData<TObject>() where TObject : class
        {
            return Context.Set<TObject>().Local.ToList();
        }

        public IQueryable<TObject> GetExistingData<TObject>() where TObject : class
        {
            return Context.Set<TObject>().AsQueryable();
        }

        public bool TryGetObjectState<TObject>(IKey<TObject> key, out EntityState? state, out TObject entity) where TObject : class
        {
            entity = null;
            state = null;

            var existingEntity = Context.ChangeTracker.Entries<TObject>().SingleOrDefault(e => key.FindByPredicate.Compile().Invoke(e.Entity));

            if(existingEntity == null)
            {
                return false;
            }

            entity = existingEntity.Entity;
            state = existingEntity.State;
            return true;
        }
    }
}