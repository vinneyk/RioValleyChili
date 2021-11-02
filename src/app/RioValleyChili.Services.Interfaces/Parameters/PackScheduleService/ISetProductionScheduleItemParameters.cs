namespace RioValleyChili.Services.Interfaces.Parameters.PackScheduleService
{
    public interface ISetProductionScheduleItemParameters
    {
        int Index { get; }
        bool FlushBefore { get; }
        string FlushBeforeInstructions { get; }
        bool FlushAfter { get; }
        string FlushAfterInstructions { get; }
        string PackScheduleKey { get; }
    }
}