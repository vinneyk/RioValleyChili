using RioValleyChili.Data.Models;
using Solutionhead.Data;

namespace RioValleyChili.Data.Interfaces.UnitsOfWork
{
    public interface IInventoryPickOrderUnitOfWork : IUnitOfWork
    {
        IRepository<InventoryPickOrder> InventoryPickOrderRepository { get; }
        IRepository<InventoryPickOrderItem> InventoryPickOrderItemRepository { get; }
    }
}