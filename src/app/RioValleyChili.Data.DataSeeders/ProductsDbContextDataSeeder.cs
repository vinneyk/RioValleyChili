using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Data.DataSeeders.Logging;
using RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.Helpers;
using RioValleyChili.Data.Initialization;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders
{
    public class ProductsDbContextDataSeeder : DbContextDataSeederBase<RioValleyChiliDataContext>
    {
        public override void SeedContext(RioValleyChiliDataContext newContext)
        {
            new CompanyDbContactDataSeeder().SeedContext(newContext);

            var consoleTicker = new ConsoleTicker();
            using(var oldContext = ContextsHelper.GetOldContext())
            {
                var proxyCreation = newContext.Configuration.ProxyCreationEnabled;
                var autoDetecChanges = newContext.Configuration.AutoDetectChangesEnabled;
                var lazyLoading = newContext.Configuration.LazyLoadingEnabled;

                try
                {
                    newContext.Configuration.ProxyCreationEnabled = false;
                    newContext.Configuration.AutoDetectChangesEnabled = false;
                    newContext.Configuration.LazyLoadingEnabled = false;

                    Console.WriteLine("Loading Products");

                    var chileProducts = LoadProducts(newContext, new ChileProductEntityObjectMother(oldContext, newContext, RVCDataLoadLoggerGate.ChileProductLoadLoggerCallback), consoleTicker, "\tChile Products", c => c.Product);
                    LoadRecords(newContext, chileProducts.SelectMany(c => c.ProductAttributeRanges), "\t\tproduct attribute ranges", consoleTicker);
                    LoadRecords(newContext, new ChileProductIngredientsMother(oldContext, newContext, RVCDataLoadLoggerGate.ChileProductIngredientsLoadLoggerCallback), "\t\tingredients", consoleTicker);

                    LoadProducts(newContext, new PackagingProductEntityObjectMother(oldContext, newContext, RVCDataLoadLoggerGate.PackagingProductLoadLoggerCallback), consoleTicker, "\tPackaging Products", p => p.Product);
                    LoadProducts(newContext, new AdditiveProductEntityObjectMother(oldContext, newContext, RVCDataLoadLoggerGate.AdditiveProductLoadLoggerCallback), consoleTicker, "\tAdditive Products", a => a.Product);

                    LoadProducts(newContext, new NonInventoryProductEntityObjectMother(oldContext, newContext, RVCDataLoadLoggerGate.NonInventoryProductLoadLoggerCallback), consoleTicker, "\tNon-Inventory Products");

                    StartWatch();
                    var specs = new List<CustomerProductAttributeRange>();
                    ProcessedBirth(new CustomerProductSpecMother(oldContext, newContext, RVCDataLoadLoggerGate.CustomerSpecsLoadLoggerCallback), "Loading Customer Product Specs", consoleTicker, specs.Add);
                    consoleTicker.ReplaceCurrentLine("Loading Customer Product Specs");
                    LoadRecords(newContext, specs, "\tcustomer product attribute ranges", consoleTicker);
                    consoleTicker.WriteTimeElapsed(StopWatch());

                    StartWatch();
                    var codes = new List<CustomerProductCode>();
                    ProcessedBirth(new CustomerProductCodeMother(oldContext, newContext, RVCDataLoadLoggerGate.CustomerProductCodeLoadLoggerCallback), "Loading Customer Product Codes", consoleTicker, codes.Add);
                    consoleTicker.ReplaceCurrentLine("Loading Customer Product Codes");
                    LoadRecords(newContext, codes, "\tcustomer product codes", consoleTicker);
                    consoleTicker.WriteTimeElapsed(StopWatch());

                    oldContext.ClearContext();
                }
                finally
                {
                    newContext.Configuration.ProxyCreationEnabled = proxyCreation;
                    newContext.Configuration.AutoDetectChangesEnabled = autoDetecChanges;
                    newContext.Configuration.LazyLoadingEnabled = lazyLoading;
                }
            }
        }

        private static List<TEntity> LoadProducts<TEntity>(IBulkInsertContext newContext, IMother<TEntity> mother, ConsoleTicker consoleTicker, string message, Func<TEntity, Product> productSelector = null) where TEntity : class
        {
            var products = mother.BirthAll(() => consoleTicker.TickConsole(message + "...")).ToList();
            consoleTicker.ReplaceCurrentLine(message);

            if(productSelector != null)
            {
                LoadRecords(newContext, products.Select(productSelector), "\t\tproducts", consoleTicker);
            }
            LoadRecords(newContext, products, "\t" + message.ToLower(), consoleTicker);

            return products;
        }
    }
}
