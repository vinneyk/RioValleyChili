using Solutionhead.Data;

namespace RioValleyChili.Data.Interfaces
{
    public interface IUnitOfWorkContainer<out T> where T : IUnitOfWork
    {
        T UnitOfWork { get; }
    }
}
