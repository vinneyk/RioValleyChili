using RioValleyChili.Core.Interfaces;
using RioValleyChili.Services.Interfaces.Returns.PackScheduleService;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class PackScheduleBaseParametersReturn : PackScheduleBaseReturn, IPackScheduleBaseParametersReturn
    {
        public IProductionBatchTargetParameters TargetParameters { get; internal set; }
    }
}