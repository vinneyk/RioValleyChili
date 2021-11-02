// ReSharper disable RedundantExtendsListEntry

using RioValleyChili.Data.Models;
using Solutionhead.Data;

namespace RioValleyChili.Data.Interfaces.UnitsOfWork
{
    public interface IProductionUnitOfWork : IUnitOfWork,
        IPickedInventoryUnitOfWork,
        IChileLotProductionUnitOfWork,
        ICompanyUnitOfWork
    {
        IRepository<PackSchedule> PackScheduleRepository { get; }
        IRepository<ProductionSchedule> ProductionScheduleRepository { get; }
        IRepository<ProductionScheduleItem> ProductionScheduleItemRepository { get; }
        IRepository<WorkType> WorkTypeRepository { get; }
        IRepository<Location> LocationRepository { get; }
        IRepository<Instruction> InstructionRepository { get; }
    }
}

// ReSharper restore RedundantExtendsListEntry