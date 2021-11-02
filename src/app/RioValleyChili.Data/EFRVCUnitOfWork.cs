using System.Data.Entity;
using RioValleyChili.Data.Interfaces;
using RioValleyChili.Data.Models;
using Solutionhead.Data;
using Solutionhead.Data.EntityFramework;

namespace RioValleyChili.Data
{
    public class EFRVCUnitOfWork : EFUnitOfWorkBase, IRVCUnitOfWork
    {
        public EFRVCUnitOfWork() { }

        public EFRVCUnitOfWork(DbContext context) : base(context) { }

        #region Fields.

        private EFRepository<Lot> _lotRepository;
        private EFRepository<LotHistory> _lotHistoryRepository;
        private EFRepository<AdditiveLot> _additiveLotRepository;
        private EFRepository<ChileLot> _chileLotRepository;
        private EFRepository<PackagingLot> _packagingLotRepository;
        private EFRepository<LotAttribute> _lotAttributeRepository;
        private EFRepository<LotDefect> _lotDefectRepository;
        private EFRepository<LotAttributeDefect> _lotAttributeDefectRepository;
        private EFRepository<LotDefectResolution> _lotDefectResolutionRepository;
        private EFRepository<Inventory> _inventoryRepository;
        private EFRepository<InventoryAdjustment> _inventoryAdjustmentRepository;
        private EFRepository<InventoryAdjustmentItem> _inventoryAdjustmentItemRepository;
        private EFRepository<Facility> _facilityRepository;
        private EFRepository<Location> _locationRepository;
        private EFRepository<ShipmentInformation> _shipmentInformationRepository;
        private EFRepository<TreatmentOrder> _treatmentOrderRepository;
        private EFRepository<InventoryTreatment> _inventoryTreatmentRepository;
        private EFRepository<IntraWarehouseOrder> _intraWarehouseOrderRepository;
        private EFRepository<PickedInventory> _pickedInventoryRepository;
        private EFRepository<PickedInventoryItem> _pickedInventoryItemRepository;
        private EFRepository<InventoryPickOrder> _inventoryPickOrderRepository;
        private EFRepository<InventoryPickOrderItem> _inventoryPickOrderItemRepository;
        private EFRepository<PackSchedule> _packScheduleRepository;
        private EFRepository<ProductionBatch> _productionBatchRepository;
        private EFRepository<ProductionSchedule> _productionScheduleRepository;
        private EFRepository<ProductionScheduleItem> _productionScheduleItemRepository;
        private EFRepository<Instruction> _instructionRepository;
        private EFRepository<WorkType> _workTypeRepository;
        private EFRepository<Product> _productRepository;
        private EFRepository<AdditiveProduct> _additiveProductRepository;
        private EFRepository<AdditiveType> _additiveTypeRepository;
        private EFRepository<ChileProduct> _chileProductRepository;
        private EFRepository<ChileType> _chileTypeRepository;
        private EFRepository<ChileProductAttributeRange> _chileProductAttributeRangeRepository;
        private EFRepository<ChileProductIngredient> _chileProductIngredientRepository;
        private EFRepository<AttributeName> _attributeNameRepository;
        private EFRepository<PackagingProduct> _packagingProductRepository;
        private EFRepository<ChileMaterialsReceived> _chileMaterialsReceivedRepository;
        private EFRepository<ChileMaterialsReceivedItem> _chileMaterialsReceivedItemsRepository;
        private EFRepository<Company> _companiesRepository;
        private EFRepository<CompanyTypeRecord> _companyTypeRecords;
        private EFRepository<Customer> _customerRepository;
        private EFRepository<CustomerNote> _customerNoteRepository;
        private EFRepository<CustomerProductCode> _customerProductCodeRepository;
        private EFRepository<CustomerProductAttributeRange> _customerProductAttributeRangeRepository;
        private EFRepository<Contract> _contractRepository;
        private EFRepository<ContractItem> _contractItemRepository;
        private EFRepository<Contact> _contactRepository;
        private EFRepository<ContactAddress> _contactAddressRepository;
        private EFRepository<SalesOrder> _salesOrderRepository;
        private EFRepository<SalesOrderItem> _salesOrderItemRepository;
        private EFRepository<SalesOrderPickedItem> _salesOrderPickedItemRepository;
        private EFRepository<LotContractAllowance> _lotContractAllowanceRepository;
        private EFRepository<LotSalesOrderAllowance> _lotSalesOrderAllowanceRepository;
        private EFRepository<LotCustomerAllowance> _lotCustomerAllowanceRepository;
        private EFRepository<Notebook> _notebookRepository;
        private EFRepository<Note> _noteRepository;
        private EFRepository<Employee> _employeesRepository;
        private EFRepository<ChileLotProduction> _chileLotProductionRepository;
        private EFRepository<LotProductionResults> _lotProductionResultsRepository;
        private EFRepository<LotProductionResultItem> _lotProductionResultItemsRepository;
        private EFRepository<InventoryShipmentOrder> _inventoryShipmentOrderRepository;
        private EFRepository<InventoryTransaction> _inventoryTransactionsRepository;
        private EFRepository<SampleOrder> _sampleOrderRepository;
        private EFRepository<SampleOrderItem> _sampleOrderItemRepository;
        private EFRepository<SampleOrderItemMatch> _sampleOrderItemMatchRepository;
        private EFRepository<SampleOrderItemSpec> _sampleOrderItemSpecRepository;
        private EFRepository<SampleOrderJournalEntry> _sampleOrderJournalEntryRepository;
        private EFRepository<SalesQuote> _salesQuoteRepository;
        private EFRepository<SalesQuoteItem> _salesQuoteItemRepository;

