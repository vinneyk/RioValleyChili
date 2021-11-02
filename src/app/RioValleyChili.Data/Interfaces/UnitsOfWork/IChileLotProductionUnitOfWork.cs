// ReSharper disable RedundantExtendsListEntry

using RioValleyChili.Data.Models;
using Solutionhead.Data;

namespace RioValleyChili.Data.Interfaces.UnitsOfWork
{
    public interface IChileLotProductionUnitOfWork : IUnitOfWork,
        IPickedInventoryUnitOfWork
    {
        IRepository<ChileLotProduction> ChileLotProductionRepository { get; }
        IRepository<LotProductionResults> LotProductionResultsRepository { get; }
        IRepository<LotProductionResultItem> LotProductionResultItemsRepository { get; }
    }
}

// ReSharper restore RedundantExtendsListEntry