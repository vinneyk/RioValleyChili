using RioValleyChili.Core.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Services.Interfaces.Parameters.PackScheduleService
{
    public interface IUpdateProductionBatchParameters : IUserIdentifiable, IProductionBatchTargetParameters
    {
        string ProductionBatchKey { get; }
        string Notes { get; }
    }
}