        #endregion

        public IRepository<Lot> LotRepository
        {
            get { return _lotRepository ?? (_lotRepository = new EFRepository<Lot>(Context)); }
        }

        public IRepository<LotHistory> LotHistoryRepository
        {
            get { return _lotHistoryRepository ?? (_lotHistoryRepository = new EFRepository<LotHistory>(Context)); }
        }

        public IRepository<AdditiveLot> AdditiveLotRepository
        {
            get { return _additiveLotRepository ?? (_additiveLotRepository = new EFRepository<AdditiveLot>(Context)); }
        }

        public IRepository<ChileLot> ChileLotRepository
        {
            get { return _chileLotRepository ?? (_chileLotRepository = new EFRepository<ChileLot>(Context)); }
        }

        public IRepository<PackagingLot> PackagingLotRepository
        {
            get { return _packagingLotRepository ?? (_packagingLotRepository = new EFRepository<PackagingLot>(Context)); }
        }

        public IRepository<AttributeName> AttributeNameRepository
        {
            get { return _attributeNameRepository ?? (_attributeNameRepository = new EFRepository<AttributeName>(Context)); }
        }

        public IRepository<LotAttribute> LotAttributeRepository
        {
            get { return _lotAttributeRepository ?? (_lotAttributeRepository = new EFRepository<LotAttribute>(Context)); }
        }

        public IRepository<LotDefect> LotDefectRepository
        {
            get { return _lotDefectRepository ?? (_lotDefectRepository = new EFRepository<LotDefect>(Context)); }
        }

        public IRepository<Inventory> InventoryRepository
        {
            get { return _inventoryRepository ?? (_inventoryRepository = new EFRepository<Inventory>(Context)); }
        }

        public IRepository<InventoryAdjustment> InventoryAdjustmentRepository
        {
            get { return _inventoryAdjustmentRepository ?? (_inventoryAdjustmentRepository = new EFRepository<InventoryAdjustment>(Context)); }
        }

        public IRepository<InventoryAdjustmentItem> InventoryAdjustmentItemRepository
        {
            get { return _inventoryAdjustmentItemRepository ?? (_inventoryAdjustmentItemRepository = new EFRepository<InventoryAdjustmentItem>(Context)); }
        }

        public IRepository<LotDefectResolution> LotDefectResolutionRepository
        {
            get { return _lotDefectResolutionRepository ?? (_lotDefectResolutionRepository = new EFRepository<LotDefectResolution>(Context)); }
        }

        public IRepository<LotAttributeDefect> LotAttributeDefectRepository
        {
            get { return _lotAttributeDefectRepository ?? (_lotAttributeDefectRepository = new EFRepository<LotAttributeDefect>(Context)); }
        }

