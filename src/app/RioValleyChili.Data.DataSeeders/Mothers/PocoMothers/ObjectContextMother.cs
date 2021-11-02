using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;

namespace RioValleyChili.Data.DataSeeders.Mothers.PocoMothers
{
    class ObjectContextMother<TChild> : IMother<TChild> where TChild : class
    {
        private readonly ObjectContext _context;
        protected ObjectSet<TChild> ObjectSet; 

        public ObjectContextMother(ObjectContext context)
        {
            if(context == null) throw new ArgumentNullException("context");

            _context = context;
        }

        public IEnumerable<TChild> BirthAll(Action consoleCallback = null)
        {
            ObjectSet = _context.CreateObjectSet<TChild>();
            return ObjectSet.Select(a => a);
        }
    }
}
