using RioValleyChili.Services.Interfaces.Parameters.PackScheduleService;

namespace RioValleyChili.Services.Models.Parameters
{
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