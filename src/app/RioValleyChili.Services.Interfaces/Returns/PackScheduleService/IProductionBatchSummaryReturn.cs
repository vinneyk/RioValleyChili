using RioValleyChili.Core.Interfaces;
using RioValleyChili.Services.Interfaces.Returns.ProductService;

namespace RioValleyChili.Services.Interfaces.Returns.PackScheduleService
{
    public interface IProductionBatchSummaryReturn : IProductionBatchTargetParameters
    {
        string ProductionBatchKey { get; }
        string OutputLotKey { get; }
        bool HasProductionBeenCompleted { get; }
        string Notes { get; }
        IPackagingProductReturn PackagingProduct { get; }
    }
}