        public IRepository<Facility> FacilityRepository
        {
            get { return _facilityRepository ?? (_facilityRepository = new EFRepository<Facility>(Context)); }
        }

        public IRepository<Location> LocationRepository
        {
            get { return _locationRepository ?? (_locationRepository = new EFRepository<Location>(Context)); }
        }

        public IRepository<ShipmentInformation> ShipmentInformationRepository
        {
            get { return _shipmentInformationRepository ?? (_shipmentInformationRepository = new EFRepository<ShipmentInformation>(Context)); }
        }

        public IRepository<TreatmentOrder> TreatmentOrderRepository
        {
            get { return _treatmentOrderRepository ?? (_treatmentOrderRepository = new EFRepository<TreatmentOrder>(Context)); }
        }

        public IRepository<InventoryTreatment> InventoryTreatmentRepository
        {
            get { return _inventoryTreatmentRepository ?? (_inventoryTreatmentRepository = new EFRepository<InventoryTreatment>(Context)); }
        }

        public IRepository<IntraWarehouseOrder> IntraWarehouseOrderRepository
        {
            get { return _intraWarehouseOrderRepository ?? (_intraWarehouseOrderRepository = new EFRepository<IntraWarehouseOrder>(Context)); }
        }

        public IRepository<PickedInventory> PickedInventoryRepository
        {
            get { return _pickedInventoryRepository ?? (_pickedInventoryRepository = new EFRepository<PickedInventory>(Context)); }
        }

        public IRepository<PickedInventoryItem> PickedInventoryItemRepository
        {
            get { return _pickedInventoryItemRepository ?? (_pickedInventoryItemRepository = new EFRepository<PickedInventoryItem>(Context)); }
        }

        public IRepository<InventoryPickOrder> InventoryPickOrderRepository
        {
            get { return _inventoryPickOrderRepository ?? (_inventoryPickOrderRepository = new EFRepository<InventoryPickOrder>(Context)); }
        }

        public IRepository<InventoryPickOrderItem> InventoryPickOrderItemRepository
        {
            get { return _inventoryPickOrderItemRepository ?? (_inventoryPickOrderItemRepository = new EFRepository<InventoryPickOrderItem>(Context)); }
        }
        
        public IRepository<PackSchedule> PackScheduleRepository
        {
            get { return _packScheduleRepository ?? (_packScheduleRepository = new EFRepository<PackSchedule>(Context)); }
        }

        public IRepository<ProductionBatch> ProductionBatchRepository
        {
            get { return _productionBatchRepository ?? (_productionBatchRepository = new EFRepository<ProductionBatch>(Context)); }
        }

        public IRepository<ProductionSchedule> ProductionScheduleRepository
        {
            get { return _productionScheduleRepository ?? (_productionScheduleRepository = new EFRepository<ProductionSchedule>(Context)); }
        }

        public IRepository<ProductionScheduleItem> ProductionScheduleItemRepository
        {
            get { return _productionScheduleItemRepository ?? (_productionScheduleItemRepository = new EFRepository<ProductionScheduleItem>(Context)); }
        }

        public IRepository<Instruction> InstructionRepository
        {
            get { return _instructionRepository ?? (_instructionRepository = new EFRepository<Instruction>(Context)); }
        }

        public IRepository<WorkType> WorkTypeRepository
        {
            get { return _workTypeRepository ?? (_workTypeRepository = new EFRepository<WorkType>(Context)); }
        }

        #region Implementation of IProductUnitOfWork

        public IRepository<Product> ProductRepository
        {
            get { return _productRepository ?? (_productRepository = new EFRepository<Product>(Context)); }
        }

        public IRepository<AdditiveProduct> AdditiveProductRepository
        {
            get { return _additiveProductRepository ?? (_additiveProductRepository = new EFRepository<AdditiveProduct>(Context)); }
        }

        public IRepository<AdditiveType> AdditiveTypeRepository
        {
            get { return _additiveTypeRepository ?? (_additiveTypeRepository = new EFRepository<AdditiveType>(Context)); }
        }

        public IRepository<ChileProduct> ChileProductRepository
        {
            get { return _chileProductRepository ?? (_chileProductRepository = new EFRepository<ChileProduct>(Context)); }
        }

