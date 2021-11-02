using RioValleyChili.Business.Core.Keys;

namespace RioValleyChili.Services.Utilities.Conductors.Parameters
{
    internal class SetProductionScheduleItemParameters
    {
        internal int Index { get; set; }
        internal bool FlushBefore { get; set; }
        internal string FlushBeforeInstructions { get; set; }
        internal bool FlushAfter { get; set; }
        internal string FlushAfterInstructions { get; set; }
        internal PackScheduleKey PackScheduleKey { get; set; }
    }
}