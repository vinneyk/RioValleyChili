using System;
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
    public class OrderDbContextDataSeeder : DbContextDataSeederBase<RioValleyChiliDataContext>
    {
        public override void SeedContext(RioValleyChiliDataContext newContext)
        {
            new ProductionDataSeeder().SeedContext(newContext);

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
                    newContext.Configuration.UseDatabaseNullSemantics = true;

                    oldContext.CommandTimeout = 1800;

                    LoadContracts(oldContext, newContext, consoleTicker);
                    LoadSalesOrders(oldContext, newContext, consoleTicker);
                    LoadTreatmentOrders(oldContext, newContext, consoleTicker);
                    LoadInterWarehouseOrders(oldContext, newContext, consoleTicker);
                    LoadIntraWarehouserOrders(oldContext, newContext, consoleTicker);
                    LoadLotAllowances(oldContext, newContext, consoleTicker);
                    LoadSampleOrders(oldContext, newContext, consoleTicker);
                    LoadSalesQuotes(oldContext, newContext, consoleTicker);
                }
                finally
                {
                    newContext.Configuration.ProxyCreationEnabled = proxyCreation;
                    newContext.Configuration.AutoDetectChangesEnabled = autoDetectChangesEnabled;
                    newContext.Configuration.LazyLoadingEnabled = lazyLoading;
                }
            }
        }

        private static void LoadContracts(RioAccessSQLEntities oldContext, RioValleyChiliDataContext newContext, ConsoleTicker consoleTicker)
        {
            var keySyncContracts = new List<Contract>();
            var contracts = new List<Contract>();
            var contractItems = new List<ContractItem>();
            var notebooks = new List<Notebook>();
            var notes = new List<Note>();

            ProcessedBirth(new ContractEntityObjectMother(oldContext, newContext, RVCDataLoadLoggerGate.ContractEntityObjectLoadLoggerCallback), "Loading Contracts", consoleTicker, result =>
                {
                    if(result.RequiresKeySync)
                    {
                        keySyncContracts.Add(result);
                    }

                    contracts.Add(result);
                    contractItems.AddRange(result.ContractItems);
                    notebooks.Add(result.Comments);
                    notes.AddRange(result.Comments.Notes);
                });

            consoleTicker.ReplaceCurrentLine("Loading Contracts");

            LoadRecords(newContext, notebooks, "\tnotebooks", consoleTicker);
            LoadRecords(newContext, notes, "\tnotes", consoleTicker);
            LoadRecords(newContext, contracts, "\tcontracts", consoleTicker);
            LoadRecords(newContext, contractItems, "\tcontracts items", consoleTicker);

            newContext.SaveChanges();

            new ContractKeySync(oldContext, consoleTicker).SyncOldModels(keySyncContracts);
            
            oldContext.ClearContext();
        }

        private static void LoadSalesOrders(RioAccessSQLEntities oldContext, RioValleyChiliDataContext newContext, ConsoleTicker consoleTicker)
        {
            var keySyncOrders = new List<SalesOrder>();
            var orders = new List<SalesOrder>();
            var inventoryShipmentOrders = new List<InventoryShipmentOrder>();
            var salesOrderItems = new List<SalesOrderItem>();
            var orderItems = new List<InventoryPickOrderItem>();
            var salesOrderPickedItems = new List<SalesOrderPickedItem>();
            var pickedItems = new List<PickedInventoryItem>();
            var pickedInventories = new List<PickedInventory>();
            var inventoryPickOrders = new List<InventoryPickOrder>();
            var shipmentInformations = new List<ShipmentInformation>();

            ProcessedBirth(new SalesOrderEntityObjectMother(newContext, oldContext, RVCDataLoadLoggerGate.SalesOrderLoadLoggerCallback), "Loading SalesOrders", consoleTicker, result =>
                {
                    if(result.RequiresKeySync)
                    {
                        keySyncOrders.Add(result);
                    }

                    orders.Add(result);
                    inventoryShipmentOrders.Add(result.InventoryShipmentOrder);
                    pickedInventories.Add(result.InventoryShipmentOrder.PickedInventory);
                    inventoryPickOrders.Add(result.InventoryShipmentOrder.InventoryPickOrder);
                    shipmentInformations.Add(result.InventoryShipmentOrder.ShipmentInformation);
                    if(result.SalesOrderItems != null)
                    {
                        foreach(var item in result.SalesOrderItems)
                        {
                            salesOrderItems.Add(item);
                            orderItems.Add(item.InventoryPickOrderItem);
                        }
                    }
                    if(result.SalesOrderPickedItems != null)
                    {
                        foreach(var item in result.SalesOrderPickedItems)
                        {
                            salesOrderPickedItems.Add(item);
                            pickedItems.Add(item.PickedInventoryItem);
                        }
                    }
                });

            consoleTicker.ReplaceCurrentLine("Loading SalesOrders");

            try
            {
                LoadRecords(newContext, orders, "\tsales orders", consoleTicker);
                LoadRecords(newContext, inventoryShipmentOrders, "\tinventory shipment orders", consoleTicker);
                LoadRecords(newContext, pickedInventories, "\tpicked inventories", consoleTicker);
                LoadRecords(newContext, inventoryPickOrders, "\tinventory pick orders", consoleTicker);
                LoadRecords(newContext, shipmentInformations, "\tshipment informations", consoleTicker);
                LoadRecords(newContext, salesOrderItems, "\tsales order items", consoleTicker);
                LoadRecords(newContext, orderItems, "\tinventory pick order items", consoleTicker);
                LoadRecords(newContext, salesOrderPickedItems, "\tsales order picked items", consoleTicker);
                LoadRecords(newContext, pickedItems, "\tpicked inventory items", consoleTicker);

                newContext.SaveChanges();
            }
            catch(Exception ex)
            {
                throw new Exception("Exception during SalesOrders loading - possibly caused by reserved PickedInventory key having being previously assigned.", ex);
            }

            new SalesOrderKeySync(oldContext, consoleTicker).SyncOldModels(keySyncOrders);
            
            oldContext.ClearContext();
        }

        private static void LoadTreatmentOrders(RioAccessSQLEntities oldContext, RioValleyChiliDataContext newContext, ConsoleTicker consoleTicker)
        {
            var treatmentOrders = new List<TreatmentOrder>();
            var inventoryShipmentOrders = new List<InventoryShipmentOrder>();
            var pickOrders = new List<InventoryPickOrder>();
            var pickOrderItems = new List<InventoryPickOrderItem>();
            var pickedInventories = new List<PickedInventory>();
            var pickedInventoryItems = new List<PickedInventoryItem>();
            var shipmentInformations = new List<ShipmentInformation>();

            ProcessedBirth(new TreatmentOrdersMother(oldContext, newContext, RVCDataLoadLoggerGate.TreatmentOrdersLoadLoggerCallback), "Loading Treatment Orders", consoleTicker, result =>
                {
                    treatmentOrders.Add(result);
                    inventoryShipmentOrders.Add(result.InventoryShipmentOrder);
                    pickOrders.Add(result.InventoryShipmentOrder.InventoryPickOrder);
                    if(result.InventoryShipmentOrder.InventoryPickOrder.Items != null)
                    {
                        pickOrderItems.AddRange(result.InventoryShipmentOrder.InventoryPickOrder.Items);
                    }
                    pickedInventories.Add(result.InventoryShipmentOrder.PickedInventory);
                    if(result.InventoryShipmentOrder.PickedInventory.Items != null)
                    {
                        pickedInventoryItems.AddRange(result.InventoryShipmentOrder.PickedInventory.Items);
                    }
                    shipmentInformations.Add(result.InventoryShipmentOrder.ShipmentInformation);
                });

            consoleTicker.ReplaceCurrentLine("Loading Treatment Orders");

            LoadRecords(newContext, treatmentOrders, "\ttreatment orders", consoleTicker);
            LoadRecords(newContext, inventoryShipmentOrders, "\tinventory shipment orders", consoleTicker);
            LoadRecords(newContext, pickOrders, "\tpick orders", consoleTicker);
            LoadRecords(newContext, pickOrderItems, "\tpick order items", consoleTicker);
            LoadRecords(newContext, pickedInventories, "\tpicked inventories", consoleTicker);
            LoadRecords(newContext, pickedInventoryItems, "\tpicked inventory items", consoleTicker);
            LoadRecords(newContext, shipmentInformations, "\tshipment informations", consoleTicker);
            
            newContext.SaveChanges();
            oldContext.ClearContext();
        }

        private static void LoadInterWarehouseOrders(RioAccessSQLEntities oldContext, RioValleyChiliDataContext newContext, ConsoleTicker consoleTicker)
        {
            var inventoryShipmentOrders = new List<InventoryShipmentOrder>();
            var pickOrders = new List<InventoryPickOrder>();
            var pickOrderItems = new List<InventoryPickOrderItem>();
            var pickedInventories = new List<PickedInventory>();
            var pickedInventoryItems = new List<PickedInventoryItem>();
            var shipmentInformations = new List<ShipmentInformation>();

            ProcessedBirth(new InterWarehouseOrdersMother(oldContext, newContext, RVCDataLoadLoggerGate.InterWarehouseOrdersLoadLoggerCallback), "Loading InterWarehouse Orders", consoleTicker, result =>
                {
                    inventoryShipmentOrders.Add(result);
                    pickOrders.Add(result.InventoryPickOrder);
                    if(result.InventoryPickOrder.Items != null)
                    {
                        pickOrderItems.AddRange(result.InventoryPickOrder.Items);
                    }
                    pickedInventories.Add(result.PickedInventory);
                    if(result.PickedInventory.Items != null)
                    {
                        pickedInventoryItems.AddRange(result.PickedInventory.Items);
                    }
                    shipmentInformations.Add(result.ShipmentInformation);
                });

            consoleTicker.ReplaceCurrentLine("Loading InterWarehouse Orders");

            LoadRecords(newContext, inventoryShipmentOrders, "\tinventory shipment orders", consoleTicker);
            LoadRecords(newContext, pickOrders, "\tpick orders", consoleTicker);
            LoadRecords(newContext, pickOrderItems, "\tpick order items", consoleTicker);
            LoadRecords(newContext, pickedInventories, "\tpicked inventories", consoleTicker);
            LoadRecords(newContext, pickedInventoryItems, "\tpicked inventory items", consoleTicker);
            LoadRecords(newContext, shipmentInformations, "\tshipment informations", consoleTicker);

            newContext.SaveChanges();
            oldContext.ClearContext();
        }

        private static void LoadIntraWarehouserOrders(RioAccessSQLEntities oldContext, RioValleyChiliDataContext newContext, ConsoleTicker consoleTicker)
        {
            var intraWarehouseOrders = new List<IntraWarehouseOrder>();
            var pickedInventories = new List<PickedInventory>();
            var pickedInventoryItems = new List<PickedInventoryItem>();

            ProcessedBirth(new IntraWarehouseOrdersMother(oldContext, newContext, RVCDataLoadLoggerGate.IntraWarehouseOrdersLoadLoggerCallback), "Loading IntraWarehouse Orders", consoleTicker, result =>
                {
                    intraWarehouseOrders.Add(result);
                    pickedInventories.Add(result.PickedInventory);
                    pickedInventoryItems.AddRange(result.PickedInventory.Items);
                });

            consoleTicker.ReplaceCurrentLine("Loading IntraWarehouse Orders");

            LoadRecords(newContext, intraWarehouseOrders, "\tintrawarehouse orders", consoleTicker);
            LoadRecords(newContext, pickedInventories, "\tpicked inventories", consoleTicker);
            LoadRecords(newContext, pickedInventoryItems, "\tpicked inventory items", consoleTicker);

            newContext.SaveChanges();
            oldContext.ClearContext();
        }

        private static void LoadLotAllowances(RioAccessSQLEntities oldContext, RioValleyChiliDataContext newContext, ConsoleTicker consoleTicker)
        {
            var customerAllowances = new List<LotCustomerAllowance>();
            var contractAllowances = new List<LotContractAllowance>();
            var customerOrderAllowances = new List<LotSalesOrderAllowance>();

            ProcessedBirth(new LotAllowancesEntityObjectMother(oldContext, newContext, RVCDataLoadLoggerGate.LotAllowancesLoadLoggerCallback), "Loading Lot Allowances", consoleTicker, result =>
                {
                    customerAllowances.AddRange(result.CustomerAllowances);
                    contractAllowances.AddRange(result.ContractAllowances);
                    customerOrderAllowances.AddRange(result.CustomerOrderAllowances);
                });

            consoleTicker.ReplaceCurrentLine("Loading Lot Allowances");

            LoadRecords(newContext, customerAllowances, "\tlot customer allowances", consoleTicker);
            LoadRecords(newContext, contractAllowances, "\tlot contract allowances", consoleTicker);
            LoadRecords(newContext, customerOrderAllowances, "\tlot customer order allowances", consoleTicker);

            newContext.SaveChanges();
            oldContext.ClearContext();
        }

        private static void LoadSampleOrders(RioAccessSQLEntities oldContext, RioValleyChiliDataContext newContext, ConsoleTicker consoleTicker)
        {
            var sampleOrders = new List<SampleOrder>();
            var sampleJournalEntries = new List<SampleOrderJournalEntry>();
            var sampleOrderItems = new List<SampleOrderItem>();
            var sampleOrderItemSpecs = new List<SampleOrderItemSpec>();
            var sampleOrderItemMatches = new List<SampleOrderItemMatch>();

            ProcessedBirth(new SampleOrderEntityObjectMother(oldContext, newContext, RVCDataLoadLoggerGate.SampleOrdersLoadLoggerCallback), "Loading Sample Orders", consoleTicker, result =>
                {
                    sampleOrders.Add(result);
                    sampleJournalEntries.AddRange(result.JournalEntries);
                    sampleOrderItems.AddRange(result.Items);
                    sampleOrderItemSpecs.AddRange(result.Items.Select(i => i.Spec).Where(s => s != null));
                    sampleOrderItemMatches.AddRange(result.Items.Select(i => i.Match).Where(m => m != null));
                });

            consoleTicker.ReplaceCurrentLine("Loading SampleOrders");

            LoadRecords(newContext, sampleOrders, "\tsample orders", consoleTicker);
            LoadRecords(newContext, sampleJournalEntries, "\tsample order journal entries", consoleTicker);
            LoadRecords(newContext, sampleOrderItems, "\tsample order items", consoleTicker);
            LoadRecords(newContext, sampleOrderItemSpecs, "\tsample order item specs", consoleTicker);
            LoadRecords(newContext, sampleOrderItemMatches, "\tsample order item matches", consoleTicker);

            newContext.SaveChanges();
            oldContext.ClearContext();
        }

        private static void LoadSalesQuotes(RioAccessSQLEntities oldContext, RioValleyChiliDataContext newContext, ConsoleTicker consoleTicker)
        {
            var salesQuotes = new List<SalesQuote>();
            var shipmentInformations = new List<ShipmentInformation>();
            var salesQuoteItems = new List<SalesQuoteItem>();

            ProcessedBirth(new SalesQuoteEntityObjectMother(newContext, oldContext, RVCDataLoadLoggerGate.SalesQuoteLoadLoggerCallback), "Loading Sales Quotes", consoleTicker, result =>
                {
                    salesQuotes.Add(result);
                    shipmentInformations.Add(result.ShipmentInformation);
                    salesQuoteItems.AddRange(result.Items);
                });

            consoleTicker.ReplaceCurrentLine("Loading Sales Quotes");

            LoadRecords(newContext, salesQuotes, "\tsales quotes", consoleTicker);
            LoadRecords(newContext, shipmentInformations, "\tshipment informations", consoleTicker);
            LoadRecords(newContext, salesQuoteItems, "\tsales quote items", consoleTicker);

            newContext.SaveChanges();
            oldContext.ClearContext();
        }
    }
}