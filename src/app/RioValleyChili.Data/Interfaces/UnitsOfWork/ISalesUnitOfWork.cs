// ReSharper disable RedundantExtendsListEntry

using RioValleyChili.Data.Models;
using Solutionhead.Data;

namespace RioValleyChili.Data.Interfaces.UnitsOfWork
{
    public interface ISalesUnitOfWork : IUnitOfWork,
        ICompanyUnitOfWork,
        IPickedInventoryUnitOfWork,
        IInventoryPickOrderUnitOfWork,
        IShipmentUnitOfWork,
        IFacilityUnitOfWork
    {
        IRepository<CustomerProductCode> CustomerProductCodeRepository { get; }
        IRepository<Contract> ContractRepository { get; }
        IRepository<ContractItem> ContractItemRepository { get; }
        IRepository<SalesOrder> SalesOrderRepository { get; }
        IRepository<SalesOrderItem> SalesOrderItemRepository { get; }
        IRepository<SalesOrderPickedItem> SalesOrderPickedItemRepository { get; }
        IRepository<LotSalesOrderAllowance> LotSalesOrderAllowanceRepository { get; }
        IRepository<SalesQuote> SalesQuoteRepository { get; }
        IRepository<SalesQuoteItem> SalesQuoteItemRepository { get; }
    }
}

// ReSharper restore RedundantExtendsListEntry