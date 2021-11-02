using System.Data.Entity;

namespace RioValleyChili.Data.Initialization
{
    public interface IDataContextInitializationAdapter<TContext>
        where TContext : DbContext
    {
        void InitializeDataContext(ref TContext context);
    }
}