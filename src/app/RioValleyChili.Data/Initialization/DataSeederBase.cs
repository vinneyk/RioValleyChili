using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace RioValleyChili.Data.Initialization
{
    public abstract class DataSeederBase<TContext> : IDataSeeder, IDisposable
        where TContext : DbContext
    {
        protected readonly TContext DbContext;
        private readonly bool _disposeDbContext;

        protected DataSeederBase(TContext dbContext, bool disposeDbContextAfterLoad = true)
        {
            if(dbContext == null) throw new ArgumentNullException("dbContext");

            DbContext = dbContext;
            _disposeDbContext = disposeDbContextAfterLoad;
        }

        public abstract void Seed();

        protected void InsertItems<TEntity>(List<TEntity> entities) where TEntity : class
        {
            entities.ForEach(c => DbContext.Set<TEntity>().Add(c));
        }

        public virtual void Dispose()
        {
            if (_disposeDbContext)
            {
                DbContext.Dispose();
            }
        }
    }
}