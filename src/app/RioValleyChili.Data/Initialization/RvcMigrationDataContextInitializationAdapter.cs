using RioValleyChili.Data.Migrations;

namespace RioValleyChili.Data.Initialization
{
    public sealed class RvcMigrationDataContextInitializationAdapter
        : MigrationDataContextInitializationAdapter<RioValleyChiliDataContext, ConfigurationWithSeeder<RioValleyChiliDataContext, DoNothingDbContextDataSeeder<RioValleyChiliDataContext>>>
    { }


    public sealed class RvcMigrationDataContextInitializationAdapter<TDataSeeder>
        : MigrationDataContextInitializationAdapter<RioValleyChiliDataContext, ConfigurationWithSeeder<RioValleyChiliDataContext, TDataSeeder>>
        where TDataSeeder : class, IDataContextSeeder<RioValleyChiliDataContext>, new()
    { }
}