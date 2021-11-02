using System.Data.Entity;

namespace RioValleyChili.Data
{
    public interface IDataContextSeeder<in TContext> where TContext : DbContext
    {
        void SeedContext(TContext newContext);
    }
}