        public IRepository<ChileType> ChileTypeRepository
        {
            get { return _chileTypeRepository ?? (_chileTypeRepository = new EFRepository<ChileType>(Context)); }
        }

        public IRepository<ChileProductAttributeRange> ChileProductAttributeRangeRepository
        {
            get { return _chileProductAttributeRangeRepository ?? (_chileProductAttributeRangeRepository = new EFRepository<ChileProductAttributeRange>(Context)); }
        }

        public IRepository<ChileProductIngredient> ChileProductIngredientRepository
        {
            get { return _chileProductIngredientRepository ?? (_chileProductIngredientRepository = new EFRepository<ChileProductIngredient>(Context)); }
        }

        public IRepository<PackagingProduct> PackagingProductRepository
        {
            get { return _packagingProductRepository ?? (_packagingProductRepository = new EFRepository<PackagingProduct>(Context)); }
        }

        #endregion

        #region Implementation of IDehydratedMaterialsUnitOfWork

        public IRepository<ChileMaterialsReceived> ChileMaterialsReceivedRepository
        {
            get { return _chileMaterialsReceivedRepository ?? (_chileMaterialsReceivedRepository = new EFRepository<ChileMaterialsReceived>(Context)); }
        }

        public IRepository<ChileMaterialsReceivedItem> ChileMaterialsReceivedItemRepository
        {
            get { return _chileMaterialsReceivedItemsRepository ?? (_chileMaterialsReceivedItemsRepository = new EFRepository<ChileMaterialsReceivedItem>(Context)); }
        }

        public IRepository<Company> CompanyRepository
        {
            get { return _companiesRepository ?? (_companiesRepository = new EFRepository<Company>(Context)); }
        }

        public IRepository<CompanyTypeRecord> CompanyTypeRecords
        {
            get { return _companyTypeRecords ?? (_companyTypeRecords = new EFRepository<CompanyTypeRecord>(Context)); }
        }

        #endregion

        public IRepository<Customer> CustomerRepository
        {
            get { return _customerRepository ?? (_customerRepository = new EFRepository<Customer>(Context)); }
        }

        public IRepository<CustomerNote> CustomerNoteRepository
        {
            get { return _customerNoteRepository ?? (_customerNoteRepository = new EFRepository<CustomerNote>(Context)); }
        }

        public IRepository<CustomerProductCode> CustomerProductCodeRepository
        {
            get { return _customerProductCodeRepository ?? (_customerProductCodeRepository = new EFRepository<CustomerProductCode>(Context)); }
        }

        public IRepository<CustomerProductAttributeRange> CustomerProductAttributeRangeRepository
        {
            get { return _customerProductAttributeRangeRepository ?? (_customerProductAttributeRangeRepository = new EFRepository<CustomerProductAttributeRange>(Context)); }
        }

        public IRepository<Contract> ContractRepository
        {
            get { return _contractRepository ?? (_contractRepository = new EFRepository<Contract>(Context)); }
        }

        public IRepository<ContractItem> ContractItemRepository
        {
            get { return _contractItemRepository ?? (_contractItemRepository = new EFRepository<ContractItem>(Context)); }
        }

        public IRepository<Contact> ContactRepository
        {
            get { return _contactRepository ?? (_contactRepository = new EFRepository<Contact>(Context)); }
        }

        public IRepository<ContactAddress> ContactAddressRepository
        {
            get { return _contactAddressRepository ?? (_contactAddressRepository = new EFRepository<ContactAddress>(Context)); }
        }

        public IRepository<SalesOrder> SalesOrderRepository
        {
            get { return _salesOrderRepository ?? (_salesOrderRepository = new EFRepository<SalesOrder>(Context)); }
        }

        public IRepository<SalesOrderItem> SalesOrderItemRepository
        {
            get { return _salesOrderItemRepository ?? (_salesOrderItemRepository = new EFRepository<SalesOrderItem>(Context)); }
        }

        public IRepository<SalesOrderPickedItem> SalesOrderPickedItemRepository
        {
            get { return _salesOrderPickedItemRepository ?? (_salesOrderPickedItemRepository = new EFRepository<SalesOrderPickedItem>(Context)); }
        }

