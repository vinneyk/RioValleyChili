// ReSharper disable RedundantExtendsListEntry

using RioValleyChili.Data.Models;
using Solutionhead.Data;

namespace RioValleyChili.Data.Interfaces.UnitsOfWork
{
    public interface IIntraWarehouseOrderUnitOfWork : IUnitOfWork,
        IPickedInventoryUnitOfWork,
        IFacilityUnitOfWork
    {
        IRepository<IntraWarehouseOrder> IntraWarehouseOrderRepository { get; }
    }
}

// ReSharper restore RedundantExtendsListEntry