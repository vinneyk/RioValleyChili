using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Data.DataSeeders.Logging;
using RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers;
using RioValleyChili.Data.DataSeeders.Sync;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.Helpers;
using RioValleyChili.Data.Initialization;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders
{
    public class ProductionDataSeeder : DbContextDataSeederBase<RioValleyChiliDataContext>
    {
        public override void SeedContext(RioValleyChiliDataContext newContext)
        {
            new InventoryDbContextDataSeeder().SeedContext(newContext);

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

                    oldContext.CommandTimeout = 2400;
                    oldContext.ClearContext();

                    LoadPackSchedules(oldContext, newContext, consoleTicker);
                    LoadProductionBatches(oldContext, newContext, consoleTicker);
                    LoadProductionSchedules(oldContext, newContext, consoleTicker);
                    LoadInstructions(oldContext, newContext, consoleTicker);
                    LoadChileMaterialsReceived(oldContext, newContext, consoleTicker);
                    LoadMillAndWetdown(oldContext, newContext, consoleTicker);
                }
                finally
                {
                    newContext.Configuration.ProxyCreationEnabled = proxyCreation;
                    newContext.Configuration.AutoDetectChangesEnabled = autoDetectChangesEnabled;
                    newContext.Configuration.LazyLoadingEnabled = lazyLoading;
                }
            }
        }

        private static void LoadPackSchedules(RioAccessSQLEntities oldContext, RioValleyChiliDataContext newContext, ConsoleTicker consoleTicker)
        {
            StartWatch();
            var packScheduleKeySync = new List<PackSchedule>();
            var packSchedules = new List<PackSchedule>();
            var productionBatches = new List<ProductionBatch>();
            var notebooks = new List<Notebook>();
            var productions = new List<ChileLotProduction>();
            var pickedInventories = new List<PickedInventory>();
            ProcessedBirth(new PackScheduleEntityObjectMother(oldContext, newContext, RVCDataLoadLoggerGate.PackScheduleLoadLoggerCallback), "Loading Pack Schedules", consoleTicker, result =>
                {
                    if(result.RequiresKeySync)
                    {
                        packScheduleKeySync.Add(result);
                    }

                    packSchedules.Add(result);
                    foreach(var batch in result.ProductionBatches)
                    {
                        productionBatches.Add(batch);
                        notebooks.Add(batch.InstructionNotebook);
                        productions.Add(batch.Production);
                        pickedInventories.Add(batch.Production.PickedInventory);
                    }
                });

            consoleTicker.ReplaceCurrentLine("Loading Pack Schedules");

            LoadRecords(newContext, packSchedules, "\tpack schedules", consoleTicker);
            LoadRecords(newContext, productionBatches, "\tproduction batches", consoleTicker);
            LoadRecords(newContext, notebooks, "\tnotebooks", consoleTicker);
            LoadRecords(newContext, productions, "\tchile lot productions", consoleTicker);
            LoadRecords(newContext, pickedInventories, "\tpicked inventories", consoleTicker);

            consoleTicker.WriteTimeElapsed(StopWatch());

            new PackScheduleKeySync(oldContext, consoleTicker).SyncOldModels(packScheduleKeySync);

            oldContext.ClearContext();
        }

        private static void LoadProductionBatches(RioAccessSQLEntities oldContext, RioValleyChiliDataContext newContext, ConsoleTicker consoleTicker)
        {
            StartWatch();

            var productionResults = new List<LotProductionResults>();
            var productionResultItems = new List<LotProductionResultItem>();
            var pickedInventories = new List<PickedInventory>();
            var pickedInventoryItems = new List<PickedInventoryItem>();
            var productions = new List<ChileLotProduction>();
            var productionBatches = new List<ProductionBatch>();
            var notebooks = new List<Notebook>();

            ProcessedBirth(new ProductionBatchEntityObjectMother(oldContext, newContext, RVCDataLoadLoggerGate.ProductionBatchLoadLoggerCallback), "Loading Production Batches", consoleTicker, productionBatch =>
                {
                    if(productionBatch.Production.Results != null)
                    {
                        productionResults.Add(productionBatch.Production.Results);
                        productionResultItems.AddRange(productionBatch.Production.Results.ResultItems);
                    }
                    pickedInventories.Add(productionBatch.Production.PickedInventory);
                    pickedInventoryItems.AddRange(productionBatch.Production.PickedInventory.Items);
                    productions.Add(productionBatch.Production);
                    productionBatches.Add(productionBatch);
                    notebooks.Add(productionBatch.InstructionNotebook);
                });

            consoleTicker.ReplaceCurrentLine("Loading Production Batches");

            LoadRecords(newContext, productionResults, "\tproduction results", consoleTicker);
            LoadRecords(newContext, productionResultItems, "\tproduction result items", consoleTicker);
            LoadRecords(newContext, pickedInventories, "\tpicked inventories", consoleTicker);
            LoadRecords(newContext, pickedInventoryItems, "\tpicked inventory items", consoleTicker);
            LoadRecords(newContext, productions, "\tchile lot productions", consoleTicker);
            LoadRecords(newContext, productionBatches, "\tproduction batches", consoleTicker);
            LoadRecords(newContext, notebooks, "\tnotebooks", consoleTicker);

            consoleTicker.WriteTimeElapsed(StopWatch());
            
            oldContext.ClearContext();
        }

        private static void LoadProductionSchedules(RioAccessSQLEntities oldContext, RioValleyChiliDataContext newContext, ConsoleTicker consoleTicker)
        {
            StartWatch();

            var productionSchedules = new List<ProductionSchedule>();
            var productionScheduleItems = new List<ProductionScheduleItem>();
            ProcessedBirth(new ProductionScheduleEntityObjectMother(oldContext, newContext, RVCDataLoadLoggerGate.ProductionScheduleLoadLoggerCallback), "Loading Production Schedules", consoleTicker, result =>
                {
                    productionSchedules.Add(result);
                    productionScheduleItems.AddRange(result.ScheduledItems);
                });

            consoleTicker.ReplaceCurrentLine("Loading Production Schedules");

            LoadRecords(newContext, productionSchedules, "\tproduction schedules", consoleTicker);
            LoadRecords(newContext, productionScheduleItems, "\tproduction schedule items", consoleTicker);

            consoleTicker.WriteTimeElapsed(StopWatch());

            oldContext.ClearContext();
        }

        private static void LoadInstructions(RioAccessSQLEntities oldContext, RioValleyChiliDataContext newContext, ConsoleTicker consoleTicker)
        {
            var instructions = new List<Instruction>();
            var notes = new List<Note>();
            ProcessedBirth(new InstructionEntityObjectMother(oldContext, newContext, RVCDataLoadLoggerGate.InstructionLoadLoggerCallback), "Loading Instructions", consoleTicker, result =>
                {
                    notes.Add(result.Note);
                    if(result.Instruction != null)
                    {
                        instructions.Add(result.Instruction);
                    }
                });

            consoleTicker.ReplaceCurrentLine("Loading Instructions");

            LoadRecords(newContext, instructions, "\tinstructions", consoleTicker);
            LoadRecords(newContext, notes, "\tnotes", consoleTicker);

            consoleTicker.WriteTimeElapsed(StopWatch());
        }

        private static void LoadChileMaterialsReceived(RioAccessSQLEntities oldContext, RioValleyChiliDataContext newContext, ConsoleTicker consoleTicker)
        {
            StartWatch();
            var chileMaterialsReceived = Birth(new ChileMaterialsReceivedEntityMother(oldContext, newContext, RVCDataLoadLoggerGate.DehydratedMaterialsReceivedLoadLoggerCallback), "Loading Chile Materials Received", consoleTicker);

            consoleTicker.ReplaceCurrentLine("Loading Chile Materials Received");

            LoadRecords(newContext, chileMaterialsReceived, "\tchile materials received", consoleTicker);
            LoadRecords(newContext, chileMaterialsReceived.SelectMany(d => d.Items), "\tchile materials received items", consoleTicker);

            consoleTicker.WriteTimeElapsed(StopWatch());
        }

        private static void LoadMillAndWetdown(RioAccessSQLEntities oldContext, RioValleyChiliDataContext newContext, ConsoleTicker consoleTicker)
        {
            StartWatch();
            var productions = new List<ChileLotProduction>();
            var picked = new List<PickedInventory>();
            var pickedItems = new List<PickedInventoryItem>();
            var results = new List<LotProductionResults>();
            var resultItems = new List<LotProductionResultItem>();
            
            ProcessedBirth(new MillAndWetdownMother(oldContext, newContext, RVCDataLoadLoggerGate.MillAndWetdownLoadLoggerCallback), "Loading Mill And Wetdown Records", consoleTicker, production =>
                {
                    productions.Add(production);

                    picked.Add(production.PickedInventory);
                    if(production.PickedInventory.Items != null)
                    {
                        pickedItems.AddRange(production.PickedInventory.Items);
                    }

                    results.Add(production.Results);
                    if(production.Results.ResultItems != null)
                    {
                        resultItems.AddRange(production.Results.ResultItems);
                    }
                });

            consoleTicker.ReplaceCurrentLine("Loading Mill And Wetdown Records");

            LoadRecords(newContext, productions, "\tchile lot productions", consoleTicker);
            LoadRecords(newContext, picked, "\tpicked inventories", consoleTicker);
            LoadRecords(newContext, pickedItems, "\tpicked inventory items", consoleTicker);
            LoadRecords(newContext, results, "\tlot production results", consoleTicker);
            LoadRecords(newContext, resultItems, "\tlot production result items", consoleTicker);

            consoleTicker.WriteTimeElapsed(StopWatch());
        }
    }
}