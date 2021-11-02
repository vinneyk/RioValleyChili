// ReSharper disable RedundantExtendsListEntry

using RioValleyChili.Data.Models;
using Solutionhead.Data;

namespace RioValleyChili.Data.Interfaces.UnitsOfWork
{
    public interface IMaterialsReceivedUnitOfWork : IUnitOfWork,
        IInventoryUnitOfWork,
        ICompanyUnitOfWork,
        ICoreUnitOfWork
    {
        IRepository<ChileMaterialsReceived> ChileMaterialsReceivedRepository { get; }
    }
}

// ReSharper restore RedundantExtendsListEntry