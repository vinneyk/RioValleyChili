using System;
using System.Data.Entity;

namespace RioValleyChili.Data.Initialization
{
    // candidate for Solutionhead.Data.EntityFramework
    public class DropCreateAndSeedDatabaseAlways<TContext> : DropCreateDatabaseAlways<TContext> 
        where TContext : DbContext, new()
    {
        private readonly IDataContextSeeder<TContext> _seeder;

        public DropCreateAndSeedDatabaseAlways()
            : this(new DoNothingDbContextDataSeeder<TContext>()) { }

        public DropCreateAndSeedDatabaseAlways(IDataContextSeeder<TContext> seeder)
        {
            if (seeder == null) { throw new ArgumentNullException("seeder"); }
            _seeder = seeder;
        }

        protected override void Seed(TContext context)
        {
            _seeder.SeedContext(context);
        }
    }
}