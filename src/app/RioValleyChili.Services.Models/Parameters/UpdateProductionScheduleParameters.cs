using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Parameters.PackScheduleService;

namespace RioValleyChili.Services.Models.Parameters
{
    public class UpdateProductionScheduleParameters : IUpdateProductionScheduleParameters
    {
        public string UserToken { get; set; }
        public string ProductionScheduleKey { get; set; }
        public IEnumerable<SetProductionScheduleItemParameters> ScheduledItems { get; set; }

        IEnumerable<ISetProductionScheduleItemParameters> IUpdateProductionScheduleParameters.ScheduledItems { get { return ScheduledItems; } }
    }
}