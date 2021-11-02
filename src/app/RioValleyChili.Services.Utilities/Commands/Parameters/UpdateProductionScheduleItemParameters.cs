using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Services.Interfaces.Parameters.PackScheduleService;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class UpdateProductionScheduleItemParameters
    {
        internal ISetProductionScheduleItemParameters Parameters { get; set; }
        internal PackScheduleKey PackScheduleKey { get; set; }
    }
}