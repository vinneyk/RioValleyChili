// ReSharper disable RedundantExtendsListEntry

using RioValleyChili.Data.Models;
using Solutionhead.Data;

namespace RioValleyChili.Data.Interfaces.UnitsOfWork
{
    public interface ILotUnitOfWork : IUnitOfWork,
        ICoreUnitOfWork,
        INotebookUnitOfWork
    {
        IRepository<Lot> LotRepository { get; }

        IRepository<LotHistory> LotHistoryRepository { get; }

        IRepository<LotContractAllowance> LotContractAllowanceRepository { get; }

        IRepository<LotSalesOrderAllowance> LotSalesOrderAllowanceRepository { get; }

        IRepository<LotCustomerAllowance> LotCustomerAllowanceRepository { get; }
            
        IRepository<Inventory> InventoryRepository { get; }

        IRepository<InventoryTreatment> InventoryTreatmentRepository { get; }
        
        IRepository<AdditiveLot> AdditiveLotRepository { get; }

        IRepository<ChileLot> ChileLotRepository { get; }

        IRepository<PackagingLot> PackagingLotRepository { get; }

        IRepository<AttributeName> AttributeNameRepository { get; }

        IRepository<LotAttribute> LotAttributeRepository { get; }

        IRepository<LotDefect> LotDefectRepository { get; }

        IRepository<LotAttributeDefect> LotAttributeDefectRepository { get; }
            
        IRepository<LotDefectResolution> LotDefectResolutionRepository { get; }

        IRepository<PickedInventoryItem> PickedInventoryItemRepository { get; }

        IRepository<ChileMaterialsReceivedItem> ChileMaterialsReceivedItemRepository { get; }

        IRepository<ChileProduct> ChileProductRepository { get; }

        IRepository<AdditiveProduct> AdditiveProductRepository { get; }
            
        IRepository<PackagingProduct> PackagingProductRepository { get; }

        IRepository<ProductionBatch> ProductionBatchRepository { get; }

        IRepository<ChileLotProduction> ChileLotProductionRepository { get; }

        IRepository<LotProductionResults> LotProductionResultsRepository { get; }

        IRepository<SalesOrder> SalesOrderRepository { get; }
    }
}

// ReSharper restore RedundantExtendsListEntry