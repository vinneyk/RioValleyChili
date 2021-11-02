using RioValleyChili.Data.Models;
using Solutionhead.Data;

namespace RioValleyChili.Data.Interfaces.UnitsOfWork
{
    public interface ICoreUnitOfWork : IUnitOfWork
    {
        IRepository<Employee> EmployeesRepository { get; } 

        IRepository<InventoryTransaction> InventoryTransactionsRepository { get; }
    }
}