        public IRepository<LotContractAllowance> LotContractAllowanceRepository
        {
            get { return _lotContractAllowanceRepository ?? (_lotContractAllowanceRepository = new EFRepository<LotContractAllowance>(Context)); }
        }

        public IRepository<LotSalesOrderAllowance> LotSalesOrderAllowanceRepository
        {
            get { return _lotSalesOrderAllowanceRepository ?? (_lotSalesOrderAllowanceRepository = new EFRepository<LotSalesOrderAllowance>(Context)); }
        }

        public IRepository<LotCustomerAllowance> LotCustomerAllowanceRepository
        {
            get { return _lotCustomerAllowanceRepository ?? (_lotCustomerAllowanceRepository = new EFRepository<LotCustomerAllowance>(Context)); }
        }

        public IRepository<Notebook> NotebookRepository
        {
            get { return _notebookRepository ?? (_notebookRepository = new EFRepository<Notebook>(Context)); }
        }

        public IRepository<Note> NoteRepository
        {
            get { return _noteRepository ?? (_noteRepository = new EFRepository<Note>(Context)); }
        }

        public IRepository<Employee> EmployeesRepository
        {
            get { return _employeesRepository ?? (_employeesRepository = new EFRepository<Employee>(Context)); }
        }

        public IRepository<ChileLotProduction> ChileLotProductionRepository
        {
            get { return _chileLotProductionRepository ?? (_chileLotProductionRepository = new EFRepository<ChileLotProduction>(Context)); }
        }

        public IRepository<LotProductionResults> LotProductionResultsRepository
        {
            get { return _lotProductionResultsRepository ?? (_lotProductionResultsRepository = new EFRepository<LotProductionResults>(Context)); }
        }

        public IRepository<LotProductionResultItem> LotProductionResultItemsRepository
        {
            get { return _lotProductionResultItemsRepository ?? (_lotProductionResultItemsRepository = new EFRepository<LotProductionResultItem>(Context)); }
        }

        public IRepository<InventoryShipmentOrder> InventoryShipmentOrderRepository
        {
            get { return _inventoryShipmentOrderRepository ?? (_inventoryShipmentOrderRepository = new EFRepository<InventoryShipmentOrder>(Context)); }
        }

        public IRepository<InventoryTransaction> InventoryTransactionsRepository
        {
            get { return _inventoryTransactionsRepository ?? (_inventoryTransactionsRepository = new EFRepository<InventoryTransaction>(Context)); }
        }

        public IRepository<SampleOrder> SampleOrderRepository
        {
            get { return _sampleOrderRepository ?? (_sampleOrderRepository = new EFRepository<SampleOrder>(Context)); }
        }

        public IRepository<SampleOrderItem> SampleOrderItemRepository
        {
            get { return _sampleOrderItemRepository ?? (_sampleOrderItemRepository = new EFRepository<SampleOrderItem>(Context)); }
        }

        public IRepository<SampleOrderItemMatch> SampleOrderItemMatchRepository
        {
            get { return _sampleOrderItemMatchRepository ?? (_sampleOrderItemMatchRepository = new EFRepository<SampleOrderItemMatch>(Context)); }
        }

        public IRepository<SampleOrderItemSpec> SampleOrderItemSpecRepository
        {
            get { return _sampleOrderItemSpecRepository ?? (_sampleOrderItemSpecRepository = new EFRepository<SampleOrderItemSpec>(Context)); }
        }

        public IRepository<SampleOrderJournalEntry> SampleOrderJournalEntryRepository
        {
            get { return _sampleOrderJournalEntryRepository ?? (_sampleOrderJournalEntryRepository = new EFRepository<SampleOrderJournalEntry>(Context)); }
        }

        public IRepository<SalesQuote> SalesQuoteRepository
        {
            get { return _salesQuoteRepository ?? (_salesQuoteRepository = new EFRepository<SalesQuote>(Context)); }
        }

        public IRepository<SalesQuoteItem> SalesQuoteItemRepository
        {
            get { return _salesQuoteItemRepository ?? (_salesQuoteItemRepository = new EFRepository<SalesQuoteItem>(Context)); }
        }
    }
}