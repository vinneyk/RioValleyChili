// ReSharper disable RedundantExtendsListEntry

using RioValleyChili.Data.Models;
using Solutionhead.Data;

namespace RioValleyChili.Data.Interfaces.UnitsOfWork
{
    public interface IPickedInventoryUnitOfWork : IUnitOfWork, IInventoryUnitOfWork
    {
        IRepository<PickedInventory> PickedInventoryRepository { get; }

        IRepository<PickedInventoryItem> PickedInventoryItemRepository { get; }
    }
}

// ReSharper restore RedundantExtendsListEntry