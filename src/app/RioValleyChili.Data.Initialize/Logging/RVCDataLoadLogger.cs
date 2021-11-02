using System;
using System.IO;
using RioValleyChili.Data.DataSeeders.Logging;
using RioValleyChili.Data.DataSeeders.Mothers.Base;
using RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.Initialize.Logging
{
    public class RVCDataLoadLogger: IRVCDataLoadLogger
    {
        public static RVCDataLoadLogger GetDataLoadLogger(string logsPath)
        {
            if(string.IsNullOrWhiteSpace(logsPath))
            {
                logsPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            }

            var dateString = DateTime.Now.ToString("yyyyMMdd");
            var directoryIndex = 0;
            string newDirectory;
            do
            {
                newDirectory = string.Format("{0}\\{1}-{2}", logsPath, dateString, directoryIndex++);
            } while(Directory.Exists(newDirectory));
            Directory.CreateDirectory(newDirectory);

            Console.WriteLine("Data Log Location: {0}", newDirectory);
            return new RVCDataLoadLogger(newDirectory);
        }

        public ILoggerCallback<EmployeeObjectMother.CallbackParameters> EmployeeLoadLogger
        {
            get { return new EmployeeLogger(GetLogFilePath("employees.txt")); }
        }

        public ILoggerCallback<InventoryTreatmentMother.CallbackParameters> InventoryTreatmentLoadLogger
        {
            get { return new InventoryTreatmentLogger(GetLogFilePath("inventoryTreatments.txt")); }
        }

        public ILoggerCallback<FacilityEntityObjectMother.CallbackParameters> FacilityLoadLogger
        {
            get { return new FacilityLogger(GetLogFilePath("facilities.txt")); }
        }

        public ILoggerCallback<CompanyAndContactsMother.CallbackParameters> CompanyAndContactsLoadLogger
        {
            get { return new CompanyAndContactsLogger(GetLogFilePath("companyAndContacts.txt")); }
        }

        public ILoggerCallback<NonInventoryProductEntityObjectMother.CallbackParameters> NonInventoryProductsLoadLogger
        {
            get { return new NonInventoryProductLogger(GetLogFilePath("nonInventoryProducts.txt")); }
        }

        public ILoggerCallback<ChileProductEntityObjectMother.CallbackParameters> ChileProductsLoadLogger
        {
            get { return new ChileProductLogger(GetLogFilePath("chileProducts.txt")); }
        }

        public ILoggerCallback<ChileProductIngredientsMother.CallbackParameters> ChileProductIngredientLoadLogger
        {
            get { return new ChileProductIngredientsLogger(GetLogFilePath("chileProductIngredients.txt")); }
        }

        public ILoggerCallback<PackagingProductEntityObjectMother.CallbackParameters> PackagingProductLoadLogger
        {
            get { return new PackagingProductLogger(GetLogFilePath("packagingProducts.txt")); }
        }

        public ILoggerCallback<AdditiveProductEntityObjectMother.CallbackParameters> AdditiveProductLoadLogger
        {
            get { return new AdditiveProductLogger(GetLogFilePath("additiveProducts.txt")); }
        }

        public ILoggerCallback<CustomerProductSpecMother.CallbackParameters> CustomerSpecsLoadLogger
        {
            get { return new CustomerSpecsLogger(GetLogFilePath("customerSpecs.txt")); }
        }

        public ILoggerCallback<CustomerProductCodeMother.CallbackParameters> CustomerProductCodeLoadLogger
        {
            get { return new CustomerProductCodesLogger(GetLogFilePath("customerProductCodes.txt")); }
        }

        public ILoggerCallback<LotInventoryEntityObjectMotherBase<AdditiveLot>.CallbackParameters> AdditiveLotInventoryLoadLogger
        {
            get { return new LotInventoryLogger<AdditiveLot>(GetLogFilePath("inventoryAdditive.txt"), "InventoryAdditiveLogger"); }
        }

        public ILoggerCallback<LotInventoryEntityObjectMotherBase<PackagingLot>.CallbackParameters> PackagingLotInventoryLoadLogger
        {
            get { return new LotInventoryLogger<PackagingLot>(GetLogFilePath("inventoryPackaging.txt"), "InventoryPackagingLogger"); }
        }

        public ILoggerCallback<LotInventoryEntityObjectMotherBase<ChileLot>.CallbackParameters> WIPChileLotInventoryLoadLogger
        {
            get { return new LotInventoryLogger<ChileLot>(GetLogFilePath("inventoryChileWIP.txt"), "InventoryChileWIPLogger"); }
        }

        public ILoggerCallback<LotInventoryEntityObjectMotherBase<ChileLot>.CallbackParameters> FinishedGoodsChileLotInventoryLoadLogger
        {
            get { return new LotInventoryLogger<ChileLot>(GetLogFilePath("inventoryChileFG.txt"), "InventoryChileFGLogger"); }
        }

        public ILoggerCallback<LotInventoryEntityObjectMotherBase<ChileLot>.CallbackParameters> DehyChileLotInventoryLoadLogger
        {
            get { return new LotInventoryLogger<ChileLot>(GetLogFilePath("inventoryChileDehydrated.txt"), "InventoryChileDehydratedLogger"); }
        }

        public ILoggerCallback<LotInventoryEntityObjectMotherBase<ChileLot>.CallbackParameters> RawChileLotInventoryLoadLogger
        {
            get { return new LotInventoryLogger<ChileLot>(GetLogFilePath("inventoryChileRaw.txt"), "InventoryChileRawLogger"); }
        }

        public ILoggerCallback<LotInventoryEntityObjectMotherBase<ChileLot>.CallbackParameters> GRPChileLotInventoryLoadLogger
        {
            get { return new LotInventoryLogger<ChileLot>(GetLogFilePath("inventoryChileGRP.txt"), "InventoryChileGRPLogger"); }
        }

        public ILoggerCallback<LotInventoryEntityObjectMotherBase<ChileLot>.CallbackParameters> OtherChileLotInventoryLoadLogger
        {
            get { return new LotInventoryLogger<ChileLot>(GetLogFilePath("inventoryChileOther.txt"), "InventoryChileOtherLogger"); }
        }

        public ILoggerCallback<LotEntityObjectMother.CallbackParameters> LotLoadLogger
        {
            get { return new LotLogger(GetLogFilePath("lots.txt")); }
        }

        public ILoggerCallback<LotHistoryEntityObjectMother.CallbackParameters> LotHistoryLogger
        {
            get { return new LotHistoryLogger(GetLogFilePath("lotHistories.txt")); }
        }

        public ILoggerCallback<InventoryAdjustmentsMother.CallbackParameters> InventoryAdjustmentsLoadLogger
        {
            get { return new InventoryAdjustmentsLogger(GetLogFilePath("inventoryAdjustments.txt")); }
        }

        public ILoggerCallback<PackScheduleEntityObjectMother.CallbackParameters> PackScheduleLoadLogger
        {
            get { return new PackScheduleLogger(GetLogFilePath("packSchedules.txt")); }
        }

        public ILoggerCallback<ProductionBatchEntityObjectMother.CallbackParameters> ProductionBatchLoadLogger
        {
            get { return new ProductionBatchLogger(GetLogFilePath("productionBatches.txt")); }
        }

        public ILoggerCallback<ProductionScheduleEntityObjectMother.CallbackParameters> ProductionScheduleLoadLogger
        {
            get { return new ProductionScheduleLogger(GetLogFilePath("productionSchedules.txt")); }
        }

        public ILoggerCallback<InstructionEntityObjectMother.CallbackParameters> InstructionLoadLogger
        {
            get { return new InstructionLogger(GetLogFilePath("instructions.txt")); }
        }

        public ILoggerCallback<ChileMaterialsReceivedEntityMother.CallbackParameters> DehydratedMaterialsReceivedLoadLogger
        {
            get { return new ChileMaterialsReceivedLogger(GetLogFilePath("chileMaterialsReceived.txt")); }
        }

        public ILoggerCallback<MillAndWetdownMother.CallbackParameters> MillAndWetdownLoadLogger
        {
            get { return new MillAndWetdownLogger(GetLogFilePath("millAndWetdown.txt")); }
        }

        public ILoggerCallback<MovementOrdersMotherBase<TreatmentOrder>.CallbackParameters> TreatmentOrdersLoadLogger
        {
            get { return new MovementOrdersLogger<TreatmentOrder>(GetLogFilePath("treatmentOrders.txt")); }
        }

        public ILoggerCallback<MovementOrdersMotherBase<InventoryShipmentOrder>.CallbackParameters> InterWarehouseOrdersLoadLogger
        {
            get { return new MovementOrdersLogger<InventoryShipmentOrder>(GetLogFilePath("interWarehouseOrders.txt")); }
        }

        public ILoggerCallback<IntraWarehouseOrdersMother.CallbackParameters> IntraWarehouseOrdersLoadLogger
        {
            get { return new IntraWarehouseOrdersLogger(GetLogFilePath("intraWarehouseOrders.txt")); }
        }

        public ILoggerCallback<ContractEntityObjectMother.CallbackParameters> ContractLoadLogger
        {
            get { return new ContractLogger(GetLogFilePath("contracts.txt")); }
        }
        
        public ILoggerCallback<SalesOrderEntityObjectMother.CallbackParameters> SalesOrderLoadLogger
        {
            get { return new SalesOrderLogger(GetLogFilePath("salesOrders.txt")); }
        }

        public ILoggerCallback<InventoryTransactionsMother.CallbackParameters> InventoryTransactionsLoadLogger
        {
            get { return new InventoryTransactionsLogger(GetLogFilePath("inventoryTransactions.txt")); }
        }

        public ILoggerCallback<LotAllowancesEntityObjectMother.CallbackParameters> LotAllowancesLoadLogger
        {
            get { return new LotAllowancesLogger(GetLogFilePath("lotAllowances.txt")); }
        }

        public ILoggerCallback<SampleOrderEntityObjectMother.CallbackParameters> SampleOrdersLoadLogger
        {
            get { return new SampleOrdersLoadLogger(GetLogFilePath("sampleOrders.txt")); }
        }

        public ILoggerCallback<SalesQuoteEntityObjectMother.CallbackParameters> SalesQuoteLoadLogger
        {
            get { return new SalesQuoteLoadLogger(GetLogFilePath("salesQuotes.txt")); }
        }

        public Action<string> LogSummaryMessage(string logName)
        {
            return l => EntityLoggerBase.LogSummaryMessage(logName, l);
        }

        public void WriteLogSummary()
        {
            EntityLoggerBase.WriteLogSummary(DataLoadSummaryPath);
        }

        public void WriteException(Exception exception)
        {
            EntityLoggerBase.WriteException(exception, ExceptionPath);
        }

        public string LogPath { get; private set; }
        public string DataLoadSummaryPath { get; private set; }
        public string ExceptionPath { get; private set; }

        private RVCDataLoadLogger(string filePath)
        {
            LogPath = filePath;
            DataLoadSummaryPath = GetLogFilePath("_DataLoadSummary.txt");
            ExceptionPath = GetLogFilePath("_Exception.txt");
        }

        private string GetLogFilePath(string fileName)
        {
            return string.Format("{0}\\{1}", LogPath, fileName);
        }
    }
}