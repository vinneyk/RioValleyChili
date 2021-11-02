using RioValleyChili.Core.Interfaces;

namespace RioValleyChili.Services.Interfaces.Returns.PackScheduleService
{
    public interface IPackScheduleBaseParametersReturn : IPackScheduleBaseReturn
    {
        IProductionBatchTargetParameters TargetParameters { get; }
    }
}