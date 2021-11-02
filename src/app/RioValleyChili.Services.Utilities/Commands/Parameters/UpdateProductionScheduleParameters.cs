using System.Collections.Generic;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Services.Interfaces.Parameters.PackScheduleService;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class UpdateProductionScheduleParameters
    {
        internal IUpdateProductionScheduleParameters Parameters { get; set; }

        internal ProductionScheduleKey ProductionScheduleKey { get; set; }
        internal IEnumerable<UpdateProductionScheduleItemParameters> ScheduledItems { get; set; }
    }
}