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
    public class InventoryDbContextDataSeeder : DbContextDataSeederBase<RioValleyChiliDataContext>
    {
        public override void SeedContext(RioValleyChiliDataContext newContext)
        {
            new ProductsDbContextDataSeeder().SeedContext(newContext);
            
            var consoleTicker = new ConsoleTicker();

            using(var oldContext = ContextsHelper.GetOldContext())
            {
                var proxyCreation = newContext.Configuration.ProxyCreationEnabled;
                var autoDetectChangesEnabled = newContext.Configuration.AutoDetectChangesEnabled;
                var lazyLoading = newContext.Configuration.LazyLoadingEnabled;

                try
                {
                    newContext.Configuration.ProxyCreationEnabled = false;
                    newContext.Configuration.AutoDetectChangesEnabled = false;
                    newContext.Configuration.LazyLoadingEnabled = false;
                    
                    oldContext.CommandTimeout = 1800;

                    LoadLots(newContext, oldContext, consoleTicker);
                    LoadLotAttributeHistory(newContext, oldContext, consoleTicker);
                    LoadInventory(newContext, oldContext, consoleTicker);
                    LoadInventoryAdjustments(newContext, oldContext, consoleTicker);
                }
                finally
                {
                    newContext.Configuration.ProxyCreationEnabled = proxyCreation;
                    newContext.Configuration.AutoDetectChangesEnabled = autoDetectChangesEnabled;
                    newContext.Configuration.LazyLoadingEnabled = lazyLoading;
                }
            }
        }

        private static void LoadLots(RioValleyChiliDataContext newContext, RioAccessSQLEntities oldContext, ConsoleTicker consoleTicker)
        {
            StartWatch();
            var lots = new List<Lot>();
            var chileLots = new List<ChileLot>();
            var additiveLots = new List<AdditiveLot>();
            var packagingLots = new List<PackagingLot>();
            var lotAttributes = new List<LotAttribute>();
            var lotDefects = new List<LotDefect>();
            var lotAttributeDefects = new List<LotAttributeDefect>();
            var lotDefectResolutions = new List<LotDefectResolution>();
            
            ProcessedBirth(new LotEntityObjectMother(oldContext, newContext, RVCDataLoadLoggerGate.LotLoadLoggerCallback), "Loading Lots", consoleTicker, lotResult =>
                {
                    if(lotResult.ChileLot != null)
                    {
                        chileLots.Add(lotResult.ChileLot);
                        lots.Add(lotResult.ChileLot.Lot);
                        lotAttributes.AddRange(lotResult.ChileLot.Lot.Attributes);

                        if(lotResult.ChileLot.Lot.LotDefects != null)
                        {
                            lotDefects.AddRange(lotResult.ChileLot.Lot.LotDefects);
                            lotDefectResolutions.AddRange(lotResult.ChileLot.Lot.LotDefects.Select(d => d.Resolution).Where(r => r != null));
                        }
                    }
                    else if(lotResult.AdditiveLot != null)
                    {
                        additiveLots.Add(lotResult.AdditiveLot);
                        lots.Add(lotResult.AdditiveLot.Lot);
                        lotAttributes.AddRange(lotResult.AdditiveLot.Lot.Attributes);
                    }
                    else if(lotResult.PackagingLot != null)
                    {
                        packagingLots.Add(lotResult.PackagingLot);
                        lots.Add(lotResult.PackagingLot.Lot);
                    }

                    if(lotResult.LotAttributeDefects != null)
                    {
                        lotAttributeDefects.AddRange(lotResult.LotAttributeDefects);
                    }
                });
            
            consoleTicker.ReplaceCurrentLine("Loading Lots");
            LoadRecords(newContext, lots, "\tlots", consoleTicker);
            LoadRecords(newContext, chileLots, "\tchile lots", consoleTicker);
            LoadRecords(newContext, lotAttributes, "\tchile lot attributes", consoleTicker);
            LoadRecords(newContext, additiveLots, "\tadditive lots", consoleTicker);
            LoadRecords(newContext, packagingLots, "\tpackaging lots", consoleTicker);
            LoadRecords(newContext, lotDefects, "\tlot defects", consoleTicker);
            LoadRecords(newContext, lotDefectResolutions, "\tlot defect resolutions", consoleTicker);
            LoadRecords(newContext, lotAttributeDefects, "\tlot attribute defects", consoleTicker);
            
            consoleTicker.WriteTimeElapsed(StopWatch());
        }

        private static void LoadLotAttributeHistory(RioValleyChiliDataContext newContext, RioAccessSQLEntities oldContext, ConsoleTicker consoleTicker)
        {
            StartWatch();
            var lots = new List<LotHistory>();

            ProcessedBirth(new LotHistoryEntityObjectMother(oldContext, newContext, RVCDataLoadLoggerGate.LotHistoryLoggerCallback), "Loading Lot History", consoleTicker, lots.Add);

            consoleTicker.ReplaceCurrentLine("Loading Lot Histories");
            LoadRecords(newContext, lots, "\tlotHistory", consoleTicker);

            consoleTicker.WriteTimeElapsed(StopWatch());
        }

        private static void LoadInventory(RioValleyChiliDataContext newContext, RioAccessSQLEntities oldContext, ConsoleTicker consoleTicker)
        {
            StartWatch();
            Console.WriteLine("Loading Inventory");
            LoadRecords(newContext, new AdditiveLotInventoryEntityObjectMother(oldContext, newContext, RVCDataLoadLoggerGate.AdditiveLotInventoryLoadLogger), "\tAdditive", consoleTicker);
            LoadRecords(newContext, new PackagingLotInventoryItemEntityObjectMother(oldContext, newContext, RVCDataLoadLoggerGate.PackagingLotInventoryLoadLogger), "\tPackaging", consoleTicker);
            LoadRecords(newContext, new WIPChileLotInventoryItemEntityObjectMother(oldContext, newContext, RVCDataLoadLoggerGate.WIPChileLotInventoryLoadLogger), "\tChile WIP", consoleTicker);
            LoadRecords(newContext, new FinishedGoodsChileLotInventoryItemEntityObjectMother(oldContext, newContext, RVCDataLoadLoggerGate.FinishedGoodsChileLotInventoryLoadLogger), "\tChile Finished Goods", consoleTicker);
            LoadRecords(newContext, new DehyChileLotInventoryItemEntityObjectMother(oldContext, newContext, RVCDataLoadLoggerGate.DehyChileLotInventoryLoadLogger), "\tChile Dehydrated", consoleTicker);
            LoadRecords(newContext, new RawChileLotInventoryItemEntityObjectMother(oldContext, newContext, RVCDataLoadLoggerGate.RawChileLotInventoryLoadLogger), "\tChile Raw", consoleTicker);
            LoadRecords(newContext, new OtherChileLotInventoryItemEntityObjectMother(oldContext, newContext, RVCDataLoadLoggerGate.OtherChileLotInventoryLoadLogger), "\tChile Other", consoleTicker);
            LoadRecords(newContext, new GRPChileLotInventoryItemEntityObjectMother(oldContext, newContext, RVCDataLoadLoggerGate.GRPChileLotInventoryLoadLogger), "\tChile GRP", consoleTicker);
            consoleTicker.WriteTimeElapsed(StopWatch());
        }

        private static void LoadInventoryAdjustments(RioValleyChiliDataContext newContext, RioAccessSQLEntities oldContext, ConsoleTicker consoleTicker)
        {
            StartWatch();
            var adjustments = new List<InventoryAdjustment>();
            var adjustmentItems = new List<InventoryAdjustmentItem>();
            var notebooks = new List<Notebook>();
            var notes = new List<Note>();
            ProcessedBirth(new InventoryAdjustmentsMother(oldContext, newContext, RVCDataLoadLoggerGate.InventoryAdjustmentsLoadLoggerCallback), "Loading Inventory Adjustments", consoleTicker, adjustment =>
                {
                    adjustments.Add(adjustment);
                    adjustmentItems.AddRange(adjustment.Items);
                    notebooks.Add(adjustment.Notebook);
                    notes.AddRange(adjustment.Notebook.Notes);
                });
            consoleTicker.ReplaceCurrentLine("Loading Inventory Adjustments");
            LoadRecords(newContext, notebooks, "\tNotebooks", consoleTicker);
            LoadRecords(newContext, notes, "\tNotes", consoleTicker);
            LoadRecords(newContext, adjustments, "\tInventory Adjustments", consoleTicker);
            LoadRecords(newContext, adjustmentItems, "\tInventory Adjustments Items", consoleTicker);
            consoleTicker.WriteTimeElapsed(StopWatch());
        }
    }
}
