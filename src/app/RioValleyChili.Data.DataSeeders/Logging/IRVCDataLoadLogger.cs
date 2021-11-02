using System;
using RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers;

namespace RioValleyChili.Data.DataSeeders.Logging
{
    public interface IRVCDataLoadLogger
    {
        ILoggerCallback<EmployeeObjectMother.CallbackParameters> EmployeeLoadLogger { get; }

        ILoggerCallback<InventoryTreatmentMother.CallbackParameters> InventoryTreatmentLoadLogger { get; }
            
        ILoggerCallback<FacilityEntityObjectMother.CallbackParameters> FacilityLoadLogger { get; }

        ILoggerCallback<CompanyAndContactsMother.CallbackParameters> CompanyAndContactsLoadLogger { get; }

        ILoggerCallback<NonInventoryProductEntityObjectMother.CallbackParameters> NonInventoryProductsLoadLogger { get; }
            
        ILoggerCallback<ChileProductEntityObjectMother.CallbackParameters> ChileProductsLoadLogger { get; }

        ILoggerCallback<ChileProductIngredientsMother.CallbackParameters> ChileProductIngredientLoadLogger { get; }

        ILoggerCallback<PackagingProductEntityObjectMother.CallbackParameters> PackagingProductLoadLogger { get; }

        ILoggerCallback<AdditiveProductEntityObjectMother.CallbackParameters> AdditiveProductLoadLogger { get; }

        ILoggerCallback<CustomerProductSpecMother.CallbackParameters> CustomerSpecsLoadLogger { get; }

        ILoggerCallback<CustomerProductCodeMother.CallbackParameters> CustomerProductCodeLoadLogger { get; }
            
        ILoggerCallback<AdditiveLotInventoryEntityObjectMother.CallbackParameters> AdditiveLotInventoryLoadLogger { get; }

        ILoggerCallback<PackagingLotInventoryItemEntityObjectMother.CallbackParameters> PackagingLotInventoryLoadLogger { get; }

        ILoggerCallback<WIPChileLotInventoryItemEntityObjectMother.CallbackParameters> WIPChileLotInventoryLoadLogger { get; }

        ILoggerCallback<FinishedGoodsChileLotInventoryItemEntityObjectMother.CallbackParameters> FinishedGoodsChileLotInventoryLoadLogger { get; }

        ILoggerCallback<DehyChileLotInventoryItemEntityObjectMother.CallbackParameters> DehyChileLotInventoryLoadLogger { get; }

        ILoggerCallback<RawChileLotInventoryItemEntityObjectMother.CallbackParameters> RawChileLotInventoryLoadLogger { get; }

        ILoggerCallback<GRPChileLotInventoryItemEntityObjectMother.CallbackParameters> GRPChileLotInventoryLoadLogger { get; }

        ILoggerCallback<OtherChileLotInventoryItemEntityObjectMother.CallbackParameters> OtherChileLotInventoryLoadLogger { get; }

        ILoggerCallback<LotEntityObjectMother.CallbackParameters> LotLoadLogger { get; }

        ILoggerCallback<LotHistoryEntityObjectMother.CallbackParameters> LotHistoryLogger { get; }

        ILoggerCallback<InventoryAdjustmentsMother.CallbackParameters> InventoryAdjustmentsLoadLogger { get; }

        ILoggerCallback<PackScheduleEntityObjectMother.CallbackParameters> PackScheduleLoadLogger { get; }

        ILoggerCallback<ProductionBatchEntityObjectMother.CallbackParameters> ProductionBatchLoadLogger { get; }

        ILoggerCallback<ProductionScheduleEntityObjectMother.CallbackParameters> ProductionScheduleLoadLogger { get; }

        ILoggerCallback<InstructionEntityObjectMother.CallbackParameters> InstructionLoadLogger { get; }
            
        ILoggerCallback<ChileMaterialsReceivedEntityMother.CallbackParameters> DehydratedMaterialsReceivedLoadLogger { get; }

        ILoggerCallback<MillAndWetdownMother.CallbackParameters> MillAndWetdownLoadLogger { get; }

        ILoggerCallback<TreatmentOrdersMother.CallbackParameters> TreatmentOrdersLoadLogger { get; }

        ILoggerCallback<InterWarehouseOrdersMother.CallbackParameters> InterWarehouseOrdersLoadLogger { get; }

        ILoggerCallback<IntraWarehouseOrdersMother.CallbackParameters> IntraWarehouseOrdersLoadLogger { get; }

        ILoggerCallback<ContractEntityObjectMother.CallbackParameters> ContractLoadLogger { get; }

        ILoggerCallback<SalesOrderEntityObjectMother.CallbackParameters> SalesOrderLoadLogger { get; }

        ILoggerCallback<InventoryTransactionsMother.CallbackParameters> InventoryTransactionsLoadLogger { get; }

        ILoggerCallback<LotAllowancesEntityObjectMother.CallbackParameters> LotAllowancesLoadLogger { get; }

        ILoggerCallback<SampleOrderEntityObjectMother.CallbackParameters> SampleOrdersLoadLogger { get; }

        ILoggerCallback<SalesQuoteEntityObjectMother.CallbackParameters> SalesQuoteLoadLogger { get; }
            
        Action<string> LogSummaryMessage(string logName);

        void WriteLogSummary();

        void WriteException(Exception exception);

        string LogPath { get; }

        string DataLoadSummaryPath { get; }
    }
}
