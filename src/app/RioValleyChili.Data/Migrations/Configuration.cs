using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;

namespace RioValleyChili.Data.Migrations
{
    public class ConfigurationForPackageManagerConsole : DbMigrationsConfiguration<RioValleyChiliDataContext>
    {
        public ConfigurationForPackageManagerConsole()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = typeof(RioValleyChiliDataContext).Name;
        }

        protected override void Seed(RioValleyChiliDataContext context) { }
    }
    
    public sealed class ConfigurationWithSeeder<TContext, TDataSeeder> 
        : DbMigrationsConfiguration<TContext>, IDataContextSeeder<TContext> 
        where TContext : DbContext
        where TDataSeeder : class, IDataContextSeeder<TContext>, new()
    {
        private readonly IDataContextSeeder<TContext> _seeder;

        public ConfigurationWithSeeder()
        {
            _seeder = new TDataSeeder();
            AutomaticMigrationsEnabled = false;
            ContextKey = typeof(TContext).Name;
        }

        public ConfigurationWithSeeder(TDataSeeder seeder)
        {
            if(seeder == null) { throw new ArgumentNullException("seeder"); }
            _seeder = seeder;
            AutomaticMigrationsEnabled = false;
            ContextKey = typeof(TContext).Name;
        }

        void IDataContextSeeder<TContext>.SeedContext(TContext newContext)
        {
            Seed(newContext);
        }

        protected override void Seed(TContext context)
        {
            _seeder.SeedContext(context);
        }
    }
}
