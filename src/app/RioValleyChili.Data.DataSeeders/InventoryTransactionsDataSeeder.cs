using System.Collections.Generic;
using RioValleyChili.Data.DataSeeders.Logging;
using RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.Helpers;
using RioValleyChili.Data.Initialization;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders
{
    public class InventoryTransactionsDataSeeder : DbContextDataSeederBase<RioValleyChiliDataContext>
    {
        public override void SeedContext(RioValleyChiliDataContext newContext)
        {
            new OrderDbContextDataSeeder().SeedContext(newContext);
            var consoleTicker = new ConsoleTicker();

            using(var oldContext = ContextsHelper.GetOldContext())
            {
                var proxyCreation = newContext.Configuration.ProxyCreationEnabled;
                var autoDetectChangesEnabled = newContext.Configuration.AutoDetectChangesEnabled;
                var lazyLoading = newContext.Configuration.LazyLoadingEnabled;

                try
                {
                    newContext.Configuration.ProxyCreationEnabled = false;
                    newContext.Configuration.AutoDetectChangesEnabled = true;
                    newContext.Configuration.LazyLoadingEnabled = false;

                    oldContext.CommandTimeout = 1200;

                    LoadInventoryTransactions(oldContext, newContext, consoleTicker);
                }
                finally
                {
                    newContext.Configuration.ProxyCreationEnabled = proxyCreation;
                    newContext.Configuration.AutoDetectChangesEnabled = autoDetectChangesEnabled;
                    newContext.Configuration.LazyLoadingEnabled = lazyLoading;
                }
            }
        }

        private static void LoadInventoryTransactions(RioAccessSQLEntities oldContext, RioValleyChiliDataContext newContext, ConsoleTicker consoleTicker)
        {
            var transactions = new List<InventoryTransaction>();
            const string loadingMessage = "Loading InventoryTransactions";

            StartWatch();

            ProcessedBirth(new InventoryTransactionsMother(oldContext, newContext, RVCDataLoadLoggerGate.InventoryTransactionsLoadLoggerCallback), loadingMessage, consoleTicker, transactions.Add);

            consoleTicker.ReplaceCurrentLine(loadingMessage);
            LoadRecords(newContext, transactions, "\tinventory transactions", consoleTicker);

            newContext.SaveChanges();
            consoleTicker.WriteTimeElapsed(StopWatch());

            oldContext.ClearContext();
        }
    }
}