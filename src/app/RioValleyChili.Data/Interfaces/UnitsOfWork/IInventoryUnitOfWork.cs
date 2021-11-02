// ReSharper disable RedundantExtendsListEntry

using RioValleyChili.Data.Models;
using Solutionhead.Data;

namespace RioValleyChili.Data.Interfaces.UnitsOfWork
{
    public interface IInventoryUnitOfWork : IUnitOfWork,
        ILotUnitOfWork,
        INotebookUnitOfWork,
        ICoreUnitOfWork,
        IFacilityUnitOfWork
    {
        IRepository<InventoryAdjustment> InventoryAdjustmentRepository { get; }
        IRepository<InventoryAdjustmentItem> InventoryAdjustmentItemRepository { get; }
    }
}

// ReSharper restore RedundantExtendsListEntry