using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace RioValleyChili.Data.DataSeeders.Mothers.PocoMothers
{
    class DbContextMother<TChild> : IMother<TChild> where TChild : class
    {
        protected DbContext DbContext;

        public DbContextMother(DbContext context)
        {
            if(context == null) throw new ArgumentNullException("context");

            DbContext = context;
        }

        public IEnumerable<TChild> BirthAll(Action consoleCallback = null)
        {
            return DbContext.Set<TChild>().ToList();
        }
    }
}
