using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using RioValleyChili.Core;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Data.DataSeeders.Logging;
using RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers;
using RioValleyChili.Data.DataSeeders.Mothers.PocoMothers;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.Helpers;
using RioValleyChili.Data.Initialization;

namespace RioValleyChili.Data.DataSeeders
{
    public class CoreDbContextDataSeeder : DbContextDataSeederBase<RioValleyChiliDataContext>
    {
        public override void SeedContext(RioValleyChiliDataContext newContext)
        {
            var consoleTicker = new ConsoleTicker();

            using(var oldContext = ContextsHelper.GetOldContext())
            {
                InitializeOldContext();

                var proxyCreation = newContext.Configuration.ProxyCreationEnabled;
                var autoDetecChanges = newContext.Configuration.AutoDetectChangesEnabled;
                var lazyLoading = newContext.Configuration.LazyLoadingEnabled;

                try
                {
                    newContext.Configuration.ProxyCreationEnabled = false;
                    newContext.Configuration.AutoDetectChangesEnabled = false;
                    newContext.Configuration.LazyLoadingEnabled = false;

                    CreateOldContextRecords(oldContext);

                    LoadRecords(newContext, new EmployeeObjectMother(oldContext, RVCDataLoadLoggerGate.EmployeeLoadLoggerCallback), "Loading Employees", consoleTicker);
                    LoadRecords(newContext, new NotebookMother(RVCDataLoadLoggerGate.LogSummaryEntry("Notebooks")), "Loading Notebooks", consoleTicker);
                    LoadRecords(newContext, new ChileTypeMother(), "Loading Chile Types", consoleTicker);
                    LoadRecords(newContext, new AdditiveTypeMother(), "Loading Additive Types", consoleTicker);
                    LoadRecords(newContext, new WorkTypeMother(), "Loading Work Types", consoleTicker);
                    LoadRecords(newContext, new AttributeNameMother(RVCDataLoadLoggerGate.LogSummaryEntry("Attribute Names")), "Loading Attribute Names", consoleTicker);
                    LoadRecords(newContext, new InventoryTreatmentMother(oldContext, RVCDataLoadLoggerGate.InventoryTreatmentLoadLoggerCallback), "Loading Inventory Treatment Types", consoleTicker);
                    LoadRecords(newContext, new InventoryTreatmentForAttributeMother(), "Loading inventory treatment valdiation rules", consoleTicker);

                    var message = "Loading Facilities";
                    var facilities = new FacilityEntityObjectMother(oldContext, newContext, RVCDataLoadLoggerGate.WarehouseLoadLoggerCallback).BirthAll(() => consoleTicker.TickConsole(message + "...")).ToList();
                    consoleTicker.ReplaceCurrentLine(message);
                    LoadRecords(newContext, facilities, "\tfacilities", consoleTicker);
                    LoadRecords(newContext, facilities.SelectMany(w => w.Locations), "\tlocations", consoleTicker);

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

        private void CreateOldContextRecords(RioAccessSQLEntities oldContext)
        {
            if(oldContext.tblLotStatus.FirstOrDefault(s => s.LotStatID == (decimal) LotStat.Rejected) == null)
            {
                oldContext.tblLotStatus.AddObject(new tblLotStatu
                    {
                        LotStatID = (int) LotStat.Rejected,
                        LotStat = "Rejected",
                        LotStatDesc = "Rejected",
                        EmployeeID = 99,
                        EntryDate = DateTime.Now.RoundMillisecondsForSQL(),
                        InActive = false,
                        SortOrder = "5a",
                        LotStatType = "QA",
                        RptStatus = "Fail",
                        Avail4Pick = false,
                        Avail2Batch = false
                    });
                oldContext.SaveChanges();
                Console.WriteLine("Added 'Rejected' tblLotStatus record.");
            }
        }

        private void InitializeOldContext()
        {
            var addColumnsScript = GetAddColumnsScript();
            
            if(addColumnsScript != null)
            {
                var originalEntityConnection = ContextsHelper.GetOldContextConnection();
                if(originalEntityConnection != null)
                {
                    using(originalEntityConnection)
                    using(var connection = new SqlConnection(originalEntityConnection.StoreConnection.ConnectionString))
                    {
                        connection.Open();
                        AddColumns(connection, addColumnsScript);
                        connection.Close();
                    }
                }
            }
        }

        private static string GetAddColumnsScript()
        {
            var assembly = Assembly.GetExecutingAssembly();
            const string scriptName = "Add RioAccessSQL Columns.sql";
            var resourceName = assembly.GetManifestResourceNames().FirstOrDefault(p => p.EndsWith(scriptName));
            if(resourceName == null)
            {
                Console.WriteLine("Script[{0}] not found.", scriptName);
                return null;
            }

            string script;
            using(var stream = assembly.GetManifestResourceStream(resourceName))
            using(var reader = new StreamReader(stream))
            {
                script = reader.ReadToEnd();
            }

            return script;
        }

        private void AddColumns(SqlConnection connection, string addColumnsString)
        {
            if(string.IsNullOrWhiteSpace(addColumnsString))
            {
                return;
            }

            Console.WriteLine("Running add columns script on database[{0}]", connection.Database);

            new SqlCommand(string.Format("USE {0}\n{1}", connection.Database, addColumnsString), connection).ExecuteNonQuery();
        }

        private static void CreateViews(SqlConnection connection)
        {
            var command = new SqlCommand(
                    @"SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[ViewInventoryLoad]')", connection);
            var readerResults = command.ExecuteReader();
            var hasRows = readerResults.HasRows;
            readerResults.Close();
            if(hasRows)
            {
                new SqlCommand(@"DROP VIEW [dbo].[ViewInventoryLoad]", connection).ExecuteNonQuery();
            }
                
            new SqlCommand(@"CREATE VIEW [dbo].[ViewInventoryLoad] AS SELECT
            dbo.ViewInOutWithPending.Lot, SUM(dbo.ViewInOutWithPending.Qty) AS Quantity, SUM(dbo.ViewInOutWithPending.Wgt) AS TtlWgt, 
            dbo.ViewInOutWithPending.NetWgt, dbo.ViewInOutWithPending.PkgID, dbo.ViewInOutWithPending.TrtmtID, dbo.ViewInOutWithPending.LocID, 
            ISNULL(dbo.ViewInOutWithPending.Tote, '') AS Tote, dbo.tblLot.ProdID
            FROM dbo.ViewInOutWithPending INNER JOIN
            dbo.tblLot ON dbo.ViewInOutWithPending.Lot = dbo.tblLot.Lot
            GROUP BY dbo.ViewInOutWithPending.Lot, dbo.ViewInOutWithPending.NetWgt, dbo.ViewInOutWithPending.PkgID, dbo.ViewInOutWithPending.TrtmtID, 
            dbo.ViewInOutWithPending.LocID, ISNULL(dbo.ViewInOutWithPending.Tote, ''), dbo.tblLot.ProdID", connection)
                .ExecuteNonQuery();

            command =
                new SqlCommand(
                    @"SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[ViewInventoryLoadSelected]')",
                    connection);
            readerResults = command.ExecuteReader();
            hasRows = readerResults.HasRows;
            readerResults.Close();
            if(!hasRows)
            {
                new SqlCommand(
                    @"CREATE VIEW [dbo].[ViewInventoryLoadSelected] AS SELECT dbo.ViewInventoryLoad.* FROM dbo.ViewInventoryLoad",
                    connection).ExecuteNonQuery();
            }
        }
    }
}
