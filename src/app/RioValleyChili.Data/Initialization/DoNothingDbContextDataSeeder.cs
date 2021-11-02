using System.Data.Entity;

namespace RioValleyChili.Data.Initialization
{
    public class DoNothingDbContextDataSeeder<TContext> : DbContextDataSeederBase<TContext> where TContext : DbContext, new()
    {
        #region Overrides of DbContextDataSeederBase<RioValleyChiliDataContext>

        public override void SeedContext(TContext newContext) { }

        protected override void ResetContext(ref TContext context) { }

        #endregion
    }
}