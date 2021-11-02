using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Parameters.PackScheduleService;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class UpdateProductionScheduleParameters : IUpdateProductionScheduleParameters
    {
        public string UserToken { get; set; }
        public string ProductionScheduleKey { get; set; }
        public IEnumerable<ISetProductionScheduleItemParameters> ScheduledItems { get; set; }
    }

    public class SetProductionScheduleItemParameters : ISetProductionScheduleItemParameters
    {
        public int Index { get; set; }
        public bool FlushBefore { get; set; }
        public string FlushBeforeInstructions { get; set; }
        public bool FlushAfter { get; set; }
        public string FlushAfterInstructions { get; set; }
        public string PackScheduleKey { get; set; }
    }
}