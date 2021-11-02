// ReSharper disable RedundantExtendsListEntry

using RioValleyChili.Data.Models;
using Solutionhead.Data;

namespace RioValleyChili.Data.Interfaces.UnitsOfWork
{
    public interface IFacilityUnitOfWork : ICoreUnitOfWork, IUnitOfWork
    {
        IRepository<Location> LocationRepository { get; }
        IRepository<Facility> FacilityRepository { get; }
    }
}

// ReSharper restore RedundantExtendsListEntry