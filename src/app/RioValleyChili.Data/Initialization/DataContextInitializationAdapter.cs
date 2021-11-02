using System.Data.Entity;
using System.Threading.Tasks;

namespace RioValleyChili.Data.Initialization
{
    //todo: extract into Solutionhead.Data.EntityFramework library and create DbContextBootstrapper utility

    public class DataContextInitializationAdapter<TContext> : IDataContextInitializationAdapter<TContext>
        where TContext : DbContext
    {
        private readonly IDatabaseInitializer<TContext> _contextInitializer;

        public DataContextInitializationAdapter(IDatabaseInitializer<TContext> initializer)
        {
            _contextInitializer = initializer;
            Database.SetInitializer<TContext>(null);
        }

        public void InitializeDataContext(ref TContext context)
        {
            _contextInitializer.InitializeDatabase(context);
        }

        public Task InitializeDataContextAsync(TContext context)
        {
            _contextInitializer.InitializeDatabase(context);
            return null;
        }
    }
}
