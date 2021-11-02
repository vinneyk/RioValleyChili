using RioValleyChili.Data;
using Solutionhead.Data;

namespace RioValleyChili.Services.Utilities.Extensions
{
    public static class UnitOfWorkExtensions
    {
        public static T OptimizeForReadonly<T>(this T unitOfWork)
            where T : IUnitOfWork
        {
            var ef = unitOfWork as EFUnitOfWorkBase;
            if(ef != null)
            {
                ef.Context.Configuration.AutoDetectChangesEnabled = false;
                ef.Context.Configuration.LazyLoadingEnabled = false;
                ef.Context.Configuration.ProxyCreationEnabled = false;
                ef.Context.Configuration.ValidateOnSaveEnabled = false;
            }
            return unitOfWork;
        }
    }
}