using System;
using RioValleyChili.Data.DataSeeders.Mothers.Base;
using RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Logging
{
    public static class RVCDataLoadLoggerGate
    {
        public static IRVCDataLoadLogger RVCDataLoadLogger { get; set; }

        public static Action<EmployeeObjectMother.CallbackParameters> EmployeeLoadLoggerCallback
        {
            get { return GetCallback(r => r.EmployeeLoadLogger); }
        }

        public static Action<InventoryTreatmentMother.CallbackParameters> InventoryTreatmentLoadLoggerCallback
        {
            get { return GetCallback(r => r.InventoryTreatmentLoadLogger); }
        }

        public static Action<FacilityEntityObjectMother.CallbackParameters> WarehouseLoadLoggerCallback
        {
            get { return GetCallback(r => r.FacilityLoadLogger); }
        }

        public static Action<CompanyAndContactsMother.CallbackParameters> CompanyAndContactsLoadLoggerCallback
        {
            get { return GetCallback(r => r.CompanyAndContactsLoadLogger); }
        }

        public static Action<NonInventoryProductEntityObjectMother.CallbackParameters> NonInventoryProductLoadLoggerCallback
        {
            get { return GetCallback(r => r.NonInventoryProductsLoadLogger); }
        }

        public static Action<ChileProductEntityObjectMother.CallbackParameters> ChileProductLoadLoggerCallback
        {
            get { return GetCallback(r => r.ChileProductsLoadLogger); }
        }

        public static Action<ChileProductIngredientsMother.CallbackParameters> ChileProductIngredientsLoadLoggerCallback
        {
            get { return GetCallback(r => r.ChileProductIngredientLoadLogger); }
        }

        public static Action<PackagingProductEntityObjectMother.CallbackParameters> PackagingProductLoadLoggerCallback
        {
            get { return GetCallback(r => r.PackagingProductLoadLogger); }
        }

        public static Action<AdditiveProductEntityObjectMother.CallbackParameters> AdditiveProductLoadLoggerCallback
        {
            get { return GetCallback(r => r.AdditiveProductLoadLogger); }
        }

        public static Action<CustomerProductSpecMother.CallbackParameters> CustomerSpecsLoadLoggerCallback
        {
            get { return GetCallback(r => r.CustomerSpecsLoadLogger); }
        }

        public static Action<CustomerProductCodeMother.CallbackParameters> CustomerProductCodeLoadLoggerCallback
        {
            get { return GetCallback(r => r.CustomerProductCodeLoadLogger); }
        }

        public static Action<AdditiveLotInventoryEntityObjectMother.CallbackParameters> AdditiveLotInventoryLoadLogger
        {
            get { return GetCallback(r => r.AdditiveLotInventoryLoadLogger); }
        }

        public static Action<PackagingLotInventoryItemEntityObjectMother.CallbackParameters> PackagingLotInventoryLoadLogger
        {
            get { return GetCallback(r => r.PackagingLotInventoryLoadLogger); }
        }

        public static Action<WIPChileLotInventoryItemEntityObjectMother.CallbackParameters> WIPChileLotInventoryLoadLogger
        {
            get { return GetCallback(r => r.WIPChileLotInventoryLoadLogger); }
        }

        public static Action<FinishedGoodsChileLotInventoryItemEntityObjectMother.CallbackParameters> FinishedGoodsChileLotInventoryLoadLogger
        {
            get { return GetCallback(r => r.FinishedGoodsChileLotInventoryLoadLogger); }
        }

        public static Action<DehyChileLotInventoryItemEntityObjectMother.CallbackParameters> DehyChileLotInventoryLoadLogger
        {
            get { return GetCallback(r => r.DehyChileLotInventoryLoadLogger); }
        }

        public static Action<RawChileLotInventoryItemEntityObjectMother.CallbackParameters> RawChileLotInventoryLoadLogger
        {
            get { return GetCallback(r => r.RawChileLotInventoryLoadLogger); }
        }

        public static Action<GRPChileLotInventoryItemEntityObjectMother.CallbackParameters> GRPChileLotInventoryLoadLogger
        {
            get { return GetCallback(r => r.GRPChileLotInventoryLoadLogger); }
        }

        public static Action<OtherChileLotInventoryItemEntityObjectMother.CallbackParameters> OtherChileLotInventoryLoadLogger
        {
            get { return GetCallback(r => r.OtherChileLotInventoryLoadLogger); }
        }

        public static Action<LotEntityObjectMother.CallbackParameters> LotLoadLoggerCallback
        {
            get { return GetCallback(r => r.LotLoadLogger); }
        }

        public static Action<LotHistoryEntityObjectMother.CallbackParameters> LotHistoryLoggerCallback
        {
            get { return GetCallback(r => r.LotHistoryLogger); }
        }

        public static Action<InventoryAdjustmentsMother.CallbackParameters> InventoryAdjustmentsLoadLoggerCallback
        {
            get { return GetCallback(r => r.InventoryAdjustmentsLoadLogger); }
        }

        public static Action<PackScheduleEntityObjectMother.CallbackParameters> PackScheduleLoadLoggerCallback
        {
            get { return GetCallback(r => r.PackScheduleLoadLogger); }
        }

        public static Action<ProductionBatchEntityObjectMother.CallbackParameters> ProductionBatchLoadLoggerCallback
        {
            get { return GetCallback(r => r.ProductionBatchLoadLogger); }
        }

        public static Action<ProductionScheduleEntityObjectMother.CallbackParameters> ProductionScheduleLoadLoggerCallback
        {
            get { return GetCallback(r => r.ProductionScheduleLoadLogger); }
        }

        public static Action<InstructionEntityObjectMother.CallbackParameters> InstructionLoadLoggerCallback
        {
            get { return GetCallback(r => r.InstructionLoadLogger); }
        }

        public static Action<ChileMaterialsReceivedEntityMother.CallbackParameters> DehydratedMaterialsReceivedLoadLoggerCallback
        {
            get { return GetCallback(r => r.DehydratedMaterialsReceivedLoadLogger); }
        }

        public static Action<MillAndWetdownMother.CallbackParameters> MillAndWetdownLoadLoggerCallback
        {
            get { return GetCallback(r => r.MillAndWetdownLoadLogger); }
        }

        public static Action<MovementOrdersMotherBase<TreatmentOrder>.CallbackParameters> TreatmentOrdersLoadLoggerCallback
        {
            get { return GetCallback(r => r.TreatmentOrdersLoadLogger); }
        }

        public static Action<MovementOrdersMotherBase<InventoryShipmentOrder>.CallbackParameters> InterWarehouseOrdersLoadLoggerCallback
        {
            get { return GetCallback(r => r.InterWarehouseOrdersLoadLogger); }
        }

        public static Action<IntraWarehouseOrdersMother.CallbackParameters> IntraWarehouseOrdersLoadLoggerCallback
        {
            get { return GetCallback(r => r.IntraWarehouseOrdersLoadLogger); }
        }

        public static Action<ContractEntityObjectMother.CallbackParameters> ContractEntityObjectLoadLoggerCallback
        {
            get { return GetCallback(r => r.ContractLoadLogger); }
        }

        public static Action<SalesOrderEntityObjectMother.CallbackParameters> SalesOrderLoadLoggerCallback
        {
            get { return GetCallback(r => r.SalesOrderLoadLogger); }
        }

        public static Action<InventoryTransactionsMother.CallbackParameters> InventoryTransactionsLoadLoggerCallback
        {
            get { return GetCallback(r => r.InventoryTransactionsLoadLogger); }
        }

        public static Action<LotAllowancesEntityObjectMother.CallbackParameters> LotAllowancesLoadLoggerCallback
        {
            get { return GetCallback(r => r.LotAllowancesLoadLogger); }
        }

        public static Action<SampleOrderEntityObjectMother.CallbackParameters> SampleOrdersLoadLoggerCallback
        {
            get { return GetCallback(r => r.SampleOrdersLoadLogger); }
        }

        public static Action<SalesQuoteEntityObjectMother.CallbackParameters> SalesQuoteLoadLoggerCallback
        {
            get { return GetCallback(r => r.SalesQuoteLoadLogger); }
        }

        public static Action<string> LogSummaryEntry(string logName)
        {
            return RVCDataLoadLogger != null ? RVCDataLoadLogger.LogSummaryMessage(logName) : null;
        }

        private static Action<TCallbackParameters> GetCallback<TCallbackParameters>(Func<IRVCDataLoadLogger, ILoggerCallback<TCallbackParameters>> selectLogger)
        {
            if(selectLogger == null || RVCDataLoadLogger == null)
            {
                return null;
            }

            var logger = selectLogger(RVCDataLoadLogger);
            return logger == null ? null : logger.Callback;
        }
    }
}