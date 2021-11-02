using System.Data.Entity;
using RioValleyChili.Data.Migrations;

namespace RioValleyChili.Data.Initialization
{
    public class MigrationDataContextInitializationAdapter<TContext, TDataSeeder> : IDataContextInitializationAdapter<TContext>
        where TContext : DbContext
        where TDataSeeder : class, IDataContextSeeder<TContext>, new()
    {
        private readonly IDatabaseInitializer<TContext> _initializer = 
            new MigrateDatabaseToLatestVersion<TContext, ConfigurationWithSeeder<TContext, TDataSeeder>>();

        public MigrationDataContextInitializationAdapter()
        {
            Database.SetInitializer<RioValleyChiliDataContext>(null);
        }

        public void InitializeDataContext(ref TContext context)
        {
            _initializer.InitializeDatabase(context);
        }
    }
}