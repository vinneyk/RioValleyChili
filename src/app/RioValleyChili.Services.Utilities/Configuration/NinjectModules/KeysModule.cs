using Ninject.Modules;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using Solutionhead.EntityKey;

namespace RioValleyChili.Services.Utilities.Configuration.NinjectModules
{
    internal class KeysModule : NinjectModule
    {
        #region Overrides of NinjectModule

        public override void Load()
        {
            RegisterKey<IProductKey, ProductKey>();
            RegisterKey<IAdditiveProductKey, AdditiveProductKey>();
            RegisterKey<IAdditiveTypeKey, AdditiveTypeKey>();
            RegisterKey<IChileProductIngredientKey, ChileProductIngredientKey>();
            RegisterKey<IChileProductKey, ChileProductKey>();
            RegisterKey<IChileProductAttributeRangeKey, ChileProductAttributeRangeKey>();
            RegisterKey<IChileTypeKey, ChileTypeKey>();
            RegisterKey<IEmployeeKey, EmployeeKey>();
            RegisterKey<IInstructionKey, InstructionKey>();
            RegisterKey<IInventoryKey, InventoryKey>();
            RegisterKey<IInventoryAdjustmentKey, InventoryAdjustmentKey>();
            RegisterKey<ILotKey, LotKey>();
            RegisterKey<IPackagingProductKey, PackagingProductKey>();
            RegisterKey<IPackScheduleKey, PackScheduleKey>();
            RegisterKey<ILotProductionResultItemKey, LotProductionResultItemKey>();
            RegisterKey<IProductionScheduleKey, ProductionScheduleKey>();
            RegisterKey<IFacilityKey, FacilityKey>();
            RegisterKey<ILocationKey, LocationKey>();
            RegisterKey<IInventoryTreatmentKey, InventoryTreatmentKey>();
            RegisterKey<IWorkTypeKey, WorkTypeKey>();
            RegisterKey<IPickedInventoryKey, PickedInventoryKey>();
            RegisterKey<IPickedInventoryItemKey, PickedInventoryItemKey>();
            RegisterKey<IInventoryPickOrderKey, InventoryPickOrderKey>();
            RegisterKey<IInventoryPickOrderItemKey, InventoryPickOrderItemKey>();
            RegisterKey<ITreatmentOrderKey, TreatmentOrderKey>();
            RegisterKey<IIntraWarehouseOrderKey, IntraWarehouseOrderKey>();
            RegisterKey<IShipmentInformationKey, ShipmentInformationKey>();
            RegisterKey<ILotDefectKey, LotDefectKey>();
            RegisterKey<ILotAttributeDefectKey, LotAttributeDefectKey>();
            RegisterKey<ILotDefectResolutionKey, LotDefectResolutionKey>();
            RegisterKey<IAttributeNameKey, AttributeNameKey>();
            RegisterKey<IChileMaterialsReceivedItemKey, ChileMaterialsReceivedItemKey>();
            RegisterKey<ICompanyKey, CompanyKey>();
            RegisterKey<IContactKey, ContactKey>();
            RegisterKey<IContactAddressKey, ContactAddressKey>();
            RegisterKey<ICustomerKey, CustomerKey>();
            RegisterKey<ICustomerNoteKey, CustomerNoteKey>();
            RegisterKey<ICustomerProductCodeKey, CustomerProductCodeKey>();
            RegisterKey<ICustomerProductAttributeRangeKey, CustomerProductAttributeRangeKey>();
            RegisterKey<IContractKey, ContractKey>();
            RegisterKey<IContractItemKey, ContractItemKey>();
            RegisterKey<ISalesOrderKey, SalesOrderKey>();
            RegisterKey<ISalesOrderItemKey, SalesOrderItemKey>();
            RegisterKey<ISalesOrderPickedItemKey, SalesOrderPickedItemKey>();
            RegisterKey<INotebookKey, NotebookKey>();
            RegisterKey<INoteKey, NoteKey>();
            RegisterKey<IInventoryShipmentOrderKey, InventoryShipmentOrderKey>();
            RegisterKey<IInventoryTransactionKey, InventoryTransactionKey>();
            RegisterKey<ISampleOrderKey, SampleOrderKey>();
            RegisterKey<ISampleOrderItemKey, SampleOrderItemKey>();
            RegisterKey<ISampleOrderJournalEntryKey, SampleOrderJournalEntryKey>();
            RegisterKey<ISalesQuoteKey, SalesQuoteKey>();
            RegisterKey<ISalesQuoteItemKey, SalesQuoteItemKey>();
        }

        #endregion

        private void RegisterKey<TKey, TKeyObject>()
            where TKey : class
            where TKeyObject : class, IKeyParser<TKey>, IKeyStringBuilder<TKey> 
        {
            RegisterParser<TKey, TKeyObject>();
            RegisterStringBuilder<TKey, TKeyObject>();
        }

        private void RegisterParser<TKey, TParser>()
            where TKey : class
            where TParser : IKeyParser<TKey> 
        {
            Kernel.Bind<IKeyParser<TKey>>().To<TParser>().InSingletonScope();
        }

        private void RegisterStringBuilder<TKey, TKeyStringBuilder>()
            where TKey : class
            where TKeyStringBuilder : IKeyStringBuilder<TKey>
        {
            Kernel.Bind<IKeyStringBuilder<TKey>>().To<TKeyStringBuilder>().InSingletonScope();
        }
    }
}